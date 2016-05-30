using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhyj.Common;
using JiaQin.Entity;
using JiaQin.Data;
using System.IO;
using System.Web;
namespace JiaQin.Services
{
    class UserServ : AppBase
    {
        
        
        public void Logon() {
            SysUser u = Request.PackageRequest<SysUser>();
            Base64Encoding coding= new Base64Encoding();
            SysUserData userD = dataExecutorImp.GetInstance<SysUserData>();
            SysUser u2 = userD.Info(u.UserName, coding.Encode(u.Password));
            if (u2==null)
	        {
                TemplateData[JsonKeyValue.res]=JsonKeyValue.fail;
                TemplateData[JsonKeyValue.tip]="用户名密码错误";
                return;
	        }
            ICached[u2.UserName] = u2;
            Response.Cookies.Add(new HttpCookie(aspx_username, coding.Encode(u2.UserName,true,true)));
            TemplateData[JsonKeyValue.res] = JsonKeyValue.ok;
            TemplateData[JsonKeyValue.tip] = "登录成功";
        }
        public void Logout() {
            if (CurrentUser!=null)
            {
                ICached.Remove(CurrentUser.UserName);                
            }
            Response.Cookies.Clear();
            TemplateData[JsonKeyValue.res] = JsonKeyValue.ok;
            TemplateData[JsonKeyValue.tip] = "注销完成";

        }

        public void MainPage() {
            permissionData = dataExecutorImp.GetInstance<SysPermissionData>();
            SysPermission [] list= permissionData.ListByUserId(CurrentUser.Id,0);
            TemplateData[listDataStr] = list;
        }
        SysPermissionData permissionData = null;
        public SysPermission[] ChildrenList(SysPermission permission) {
            SysPermission[] list = permissionData.ListByUserId(CurrentUser.Id, permission.Id);
            return list;
        }

        public void AddEvent() { 
            SysUser user = Request.PackageRequest<SysUser>();
            string deptId = Request["department"];
            SysUserData userData=dataExecutorImp.GetInstance<SysUserData>();
            string roleId = Request["jobRole"];
            if (userData.Exist(user.UserName))
            {
                SysUser u2= userData.Info(user.UserName);
                TemplateData[JsonKeyValue.res] = JsonKeyValue.exists;
                TemplateData[JsonKeyValue.tip] = "用户名已经存在了："+u2.UserName+"  " +u2.Name;
                TemplateData[JsonKeyValue.field] = "UserName";
                return;
            }
            user.Password = new Base64Encoding().Encode("999999");
            userData.Insert(user, Convert.ToInt32(deptId), Convert.ToInt32(roleId));
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "用户添加完成";
        }
        public void UpdatePage()
        {
            SysUserData userData = dataExecutorImp.GetInstance<SysUserData>();
            SysUser user= userData.Info(this.ID);
            TemplateData[dataStr] = user;
        }
        public void UpdateEvent() {


            SysUser user = Request.PackageRequest<SysUser>();
            string roleId = Request["jobRole"];
            SysUserData userData = dataExecutorImp.GetInstance<SysUserData>();

            userData.Update(user,Convert.ToInt32(roleId));
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "用户编辑完成";

        
        }
        public void DeleteEvent() {
            SysUserData userData = dataExecutorImp.GetInstance<SysUserData>();

            userData.Delete(this.ID);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "用户已删除完成";
        }
        public void DepartmentUserList() {

            DepartmentData depData = dataExecutorImp.GetInstance<DepartmentData>();
            Department dept = depData.Info(this.ID);
            Department[] deptList = dept.ChildrenDepartmentList;
            TemplateData["dep"] = dept;
            TemplateData[dataStr] = deptList;



            SysUserData userD=dataExecutorImp.GetInstance<SysUserData>();
            SysUser[]userList= userD.ListByDpartmentId(Request["name"],this.ID,PageSize,PageNum,out rowCount);
            Page();
            TemplateData[listDataStr] = userList;
        }
        public void UpdatePassword() { 
            string pwd=Request["pwd"];
            string newpwd = Request["newpwd"];
            string renewpwd = Request["renewpwd"];
            if (string.IsNullOrEmpty(newpwd)||string.IsNullOrEmpty(renewpwd) || string.IsNullOrEmpty(pwd) || !newpwd.Equals(renewpwd))
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.alert;
                TemplateData[JsonKeyValue.tip] = "请输入密码";
                return;
            }
            SysUserData userData=dataExecutorImp.GetInstance<SysUserData>();
            Base64Encoding encode= new Base64Encoding();
            SysUser user= userData.Info(CurrentUser.UserName, encode.Encode(pwd));
            if (user==null || user.Id!=CurrentUser.Id)
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.alert;
                TemplateData[JsonKeyValue.tip] = "原始密码输入错误";
                return;
            }
            userData.UpdatePwd(user.Id, encode.Encode(newpwd));
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOk;
            TemplateData[JsonKeyValue.tip] = "密码修改完成";
            return; 
        }






    }
}

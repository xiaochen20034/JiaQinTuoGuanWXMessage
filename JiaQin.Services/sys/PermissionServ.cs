using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JiaQin.Data;
using JiaQin.Entity;
using Zhyj.Common;
namespace JiaQin.Service
{
    public class PermissionServ:AppBase
    {
        /// <summary>
        /// 角色有权限，去除，没有权限，添加
        /// </summary>
        public void SwitchRolePermission() { 
            string roleId=Request["roleId"];
            string permissionId=Request["permissionId"];

            SysPermissionData permissionData=dataExecutorImp.GetInstance<SysPermissionData>();
            bool flag= permissionData.SwitchRolePermission(Convert.ToInt32(roleId),Convert.ToInt32(permissionId));
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "角色权限已"+(flag?"【添加】":"【移除】")+"切换完成";
            TemplateData[JsonKeyValue.field] = flag;
            
        }


        public void PermissionFigure()
        {
            SysPermissionData departData = dataExecutorImp.GetInstance<SysPermissionData>(); ;
            SysPermission[] depList = departData.RootList();
            TemplateData[listDataStr] = depList;

        }
        public void AddEvent()
        {
            SysPermission dep = Request.PackageRequest<SysPermission>();
            SysPermissionData depData = dataExecutorImp.GetInstance<SysPermissionData>();
            if (depData.Info(dep.Code) != null)
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.exists;
                TemplateData[JsonKeyValue.tip] = "编码已存在";
                TemplateData[JsonKeyValue.field] = "Code";
                return;
            }
            int parentId = 0;
            // dep.ParentId!=null && dep.ParentId.Value>0
            if (int.TryParse(Request["ParentId"],out parentId) && parentId>0)
            {
                dep.ParentId = parentId;
                SysPermission depTemp = depData.Info(dep.ParentId.Value);
                if (depTemp == null)
                {
                    TemplateData[JsonKeyValue.res] = JsonKeyValue.fail;
                    TemplateData[JsonKeyValue.tip] = "父级不存在";
                    return;
                }
                dep.FullParent = depTemp.FullParent + "," + depTemp.Id;
            }
            depData.Add(dep);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "添加完成";

        }
        public void UpdatePage()
        {
            SysPermissionData depData = dataExecutorImp.GetInstance<SysPermissionData>();
            TemplateData["dep"] = depData.Info(this.ID);
        }
        public void UpdateEvent()
        {
            SysPermission dep = Request.PackageRequest<SysPermission>();
            SysPermissionData depData = dataExecutorImp.GetInstance<SysPermissionData>();

            if (dep.ParentId != null && dep.ParentId.Value > 0)
            {
                SysPermission depTemp = depData.Info(dep.ParentId.Value);
                if (depTemp == null)
                {
                    TemplateData[JsonKeyValue.res] = JsonKeyValue.fail;
                    TemplateData[JsonKeyValue.tip] = "父级不存在";
                    return;
                }
                dep.FullParent = depTemp.FullParent + "," + depTemp.Id;
            }

            depData.Update(dep);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "编辑完成";

        }
        public void DeleteEvent()
        {

            SysPermissionData depData = dataExecutorImp.GetInstance<SysPermissionData>();
            SysPermission info=depData.Info(this.ID);
            SysPermission[] depChilden = info.ChildrenPermissionList;
            if (depChilden != null && depChilden.Length > 0)
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.alert;
                TemplateData[JsonKeyValue.tip] = "有子级，不能删除";
            }
            depData.Delete(info);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "删除完成";

        }
        public void PermissionChildrenLi()
        {
            SysPermissionData depData = dataExecutorImp.GetInstance<SysPermissionData>();
            SysPermission dept = null;
            if (this.ID != 0)
            {
                dept = depData.Info(this.ID);
            }
            else {
                dept = new SysPermission() {  Id=0, Name="系统权限"};
                dept.ChildrenPermissionList = depData.RootList();
            }
            
            SysPermission[] deptList = dept.ChildrenPermissionList;
            TemplateData["dep"] = dept;
            TemplateData[dataStr] = deptList;


        }
    }
}

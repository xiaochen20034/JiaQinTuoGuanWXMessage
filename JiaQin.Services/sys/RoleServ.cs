using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JiaQin.Entity;
using JiaQin.Data;
using Zhyj.Common;

namespace JiaQin.Service
{
    public class RoleServ:AppBase
    {

        public void FigurePage() { 
            SysRoleData roleData= dataExecutorImp.GetInstance<SysRoleData>();
            TemplateData[dataStr]= roleData.List();
        }
        public void RolePermissionList() { 
            SysRoleData roleData=dataExecutorImp.GetInstance<SysRoleData>();
            SysPermissionData permissionData=dataExecutorImp.GetInstance<SysPermissionData>();
            SysRole role= roleData.Info(this.ID);

            SysPermission[] list = permissionData.List(0);
            
            TemplateData[dataStr] = role;
            TemplateData[listDataStr] = list;


        }
        public bool RoleHasPermission(SysRole role,SysPermission permission) {
            return role.SysPermissionList.FirstOrDefault(new Func<SysPermission, bool>(delegate(SysPermission per) {
                return permission.Id == per.Id;                    
            })) !=null;


        }
        public void AddEvent() { 
            SysRole role = Request.PackageRequest<SysRole>();
            SysRoleData roleData=dataExecutorImp.GetInstance<SysRoleData>();
            SysRole role2= roleData.Info(role.Code);
            if (role2!=null)
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.alert;
                TemplateData[JsonKeyValue.tip] = "编码已经存在";
                TemplateData[JsonKeyValue.field] = "Code";
                return;
            }
            roleData.Add(role);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "角色添加完成";
        }
        public void UpdatePage() { 
             SysRoleData roleData=dataExecutorImp.GetInstance<SysRoleData>();
             SysRole role = roleData.Info(this.ID);
             TemplateData[dataStr] = role;
        }
        public void UpdateEvent() {
            SysRole role = Request.PackageRequest<SysRole>();
            SysRoleData roleData = dataExecutorImp.GetInstance<SysRoleData>();
            roleData.Update(role);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "角色编辑完成";
        }
        public void DeleteEvent() {
            SysRoleData roleData = dataExecutorImp.GetInstance<SysRoleData>();
            roleData.Delete(this.ID);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "角色删除完成";
        }
    }
}

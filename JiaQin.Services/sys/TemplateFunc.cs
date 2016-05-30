using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JiaQin.Data;
using JiaQin.Entity;

namespace JiaQin.Services
{
    public partial class AppBase
    {

        Zhyj.AderTemplates.TemplateManager templageManager = null;
        
        public virtual bool regist(Zhyj.AderTemplates.TemplateManager templageManager)
        {
            this.templageManager = templageManager;
            this.templageManager.variables["user"] = CurrentUser;

            DepartmentData depData = dataExecutorImp.GetInstance<DepartmentData>();
            SysUserData userD = dataExecutorImp.GetInstance<SysUserData>();
            SysRoleData roleD = dataExecutorImp.GetInstance<SysRoleData>();

            templageManager.Functions.Add("departmentUserList", new Zhyj.AderTemplates.TemplateFunction(delegate(object[] obj)
            {
                return userD.ListByDpartmentId(Convert.ToInt32(obj[0]));
            }));

            templageManager.Functions.Add("roleList", new Zhyj.AderTemplates.TemplateFunction(delegate(object[] obj)
            {
                return roleD.List();
            }));
            templageManager.Functions.Add("deartmentChildren", new Zhyj.AderTemplates.TemplateFunction(delegate(object[] obj)
            {
                string code = obj[0] as string;

                return depData.ChildrenList(code);
            }));


            templageManager.Functions.Add("userDeartment", new Zhyj.AderTemplates.TemplateFunction(delegate(object[] obj)
            {
                return depData.UserDepartmentList(CurrentUser.Id);
            }));


            #region hasPermission  当前用户是否有此权限
            templageManager.Functions.Add("hasPermission", new Zhyj.AderTemplates.TemplateFunction(delegate(object[] obj)
            {
                string code = obj[0] as string;
                return CurrentUser.SysPermissionList.FirstOrDefault<Entity.SysPermission>(new Func<Entity.SysPermission, bool>(delegate(Entity.SysPermission per)
                {
                    return per.Code.Equals(code);
                })) != null;

            }));
            #endregion



           


            return true;
        }
    }
}

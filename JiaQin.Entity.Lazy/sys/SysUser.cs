using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Entity.Lazy
{
    /// <summary>
    /// 系统用户
    /// </summary>
    public class SysUserLazy:SysUser
    {
        public Func<int, SysPermission[]> SysPermissionListLazy;
        public override SysPermission[] SysPermissionList
        {
            get
            {
                if (base.SysPermissionList==null && SysPermissionListLazy!=null)
                {
                    return SysPermissionListLazy(base.Id);
                }
                return base.SysPermissionList;
            }
            set
            {
                base.SysPermissionList = value;
            }
        }
        public Func<int, SysRole[]> SysRoleListLazy;
        public override SysRole[] SysRoleList
        {
            get
            {
                if (base.SysRoleList==null && SysRoleListLazy!=null)
                {
                    return SysRoleListLazy(base.Id);
                }
                return base.SysRoleList;
            }
            set
            {
                base.SysRoleList = value;
            }
        }
       
    }
}

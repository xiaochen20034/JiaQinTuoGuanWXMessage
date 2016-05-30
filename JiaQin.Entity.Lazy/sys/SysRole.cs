using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Entity.Lazy
{
    /// <summary>
    /// 系统角色
    /// </summary>
    public class SysRoleLazy:SysRole
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
        public Func<int, SysUser[]> SysUserListLazy;
        public override SysUser[] SysUserList
        {
            get
            {
                if (base.SysUserList==null && SysUserListLazy!=null)
                {
                    return SysUserListLazy(base.Id);
                }
                return base.SysUserList;
            }
            set
            {
                base.SysUserList = value;
            }
        }
       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Entity.Lazy
{
    /// <summary>
    /// 系统权限
    /// </summary>
    public class SysPermissionLazy:SysPermission
    {
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
        public Func<int, SysPermission> ParentPermissionInfoLazy;
        public override SysPermission ParentPermissionInfo
        {
            get
            {
                if (base.ParentId==null)
                {
                    return null;
                }
                if (base.ParentPermissionInfo==null && ParentPermissionInfoLazy!=null)
                {
                    base.ParentPermissionInfo = ParentPermissionInfoLazy(base.ParentId.Value);
                }
                return base.ParentPermissionInfo;
            }
            set
            {
                base.ParentPermissionInfo = value;
            }
        }
        public Func<int, SysPermission[]> ChildrenPermissionListLazy;
        public override SysPermission[] ChildrenPermissionList
        {
            get
            {
                if (base.ChildrenPermissionList == null && ChildrenPermissionListLazy != null)
                {
                    base.ChildrenPermissionList = ChildrenPermissionListLazy(base.Id);
                }
                return base.ChildrenPermissionList;
            }
            set
            {
                base.ChildrenPermissionList = value;
            }
        }
    }
}

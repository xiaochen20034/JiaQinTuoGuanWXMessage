using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Entity
{
    /// <summary>
    /// 系统权限
    /// </summary>
    public class SysPermission
    {
        public int Id { set; get; }
        public string Code { set; get; }
        public string Name{ set; get; }
        public string Remark { set; get; }
        public int? ParentId { set; get; }
        public string FullParent { set; get; }
        public int? LevelNum { set; get; }
        public int? Sort { set; get; }
        public string Action { set; get; }
        public virtual SysPermission ParentPermissionInfo { set; get; }
        public virtual SysPermission[] ChildrenPermissionList { set; get; }
        public virtual SysRole[] SysRoleList { set; get; }
        
    }
}

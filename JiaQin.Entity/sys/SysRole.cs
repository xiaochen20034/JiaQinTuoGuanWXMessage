using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Entity
{
    /// <summary>
    /// 系统角色
    /// </summary>
    public class SysRole
    {
        public int Id { set; get; }
        public string Code { set; get; }
        public string Name { set; get; }
        public int ParentId { set; get; }
        public string FullParent { set; get; }

        public virtual SysPermission[] SysPermissionList { set; get; }
       
        public virtual SysUser[] SysUserList { set; get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Entity
{
    /// <summary>
    /// 系统用户
    /// </summary>
    public class SysUser
    {
        public int Id { set; get; }
        public string Code { set; get; }
        public string UserName { set;get; }
        public string Password { set; get; }
        public string Name { set; get; }
        public string Gender { set; get; }
        public string Phone { set; get; }
        public string Photo { set; get; }
        /// <summary>
        /// 临时的数据存储
        /// </summary>
        public int TempInt { set; get; }
        /// <summary>
        /// 临时的数据存储
        /// </summary>
        public int TempInt2 { set; get; }

        public virtual School SchoolInfo { set; get; }
        public virtual SysRole[] SysRoleList { set; get; }
        public virtual SysPermission[] SysPermissionList { set; get; }
    }
}

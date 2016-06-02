using System;
using System.Collections.Generic;
using System.Text;
namespace JiaQin.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class School
    {
		/// <summary>
		/// 
		/// </summary>
		public int ID{set;get;}
		/// <summary>
		/// 
		/// </summary>
		public string Name{set;get;}
		/// <summary>
		/// 
		/// </summary>
		public string Des{set;get;}
		/// <summary>
		/// 联系电话
		/// </summary>
		public string ContactPhone{set;get;}

        public int UserId { set; get; }
        
		/// <summary>
		/// 联系人
		/// </summary>
		public string ContactName{set;get;}
        /// <summary>
        /// 校区管理员
        /// </summary>
        public virtual SysUser SysUserInfo { set; get; }
        public virtual Student[] StudentList{set;get;}
        public virtual Teacher[] TeacherList{set;get;}
        public virtual Tag[] TagList{set;get;}
    }
}
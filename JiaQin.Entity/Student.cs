using System;
using System.Collections.Generic;
using System.Text;
namespace JiaQin.Entity
{
    /// <summary>
    /// 学生，基础信息来自user
    /// </summary>
    public class Student
    {
		/// <summary>
		/// 
		/// </summary>
		public int ID{set;get;}
		/// <summary>
		/// 
		/// </summary>
		public int VipUserID{set;get;}
		/// <summary>
		/// 剩余可发短信次数
		/// </summary>
		public int Times{set;get;}
		/// <summary>
		/// 学校ID
		/// </summary>
		public int SchoolId{set;get;}

        public int Age { set; get; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeleteDate { set; get; }
            public virtual School SchoolInfo{set;get;}
        public virtual Parent[] ParentList{set;get;}
        /// <summary>
        /// 学生的第一家长
        /// </summary>
        public virtual Parent ParentInfo { set; get; }
        public virtual VipUser VipUserInfo{set;get;}
        public virtual SignRecord[] SignRecordList{set;get;}
       /// <summary>
        /// 学生的标签
        /// </summary>
        public virtual StudentTag[] StudentTagList { set; get; } 
    }
}
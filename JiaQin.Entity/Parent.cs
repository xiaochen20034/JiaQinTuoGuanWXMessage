using System;
using System.Collections.Generic;
using System.Text;
namespace JiaQin.Entity
{
    /// <summary>
    /// 学生家长
    /// </summary>
    public class Parent
    {
		/// <summary>
		/// 
		/// </summary>
		public int ID{set;get;}
		/// <summary>
		/// 
		/// </summary>
		public int StudentID{set;get;}
		/// <summary>
		/// 
		/// </summary>
		public int VipUserID{set;get;}
		/// <summary>
		/// 学生用户ID
		/// </summary>
		public int StuVipUserId{set;get;}
            public virtual Student StudentInfo{set;get;}
            public virtual VipUser VipUserInfo{set;get;}
            public virtual VipUser StudentVipUserInfo{set;get;}
    }
}
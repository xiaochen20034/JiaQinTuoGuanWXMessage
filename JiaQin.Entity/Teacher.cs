using System;
using System.Collections.Generic;
using System.Text;
namespace JiaQin.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class Teacher
    {
		/// <summary>
		/// 
		/// </summary>
		public int VipUserId{set;get;}
		/// <summary>
		/// 
		/// </summary>
		public int SchoolId{set;get;}
		/// <summary>
		/// 
		/// </summary>
		public DateTime AddDate{set;get;}
            public virtual VipUser VipUserInfo{set;get;}
            public virtual School SchoolInfo{set;get;}
    }
}
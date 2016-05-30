using System;
using System.Collections.Generic;
using System.Text;
namespace JiaQin.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class Tag
    {
		/// <summary>
		/// 
		/// </summary>
		public int Id{set;get;}
		/// <summary>
		/// 标签名称
		/// </summary>
		public string Name{set;get;}
		/// <summary>
		/// 学校ID
		/// </summary>
		public int SchoolId{set;get;}
            public virtual School SchoolInfo{set;get;}
    }
}
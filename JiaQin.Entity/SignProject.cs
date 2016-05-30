using System;
using System.Collections.Generic;
using System.Text;
namespace JiaQin.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class SignProject
    {
		/// <summary>
		/// 
		/// </summary>
		public int Id{set;get;}
		/// <summary>
		/// 名称
		/// </summary>
		public string Name{set;get;}
        public virtual SignRecord[] SignRecordList{set;get;}
    }
}
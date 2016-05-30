using System;
using System.Collections.Generic;
using System.Text;

namespace JiaQin.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteNew
    {

		/// <summary>
		/// 
		/// </summary>
		public int Id{set;get;}

		/// <summary>
		/// 发布用户
		/// </summary>
		public int UserId{set;get;}

		/// <summary>
		/// 标题
		/// </summary>
		public string Title{set;get;}

		/// <summary>
		/// 内容
		/// </summary>
		public string Content{set;get;}

		/// <summary>
		/// 发布时间
		/// </summary>
		public DateTime PublishDate{set;get;}

		/// <summary>
		/// 类型ID
		/// </summary>
		public int TypeId{set;get;}

		/// <summary>
		/// 访问次数
		/// </summary>
		public int VisitCount{set;get;}

		/// <summary>
		/// 所属栏目
		/// </summary>
		public int SiteColumnId{set;get;}

		/// <summary>
		/// 首张图片
		/// </summary>
		public string BlogImg{set;get;}

		/// <summary>
		/// 审核状态
		/// </summary>
		public int Audit{set;get;}

		/// <summary>
		/// 审核时间
		/// </summary>
		public DateTime AuditDate{set;get;}

		/// <summary>
		/// 审核用户
		/// </summary>
		public int AuditVipUserId{set;get;}

            public virtual SiteColumn SiteColumnInfo{set;get;}

            public virtual SysUser UserInfo { set; get; }

    }
}
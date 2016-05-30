using System;
using System.Collections.Generic;
using System.Text;

namespace JiaQin.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteColumn
    {

		/// <summary>
		/// 
		/// </summary>
		public int ID{set;get;}

		/// <summary>
		/// 栏目名称
		/// </summary>
		public string Name{set;get;}

		/// <summary>
		/// 栏目编码
		/// </summary>
		public string Code{set;get;}

		/// <summary>
		/// 父级栏目
		/// </summary>
		public int ParentColumn{set;get;}

		/// <summary>
		/// 完整父级路径
		/// </summary>
		public string FullParentColumn{set;get;}

		/// <summary>
		/// 排序权重
		/// </summary>
		public int Sort{set;get;}

		/// <summary>
		/// 列表模版
		/// </summary>
		public string ListTemplate{set;get;}

		/// <summary>
		/// 内容模版
		/// </summary>
		public string ContentTemplate{set;get;}

		/// <summary>
		/// 移动端列表
		/// </summary>
		public string MobileListTemplate{set;get;}

		/// <summary>
		/// 移动端内容
		/// </summary>
		public string MobileContentTemplate{set;get;}

        public string WebLink { set; get; }
        public int SingleNews { set; get; }
        /// <summary>
        /// 栏目是否显示在前台
        /// </summary>
        public int ShowSite { set; get; }
        /// <summary>
        /// 栏目是否显示在后台
        /// </summary>
        public int ShowSys { set; get; }
        /// <summary>
        /// 栏目下新闻，最多30条
        /// </summary>
        public virtual SiteNew[] Site_newsList{set;get;}
        //父级栏目
        public virtual SiteColumn ParentColumnInfo { set; get; }
        /// <summary>
        /// 子集栏目，只显示后台栏目
        /// </summary>
        public virtual SiteColumn[] ChildrenColumnList { set; get; }

    }
}
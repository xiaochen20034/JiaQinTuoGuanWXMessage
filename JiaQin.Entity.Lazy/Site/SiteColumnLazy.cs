using System;
using System.Collections.Generic;
using System.Text;
using JiaQin.Entity;

namespace JiaQin.Entity.Lazy
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteColumnLazy:SiteColumn
    {

        public Func<int, SiteNew[]> SiteNewsListLazy = null;
        public override SiteNew[] Site_newsList
        {
            get
            {
                if (base.Site_newsList == null && this.SiteNewsListLazy != null)
                {
                    return this.SiteNewsListLazy(base.ID);
                }
                return base.Site_newsList;
            }
            set
            {
                base.Site_newsList = value;
            }

        }
        public Func<int, SiteColumn> ParentColumnInfoLazy;
        public override SiteColumn ParentColumnInfo
        {
            get
            {
                if (base.ParentColumn > 0 && base.ParentColumnInfo == null && ParentColumnInfoLazy != null)
                {
                    return ParentColumnInfoLazy(base.ParentColumn);
                }
                return base.ParentColumnInfo;
            }
            set
            {
                base.ParentColumnInfo = value;
            }
        }
        public Func<int, SiteColumn[]> ChildrenColumnListLazy;
        public override SiteColumn[] ChildrenColumnList
        {
            get
            {
                if (base.ChildrenColumnList == null && ChildrenColumnListLazy != null)
                {
                    return ChildrenColumnListLazy(base.ID);
                }
                return base.ChildrenColumnList;
            }
            set
            {
                base.ChildrenColumnList = value;
            }
        }
    }
}
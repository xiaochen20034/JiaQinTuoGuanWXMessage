using System;
using System.Collections.Generic;
using System.Text;
using JiaQin.Entity;
namespace JiaQin.Entity.Lazy
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteNewLazy:SiteNew
    {
        public Func<int, SiteColumn> SiteColumnInfoLazy = null;
        public override SiteColumn SiteColumnInfo
        {
            get
            {
                if (base.SiteColumnInfo == null && this.SiteColumnInfoLazy != null)
                {
                    return this.SiteColumnInfoLazy(base.SiteColumnId);
                }
                return base.SiteColumnInfo;
            }
            set
            {
                base.SiteColumnInfo = value;
            }
        }
        public Func<int, SysUser> UserInfoLazy = null;
        public override SysUser UserInfo
        {
            get
            {
                if (base.UserInfo == null && this.UserInfoLazy != null)
                {
                    return this.UserInfoLazy(base.UserId);
                }
                return base.UserInfo;
            }
            set
            {
                base.UserInfo = value;
            }
        }

    }
}
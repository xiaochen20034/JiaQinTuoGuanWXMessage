using System;
using System.Collections.Generic;
using System.Text;
using JiaQin.Entity;
namespace JiaQin.Entity.Lazy
{
    /// <summary>
    /// 
    /// </summary>
    public class TeacherLazy:Teacher
    {

        public Func<int, SysUser> SysUserInfoLazy = null;
        public override SysUser SysUserInfo
        {
              get{
                  if (base.SysUserInfo == null && this.SysUserInfoLazy != null)
                    {
                        return this.SysUserInfoLazy(base.UserId);
                    }
                  return base.SysUserInfo;
              }
              set
              {
                  base.SysUserInfo = value;
              }
        }
       public Func<int, School> SchoolInfoLazy = null;
		public override School SchoolInfo{
              get{
                     if (base.SchoolInfo == null && this.SchoolInfoLazy!= null)
                    {
                        return this.SchoolInfoLazy(base.SchoolId);
                    }
                    return base.SchoolInfo;
              }
              set
              {
                   base.SchoolInfo= value;
              }
        }
        public Func<int, Tag[]> TagListLazy;
        public override Tag[] TagList
        {
            get
            {
                if (base.TagList==null && TagListLazy!=null)
                {
                    return TagListLazy(base.Id);
                }
                return base.TagList;
            }
            set
            {
                base.TagList = value;
            }
        }

    }
}
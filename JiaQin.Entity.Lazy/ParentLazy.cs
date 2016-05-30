using System;
using System.Collections.Generic;
using System.Text;
using JiaQin.Entity;
namespace JiaQin.Entity.Lazy
{
    /// <summary>
    /// 学生家长
    /// </summary>
    public class ParentLazy:Parent
    {
       public Func<int, Student> StudentInfoLazy = null;
		public override Student StudentInfo{
              get{
                     if (base.StudentInfo == null && this.StudentInfoLazy!= null)
                    {
                        return this.StudentInfoLazy(base.StudentID);
                    }
                    return base.StudentInfo;
              }
              set
              {
                   base.StudentInfo= value;
              }
        }
       public Func<int, VipUser> VipUserInfoLazy = null;
		public override VipUser VipUserInfo{
              get{
                     if (base.VipUserInfo == null && this.VipUserInfoLazy!= null)
                    {
                        return this.VipUserInfoLazy(base.VipUserID);
                    }
                    return base.VipUserInfo;
              }
              set
              {
                   base.VipUserInfo= value;
              }
        }

        public Func<int, VipUser> StudentVipUserInfoLazy = null;
        public override VipUser StudentVipUserInfo
        {
              get{
                  if (base.StudentVipUserInfo == null && this.StudentVipUserInfoLazy != null)
                    {
                        return this.StudentVipUserInfoLazy(base.StuVipUserId);
                    }
                  return base.StudentVipUserInfo;
              }
              set
              {
                  base.StudentVipUserInfo = value;
              }
        }
    }
}
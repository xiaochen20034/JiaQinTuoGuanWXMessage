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
       public Func<int, VipUser> VipUserInfoLazy = null;
		public override VipUser VipUserInfo{
              get{
                     if (base.VipUserInfo == null && this.VipUserInfoLazy!= null)
                    {
                        return this.VipUserInfoLazy(base.VipUserId);
                    }
                    return base.VipUserInfo;
              }
              set
              {
                   base.VipUserInfo= value;
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
    }
}
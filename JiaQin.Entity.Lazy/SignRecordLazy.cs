using System;
using System.Collections.Generic;
using System.Text;
using JiaQin.Entity;
namespace JiaQin.Entity.Lazy
{
    /// <summary>
    /// 项目签到发消息记录
    /// </summary>
    public class SignRecordLazy:SignRecord
    {
       public Func<int, SignProject> SignProjectInfoLazy = null;
		public override SignProject SignProjectInfo{
              get{
                     if (base.SignProjectInfo == null && this.SignProjectInfoLazy!= null)
                    {
                        return this.SignProjectInfoLazy(base.SignProjectId);
                    }
                    return base.SignProjectInfo;
              }
              set
              {
                   base.SignProjectInfo= value;
              }
        }
       public Func<int, Student> StudentInfoLazy = null;
		public override Student StudentInfo{
              get{
                     if (base.StudentInfo == null && this.StudentInfoLazy!= null)
                    {
                        return this.StudentInfoLazy(base.StuId);
                    }
                    return base.StudentInfo;
              }
              set
              {
                   base.StudentInfo= value;
              }
        }

        public Func<int, SysUser> TeacherUserInfoLazy = null;
        public override SysUser TeacherUserInfo
       {
              get{
                  if (base.TeacherUserInfo == null && this.TeacherUserInfoLazy != null)
                    {
                        return this.TeacherUserInfoLazy(base.TeaUserId);
                    }
                  return base.TeacherUserInfo;
              }
              set
              {
                  base.TeacherUserInfo = value;
              }
        }
    }
}
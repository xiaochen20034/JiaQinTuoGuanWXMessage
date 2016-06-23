using System;
using System.Collections.Generic;
using System.Text;
using JiaQin.Entity;
namespace JiaQin.Entity.Lazy
{
    /// <summary>
    /// 学生，基础信息来自user
    /// </summary>
    public class StudentLazy:Student
    {
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
        public Func<int, Parent> ParentInfoLazy = null;   
        public override Parent ParentInfo
        {
            get
            {
                if (base.ParentInfo == null && this.ParentInfoLazy != null)
                {
                    return this.ParentInfoLazy(base.ID);
                }
                return base.ParentInfo;
            }
            set
            {
                base.ParentInfo = value;
            }
        }
        public Func<int, Parent[]> ParentListLazy = null;   
		public override Parent[] ParentList{
               get{
                     if (base.ParentList == null && this.ParentListLazy != null)
                    {
                          return  this.ParentListLazy(base.ID);
                    }
                    return base.ParentList ;
              }
              set
              {
                   base.ParentList = value;
              }
    }
        public Func<int, VipUser> VipUserInfoLazy = null;
        public override VipUser VipUserInfo
        {
               get{
                   if (base.VipUserInfo == null && this.VipUserInfoLazy != null)
                    {
                        return this.VipUserInfoLazy(base.VipUserID);
                    }
                   return base.VipUserInfo;
              }
              set
              {
                  base.VipUserInfo = value;
              }
    }
        public Func<int, SignRecord[]> SignRecordListLazy = null;   
		public override SignRecord[] SignRecordList{
               get{
                     if (base.SignRecordList == null && this.SignRecordListLazy != null)
                    {
                          return  this.SignRecordListLazy(base.ID);
                    }
                    return base.SignRecordList ;
              }
              set
              {
                   base.SignRecordList = value;
              }

    }


        public Func<int, StudentTag[]> StudentTagListLazy;
        public override StudentTag[] StudentTagList
        {
            get
            {
                if (base.StudentTagList == null && StudentTagListLazy != null)
                {
                    return StudentTagListLazy(base.ID);
                }
                return base.StudentTagList;
            }
            set
            {
                base.StudentTagList = value;
            }
        }
    }
}
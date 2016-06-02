using System;
using System.Collections.Generic;
using System.Text;
using JiaQin.Entity;
namespace JiaQin.Entity.Lazy
{
    /// <summary>
    /// 
    /// </summary>
    public class TagLazy:Tag
    {
       public Func<int, School> SchoolInfoLazy = null;
		public override School SchoolInfo{
              get{
                     if (base.SchoolId!=null && base.SchoolInfo == null && this.SchoolInfoLazy!= null)
                    {
                        return this.SchoolInfoLazy(base.SchoolId.Value);
                    }
                    return base.SchoolInfo;
              }
              set
              {
                   base.SchoolInfo= value;
              }
        }

        public Func<int, Student[]> StudentListLazy = null;
        public override Student[] StudentList
        {
            get
            {
                if (base.StudentList == null && this.StudentListLazy != null)
                {
                    return this.StudentListLazy(base.Id);
                }
                return base.StudentList;
            }
            set
            {
                base.StudentList = value;
            }
        }


        public Func<int, Teacher[]> TeacherListLazy = null;
        public override Teacher[] TeacherList
        {
            get
            {
                if (base.TeacherList == null && this.TeacherListLazy != null)
                {
                    return this.TeacherListLazy(base.Id);
                }
                return base.TeacherList;
            }
            set
            {
                base.TeacherList = value;
            }
        }

        public Func<int, int> TeacherCountLazy;
        public override int TeacherCount
        {
            get
            {
                if (base.TeacherCount==0 && TeacherCountLazy!=null)
                {
                    return TeacherCountLazy(base.Id);
                }
                return base.TeacherCount;
            }
            set
            {
                base.TeacherCount = value;
            }
        }
        public Func<int, int> StudentCountLazy;
        public override int StudentCount
        {
            get
            {
                if (base.StudentCount == 0 && StudentCountLazy != null)
                {
                    return StudentCountLazy(base.Id);
                }
                return base.StudentCount;
            }
            set
            {
                base.StudentCount = value;
            }
        }

    }
}
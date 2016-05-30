using System;
using System.Collections.Generic;
using System.Text;
using JiaQin.Entity;
namespace JiaQin.Entity.Lazy
{
    /// <summary>
    /// 
    /// </summary>
    public class SchoolLazy:School
    {
        public Func<int, Student[]> StudentListLazy = null;   
		public override Student[] StudentList{
               get{
                     if (base.StudentList == null && this.StudentListLazy != null)
                    {
                          return  this.StudentListLazy(base.ID);
                    }
                    return base.StudentList ;
              }
              set
              {
                   base.StudentList = value;
              }
    }
        public Func<int, Teacher[]> TeacherListLazy = null;   
		public override Teacher[] TeacherList{
               get{
                     if (base.TeacherList == null && this.TeacherListLazy != null)
                    {
                          return  this.TeacherListLazy(base.ID);
                    }
                    return base.TeacherList ;
              }
              set
              {
                   base.TeacherList = value;
              }
    }
        public Func<int, Tag[]> TagListLazy = null;   
		public override Tag[] TagList{
               get{
                     if (base.TagList == null && this.TagListLazy != null)
                    {
                          return  this.TagListLazy(base.ID);
                    }
                    return base.TagList ;
              }
              set
              {
                   base.TagList = value;
              }
    }
    }
}
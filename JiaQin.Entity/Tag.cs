using System;
using System.Collections.Generic;
using System.Text;
namespace JiaQin.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class Tag
    {
		/// <summary>
		/// 
		/// </summary>
		public int Id{set;get;}
		/// <summary>
		/// 标签名称
		/// </summary>
		public string Name{set;get;}
		/// <summary>
		/// 学校ID
		/// </summary>
		public int? SchoolId{set;get;}
            public virtual School SchoolInfo{set;get;}
        /// <summary>
        /// 使用标签的学生列表
        /// </summary>
            public virtual Student[] StudentList { set; get; }
        /// <summary>
        /// 
        /// </summary>
            public virtual Teacher[] TeacherList { set; get; }
        /// <summary>
        /// 学生使用人数
        /// </summary>
            public virtual int StudentCount { set; get; }
        /// <summary>
        /// 教师使用人数
        /// </summary>
            public virtual int TeacherCount { set; get; }

            public override bool Equals(object obj)
            {
                if (obj==null || !(obj is Tag))
                {
                    return false;
                }
                Tag t = obj as Tag;

                return t.Id==this.Id;
            }
    }
}
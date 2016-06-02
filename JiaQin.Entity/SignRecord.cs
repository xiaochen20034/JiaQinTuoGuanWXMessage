using System;
using System.Collections.Generic;
using System.Text;
namespace JiaQin.Entity
{
    /// <summary>
    /// 项目签到发消息记录
    /// </summary>
    public class SignRecord
    {
		/// <summary>
		/// 
		/// </summary>
		public int Id{set;get;}
		/// <summary>
		/// 项目ID
		/// </summary>
		public int SignProjectId{set;get;}
		/// <summary>
		/// 学生用户的用户ID
		/// </summary>
		public int StuVipUserId{set;get;}
		/// <summary>
		/// 学生ID
		/// </summary>
		public int StuId{set;get;}
		/// <summary>
		/// 老师签到时间
		/// </summary>
		public DateTime SignDate{set;get;}
		/// <summary>
		/// 微信消息送达时间
		/// </summary>
		public DateTime ReadDate{set;get;}
		/// <summary>
		/// 教师用户ID
		/// </summary>
		public int TeaUserId{set;get;}
		/// <summary>
		/// 教师ID
		/// </summary>
		public int TeaId{set;get;}

        public string Remark { set; get; }
            public virtual SignProject SignProjectInfo{set;get;}
            public virtual Student StudentInfo{set;get;}

            public virtual SysUser TeacherUserInfo { set; get; }
    }
}
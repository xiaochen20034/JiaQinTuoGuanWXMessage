using System;
using System.Collections.Generic;
using System.Text;

namespace JiaQin.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class VipUser
    {

		/// <summary>
		/// 
		/// </summary>
		public int Id{set;get;}

		/// <summary>
		/// 
		/// </summary>
		public string Code{set;get;}

		/// <summary>
		/// 
		/// </summary>
		public string UserName{set;get;}

		/// <summary>
		/// 
		/// </summary>
		public string PassWord{set;get;}

		/// <summary>
		/// 
		/// </summary>
		public string Name{set;get;}

		/// <summary>
		/// 
		/// </summary>
		public string Gender{set;get;}
        public int Age { get {
            if (BirthDate==null)
            {
                return 0;
            }
            DateTime now = DateTime.Now;
            DateTime birthDate = BirthDate.Value;
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) age--;
            return age;

        } }
        public DateTime? BirthDate { set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string Phone{set;get;}

        private string photo;
		/// <summary>
		/// 
		/// </summary>
        public string Photo { set { this.photo = value; } get {
            if (string.IsNullOrEmpty(photo))
            {
                return Headimgurl;
            }
            return this.photo;
        } }
        /// <summary>
        /// 用户在当前公众号的唯一标识
        /// </summary>
        public string OpenId { set; get; }

        /// <summary>
        /// 用户的微信全平台唯一标识
        /// </summary>
        public string Unionid { set; get; }

        /// <summary>
        /// 用户的微信头像地址（本地地址）
        /// </summary>
        public string Headimgurl { set; get; }

        /// <summary>
        /// 用户的微信昵称
        /// </summary>
        public string Nickname { set; get; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateDate{set;get;}
        /// <summary>
        /// 上次更新时间
        /// </summary>
        public DateTime? BindDate { set; get; }
        /// <summary>
        /// 上次更新时的Agent
        /// </summary>
        public string BindAgent { set; get; }


    }
}
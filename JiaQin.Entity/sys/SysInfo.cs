using System;
using System.Collections.Generic;
using System.Text;

namespace JiaQin.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class SysInfo
    {

		/// <summary>
		/// 
		/// </summary>
		public int Id{set;get;}

		/// <summary>
		/// 名称
		/// </summary>
		public string Name{set;get;}

		/// <summary>
		/// 微信的AppId
		/// </summary>
		public string WeiXinAppID{set;get;}

		/// <summary>
		/// WeiXinAppSecret
		/// </summary>
		public string WeiXinAppSecret{set;get;}

		/// <summary>
		/// WeiXinAppToken
		/// </summary>
		public string WeiXinAppToken{set;get;}

		/// <summary>
		/// WeiXinEncodingAESKey
		/// </summary>
		public string WeiXinEncodingAESKey{set;get;}

		/// <summary>
		/// 微信支付商户ID
		/// </summary>
		public string WeiXinMchID{set;get;}

		/// <summary>
		/// 微信商户的API安全密匙
		/// </summary>
		public string WeiXinMchKey{set;get;}

		/// <summary>
		/// 
		/// </summary>
		public string WxUserName{set;get;}

    }
}
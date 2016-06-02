using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhyj.Common;
using JiaQin.Entity;
using JiaQin.Data;
using System.IO;
using System.Web;
using System.Threading;
using Zhyj.Tencent.WeiXin;
using Zhyj.Tencent.WeiXin.entity;
namespace JiaQin.Services
{
    class VipUserServ : AppBase
    {
  


        
        public void Logon() {

            VipUser u = Request.PackageRequest<VipUser>();
            Base64Encoding coding= new Base64Encoding();
            VipUserData userD = dataExecutorImp.GetInstance<VipUserData>();
            VipUser u2 = userD.Info(u.UserName, coding.Encode(u.PassWord));
            if (u2==null)
	        {
                TemplateData[JsonKeyValue.res]=JsonKeyValue.fail;
                TemplateData[JsonKeyValue.tip]="用户名密码错误";
                return;
	        }
            //TODO:缺少，验证已登录用户，重复在另一台设备登陆的流程。解决此问题，加入手机验证码，可能会解决
            try
            {
                userD.setVipUserWxOpenIdNull(u2.Id);

                string openid = Request["OpenId"];
                Zhyj.Tencent.WeiXin.User user = new Zhyj.Tencent.WeiXin.User();
                Zhyj.Tencent.WeiXin.entity.User wxUserInfo = user.UserInfo(openid, WxConfig);
                u2.OpenId = openid;
                u2.Headimgurl = wxUserInfo.headimgurl;
                u2.Unionid = wxUserInfo.unionid;
                u2.Nickname = wxUserInfo.nickname;
                u2.BindDate = DateTime.Now;
                u2.BindAgent = Request.UserAgent;
                userD.setVipUserWxInfo(u2);
            }
            catch (Exception ea) {
                Zhyj.ZLogger4Web.ZLogger.LogApp("登陆设置微信信息失败");
                Zhyj.ZLogger4Web.ZLogger.LogException(ea);
            }
            ICached[u2.UserName] = u2;
            Response.Cookies.Add(new HttpCookie(aspx_vipUsername, coding.Encode(u2.UserName,true,true)));
            TemplateData[JsonKeyValue.res] = JsonKeyValue.ok;
            TemplateData[JsonKeyValue.tip] = "登录成功";


            
        }


        public void Logout() {
            if (CurrentVipUser!=null)
            {
                VipUser u2 = CurrentVipUser;
                VipUserData userD = dataExecutorImp.GetInstance<VipUserData>();
                userD.setVipUserWxOpenIdNull(u2.Id);

                ICached.Remove(CurrentVipUser.UserName);
                //ICached.Remove(CurrentVipUser.Phone);
                string usercachekey = CurrentVipUser.UserName + "_Cached";
                if (ICached.Exists(usercachekey))
                {
                    ICached.Remove(usercachekey);
                }
            }
           

            Request.Cookies.Clear();
            Response.Cookies.Clear();
            TemplateData[JsonKeyValue.res] = JsonKeyValue.ok;
            TemplateData[JsonKeyValue.tip] = "注销完成";

        }
      




        public void SetVipUserInfo() {
            VipUser vipUser = Request.PackageRequest<VipUser>();
            VipUserData userD = dataExecutorImp.GetInstance<VipUserData>();
            vipUser.Id = CurrentVipUser.Id;
            userD.SetBasicInfo(vipUser);
            CurrentVipUser = userD.Info(CurrentVipUser.Id);

            ICached[CurrentVipUser.UserName] = CurrentVipUser;
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "信息保存完成";

        }
    }
}

using JiaQin.Data;
using JiaQin.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Zhyj.ZLogger4Web;

namespace JiaQin.Services
{
    public class SiteWebPage : AppBase
    {
        public void NoPage2() { }

        public string NoPage()
        {
            string actionPath = Request.Path;
            int index = actionPath.LastIndexOf('.');
            actionPath = actionPath.Substring(0, index);
            if (Zhyj.AppHandler.UtilConfig.IsMobile)
            {
                //   actionPath = "mobile/" + actionPath;
            }

            //if (!Zhyj.AppHandler.UtilConfig.IsMobile)
            //{
            //    Response.Redirect("/sys/logon.aspx");
            //    return null;
            //}
            if (!IsMicromessenger && !AllowBroswer)
            {
                
                return null;
            }

            //site网站栏目
            #region 网站栏目访问
            if (Request.Path.StartsWith("/site/", StringComparison.CurrentCultureIgnoreCase) ||
                Request.Path.StartsWith("/me/", StringComparison.CurrentCultureIgnoreCase))
            {
                return ParseSitePage();
            }
            #endregion
            string path = Server.MapPath("/" + site + "/" +
                                                            Domain.TemplateSite + "/page/" + actionPath + ".cshtml");
            ZLogger.logAppContent("预解析页面：" + path);
            if (File.Exists(path))
            {
                return "parse:" + path;
            }

            return "parse:" + NoFoundPage;
        }
        /// <summary>
        /// 网站栏目访问的时候
        /// 访问路径为
        /// /site/{columnCode}/{dataId}.aspx
        /// /site/{columnCode}.aspx
        /// </summary>
        /// <returns></returns>
        private string ParseSitePage()
        {
            string actionPath = Request.Path;
            int index = actionPath.LastIndexOf('.');
            actionPath = actionPath.Substring(0, index);

            string[] request_path = Request.Path.Substring(0, Request.Path.Length - 5).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string columnCode = request_path[1];
            string dataId = request_path.Length == 3 ? request_path[2] : "-1";
            int dataIdInt = 0;
            try
            {
                dataIdInt = dataId.StartsWith("X", StringComparison.CurrentCultureIgnoreCase) ?
                            Convert.ToInt32(dataId.Substring(1), 16) - 10 :
                            Convert.ToInt32(dataId);
            }
            catch (Exception ea)
            {
                Zhyj.ZLogger4Web.ZLogger.LogException(ea);
                //return "parse:" + NoFoundPage;
            }
            bool IsData = dataIdInt > 0;

            TemplateData["dataId"] = dataIdInt;
            TemplateData["IsData"] = IsData;

            SiteColumnData columnData = dataExecutorImp.GetInstance<SiteColumnData>();
            SiteColumn column = columnData.getSiteColumnInfoByCode(columnCode);
            if (column == null)
            {
                return "parse:" + NoFoundPage;
            }
            if (Zhyj.AppHandler.UtilConfig.IsMobile)
            {
                actionPath = column.SingleNews == 1 || IsData
                    ? !string.IsNullOrEmpty(column.MobileContentTemplate)
                        ? column.MobileContentTemplate
                        : column.MobileListTemplate
                    : !string.IsNullOrEmpty(column.MobileListTemplate)
                        ? column.MobileListTemplate
                        : column.MobileContentTemplate;
            }
            else
            {
                actionPath = column.SingleNews == 1 || IsData
                    ? !string.IsNullOrEmpty(column.ContentTemplate)//内容
                        ? column.ContentTemplate
                        : column.ListTemplate
                    : !string.IsNullOrEmpty(column.ListTemplate)//列表
                        ? column.ListTemplate
                        : column.ContentTemplate;
            }

            string pagepath = Server.MapPath("/" + site + "/" +
                                                            Domain.TemplateSite + "/page/" + actionPath);

            TemplateData["columnInfo"] = column;
            if (File.Exists(pagepath))
            {
                return "parse:" + pagepath;
            }
            return "parse:" + NoFoundPage;
        }
        public string NoFoundPage
        {
            get
            {
                return Server.MapPath("/" + site + "/" +
                                                                Domain.TemplateSite + "/page/404.cshtml");
            }
        }
        public string WxNoPage()
        {
            string path = Request.Path.Substring(7);
            Zhyj.Tencent.WeiXin.Menu menuF = new Zhyj.Tencent.WeiXin.Menu();
            //string url = menuF.WeixinMenuOpenUrl("http://ihuodong.sxxxt.cn/pay/TestPayTwo.aspx", null);//
            string host = "http://" + Request.Url.Authority;
            string url = menuF.WeixinMenuOpenUrl(host + "/wxuserurl/" + path, SystemInfo.WeiXinAppID);//

            Response.Cookies.Add(new HttpCookie("params", Request.Url.Query.Length > 0 ? Request.Url.Query.Substring(1) : ""));
            Response.Redirect(url);
            return null;
        }
        public string WxNoPageWithOpenId()
        {

            string code = Request["code"];
            if (string.IsNullOrEmpty(code))
            {
                ZLogger.logAppContent("获取请求的code失败");
                TemplateData["error"] = "请求不是来自微信";
                return "parse:" + NoFoundPage;
            }
            string host = "http://" + Request.Url.Authority + "/";
            HttpCookie paramsCook = Request.Cookies["params"];
            if (paramsCook == null)
            {
                paramsCook = new HttpCookie("params", string.Empty);
                Response.Cookies.Add(paramsCook);
            }
            #region 获取上一步的参数进行封装
            Dictionary<string, string> paramDic = new Dictionary<string, string>();

            string[] cookies = paramsCook.Value.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in cookies)
            {
                string[] temp = item.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length == 0)
                {
                    continue;
                }
                if (temp.Length == 1)
                {
                    paramDic[temp[0]] = string.Empty;
                }
                else
                {
                    paramDic[temp[0]] = temp[1];
                }

            }


            #endregion
            int needSubscribe = 1;

            Zhyj.Tencent.WeiXin.Config WxConfig = new Zhyj.Tencent.WeiXin.Config();
            Zhyj.Tencent.WeiXin.Token token = new Zhyj.Tencent.WeiXin.Token(WxConfig);
            Zhyj.Tencent.WeiXin.entity.OauthAccessToken oauthAccessToken = token.GetOauthAccessToken(code);

            Zhyj.Tencent.WeiXin.User user = new Zhyj.Tencent.WeiXin.User();
            Zhyj.Tencent.WeiXin.entity.User wxUserInfo = user.UserInfo(oauthAccessToken.openid, WxConfig);

            if (wxUserInfo == null)
            {
                ZLogger.logAppContent("根据微信code获取用户信息失败：" + code);
                TemplateData["error"] = "请求不是来自微信";
                return "parse:" + NoFoundPage;
            }
            //验证是否关注
            if (paramDic.ContainsKey("subscribe") && int.TryParse(paramDic["subscribe"].ToString(), out needSubscribe) && needSubscribe > 0)
            {
                //未绑定微信,跳转到登陆页面进行登陆
                Response.Redirect(host + "/wxurl/login.aspx?" + paramsCook.Value + "&wxopenid=" + wxUserInfo.openid);
                return null;
            }


            //未关注公众号，引导到关注页面
            if (wxUserInfo.subscribe == 0 && needSubscribe != 0)
            {
                ZLogger.LogApp("未关注公众号，引导到关注页面");
                //Response.Redirect("未关注——————");
                Response.Write("未关注——————");
                return null;
            }

            HttpCookie openIdCookie = new HttpCookie("wxopenid", wxUserInfo.openid);
            Response.Cookies.Add(openIdCookie);

            VipUserData vipUserData = dataExecutorImp.GetInstance<VipUserData>();
            //当前用户在微信端使用，进行更新微信信息


            VipUser vipUserInfo = vipUserData.getVipUserInfoByOpenId(wxUserInfo.openid);

            if (vipUserInfo != null)
            {//已绑定了微信
                vipUserInfo.Headimgurl = wxUserInfo.headimgurl;
                vipUserInfo.Unionid = wxUserInfo.unionid;
                vipUserInfo.Nickname = wxUserInfo.nickname;
                vipUserInfo.BindDate = DateTime.Now;
                vipUserInfo.BindAgent = Request.UserAgent;
                vipUserData.setVipUserWxInfo(vipUserInfo);
            }
            else if (paramDic.ContainsKey("subscribe") && int.TryParse(paramDic["subscribe"].ToString(), out needSubscribe) && needSubscribe > 0)
            {
                //未绑定微信,跳转到登陆页面进行登陆
                Response.Redirect(host + "/wxurl/login.aspx?" + paramsCook.Value + "&wxopenid=" + wxUserInfo.openid);
                return null;
            }


            //TemplateData["wxUserInfo"] = wxUserInfo;
            //TemplateData["vipUserInfo"] = vipUserInfo;

            string actionPath = Request.Path.Substring(10);
            int index = actionPath.LastIndexOf('.');
            actionPath = actionPath.Substring(0, index);
            // actionPath = "mobile/" + actionPath;           
            string path = Server.MapPath("/" + site + "/" +
                                                            Domain.TemplateSite + "/page/" + actionPath + ".cshtml");

            //设置登陆参数

            //引导浏览器跳转到真实路径
            Response.Redirect(host + Request.Path.Substring(11) + "?" + paramsCook.Value + "&wxopenid=" + wxUserInfo.openid);
            return null;

            /*
            if (File.Exists(path))
            {
               
                Response.Redirect(host + Request.Path.Substring(11) + "?" + paramsCook.Value + "&wxopenid=" + wxUserInfo.openid);
                return null;
            }

            return "parse:" + NoFoundPage;
            */


        }

        public string RequestUrl
        {
            get
            {
                return Request.Url.AbsoluteUri;
            }
        }
        public override bool regist(Zhyj.AderTemplates.TemplateManager templageManager)
        {
            base.regist(templageManager);

            columnD = dataExecutorImp.GetInstance<SiteColumnData>();
            newsD = dataExecutorImp.GetInstance<SiteNewsData>();

            templageManager.Functions.Add("SiteColumnInfo", new Zhyj.AderTemplates.TemplateFunction(SiteColumnInfo));

            templageManager.Functions.Add("SiteColumnParent", new Zhyj.AderTemplates.TemplateFunction(SiteColumnParent));
            templageManager.Functions.Add("SiteColumnFullParent", new Zhyj.AderTemplates.TemplateFunction(SiteColumnFullParent));

            templageManager.Functions.Add("SiteColumnDataList", new Zhyj.AderTemplates.TemplateFunction(SiteColumnDataList));
            templageManager.Functions.Add("SiteColumnDataListMore", new Zhyj.AderTemplates.TemplateFunction(SiteColumnDataListMore));

            templageManager.Functions.Add("ColumnDataById", new Zhyj.AderTemplates.TemplateFunction(ColumnDataById));
            templageManager.Functions.Add("ColumnDataFirst", new Zhyj.AderTemplates.TemplateFunction(ColumnDataFirst));
            templageManager.Functions.Add("RemoveStyle", new Zhyj.AderTemplates.TemplateFunction(RemoveStyle));
            if (IsMicromessenger)
            {
                templageManager.variables["vipuser"] = CurrentVipUser;
                templageManager.variables["WXOpenId"] = WXOpenId;
                templageManager.variables["RequestUrl"] = RequestUrl;


                templageManager.variables["IsMicromessenger"] = IsMicromessenger;
                templageManager.variables["AllowBroswer"] = AllowBroswer;
                templageManager.Functions.Add("ParentInfoOfCurrentVipUser", new Zhyj.AderTemplates.TemplateFunction(ParentInfoOfCurrentVipUser));
            }
           

            return true;
        }

        public object ParentInfoOfCurrentVipUser(object[] args)
        {
            ParentData parentData=dataExecutorImp.GetInstance<ParentData>();

            Parent parentInfo= parentData.getParentInfoByVipUserId(CurrentVipUser.Id);
            return parentInfo;
            
        }


        SiteColumnData columnD = null;
        SiteNewsData newsD = null;

        public object RemoveStyle(object[] args)
        {
            if (args.Length < 1 || args[0] == null || string.IsNullOrEmpty(args[0].ToString()))
            {
                return string.Empty;
            }
            return System.Text.RegularExpressions.Regex.Replace(args[0].ToString(), @"style\s*=(['""\s]?)[^'""]*?\1", "", System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
        }

        private SiteColumn ColumnInfo(object obj)
        {
            int columnId = -1;
            SiteColumn column = null;
            if (!int.TryParse(obj.ToString(), out columnId))
            {
                column = columnD.getSiteColumnInfoByCode(obj.ToString());
            }
            else
            {
                column = columnD.getSiteColumnInfoById(columnId);
            }
            if (column == null)
            {
                throw new ArgumentException("参数columnid或者columncode有误，无法获取栏目信息：" + obj);
            }
            return column;
        }

        public object SiteColumnInfo(object[] args)
        {

            if (args == null || args.Length != 1)
            {
                throw new ArgumentException("SiteColumnInfo需要提供一个整数参数");
            }
            SiteColumn info = ColumnInfo(args[0]);
            return info;
        }
        public object SiteColumnParent(object[] args)
        {

            if (args == null || args.Length != 1)
            {
                throw new ArgumentException("SiteColumnParent需要提供一个整数参数");
            }
            SiteColumn column = ColumnInfo(args[0]);
            return columnD.getSiteColumnInfoById(column.ParentColumn);
        }
        private void FullParent(SiteColumn col, List<SiteColumn> list)
        {

            if (col.ParentColumn <= 0)
            {
                return;
            }
            SiteColumn c = columnD.getSiteColumnInfoById(col.ParentColumn);

            if (c.ParentColumn > 0)
            {
                list.Insert(0, c);
                FullParent(c, list);
            }
            else
            {
                list.Insert(0, c);
            }


        }
        public object SiteColumnFullParent(object[] args)
        {
            if (args.Length < 1)
            {
                throw new Exception("SiteColumnFullParent 函数需要传入的参数：code=栏目Code");
            }


            List<SiteColumn> list = new List<SiteColumn>();
            SiteColumn column = ColumnInfo(args[0]);
            list.Insert(0, column);
            FullParent(column, list);
            return list;
        }
        public object SiteColumnDataList(object[] args)
        {

            if (args == null)
            {
                throw new ArgumentException("SiteColumnDataList需要提供一个整数参数");
            }
            SiteColumn column = ColumnInfo(args[0]);
            int? count = args.Length >= 2 ? Convert.ToInt32(args[1]) : new Nullable<int>();
            string orderby = args.Length >= 3 ? args[2].ToString() : null;
            string where = args.Length >= 4 ? args[3].ToString() : null;
            bool audit = true;
            bool.TryParse(args.Length >= 5 ? args[4].ToString() : "true", out audit);
            SiteNewsData newsData = dataExecutorImp.GetInstance<SiteNewsData>();
            return newsData.GetSiteColumnDataList(column.ID, count, orderby, where);
        }

        public object SiteColumnDataListMore(object[] args)
        {

            if (args == null)
            {
                throw new ArgumentException("SiteColumnDataList需要提供一个整数参数");
            }
            SiteColumn column = ColumnInfo(args[0]);

            int newsId = 0;
            newsId = args.Length >= 2 && int.TryParse(args[1] + "", out newsId) ? newsId : 0;
            int count = args.Length >= 3 ? Convert.ToInt32(args[2]) : 10;
            SiteNewsData newsData = dataExecutorImp.GetInstance<SiteNewsData>();
            return newsData.List(column.ID, count, newsId);
        }


        /// <summary>
        /// 根据栏目ID以及实体ID，获取数据
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public object ColumnDataById(object[] args)
        {
            if (args == null || args.Length != 2)
            {
                throw new ArgumentException("ColumnDataById 需要提供两个整数参数");
            }
            SiteColumn column = ColumnInfo(args[0]);
            int id = Convert.ToInt32(args[1]);
            return newsD.getSite_newsInfoById(id);
        }

        public object ColumnDataFirst(object[] args)
        {

            if (args == null)
            {
                throw new ArgumentException("ColumnDataFirst需要提供一个整数参数");
            }
            SiteColumn column = ColumnInfo(args[0]);
            string orderby = args.Length >= 2 ? args[1].ToString() : null;

            string where = args.Length >= 3 ? args[2].ToString() : null;
            SiteNew news = newsD.getSiteNewsInfoByColumnId(column.ID);

            return news;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiaQin.Entity;
using JiaQin.Data;
using Zhyj.Common;
using System.Text.RegularExpressions;
namespace JiaQin.Services
{
    public class SiteServ : AppBase
    {

        public void MainPage()
        {

            SiteColumnData siteColumnData = dataExecutorImp.GetInstance<SiteColumnData>();
            SiteColumn[] list = siteColumnData.List(0);
            TemplateData[listDataStr] = list;
        }

        public string ColumnDataPage() {
            string columnId = Request["columnId"];
            int columnIdInt = 0;
            if (!int.TryParse(columnId, out columnIdInt))
            {
                Response.Write("栏目不存在");
                return null;
            }
             SiteColumnData siteColumnData = dataExecutorImp.GetInstance<SiteColumnData>();
             SiteColumn column = siteColumnData.getSiteColumnInfoById(columnIdInt);
             string path = null;
             if (column.SingleNews == 1)
             {
                 path = "figure2.html";
                 SiteNewsData newsData=dataExecutorImp.GetInstance<SiteNewsData>();
                 SiteNew news= newsData.getSiteNewsInfoByColumnId(column.ID);
                 if (news==null)
                 {
                     news = new SiteNew() { 
                         Title="",
                          Content=""
                          
                     };
                 }
                 TemplateData["newsInfo"] = news;
             }
             else {
                 path = "figure.html";
                 SiteNewsData newsData = dataExecutorImp.GetInstance<SiteNewsData>();
                 SiteNew[]newsList = newsData.List(column.ID,PageSize,PageNum,out rowCount);
                 Page();
                 TemplateData["newsList"] = newsList;
                 if (!string.IsNullOrEmpty(Request["list"]))
                 {
                     path = "list.html";
                 }
             }
             TemplateData["columnInfo"] = column;

             return "parse:" + Server.MapPath("/sys/" + Domain.Sysinfo.Folder + "/page/网站Site/siteNew/" + path);
        }

        public string AddColumnDataPage() {
            string columnId = Request["columnId"];
            int columnIdInt = 0;
            if (!int.TryParse(columnId, out columnIdInt))
            {
                Response.Write("栏目不存在");
                return null;
            }
            SiteColumnData siteColumnData = dataExecutorImp.GetInstance<SiteColumnData>();
            SiteColumn column = siteColumnData.getSiteColumnInfoById(columnIdInt);
            string path = null;
            TemplateData["columnInfo"] = column;

            return "parse:" + Server.MapPath("/sys/" + Domain.Sysinfo.Folder + "/page/网站Site/siteNew/add.html");
        }

        // <summary>
        /// 正则图片路径
        /// </summary>
        /// <returns></returns>
        public static string GetImgUrl(string text)
        {
            StringBuilder str = new StringBuilder();
            string pat = @"<img\s+[^>]*\s*src\s*=\s*([']?)(?<url>\S+)'?[^>]*>";
            Regex r = new Regex(pat, RegexOptions.Compiled);

            Match m = r.Match(text.ToLower());
            //int matchCount = 0;
            if (m.Success)
            {
                Group g = m.Groups[2];
                str.Append(g);
                //m = m.NextMatch();
            }
            return str.Replace("\"", "").ToString();
        }


        public object PickupImgUrl(object[] args)
        {
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            MatchCollection matches = regImg.Matches(args[0] as string);
            List<string> lstImg = new List<string>();

            foreach (Match match in matches)
            {
                lstImg.Add(match.Groups["imgUrl"].Value);
            }

            return lstImg;
        }
        public void AddColumnDataEvent() {
            SiteNew news = Request.PackageRequest<SiteNew>();
            news.UserId = CurrentUser.Id;// CurrentVipUser.Id;
            news.PublishDate = DateTime.Now;
            news.Audit = 1;
            news.AuditDate = DateTime.Now;
            news.AuditVipUserId = CurrentUser.Id;
            news.BlogImg = GetImgUrl(news.Content);
            

            SiteNewsData newsData=dataExecutorImp.GetInstance<SiteNewsData>();
            newsData.Add(news);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.ok;
            TemplateData[JsonKeyValue.tip] = "添加完成";
        }

        public string UpdateColumnDataPage()
        {
            string dataId = Request["dataId"];
            int dataIdInt=Convert.ToInt32(dataId);

            SiteNewsData newsData = dataExecutorImp.GetInstance<SiteNewsData>();
            SiteNew news= newsData.getSite_newsInfoById(dataIdInt);

            if (news==null)
            {
                Response.Write("数据不存在："+dataId);
                return null;
            }
            SiteColumnData siteColumnData = dataExecutorImp.GetInstance<SiteColumnData>();
            SiteColumn column = siteColumnData.getSiteColumnInfoById(news.SiteColumnId);
         
            TemplateData["newsInfo"] = news;
            TemplateData["columnInfo"] = column;

            return "parse:" + Server.MapPath("/sys/" + Domain.Sysinfo.Folder + "/page/网站Site/siteNew/update.html");
        }
        public void UpdateColumnDataEvent()
        {
            SiteNew news = Request.PackageRequest<SiteNew>();
            news.UserId = CurrentUser.Id;
            news.PublishDate = DateTime.Now;
            news.Audit = 1;
            news.AuditDate = DateTime.Now;
            news.AuditVipUserId = CurrentUser.Id;
            news.BlogImg = GetImgUrl(news.Content);

            SiteNewsData newsData = dataExecutorImp.GetInstance<SiteNewsData>();
            newsData.Update(news);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.ok;
            TemplateData[JsonKeyValue.tip] = "编辑完成";
        }
        public void DeleteColumnDataEvent() {
            SiteNewsData newsData = dataExecutorImp.GetInstance<SiteNewsData>();
            newsData.Delete(this.ID);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "删除完成";
        }


        public void AddSingleColumnDataEvent() {

            string SiteColumnId = Request["SiteColumnId"];
            int SiteColumnIdInt = Convert.ToInt32(SiteColumnId);

            string title = Request["Title"];
            string content = Request["Content"];
            SiteNewsData newsData = dataExecutorImp.GetInstance<SiteNewsData>();
            SiteNew news = newsData.getSiteNewsInfoByColumnId(SiteColumnIdInt);

            if (news == null)
            {
                SiteNew newsAdd = Request.PackageRequest<SiteNew>();
                newsAdd.UserId = CurrentUser.Id;
                newsAdd.PublishDate = DateTime.Now;
                newsAdd.Audit = 1;
                newsAdd.AuditDate = DateTime.Now;
                newsAdd.AuditVipUserId = CurrentUser.Id;
                newsAdd.Title = title;
                newsAdd.Content = content;
                newsAdd.BlogImg = GetImgUrl(content);
                newsData.Add(newsAdd);
            }
            else {
                SiteNew newsUpdate = Request.PackageRequest<SiteNew>();
                newsUpdate.Id = news.Id;
                newsUpdate.UserId = CurrentUser.Id;
                newsUpdate.PublishDate = DateTime.Now;
                newsUpdate.Audit = 1;
                newsUpdate.AuditDate = DateTime.Now;
                newsUpdate.AuditVipUserId = CurrentUser.Id;
                newsUpdate.Title = title;
                newsUpdate.Content = content;
                newsUpdate.BlogImg = GetImgUrl(content);
                newsData.Update(newsUpdate);
            }

            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "编辑完成";

        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Services
{
    public class WebPage:AppBase
    {
        public void NoPage2() { }
        public string NoPage()
        {
            string actionPath = Request.Path;
            int index = actionPath.LastIndexOf('.');
            actionPath = actionPath.Substring(0, index);
            if (Zhyj.AppHandler.UtilConfig.IsMobile)
            {
                actionPath = "mobile/" + actionPath;
            }

            string path = null;
            int sysIndex;
            if ((sysIndex = Request.Path.IndexOf("/sys/")) != -1)
            {
                actionPath = Request.Path.Substring(sysIndex + 5, Request.Path.Length - sysIndex - 5 - 5);
                path = Server.MapPath("/" + sys + "/" + Domain.Sysinfo.FolderName + "/page/" + actionPath + ".html");
            }
            if (File.Exists(path))
            {
                return "parse:" + path;
            }
            
            return "parse:" + Server.MapPath("/" + Zhyj.AppHandler.UtilConfig.sys + "/" +
                                                            Domain.TemplateSite + "/page/404.html");
        }
    }
}

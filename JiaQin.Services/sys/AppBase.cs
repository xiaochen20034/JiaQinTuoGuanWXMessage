using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using Zhyj.ZLogger4Web;
using Zhyj.AppHandler.IHandler;
using Zhyj.Common;
using JiaQin.Data;
using JiaQin.Entity;

namespace JiaQin.Services
{
    public partial class AppBase : IDnsRoute, IVerficationState, ITemplateData, IRegistTemplate, ICacheData, IInit, IRoot,IAction
    {
        public AppBase() {
            Context = HttpContext.Current;
            Request = HttpContext.Current.Request;
            Response = HttpContext.Current.Response;
            Server = HttpContext.Current.Server;
            _host = Request.Url.Host.ToString();
            _templateData = new System.Collections.Hashtable(5);
            data = _templateData;
            
        }
        /// <summary>
        /// 当前Web请求   HttpRequest 
        /// </summary>
        public HttpContext Context = null;
        /// <summary>
        /// 当前Web请求   HttpRequest 
        /// </summary>
        public HttpRequest Request = null;
        /// <summary>
        /// 当前Web响应   HttpResponse
        /// </summary>
        public HttpResponse Response = null;
        /// <summary>
        /// 当前Web     HttpServerUtility
        /// </summary>
        public HttpServerUtility Server = null;
        public const string site = "site";
        public const string sys = "sys";
        /// <summary>
        /// 返回路径在site目录，或者sys目录
        /// </summary>
        public string Root { set; get; }
        /// <summary>
        /// 数据总行数
        /// </summary>
        public const string rowcountStr = "rowcount";
        /// <summary>
        /// 常量。json 的 listData  键
        /// </summary>
        public const string listDataStr = "list";
        /// <summary>
        ///常量。 json 的 data 单数据键
        /// </summary>
        public const string dataStr = "data";
        
        /// <summary>
        /// 常量。标识当前用户
        /// </summary>
        public const string currentUserCode = "currentUserCode";

        public const string aspx_username = "aspx_username";

        public SysInfo _systemInfo;
        /// <summary>
        /// 系统信息
        /// </summary>
        public SysInfo SystemInfo {
            get { 
                return _systemInfo;
            }
        }

        //public Zhyj.Tencent.WeiXin.Config _config = null;
        /// <summary>
        /// 系统信息
        /// </summary>
        //public Zhyj.Tencent.WeiXin.Config WxConfig
        //{
        //    get
        //    {
        //        return _config;
        //    }
        //}

        public string WxUrl(string url) {
            
            return Request.Url.Scheme+ "://"+Request.Url.Authority+"/"+url;
        }

        /// <summary>
        /// 数据访问实例获取
        /// </summary>
        public IDataExecutorImp dataExecutorImp = null;
        /// <summary>
        /// 全局缓存。整个应用程序，所有学校可访问的数据缓存
        /// </summary>
        public Zhyj.ICached.ICached GlobalCached {set;get;}


        public virtual void init(Zhyj.AppHandler.ActionFactory.Entity.Action action)
        {

            ZLogger.LogApp("接受到请求：" + action.Comment + "  —— 配置文件：" + action.ConfigFile + "——请求路径：" + action.Path + "——对象实例：" + action.Name + "——实例方法：" + action.Method);

            this.Id=Request["id"];
            if (!string.IsNullOrEmpty(this.Id))
            {
                int.TryParse(this.Id,out this.ID);
            }
            if (string.Equals(action.Verification,"ignore"))
            {
                BothInit(action);
            }
        }
       
        private string _host = null;
        public string Host { get { return _host; } }
        public Pager pager = null;
        public string Id = null;
        public int ID = 0;
        public int rowCount;
        public int PageNum
        {
            get { return pager.PageNum; }
            set { pager.PageNum = value; }
        }
        public int PageSize
        {
            get
            {
                return pager.Pagesize;
            }
            set { pager.Pagesize = value; }
        }
        public void Page(string perfix=null) {
           
            pager.Page(rowCount, perfix, TemplateData);
           
            
        }
        private SysUser _currentUser = null;
        public virtual SysUser CurrentUser
        {
             get
            {
                return _currentUser;
            }
            set { _currentUser = value; }

        }

        Zhyj.DBUtils.IDBExecutor _executor = null;
        /// <summary>
        /// 主数据操作类
        /// </summary>
        protected Zhyj.DBUtils.IDBExecutor Executor
        {
            get { return this._executor; }
        }
        
        
        //private Zhyj.ICached.ICached _icached;
        /// <summary>
        /// 全局的实例
        /// </summary>
        protected Zhyj.ICached.ICached ICached { get; set; }

        protected Dictionary<string, object> _cached = null;
        /// <summary>
        /// 当前用户的缓存集合，没有登陆用户就使用公开的集合
        /// </summary>
        protected virtual Dictionary<string, object> Cached
        {
            get {
                if (_cached != null)
                {
                    return _cached;
                }
                if (CurrentUser != null)
                {
                    string cachekey = CurrentUser.UserName + "_Cached";
                    if (ICached.Exists(cachekey))
                    {
                        _cached = ICached[cachekey] as Dictionary<string, object>;
                    }
                    else
                    {
                        _cached = new Dictionary<string, object>();
                        ICached[cachekey] = _cached;
                    }

                }
                if (_cached == null)
                {
                    _cached = new Dictionary<string, object>();
                }
                return _cached;
            }
        }
        public virtual Dictionary<string, object> CachedByUserName(string userName)
        {

            //只有本次请求有效
            if (string.IsNullOrEmpty(userName))
            {
                   _cached = new Dictionary<string, object>();
                   return _cached;
            }
            //用户请求有效
            string cachekey = userName + "_Cached";
            if (ICached.Exists(cachekey))
            {
                _cached = ICached[cachekey] as Dictionary<string, object>;
            }
            else
            {
                _cached = new Dictionary<string, object>();
                ICached[cachekey] = _cached;
            }
            //只有本次请求有效
            if (_cached == null)
            {
                _cached = new Dictionary<string, object>();
            }

            return _cached;
        }

        private Zhyj.AppHandler.DnsRoute.Entity.Domain _domain;
        protected Zhyj.AppHandler.DnsRoute.Entity.Domain Domain { get { return _domain; } }
        public void setMyDns(Zhyj.AppHandler.DnsRoute.Entity.Domain dns)
        {
            this._domain = dns;
        }
        private Zhyj.AppHandler.ActionFactory.Entity.Action _action;
        protected Zhyj.AppHandler.ActionFactory.Entity.Action Action { get { return _action; } }
        
        /// <summary>
        /// 在验证与初始化都调用的属性初始化
        /// </summary>
        protected virtual void BothInit(Zhyj.AppHandler.ActionFactory.Entity.Action action)
        {
            this._action = action;
            //主库
            if (this.Domain.Sysinfo.DbConfig.StartsWith("sqlite:", StringComparison.CurrentCultureIgnoreCase))
            {
                _executor = new Zhyj.DBUtils.SqliteExecutor();
                _executor.setConnnectionStr(Server.MapPath(string.Format("/{0}/{1}/data/{2}", Zhyj.AppHandler.UtilConfig.sys, Domain.Sysinfo.FolderName, this._domain.Sysinfo.DbConfig.Substring(7))));
            }
            else
            {
                
                _executor = new Zhyj.DBUtils.MsSqlExecutor();
                _executor.setConnnectionStr(this._domain.Sysinfo.DbConfig);
            }
            _executor.OnExecuteComplateEvent += ExecutorWeiXin_OnExecuteComplateEvent;
           
            GlobalCached = Zhyj.ICached.Cached.GetCachedInstance("GlobalCache");
            dataExecutorImp = new IDataExecutorImp();
            dataExecutorImp.Executor = Executor;

            ICached = Zhyj.ICached.Cached.GetCachedInstance(this.Domain.Sys);//Domain.Host+"："+Domain.Port
            dataExecutorImp.DataCached = ICached;

            HttpCookie cook = Request.Cookies[aspx_username];
            if (cook!=null && !string.IsNullOrEmpty(cook.Value))
	        {
                _currentUser = ICached[new Base64Encoding().Decode(cook.Value,true,true)] as SysUser;	 
	        }

            this.Id = Request["id"];
            int.TryParse(this.Id, out ID);
            pager = new Pager();

            int temp = 0;
            PageNum = int.TryParse(Request["pagenum"], out temp) ? temp : 1;
            PageSize = int.TryParse(Request["pagesize"], out temp) ? temp : 20;

            if (_systemInfo==null)
	        {
                        SysInfoData sysInfoData=dataExecutorImp.GetInstance<SysInfoData>();
                        _systemInfo=sysInfoData.getSysInfo();
                       // _config = new Zhyj.Tencent.WeiXin.Config(_systemInfo.WeiXinAppID,_systemInfo.WeiXinAppSecret,_systemInfo.WeiXinAppToken,_systemInfo.WeiXinEncodingAESKey,_systemInfo.WeiXinMchID,_systemInfo.WeiXinMchKey);
                       // System.Web.HttpContext.Current.Cache["wx_config_default_from_database"] = _config;

	        }
           
        }

         
        public void ExecutorWeiXin_OnExecuteComplateEvent(string commandText, System.Data.Common.DbParameterCollection paramList)
        {
            Zhyj.ZLogger4Web.ZLogger.LogSql( "执行：" + commandText, paramList);
        }
        private System.Collections.Hashtable _templateData;
        public System.Collections.Hashtable data;
        
        protected System.Collections.Hashtable TemplateData { get { return _templateData; } }
        public System.Collections.Hashtable getTemplateData()
        {
            return _templateData;
        }

       

        public object getCacheData(object[] args)
        {
            if (args.Length != 1 || args[0] == null)
            {
                ZLogger.logAppContent("获取缓存信息，参数不正确。");
            }
            string a = args[0].ToString();
            if (Cached.ContainsKey(a))
            {
                return Cached[a];
            }
            return null;
        }

        public string GetRoot()
        {
            if (!string.IsNullOrEmpty(Root))
            {
                return Root;
            }
            if (Action.Path.Equals("System_NotFound", StringComparison.CurrentCultureIgnoreCase)
                  ||
                 Action.Path.Equals("SystemAction-Cache", StringComparison.CurrentCultureIgnoreCase))
            {
                return Zhyj.AppHandler.UtilConfig.site;
            }else{
            return Zhyj.AppHandler.UtilConfig.sys;
            }
        }



        public virtual bool verfication(Zhyj.AppHandler.ActionFactory.Entity.Action action, ref string logonPath)
        {
            BothInit(action);
            //TODO:验证
            bool flag = CurrentUser != null;
            if (!flag)
            {
                logonPath = "/sys/logon.aspx";
            }
            return flag;
        }
        public const string validateCode = Zhyj.AppHandler.UtilConfig.validateCode;
        /// <summary>
        /// 判断验证码是否正确
        /// </summary>
        public bool IsValidate
        {
            get
            {

                HttpCookie cook = Request.Cookies.Get(validateCode);
                bool over = false;
                if (cook != null && !string.IsNullOrEmpty(Request[validateCode]))
                {
                    over = string.Equals(cook.Value, MD5.encode(Request[validateCode].ToUpper(), 32), StringComparison.CurrentCultureIgnoreCase);
                }
                Request.Cookies.Remove(validateCode);
                Response.Cookies.Remove(validateCode);
                return over;
            }
        }


        
    }
}

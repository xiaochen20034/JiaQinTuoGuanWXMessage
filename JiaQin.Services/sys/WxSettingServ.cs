using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JiaQin.Entity;
using JiaQin.Data;
using Zhyj.Common;
using Zhyj.Tencent.WeiXin;
using Zhyj.Tencent.WeiXin.entity;
namespace JiaQin.Services
{
    public class WxSettingServ : AppBase
    {
        public void Figure()
        {
            SysInfoData sysInfoData=dataExecutorImp.GetInstance<SysInfoData>();
            SysInfo info= sysInfoData.getSysInfo();
            if (info==null)
            {
                info = new SysInfo();
            }
            TemplateData[dataStr] = info;
        }
        /// <summary>
        /// 微信菜单
        /// </summary>
        public void MenuList() { 

            WxMenuData wxMenuData=dataExecutorImp.GetInstance<WxMenuData>();
            WxMenu menu = null;
            if (this.ID==0)
            {
                menu = new WxMenu() { 
                     Id=0,
                      Name="微信菜单",
                      ParentId=0 
                };
            }else{
            menu= wxMenuData.getWxMenuInfo(this.ID);
            }
            TemplateData["menu"] = menu;
            TemplateData["menuList"]= wxMenuData.List(this.ID);
        }
        /// <summary>
        /// 菜单添加
        /// </summary>
        public void MenuAddPage() {
            WxMenuData wxMenuData = dataExecutorImp.GetInstance<WxMenuData>();
            WxMenu menu = null;
            if (this.ID == 0)
            {
                menu = new WxMenu()
                {
                    Id = 0,
                    Name = "微信菜单",
                    ParentId = 0
                };
            }
            else
            {
                menu = wxMenuData.getWxMenuInfo(this.ID);
            }
            TemplateData["menuInfo"] = menu;
        }
        /// <summary>
        /// 菜单添加事件
        /// </summary>
        public void MenuAddEvent()
        {
            WxMenu menu=Request.PackageRequest<WxMenu>();
            WxMenuData wxMenuData = dataExecutorImp.GetInstance<WxMenuData>();
            wxMenuData.Add(menu);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "菜单添加完成";
        }


        /// <summary>
        /// 菜单添加
        /// </summary>
        public void MenuUpdatePage()
        {
            WxMenuData wxMenuData = dataExecutorImp.GetInstance<WxMenuData>();
            WxMenu menu = null;            
            menu = wxMenuData.getWxMenuInfo(this.ID);
            TemplateData["menuInfo"] = menu;
        }
        /// <summary>
        /// 菜单添加事件
        /// </summary>
        public void MenuUpdateEvent()
        {
            WxMenu menu = Request.PackageRequest<WxMenu>();
            WxMenuData wxMenuData = dataExecutorImp.GetInstance<WxMenuData>();
            wxMenuData.Update(menu);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "菜单编辑完成";
        }
        public void MenuDeleteEvent()
        {
            WxMenuData wxMenuData = dataExecutorImp.GetInstance<WxMenuData>();
            wxMenuData.Delete(this.ID);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "菜单删除完成";
        }

        
        public void SetBasicInfo() {
            SysInfo sysInfo = Request.PackageRequest<SysInfo>();
            SysInfoData sysInfoData = dataExecutorImp.GetInstance<SysInfoData>();
            sysInfoData.SetBasicInfo(sysInfo);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alert;
            TemplateData[JsonKeyValue.tip] = "基本信息保存完成";

        }
        public void SetPayInfo()
        {
            SysInfo sysInfo = Request.PackageRequest<SysInfo>();
            SysInfoData sysInfoData = dataExecutorImp.GetInstance<SysInfoData>();
            sysInfoData.SetMchInfo(sysInfo);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alert;
            TemplateData[JsonKeyValue.tip] = "支付商户信息保存完成";
        
        }

        public void CreateMenuEvent()
        {
            Menu menuF = new Menu();
            List<MenuItem> menuList = new List<MenuItem>();

            WxMenuData wxMenuD = dataExecutorImp.GetInstance<WxMenuData>();
            WxMenu[] list = wxMenuD.List(0);
            foreach (WxMenu item in list)
            {
                MenuItem menu = new MenuItem();
                menu.Name = item.Name;
                menuList.Add(menu);
                WxMenu[] list2 = item.SubMenuItems;
                if (list2 == null || list2.Length == 0)
                {
                    if (string.Equals(item.Type, "view", StringComparison.CurrentCultureIgnoreCase))
                    {
                        menu.Url = MenuUrl(menuF, item.Url);
                        menu.Type = MenuItemType.view;
                    }
                    else
                    {
                        menu.Key = item.Key;
                        menu.Type = MenuItemType.click;
                    }
                    continue;
                }
                menu.MenuItemList = new List<MenuItem>();
                foreach (WxMenu item2 in list2)
                {

                    if (string.Equals(item2.Type, "view", StringComparison.CurrentCultureIgnoreCase))
                    {
                        menu.MenuItemList.Add(new MenuItem()
                        {
                            Name = item2.Name,
                            Url = MenuUrl(menuF, item2.Url),
                            Type = MenuItemType.view
                        });
                    }
                    else
                    {
                        menu.MenuItemList.Add(new MenuItem()
                        {
                            Name = item2.Name,
                            Key = item2.Key,
                            Type = MenuItemType.click
                        });
                    }
                }
            }

            Zhyj.Tencent.WeiXin.Config config = new Zhyj.Tencent.WeiXin.Config();
            bool flag = menuF.CreateMenu(menuList, config);
            if (flag)
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.alert;
                TemplateData[JsonKeyValue.tip] = "微信菜单已创建";
            }
            else
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.alert;
                TemplateData[JsonKeyValue.tip] = "微信菜单创建失败";
            }

        }

        public string MenuUrl(Menu menuF, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                return url;
            }
            return menuF.WeixinMenuOpenUrl("http://" + Request.Url.Authority + "/wxurl/" + url, null);
        }


        public void ReciveWeixinTemplateMessageFinishJob()
        {
            
            
             try
            {
                Zhyj.ZLogger4Web.ZLogger.logAppContent("接受到消息回执--ReciveWeixinTemplateMessageFinishJob……");

                //WeixinMessageD weixin = dataExecutorImp.GetInstance<WeixinMessageD>();
                //weixin.UpdateMessageStatu();
                System.Web.IHttpHandler handler = System.Web.HttpContext.Current.Handler;
                WeixinMessageReciveHandler messageHandler = handler as WeixinMessageReciveHandler;

                Zhyj.Tencent.WeiXin.entity.TemplateJobFinishMessage message = messageHandler.RecivedTemplateJobFinishMessage;
                WeixinMessageD msgD = dataExecutorImp.GetInstance<WeixinMessageD>();
                
                msgD.UpdateMessageStatuByMessageId(message.MsgId, message.Status, message.CreateTime,message.FromUserName,message.ToUserName);
                //System.Web.IHttpHandler previousHandler = System.Web.HttpContext.Current.PreviousHandler;
                //System.Web.IHttpHandler currentHandler = System.Web.HttpContext.Current.CurrentHandler;
            }
            catch (Exception ead) {
                Zhyj.ZLogger4Web.ZLogger.LogApp("消息回执错误：");
                Zhyj.ZLogger4Web.ZLogger.logException2(ead);
            }

        }
    }
}

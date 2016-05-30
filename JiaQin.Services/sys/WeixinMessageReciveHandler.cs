using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Services
{
    public class WeixinMessageReciveHandler : Zhyj.Tencent.WeiXin.ReciveMessageHandler
    {
        #region 用户关注通知消息

        public override Zhyj.Tencent.WeiXin.entity.MessageInfo ReciveQrsceneSubscribeEventMessageEventHandler(Zhyj.Tencent.WeiXin.entity.QrsceneSubscribeEventMessage message)
        {
            this.subscribeMessage = message;
            return SubscribeMessage();
        }

        /// <summary>
        /// 用户关注通知消息
        /// </summary>
        public Zhyj.Tencent.WeiXin.entity.MessageInfo subscribeMessage;
        public override Zhyj.Tencent.WeiXin.entity.MessageInfo ReciveSubscribeEventMessageEventHandler(Zhyj.Tencent.WeiXin.entity.SubscribeEventMessage message)
        {

            Zhyj.ZLogger4Web.ZLogger.logAppContent("接受到用户关注信息：" + message.FromUserName);
            this.subscribeMessage = message;
            try
            {
                //Server.Execute("ReciveUserSubscribeMessage.aspx?ismicromessenger=true");
                //Response.Clear();
            }
            catch (Exception ea)
            {
                Zhyj.ZLogger4Web.ZLogger.logException2(ea);
            }
            //Zhyj.ZLogger4Web.ZLogger.LogApp("ReciveUserSubscribeMessage.aspx调用完成");
            //if ((this.subscribeMessage as Zhyj.Tencent.WeiXin.entity.TextImageMessage) == null)
            //13753103223
            //
            //    Zhyj.ZLogger4Web.ZLogger.LogApp("用户关注，程序返回了不正确的结果：" + this.subscribeMessage.GetType());
            //    return null;
            //}
            return SubscribeMessage();
        }
        private Zhyj.Tencent.WeiXin.entity.MessageInfo SubscribeMessage() {
            StringBuilder content = new StringBuilder();
            content.Append("Hi ~").Append("\n");
            content.Append("终于等到你").Append("\n");
            content.Append("还好我没放弃 ~").Append("\n\n");
            content.Append("<a href='http://sirdzi.com/wxurl/register.aspx'>立即绑定</a> , 即可成为米兰阳光心理圈会员").Append("\n\n");
            content.Append("在这里你可以：").Append("\n");
            content.Append("1、选课程").Append("\n");
            content.Append("2、做测评").Append("\n");
            content.Append("3、报活动").Append("\n");
            content.Append("4、作咨询").Append("\n");
            content.Append("5、与心理学爱好者 互动").Append("\n\n");
            content.Append("快快跟我们一起互动吧！").Append("\n");
            content.Append("<a href='http://sirdzi.com/wxurl/register.aspx?a=d'>立即绑定</a>").Append("\r\n");
//一条消息内，不能有两个一摸一样的链接                  
//@"Hi ~
//终于等到你
//还好我没放弃 ~
//<a href='http://sirdzi.com/wxurl/register.aspx'>立即绑定</a> , 即可成为米兰阳光心理圈会员
//在这里你可以：
//1、选课程
//2、做测评
//3、报活动
//4、作咨询
//5、与心理学爱好者 互动
//快快跟我们一起互动把！
//<a href='http://sirdzi.com/wxurl/register.aspx?a=d'>立即绑定</a>
//"
            Zhyj.Tencent.WeiXin.entity.TextMessage replaySubscribeMessage = new Zhyj.Tencent.WeiXin.entity.TextMessage()
            {
                FromUserName = subscribeMessage.ToUserName,
                ToUserName = subscribeMessage.FromUserName,
                CreateTime = DateTime.Now,
                 MsgType_2= Zhyj.Tencent.WeiXin.entity.MessageType.Text,
                  Content=content.ToString()
            };
             return replaySubscribeMessage;
        }

        #endregion
        public override Zhyj.Tencent.WeiXin.entity.MessageInfo ReciveTextMessageEventHandler(Zhyj.Tencent.WeiXin.entity.TextMessage message)
        {
            string  con="WeixinMessageReciveHandler执行：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            return new Zhyj.Tencent.WeiXin.entity.TextMessage() { Content = con, FromUserName = message.ToUserName, ToUserName = message.FromUserName, MsgType_2 = Zhyj.Tencent.WeiXin.entity.MessageType.Text };
        }
        #region 模版消息回执
        Zhyj.Tencent.WeiXin.entity.TemplateJobFinishMessage message = null;
        public Zhyj.Tencent.WeiXin.entity.TemplateJobFinishMessage RecivedTemplateJobFinishMessage
        {
            get
            {
                if (message == null)
                {
                    throw new FieldAccessException("您访问的模版消息完成回执未设置");
                }
                return message;
            }
        }

        public override Zhyj.Tencent.WeiXin.entity.MessageInfo ReciveTemplateJobFinishMessageEventHandler(Zhyj.Tencent.WeiXin.entity.TemplateJobFinishMessage message)
        {
            Zhyj.ZLogger4Web.ZLogger.logAppContent("接受到消息回执……" + message.MsgId);
            if (message.MsgId > 0)
            {
                Zhyj.ZLogger4Web.ZLogger.logAppContent("接受到消息回执……准备跳转到：ReciveWeixinTemplateMessageFinishJob");
                this.message = message;
                Server.Execute("ReciveWeixinTemplateMessageFinishJob.aspx?ismicromessenger=true");

                //---------------
                //System.Web.HttpContext.Current.Cache
                // System.Web.HttpContext.Current.Handler.
                //System.Web.HttpContext.Current.Server.Execute();
                //System.Web.HttpContext.Current.Server.TransferRequest("");

                //UpdateMessageStatu
                return null;
            }
            return base.ReciveTemplateJobFinishMessageEventHandler(message);
        }
        #endregion

    }
}

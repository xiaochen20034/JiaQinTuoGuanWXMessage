using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Data
{
    /// <summary>
    /// 微信消息记录
    /// </summary>
    public class WeixinMessageD : IDataExecutorImp
    {
        /// <summary>
        /// 保存作业的微信发送消息记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entityId"></param>
        /// <param name="sendDate"></param>
        /// <param name="weixinmsgId"></param>
        public void SaveWeixinMsg(Int64 userId, Int64 entityId, DateTime sendDate, int weixinmsgId,string entityType)
        {

            Executor.executeNonQuery("insert into wx_msg(entityId,userId,sendDate,weixinMsgId,entityType)values(@entityId,@userId,@sendDate,@weixinMsgId,@entityType)", System.Data.CommandType.Text, new object[,]{
                    {"@entityId",entityId},
                    {"@userId",userId},
                    {"@sendDate",sendDate.ToString("yyyy-MM-dd HH:mm:ss")},
                    {"@weixinMsgId",weixinmsgId},{"@entityType",entityType}
                });
           

        }
        /// <summary>
        /// 更新维修消息的状态
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="statu"></param>
        /// <param name="finishDate"></param>
        public void UpdateMessageStatuByMessageId(long messageId, string statu, DateTime finishDate, string fromUserOpenID, string toUserName)
        {
                if (Convert.ToInt32(Executor.executeSclar("select count(1) from wx_msg where weixinMsgId=@weixinMsgId", System.Data.CommandType.Text, new object[,]{
                    {"@weixinMsgId",messageId}
                })) > 0)
                {
                    Executor.executeNonQuery("update wx_msg set  finishDate=@finishDate,status=@status where weixinMsgId=@weixinMsgId", System.Data.CommandType.Text, new object[,]{
                        {"@weixinMsgId",messageId},
                        {"@finishDate",finishDate.ToString("yyyy-MM-dd HH:mm:ss")},
                        {"@status",statu}
                    });
                }
                else
                {
                    throw new Exception("接收到微信消息回执，但是没有找到messageid记录：" + messageId);
                }

        }

    }
}

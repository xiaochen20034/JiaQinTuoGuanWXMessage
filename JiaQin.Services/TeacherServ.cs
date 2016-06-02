using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JiaQin.Entity;
using JiaQin.Data;
using Zhyj.Common;
namespace JiaQin.Services
{
    /// <summary>
    /// 校区信息
    /// </summary>
    public class TeacherServ : JiaQin.Services.AppBase
    {
        public void Figure()
        {
            List();
        }
        public void List()
        {
            int schoolId = 0;
            if (CurrentUser.SchoolInfo!=null)
            {
                schoolId = CurrentUser.SchoolInfo.ID;
            }
            TeacherData teacherData = dataExecutorImp.GetInstance<TeacherData>();
            Teacher[] list = teacherData.List(schoolId, Request["name"], PageSize, PageNum, out rowCount);
            Page();
            TemplateData["teacherList"] = list;

        }
       


        public void Add() { 
            TagData tagData=dataExecutorImp.GetInstance<TagData>();
            TemplateData["tagList"]= tagData.List();
                 
        }
        public void AddEvent() { 
            SysUser user = Request.PackageRequest<SysUser>();
            SysUserData userData=dataExecutorImp.GetInstance<SysUserData>();
            if (userData.Exist(user.Phone))
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.exists;
                TemplateData[JsonKeyValue.field] = "Phone";
                TemplateData[JsonKeyValue.tip] = "手机号码已经存在";
                return;
            }
            if (CurrentUser.SchoolInfo==null)
            {

                TemplateData[JsonKeyValue.res] = JsonKeyValue.alert;
                TemplateData[JsonKeyValue.tip] = "配置错误，请联系管理员";
                return;
            }
            string[] tag = Request.Params.GetValues("tag");            
            TeacherData teacherData=dataExecutorImp.GetInstance<TeacherData>();
            user.UserName = user.Phone;
            user.Code = "t"+user.Phone;
            user.Password = new Zhyj.Common.Base64Encoding().Encode("999999");
            teacherData.Add(user,CurrentUser.SchoolInfo.ID,tag);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "教师添加完成";
        }
        public void Update()
        {
            SysUserData userData = dataExecutorImp.GetInstance<SysUserData>();
            TemplateData["userInfo"] = userData.Info(this.ID);

            TeacherData teacherData=dataExecutorImp.GetInstance<TeacherData>();
            Teacher teacherInfo= teacherData.getTeacherInfoByUserId(this.ID);
            
            TagData tagData = dataExecutorImp.GetInstance<TagData>();
            TemplateData["tagList"] = tagData.List();

            TemplateData["userTagList"] = teacherInfo.TagList;
            
        }
        public bool ContainsTag(Tag tag,Tag[]taglist) {
            return taglist.Contains(tag);
        }

        public void UpdateEvent()
        {
            SysUser userInfo = Request.PackageRequest<SysUser>();
            SysUserData tagData = dataExecutorImp.GetInstance<SysUserData>();
            if (tagData.Exist(userInfo.Phone, userInfo.Id))
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.exists;
                TemplateData[JsonKeyValue.field] = "Phone";
                TemplateData[JsonKeyValue.tip] = "手机号码已经存在";
                return;
            }
            string[] tag = Request.Params.GetValues("tag"); 
            userInfo.UserName = userInfo.Phone;
            TeacherData teacherData = dataExecutorImp.GetInstance<TeacherData>();

            teacherData.Update(userInfo, tag);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "教师基本信息编辑完成";

        }
        public void DeleteEvent()
        {
            TeacherData teacherData=dataExecutorImp.GetInstance<TeacherData>();
            teacherData.Delete(this.ID);

            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "教师信息保留，但不能登录。操作完成";
        }

        public void RestoreSchoolTeacherEvent() {
            TeacherData teacherData = dataExecutorImp.GetInstance<TeacherData>();
            teacherData.Restore(this.ID);

            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "教师已恢复登陆权限";
        }


        public void SignMessageFigure()
        {
            TagData tagData=dataExecutorImp.GetInstance<TagData>();
            TemplateData["tagTable"] =tagData.TagTableBySchoolId(CurrentUser.SchoolInfo.ID);

        }
        public void TagStudentList()
        {
            int tagId = Convert.ToInt32(Request["tagId"]);
            string name=Request["name"];
            StudentData studentData=dataExecutorImp.GetInstance<StudentData>();
            Student[]studentList= studentData.ListByTagIdOfSchoolId(tagId,CurrentUser.SchoolInfo.ID,name);

            TagData tagData=dataExecutorImp.GetInstance<TagData>();
            TemplateData["tagInfo"] = tagData.getTagInfoById(tagId);
            TemplateData["studentList"] =studentList;

            SignProjectData signProjectData=dataExecutorImp.GetInstance<SignProjectData>();
            TemplateData["projectList"]= signProjectData.List();
        }
        /// <summary>
        /// 获取学生当他的签到情况
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public SignRecord[] TodaySignRecordList(int studentId)
        {
            SignRecordData recordData=dataExecutorImp.GetInstance<SignRecordData>();
            return recordData.getTodaySignRecordListByStuId(studentId);
        }
        /// <summary>
        /// 托管签到事件
        /// </summary>
        public void SignProjectEvent() {
            int signProjectId = Convert.ToInt32(Request["signProjectId"]);
            string[] studentId = Request.Params.GetValues("studentId");
            StringBuilder sb = new StringBuilder();
            StudentData studentData=dataExecutorImp.GetInstance<StudentData>();
            System.Collections.Hashtable hashtable = new System.Collections.Hashtable();
            SignRecordData signRecordData=dataExecutorImp.GetInstance<SignRecordData>();
            TeacherData teacherData=dataExecutorImp.GetInstance<TeacherData>();
            Teacher teacherInfo= teacherData.getTeacherInfoByUserId(CurrentUser.Id);
            foreach (string item in studentId)
            {
                int stuId = 0;
                if (int.TryParse(item,out stuId) && stuId>0)
                {
                    hashtable[item] = "学生信息不正确";
                    continue;
                }
                Student stuInfo= studentData.getStudentInfoById(stuId);
                if (stuInfo==null || stuInfo.ParentInfo==null || stuInfo.ParentInfo.VipUserInfo==null)
                {
                    hashtable[item] = "学生信息、学生家长不正确";
                    continue;
                }
                if (stuInfo.DeleteDate!=null)
                {
                    hashtable[item] = "学生已暂时删除，无法发送托管消息";
                    continue;
                }
                if (stuInfo.ParentInfo.VipUserInfo.BindDate==null || !string.IsNullOrEmpty(stuInfo.ParentInfo.VipUserInfo.Nickname))
                {
                    hashtable[item] = "学生家长未绑定微信";
                    continue;
                }
                
                signRecordData.Add(new SignRecord()
                {
                    SignDate = DateTime.Now,
                    TeaUserId = CurrentUser.Id,
                    TeaId = teacherInfo.Id,
                    SignProjectId = signProjectId,
                    StuId = stuId,
                    StuVipUserId = stuInfo.VipUserID
                });
                hashtable[item] = "托管消息已发送";

            }
        }

        public void SingleSignProjectEvent()
        {
            int signProjectId = Convert.ToInt32(Request["signProjectId"]);
            string studentId = Request["studentId"];
            StringBuilder sb = new StringBuilder();
            StudentData studentData = dataExecutorImp.GetInstance<StudentData>();
            System.Collections.Hashtable hashtable = new System.Collections.Hashtable();
            SignRecordData signRecordData = dataExecutorImp.GetInstance<SignRecordData>();
            SignProjectData signProjectData=dataExecutorImp.GetInstance<SignProjectData>();
            TeacherData teacherData = dataExecutorImp.GetInstance<TeacherData>();
            Teacher teacherInfo = teacherData.getTeacherInfoByUserId(CurrentUser.Id);
            SignProject signProjectInfo = signProjectData.getSignProjectInfoById(signProjectId);
            int stuId = 0;
            if (signProjectInfo==null)
            {
                 TemplateData[JsonKeyValue.res] = JsonKeyValue.fail;
                    TemplateData[JsonKeyValue.tip] = "托管项目信息不正确。";
                    return;
            }
            if (!int.TryParse(studentId, out stuId) || stuId <= 0)
                {
                    TemplateData[JsonKeyValue.res] = JsonKeyValue.fail;
                    TemplateData[JsonKeyValue.tip] = "学生信息不正确";
                    return;
                }
                Student stuInfo = studentData.getStudentInfoById(stuId);
                if (stuInfo == null || stuInfo.ParentInfo == null || stuInfo.ParentInfo.VipUserInfo == null)
                {
                    TemplateData[JsonKeyValue.res] = JsonKeyValue.fail;
                    TemplateData[JsonKeyValue.tip] = "学生信息、学生家长不正确";
                    return;
                }
                if (stuInfo.DeleteDate != null)
                {
                    TemplateData[JsonKeyValue.res] = JsonKeyValue.fail;
                    TemplateData[JsonKeyValue.tip] = "学生已暂时删除，无法发送托管消息";
                    return;
                }
                if (stuInfo.ParentInfo.VipUserInfo.BindDate == null || string.IsNullOrEmpty(stuInfo.ParentInfo.VipUserInfo.Nickname) || string.IsNullOrEmpty(stuInfo.ParentInfo.VipUserInfo.OpenId))
                {
                    TemplateData[JsonKeyValue.res] = JsonKeyValue.fail;
                    TemplateData[JsonKeyValue.tip] = "学生家长未绑定微信";
                    return;
                }
                if (stuInfo.Times<=0)
                {
                    TemplateData[JsonKeyValue.res] = JsonKeyValue.fail;
                    TemplateData[JsonKeyValue.tip] = "托管次数已用完，无法发送托管信息。";
                    return;

                }
                if (signRecordData.HasTodaySignRecord(stuId,signProjectId))
                {
                    TemplateData[JsonKeyValue.res] = JsonKeyValue.fail;
                    TemplateData[JsonKeyValue.tip] = "学生今天已经对项目托管签到";
                    return;
                }
                string remark = CurrentUser.Name + "已发送托管消息【" + signProjectInfo.Name+ "】给家长：" + stuInfo.ParentInfo.VipUserInfo.Nickname;
                #region 发送消息

                //TODO:发送微信消息（暂时认为，本地程序不会报错，只要微信发送成功，就本地插入数据，不做数据操作）
                //理应，先插入一半数据，再执行为微信发送，根据发送情况，进行提交或者回滚
                    try
                    {
                        Zhyj.Tencent.WeiXin.Config WxConfig = new Zhyj.Tencent.WeiXin.Config();
                        WxTemplateData wxTemplateData = dataExecutorImp.GetInstance<WxTemplateData>();
                        Zhyj.Tencent.WeiXin.TemplateMessage templateMessage = new Zhyj.Tencent.WeiXin.TemplateMessage(WxConfig);
                        string shortCode = "OPENTM207665013";
                        WxTemplate wxTemplate = wxTemplateData.InfoByCode(shortCode);
                        if (wxTemplate == null)
                        {
                            Zhyj.Tencent.WeiXin.entity.TemplateMessageResult TempalteResult = templateMessage.GetTemplateId(shortCode);
                            if (TempalteResult == null || string.IsNullOrEmpty(TempalteResult.template_id))
                            {
                                TemplateData[JsonKeyValue.res] = JsonKeyValue.fail;
                                TemplateData[JsonKeyValue.tip] = "系统设置错误，微信模版设置失败";
                                return;
                            }
                            wxTemplate = new WxTemplate()
                            {
                                Title = "学生托管提醒",
                                Trade = "IT科技",
                                Trade2 = "互联网|电子商务",
                                TemplateCode = shortCode,
                                TemplateId = TempalteResult.template_id
                            };
                            wxTemplateData.Insert(wxTemplate);
                        }
                        
                        WeixinMessageD weixin = dataExecutorImp.GetInstance<WeixinMessageD>();
                        Zhyj.Tencent.WeiXin.entity.TemplateFieldInfo firstField = new Zhyj.Tencent.WeiXin.entity.TemplateFieldInfo() { key = "first", value = string.Format("你好，老师【{0}】已为您的孩子【{1}】办理托管签到 【{2}】", teacherInfo.SysUserInfo.Name,stuInfo.VipUserInfo.Name,signProjectInfo.Name) };

                        Zhyj.Tencent.WeiXin.entity.TemplateFieldInfo keyword1Field = new Zhyj.Tencent.WeiXin.entity.TemplateFieldInfo() { key = "keyword1", value = stuInfo.VipUserInfo.Name };
                        Zhyj.Tencent.WeiXin.entity.TemplateFieldInfo keyword2Field = new Zhyj.Tencent.WeiXin.entity.TemplateFieldInfo() { key = "keyword2", value = DateTime.Now.ToString("yyyy年MM月dd日 HH：mm") };

                        Zhyj.Tencent.WeiXin.entity.TemplateFieldInfo remarkField = new Zhyj.Tencent.WeiXin.entity.TemplateFieldInfo() { key = "remark", value = "您的剩余托管次数："+(stuInfo.Times-1) };


                        //TODO:拼接活动介绍页面


                        Zhyj.Tencent.WeiXin.entity.TemplateMessageResult result = templateMessage.SendTemplateMessage(wxTemplate.TemplateId, stuInfo.ParentInfo.VipUserInfo.OpenId, base.WxUrl("index.aspx"), new Zhyj.Tencent.WeiXin.entity.TemplateFieldInfo[]{
                            firstField,keyword1Field,keyword2Field,remarkField
                        });
                        if (result.errcode!=0)
                        {
                            TemplateData[JsonKeyValue.res] = JsonKeyValue.fail;
                            TemplateData[JsonKeyValue.tip] = "消息发送失败。"+result.errcode+"：   "+result.errmsg;
                            return;
                        }
                        SignRecord rec = new SignRecord()
                        {
                            SignDate = DateTime.Now,
                            TeaUserId = CurrentUser.Id,
                            TeaId = teacherInfo.Id,
                            SignProjectId = signProjectId,
                            StuId = stuId,
                            StuVipUserId = stuInfo.VipUserID,
                            Remark = remark
                        };
                        signRecordData.Add(rec);


                        weixin.SaveWeixinMsg(stuInfo.ID, rec.Id, DateTime.Now, result.msgid, "signRecord");

                       
                    }
                    catch (Exception ea)
                    {
                        Zhyj.ZLogger4Web.ZLogger.logException2(ea);
                    }

                #endregion

                   
                
                TemplateData[JsonKeyValue.res] = JsonKeyValue.ok;
                TemplateData[JsonKeyValue.tip] = remark;
                return;
            
        }
        
    }
}

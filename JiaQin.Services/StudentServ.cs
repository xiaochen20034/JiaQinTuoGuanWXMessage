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
    public class StudentServ : JiaQin.Services.AppBase
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
            StudentData teacherData = dataExecutorImp.GetInstance<StudentData>();
            Student[] list = teacherData.List(schoolId, Request["name"], PageSize, PageNum, out rowCount);
            Page();
            TemplateData["studentList"] = list;

        }
       


        public void Add() { 
            TagData tagData=dataExecutorImp.GetInstance<TagData>();
            TemplateData["tagList"]= tagData.List();
                 
        }
        public void AddEvent() {
            VipUser user = Request.PackageRequest<VipUser>();
            VipUserData userData = dataExecutorImp.GetInstance<VipUserData>();
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
            StudentData teacherData = dataExecutorImp.GetInstance<StudentData>();
            
            teacherData.Add(Request["StudentName"], Request["Gender"], Convert.ToInt32(Request["Age"]), Request["ParentName"], Request["Phone"], CurrentUser.SchoolInfo.ID, Convert.ToInt32(Request["Times"]),tag);// ）user,CurrentUser.SchoolInfo.ID,tag);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "学生添加完成";
        }
        public void Update()
        {


            StudentData teacherData = dataExecutorImp.GetInstance<StudentData>();
            Student studentInfo= teacherData.getStudentInfoById(this.ID);
            
            TagData tagData = dataExecutorImp.GetInstance<TagData>();
            TemplateData["tagList"] = tagData.List();
            TemplateData["studentInfo"] = studentInfo;
            TemplateData["userTagList"] = studentInfo.StudentTagList;
            
        }
        public bool ContainsTag(Tag tag,Tag[]taglist) {
            return taglist.Contains(tag);
        }

        public void UpdateEvent()
        {
            string StudentName= Request["StudentName"],
                Gender=Request["Gender"], 
                ParentName= Request["ParentName"],
                Phone=Request["Phone"];
            int studentId=this.ID,
                schoolId = CurrentUser.SchoolInfo.ID,
                times = Convert.ToInt32(Request["Times"]),
                 age = Convert.ToInt32(Request["Age"]);
             string[] tag = Request.Params.GetValues("tag");

             StudentData studentData = dataExecutorImp.GetInstance<StudentData>();
             Student studentInfo = studentData.getStudentInfoById(studentId);
            VipUserData tagData = dataExecutorImp.GetInstance<VipUserData>();
            if (tagData.Exist(Phone, studentInfo.ParentInfo.VipUserID))
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.exists;
                TemplateData[JsonKeyValue.field] = "Phone";
                TemplateData[JsonKeyValue.tip] = "手机号码已经存在";
                return;
            }

          
            studentData.Update(StudentName, Gender, age,times, ParentName, Phone, studentId, studentInfo.VipUserID, studentInfo.ParentInfo.VipUserID, tag);

            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "学生基本信息编辑完成";

        }
        public void DeleteEvent()
        {
            StudentData teacherData = dataExecutorImp.GetInstance<StudentData>();
            teacherData.Delete(this.ID);

            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "学生信息保留，但不能发送托管消息。操作完成";
        }

        public void RestoreSchoolStudentEvent()
        {
            StudentData teacherData = dataExecutorImp.GetInstance<StudentData>();
            teacherData.Restore(this.ID);

            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "学生信息已恢复发送托管消息权限";
        }


        public void SetTag()
        {

            StudentData teacherData = dataExecutorImp.GetInstance<StudentData>();
            Student studentInfo = teacherData.getStudentInfoById(this.ID);

            TemplateData["studentInfo"] = studentInfo;

            TemplateData["userTagList"] = studentInfo.StudentTagList;
        }

        public void SetTagEvent()
        {
            int studentId = Convert.ToInt32(Request["studentId"]);
            StudentTagData studentTagData = dataExecutorImp.GetInstance<StudentTagData>();
            StudentTag[] list = studentTagData.ListByStudentId(studentId);

            foreach (StudentTag item in list)
            {
                int times = 0;
                if (int.TryParse(Request["stuTag_" + item.Id], out times))
                {
                    item.Times = times>=0?times:0;
                }
                else
                {
                    times = -1;
                }
            }
            studentTagData.Update(studentId,list);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh
                ;
            TemplateData[JsonKeyValue.tip] = "更新完成";
        }
    }
}

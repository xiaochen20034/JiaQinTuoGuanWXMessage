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
    /// 标签信息
    /// </summary>
    public class TagServ : JiaQin.Services.AppBase
    {
        public void Figure()
        {
            List();
        }
        public void List()
        {
            TagData tagData=dataExecutorImp.GetInstance<TagData>();
            Tag[]list= tagData.List(Request["name"],PageSize,PageNum,out rowCount);
            Page();
            TemplateData["tagList"] = list;
            if (CurrentUser.SchoolInfo != null)
            {
                TemplateData["currentSchoolId"] = CurrentUser.SchoolInfo.ID;
            }


        }
        public void Add()
        {
            SignProjectData projectData=dataExecutorImp.GetInstance<SignProjectData>();
            TemplateData["projectList"]= projectData.List();
        }
        public void AddEvent()
        {
            Tag tagInfo=Request.PackageRequest<Tag>();
            TagData tagData = dataExecutorImp.GetInstance<TagData>();
            if (tagData.ExistName(tagInfo.Name))
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.exists;
                TemplateData[JsonKeyValue.field] = "Name";
                TemplateData[JsonKeyValue.tip] = "标签名称已经存在";
                return;
            }
            if (CurrentUser.SchoolInfo!=null)
            {
                tagInfo.SchoolId = CurrentUser.SchoolInfo.ID;
            }
            tagData.Add(tagInfo);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "标签添加完成";
            
        }

        public void Update()
        {
            SignProjectData projectData = dataExecutorImp.GetInstance<SignProjectData>();
            TemplateData["projectList"] = projectData.List();

            TagData tagData = dataExecutorImp.GetInstance<TagData>();
            Tag tagInfo= tagData.getTagInfoById(this.ID);
            TemplateData["tagInfo"] = tagInfo;

        }
        public void UpdateEvent()
        {
            Tag tagInfo = Request.PackageRequest<Tag>();
            TagData tagData = dataExecutorImp.GetInstance<TagData>();
            if (tagData.ExistName(tagInfo.Name,tagInfo.Id))
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.exists;
                TemplateData[JsonKeyValue.field] = "Name";
                TemplateData[JsonKeyValue.tip] = "标签名称已经存在";
                return;
            }
            tagData.Update(tagInfo);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "标签编辑完成";

        }
        public void DeleteEvent()
        {
            TagData tagData = dataExecutorImp.GetInstance<TagData>();
            if (tagData.HasStudentOrTeacher(this.ID))
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.alert;
                TemplateData[JsonKeyValue.tip] = "标签正在被教师或者学生使用。";
                return;
            }
            tagData.Delete(this.ID);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "标签删除完成";
        }

    }
}

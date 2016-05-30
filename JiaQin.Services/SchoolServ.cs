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
    public class SchoolServ:JiaQin.Services.AppBase
    {
        public void Figure()
        {
            List();
        }
        public void List()
        {
            SchoolData schoolData=dataExecutorImp.GetInstance<SchoolData>();
            School[]list= schoolData.List(Request["name"],PageSize,PageNum,out rowCount);
            Page();
            TemplateData["schoolList"] = list;

        }
        public void Add()
        {

        }
        public void AddEvent()
        {
            School schoolInfo=Request.PackageRequest<School>();
            SchoolData schoolData = dataExecutorImp.GetInstance<SchoolData>();
            if (schoolData.ExistName(schoolInfo.Name))
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.exists;
                TemplateData[JsonKeyValue.field] = "Name";
                TemplateData[JsonKeyValue.tip] = "校区名称已经存在";
                return;
            }
            schoolData.Add(schoolInfo);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "校区添加完成";
            
        }

        public void Update()
        {
            SchoolData schoolData = dataExecutorImp.GetInstance<SchoolData>();
            School schoolInfo= schoolData.getSchoolInfoById(this.ID);
            TemplateData["schoolInfo"] = schoolInfo;

        }
        public void UpdateEvent()
        {
            School schoolInfo = Request.PackageRequest<School>();
            SchoolData schoolData = dataExecutorImp.GetInstance<SchoolData>();
            if (schoolData.ExistName(schoolInfo.Name,schoolInfo.ID))
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.exists;
                TemplateData[JsonKeyValue.field] = "Name";
                TemplateData[JsonKeyValue.tip] = "校区名称已经存在";
                return;
            }
            schoolData.Update(schoolInfo);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "校区编辑完成";

        }
        public void DeleteEvent()
        {
            SchoolData schoolData = dataExecutorImp.GetInstance<SchoolData>();
            if (schoolData.HasStudentOrTeacher(this.ID))
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.alert;                
                TemplateData[JsonKeyValue.tip] = "校区下存在老师、学生，不能删除";
                return;
            }
            schoolData.Delete(this.ID);

            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "校区删除完成";
        }




    }
}

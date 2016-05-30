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
    /// 托管项目信息
    /// </summary>
    public class SignProjectServ : JiaQin.Services.AppBase
    {
        public void Figure()
        {
            List();
        }
        public void List()
        {
            SignProjectData signProjectData=dataExecutorImp.GetInstance<SignProjectData>();
            SignProject[]list= signProjectData.List(Request["name"],PageSize,PageNum,out rowCount);
            Page();
            TemplateData["signProjectList"] = list;

        }
        public void Add()
        {

        }
        public void AddEvent()
        {
            SignProject signProjectInfo=Request.PackageRequest<SignProject>();
            SignProjectData signProjectData = dataExecutorImp.GetInstance<SignProjectData>();
            if (signProjectData.ExistName(signProjectInfo.Name))
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.exists;
                TemplateData[JsonKeyValue.field] = "Name";
                TemplateData[JsonKeyValue.tip] = "托管项目名称已经存在";
                return;
            }
            signProjectData.Add(signProjectInfo);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "托管项目添加完成";
            
        }

        public void Update()
        {
            SignProjectData signProjectData = dataExecutorImp.GetInstance<SignProjectData>();
            SignProject signProjectInfo= signProjectData.getSignProjectInfoById(this.ID);
            TemplateData["signProjectInfo"] = signProjectInfo;

        }
        public void UpdateEvent()
        {
            SignProject signProjectInfo = Request.PackageRequest<SignProject>();
            SignProjectData signProjectData = dataExecutorImp.GetInstance<SignProjectData>();
            if (signProjectData.ExistName(signProjectInfo.Name,signProjectInfo.Id))
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.exists;
                TemplateData[JsonKeyValue.field] = "Name";
                TemplateData[JsonKeyValue.tip] = "托管项目名称已经存在";
                return;
            }
            signProjectData.Update(signProjectInfo);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "托管项目编辑完成";

        }
        public void DeleteEvent()
        {
            SignProjectData signProjectData = dataExecutorImp.GetInstance<SignProjectData>();
            
            signProjectData.Delete(this.ID);

            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "托管项目删除完成";
        }




    }
}

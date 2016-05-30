using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Entity
{
    public class WxTemplate
    {
        public int Id { set; get; }
        /// <summary>
        /// 模版标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 一级行业
        /// </summary>
        public string Trade { set; get; }
        /// <summary>
        /// 二级行业
        /// </summary>
        public string Trade2 { set; get; }
        /// <summary>
        /// 程序中使用到的模版ID
        /// </summary>
        public string TemplateId { set; get; }
        /// <summary>
        /// 模版在微信模版库中的编号
        /// </summary>
        public string TemplateCode { set; get; }
    }
}

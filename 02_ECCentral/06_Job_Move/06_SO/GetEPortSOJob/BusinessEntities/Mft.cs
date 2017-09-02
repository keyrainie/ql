using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetEPortSOJob.BusinessEntities
{
    /// <summary>
    /// 申报结果信息
    /// </summary>
    public class Mft
    {
        /// <summary>
        /// 申报单号(必需)
        /// </summary>
        public string MftNo { get; set; }
        /// <summary>
        /// 订单号(必需)
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string LogisticsNo { get; set; }
        /// <summary>
        /// 预校验标识(0=未通过,1=已通过)(必需)
        /// </summary>
        public string CheckFlg { get; set; }
        /// <summary>
        /// 预校验描述
        /// </summary>
        public string CheckMsg { get; set; }
        /// <summary>
        /// 申报单当前状态(必需)
        /// 00=未申报，   01=库存不足,  99=已关闭
        /// 11=已报国检,  12=国检放行,  13=国检审核未过,  14=国检抽检
        /// 21=已报海关,  22=海关单证放行， 23=海关单证审核未过,  24=海关货物放行,  25=海关查验未过
        /// 说明：申报单流转顺序为先报国检再报海关
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 操作时间(必需)
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

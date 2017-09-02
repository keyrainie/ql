using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.ExternalSYS
{
    [Serializable]
    [DataContract]
    public class ERPSHDInfo
    {
        /// <summary>
        /// 送货商品列表
        /// </summary>
        [DataMember]
        public List<ERPSHDItem> SHDItemList { get; set; }

        /// <summary>
        /// 关联单据编号
        /// </summary>
        [DataMember]
        public string RefOrderNo { get; set; }
        /// <summary>
        /// 关联单据类型
        /// </summary>
        [DataMember]
        public string RefOrderType { get; set; }

        /// <summary>
        /// 网站客户备注
        /// </summary>
        [DataMember]
        public string CustomerMemo { get; set; }

        /// <summary>
        /// 系统备注:
        /// </summary>
        [DataMember]
        public string SysMemo { get; set; }

        /// <summary>
        /// 送货单类型备注:""(普通送货单)/"同城往返"/"返仓单"
        /// </summary>
        [DataMember]
        public string SHDTypeMemo { get; set; }

        /// <summary>
        /// 0:送货单;1:同城往返;2:返仓单
        /// </summary>
        public int? SHDType { get; set; }

        /// <summary>
        /// 送货单商品，每单限送一个商品
        /// </summary>
        public int? MainProductSysNo { get; set; }

        public int? MainProductERP_SPID { get; set; }

        #region ERP SHD 表字段
        /// <summary>
        /// 记录编号
        /// </summary>
        [DataMember]
        public int? JLBH { get; set; }

        /// <summary>
        /// 送货日期
        /// </summary>
        [DataMember]
        public DateTime? SHRQ { get; set; }

        /// <summary>
        /// 送货时间(上午、下午、全天)
        /// </summary>
        [DataMember]
        public string SHSJ { get; set; }

        /// <summary>
        /// 制单时间
        /// </summary>
        [DataMember]
        public DateTime? ZDSJ { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        [DataMember]
        public int? ZDR { get; set; }

        /// <summary>
        /// ERP备注
        /// </summary>
        [DataMember]
        public string BZ { get; set; }

        /// <summary>
        /// QHBZ
        /// </summary>
        [DataMember]
        public int? QHBZ { get; set; } 

        /// <summary>
        /// XHHD
        /// </summary>
        [DataMember]
        public int? XHHD { get; set; } 

        /// <summary>
        /// BDJK
        /// </summary>
        [DataMember]
        public int? BDJK { get; set; }

        /// <summary>
        /// 库存地点
        /// </summary>
        [DataMember]
        public string KCDD_BR { get; set; }

        /// <summary>
        /// KHID
        /// </summary>
        [DataMember]
        public int? KHID { get; set; } 

        /// <summary>
        /// 登记类型
        /// </summary>
        [DataMember]
        public int? DJLX { get; set; }

        /// <summary>
        /// 销售部门ID
        /// </summary>
        [DataMember]
        public string DEPTID { get; set; }

        /// <summary>
        /// 部门ID_别称？
        /// </summary>
        [DataMember]
        public string DEPTID_BC { get; set; }

        /// <summary>
        /// 执行人
        /// </summary>
        [DataMember]
        public int? ZXR { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        [DataMember]
        public DateTime? ZXSJ { get; set; }

        /// <summary>
        /// 网购标记
        /// </summary>
        [DataMember]
        public int? WGBJ { get; set; }

        #endregion
    }
}

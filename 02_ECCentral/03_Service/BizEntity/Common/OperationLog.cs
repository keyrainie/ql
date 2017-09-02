using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 系统操作日志信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class OperationLog
    {
        /// <summary>
        /// 日志的系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        [DataMember]
        public string Note { get; set; }

        /// <summary>
        /// 操作类型编号
        /// </summary>
        [DataMember]
        public int? TicketType { get; set; }

        /// <summary>
        /// 该操作对应业务对象的系统编号
        /// </summary>
        [DataMember]
        public int? TicketSysNo { get; set; }

        /// <summary>
        /// 操作电脑IP
        /// </summary>
        [DataMember]
        public string OperationIP { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [DataMember]
        public DateTime? OperationTime { get; set; }

        /// <summary>
        /// 操作人员系统编号
        /// </summary>
        [DataMember]
        public int? OperationUserSysNo { get; set; }


        private bool isOrderOperationLog = false;
        /// <summary>
        /// 是否是SO的操作Log，true-是SO的操作日志；默认不是SO的操作日志
        /// </summary>
        [DataMember]
        public bool IsOrderOperationLog 
        {
            get
            {
                return isOrderOperationLog;
            }
            set
            {
                isOrderOperationLog = value;
            }
        }

        /// <summary>
        /// 所属公司
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public string StoreCompanyCode { get; set; }

        /// <summary>
        /// 使用语言
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }


    }
}

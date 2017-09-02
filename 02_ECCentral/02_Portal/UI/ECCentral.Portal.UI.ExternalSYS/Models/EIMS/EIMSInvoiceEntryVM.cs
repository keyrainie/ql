using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class EIMSInvoiceEntryVM : ModelBase
    {
        /// <summary>
        /// 单据编号
        /// </summary>
        public string AssignedCode { get; set; }

        /// <summary>
        /// IPP#
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// 单据名称
        /// </summary>
        public string InvoiceName { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        public string RuleAssignedCode { get; set; }

        /// <summary>
        /// 供应商#
        /// </summary>
        public int VendorNumber { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 收款类型
        /// </summary>
        public ReceiveType ReceiveType { get; set; }

        public string ReceiveTypeDes
        {
            get
            {
                string des = string.Empty;
                switch (ReceiveType)
                {
                    case BizEntity.ExternalSYS.ReceiveType.AccountDeduction:
                        des = "帐扣";
                        break;
                    case BizEntity.ExternalSYS.ReceiveType.Cash:
                        des = "现金";
                        break;
                }
                return des;
            }
        }

        /// <summary>
        /// PM
        /// </summary>
        public string PM { get; set; }

        /// <summary>
        /// 费用类型
        /// </summary>
        public EIMSType EIMSType { get; set; }

        public string EIMSTypeDes
        {
            get
            {
                string des = string.Empty;
                switch (EIMSType)
                {
                    case BizEntity.ExternalSYS.EIMSType.Coop:
                        des = "商品管理等费用";
                        break;
                    case BizEntity.ExternalSYS.EIMSType.CR:
                        des = "现金返点";
                        break;
                    case BizEntity.ExternalSYS.EIMSType.FRF:
                        des = "运费返利";
                        break;
                    case BizEntity.ExternalSYS.EIMSType.MDF:
                        des = "市场发展基金";
                        break;
                    case BizEntity.ExternalSYS.EIMSType.MKT:
                        des = "市场活动专项费";
                        break;
                    case BizEntity.ExternalSYS.EIMSType.POR:
                        des = "日常进货奖励";
                        break;
                    case BizEntity.ExternalSYS.EIMSType.PP:
                        des = "价保";
                        break;
                    case BizEntity.ExternalSYS.EIMSType.SR:
                        des = "销售返点";
                        break;
                    case BizEntity.ExternalSYS.EIMSType.VIR:
                        des = "合同返利";
                        break;
                }
                return des;
            }
        }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal InvoiceAmount { get; set; }

        /// <summary>
        /// 单据状态        
        /// </summary>
        public InvoiceStatus Status { get; set; }

        public string StatusDes
        {
            get
            {
                string des = string.Empty;
                switch (Status)
                {
                    case InvoiceStatus.AutoClose:
                        des = "全部收到(关闭)";
                        break;
                    case InvoiceStatus.Open:
                        des = "打开";
                        break;
                    default:
                        des = "-";
                        break;
                }
                return des;
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        public string CreateDateDes
        {
            get
            {
                return CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 审批通过时间
        /// </summary>
        public DateTime? ApproveDate { get; set; }

        public string ApproveDateDes
        {
            get
            {
                if (!ApproveDate.HasValue)
                {
                    return "-";
                }
                return ApproveDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 发票录入状态
        /// 已录入、未录入
        /// </summary>
        public string InvoiceInputStatus { get; set; }



        /// <summary>
        /// 上传SAP状态      
        /// </summary>
        public string IsSAPImported { get; set; }

        public string IsSAPImportedDes
        {
            get
            {
                return IsSAPImported == "Y" ? "已上传" : "未上传";
            }
        }

        /// <summary>
        /// 录入的发票号
        /// 多个发票号以","隔开
        /// </summary>
        public string InvoiceInputSysNo
        {
            get;
            set;
        }

        private bool m_IsCheck;

        public bool IsCheck
        {
            get { return m_IsCheck; }
            set { SetValue("IsCheck", ref m_IsCheck, value); }
        }
    }
}

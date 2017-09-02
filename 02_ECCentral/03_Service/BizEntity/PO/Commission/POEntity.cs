using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO.Commission
{
    public class POEntity : ICompany
    {
        #region ICompany Members

        public string CompanyCode
        {
            get;
            set;
        }

        #endregion


        public List<POItemEntity> POItems { get; set; }

        public string ContractNumber { get; set; }

        public int? SysNo { get; set; }

        public string POID { get; set; }

        public int? VendorSysNo { get; set; }

        public int? StockSysNo { get; set; }

        public int? ShipTypeSysNo { get; set; }

        public int? PayTypeSysNo { get; set; }

        public int? CurrencySysNo { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal? TotalAmt { get; set; }

        public DateTime? CreateTime { get; set; }

        public int? CreateUserSysNo { get; set; }

        public DateTime? AuditTime { get; set; }

        public int? AuditUserSysNo { get; set; }

        public DateTime? InTime { get; set; }

        public int? InUserSysNo { get; set; }

        public int? IsApportion { get; set; }

        public DateTime? ApportionTime { get; set; }

        public int? ApportionUserSysNo { get; set; }

        public DateTime? ETP { get; set; }

        public string Memo { get; set; }

        public string Note { get; set; }

        public int ? Status { get; set; }

        public int ? IsConsign { get; set; }

        public int ? POType { get; set; }

        public string InStockMemo { get; set; }

        public decimal? CarriageCost { get; set; }

        public int? PM_ReturnPointSysNo { get; set; }

        public decimal? UsingReturnPoint { get; set; }

        public int? ReturnPointC3SysNo { get; set; }

        public decimal? TaxRate { get; set; }

        public int? PurchaseStockSysno { get; set; }

        public string PMRequestMemo { get; set; }

        public string TLRequestMemo { get; set; }

        public int? PMSysNo { get; set; }

        public int? SettlementCompany { get; set; }

        public string WHReceiptSN { get; set; }

        public string ExecptStatus { get; set; }

        public int? ComfirmUserSysNo { get; set; }

        public DateTime? ComfirmTime { get; set; }

        public List<int> Privilege { get; set; }

        //public POOperate poOperate { get; set; }
        public DateTime? ETATime { get; set; }
        public string ETAHalfDay { get; set; }
        public DateTime? AbandonTime { get; set; }
        /// <summary>
        /// 存放一些附加信息
        /// </summary>
        public string AppendMessage { get; set; }

        public string Rank { get; set; }

        public string RefuseMemo { get; set; }

        public string TPStatus { get; set; }

        /// <summary>
        /// crl17688新增 prince
        /// 2010.11.18
        /// </summary>
        public string CheckResult { get; set; }

        /// <summary>
        /// crl 17649 新增 alan
        /// 2010.12.11
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// VP对应的PO SysNo
        /// </summary>
        public int? VendorPortalPOSysNo { get; set; }

        /// <summary>
        /// 经中转的目标仓
        /// </summary>
        public int? ITStockSysNo { get; set; }

        /// <summary>
        /// 关闭人信息【显示格式 ： SysNo/DisplayName】
        /// </summary>
        public string CloseUser { get; set; }


        /// <summary>
        /// CRL 18211_2 Ray.L.Xing 2011-03-08
        /// </summary>
        public string AutoSendMail { get; set; }

        public int PaySettleCompany
        {
            get;
            set;
        }
        public string PaySettleCompanyString
        {
            get
            {
                string company = "";
                switch (PaySettleCompany)
                {
                    case 3270:
                        company = "上海分公司";
                        break;
                    case 3271:
                        company = "北京分公司";
                        break;
                }
                return company;
            }
            set { }
        }



        //  public List<POEimsEntity> POEimsEntitys { get; set; }


        public string StatusString
        {
            get
            {
                switch ((int)Status)
                {
                    case 1: return "已创建";
                    case -1: return "自动作废";
                    case 0: return "作废";
                    case 2: return "等待分摊";
                    case 3: return "等待入库";
                    case 4: return "已入库";
                    case 5: return "待审核";
                    case 6: return "部分入库";
                    case 7: return "手动关闭";
                    case 8: return "系统关闭";
                    case 9: return "供应商关闭";
                    case 10: return "待PM确认";
                    default: return "";
                }
            }
            set
            {
            }
        }

        /// <summary>
        /// 登录人SysNo，用于产品线相关判断 CRL21776 by jack.w.wang 2012-11-12
        /// </summary>
        public int LoginSysNo { get; set; }
        /// <summary>
        /// 是否存在高级权限 用于产品线相关判断 CRL21776 by jack.w.wang 2012-11-12
        /// </summary>
        public bool HasSeniorRight { get; set; }
        /// <summary>
        /// 单据对应产品线  用于产品线查询  CRL21776 by jack.w.wang 2012-11-12
        /// </summary>
        public int? ProductLineSysNo { get; set; }
        /// <summary>
        /// 当单据所属PM与单据所属产品线主PM不一致时，是否自动更正单据所属PM为产品线主PM CRL21776 by jack.w.wang 2012-12-11
        /// 补充创建PO单时使用
        /// </summary>
        public bool? IsAutoFillPM { get; set; }
    }
}

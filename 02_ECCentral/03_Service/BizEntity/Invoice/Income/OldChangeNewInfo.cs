using System;
using System.Collections.Generic;

namespace ECCentral.BizEntity.Invoice
{
    /// <summary>
    /// 以旧换新补贴款信息
    /// </summary>
    public class OldChangeNewInfo : IIdentity, ICompany
    {
        public string InUser { get; set; }

        public DateTime? InDate { get; set; }

        public string ConfirmUser { get; set; }

        public DateTime? ConfirmDate { get; set; }

        public int? StatusCode 
        {
            get
            {
                return Status;
            }
            set
            {
                value = Status;
            }
        }

        public string TradeInId { get; set; }

        public int SoSysNo { get; set; }

        public string SysNoList { get; set; }

        public string Licence { get; set; }

        public decimal Rebate { get; set; }

        public decimal ReviseRebate { get; set; }

        public string CustomerID { get; set; }

        public int CustomerSysNo { get; set; }

        public string CustomerName { get; set; }

        public string receivePhone { get; set; }

        public string ReceiveContact { get; set; }

        public string AreaInfo { get; set; }

        public string ReceiveAddress { get; set; }

        public string ShipTypeName { get; set; }

        public List<SOListInfo> SOItems { get; set; }

        public string BankName { get; set; }

        public string BranchBankName { get; set; }

        public string BankAccount { get; set; }

        public string Note { get; set; }

        public string RBNote { get; set; }

        public string ReferenceID { get; set; }

        public decimal OrderAmt { get; set; }

        public int? Status { get; set; }

        public OldChangeNewStatus oldChangeNewStatus
        {
            get
            {
                switch (Status)
                {
                    case -2:
                        return OldChangeNewStatus.RefuseAudit;
                    case -1:
                        return OldChangeNewStatus.Abandon;
                    case 0:
                        return OldChangeNewStatus.Origin;
                    case 1:
                        return OldChangeNewStatus.SubmitAudit;
                    case 2:
                        return OldChangeNewStatus.Audited; 
                    case 3:
                        return OldChangeNewStatus.Refund;
                    case 4:
                        return OldChangeNewStatus.Close;
                    default:
                        return OldChangeNewStatus.Origin;
                }
            }
            set { }
        }

        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>

        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members

        #region ICompany Members

        /// <summary>
        /// 公司代码
        /// </summary>

        public string CompanyCode
        {
            get;
            set;
        }

        #endregion ICompany Members

    }
    public class SOListInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
    }
}

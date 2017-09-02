using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    public class EnumCodeMapRegister : IStartup
    {
        public void Start()
        {
            //注册付款单审核状态
            EnumCodeMapper.AddMap<PayableAuditStatus>(new Dictionary<PayableAuditStatus, string>
            {
                { PayableAuditStatus.NotAudit,    "O" },
                { PayableAuditStatus.WaitFNAudit, "F" },
                { PayableAuditStatus.Audited,     "A" }
            });

            //注册邮局电汇收款单确认状态
            EnumCodeMapper.AddMap<PostIncomeConfirmStatus>(new Dictionary<PostIncomeConfirmStatus, string>
            {
                { PostIncomeConfirmStatus.Audit,   "A" },
                { PostIncomeConfirmStatus.Cancel,  "C" },
                { PostIncomeConfirmStatus.Related, "R" }
            });

            //注册订单跟进状态
            EnumCodeMapper.AddMap<TrackingInfoStatus>(new Dictionary<TrackingInfoStatus, string>
            {
                { TrackingInfoStatus.Follow,  "A" },
                { TrackingInfoStatus.Submit,  "S" },
                { TrackingInfoStatus.Confirm, "C" }
            });

            //注册POS支付方式
            EnumCodeMapper.AddMap<POSPayType>(new Dictionary<POSPayType, string>
            {
                {  POSPayType.Card,  "01" },
                {  POSPayType.Cash,  "02" },
                {  POSPayType.Check, "03" }
            });

            //注册POS自动确认状态
            EnumCodeMapper.AddMap<AutoConfirmStatus>(new Dictionary<AutoConfirmStatus, string>
            {
                {  AutoConfirmStatus.Success, "A" },
                {  AutoConfirmStatus.Fault,   "D" }
            });

            //注册逾期未收款订单责任人状态
            EnumCodeMapper.AddMap<ResponseUserStatus>(new Dictionary<ResponseUserStatus, string>
            {
                {  ResponseUserStatus.Active, "A" },
                {  ResponseUserStatus.Dective,   "D" }
            });

            EnumCodeMapper.AddMap<ECCentral.BizEntity.Invoice.InvoiceType>(new Dictionary<ECCentral.BizEntity.Invoice.InvoiceType, string>{
                { ECCentral.BizEntity.Invoice.InvoiceType.MET, "MET" },
                { ECCentral.BizEntity.Invoice.InvoiceType.SELF, "NEG" },
            });

            EnumCodeMapper.AddMap<ECCentral.BizEntity.Invoice.DeliveryType>(new Dictionary<ECCentral.BizEntity.Invoice.DeliveryType, string>{
                { ECCentral.BizEntity.Invoice.DeliveryType.MET, "MET" },
                { ECCentral.BizEntity.Invoice.DeliveryType.SELF, "NEG" },
            });

            EnumCodeMapper.AddMap<ECCentral.BizEntity.Invoice.StockType>(new Dictionary<ECCentral.BizEntity.Invoice.StockType, string>{
                { ECCentral.BizEntity.Invoice.StockType.MET, "MET" },
                { ECCentral.BizEntity.Invoice.StockType.SELF, "NEG" },
                { ECCentral.BizEntity.Invoice.StockType.NAM, "NAM" }
            });

            //注册付款单审核状态
            EnumCodeMapper.AddMap<SapImportedStatus>(new Dictionary<SapImportedStatus, string>
            {
                { SapImportedStatus.Origin,"A" },
                { SapImportedStatus.Success,"S" },
                { SapImportedStatus.Fault,"E" },
                { SapImportedStatus.Unfinished,"F" }
            });
            EnumCodeMapper.AddMap<APInvoiceItemStatus>(new Dictionary<APInvoiceItemStatus, string>
            {
                {  APInvoiceItemStatus.Active, "A" },
                {  APInvoiceItemStatus.Deactive,   "D" }
            });

            EnumCodeMapper.AddMap<SAPStatus>(new Dictionary<SAPStatus, string>
            {
                {  SAPStatus.Active, "A" },
                {  SAPStatus.Deactive,   "D" }
            });

            //注册以旧换新状态
            EnumCodeMapper.AddMap<OldChangeNewStatus>(new Dictionary<OldChangeNewStatus, string>
                {
                    {OldChangeNewStatus.Abandon,"-1"},
                    {OldChangeNewStatus.Audited,"2"},
                    {OldChangeNewStatus.Close,"4"},
                    {OldChangeNewStatus.Origin,"0"},
                    {OldChangeNewStatus.Refund,"3"},
                    {OldChangeNewStatus.RefuseAudit,"-2"},
                    {OldChangeNewStatus.SubmitAudit,"1"}
                });

            //退款结果
            EnumCodeMapper.AddMap<WLTRefundStatus>(new Dictionary<WLTRefundStatus, int>
            {
                {WLTRefundStatus.Success,1},
                {WLTRefundStatus.Failure,-1},
                {WLTRefundStatus.Processing,3}
            });

            // 变价单状态
            EnumCodeMapper.AddMap<RequestPriceStatus>(new Dictionary<RequestPriceStatus, int> 
            {
                {RequestPriceStatus.Auditting,0},
                {RequestPriceStatus.Audited,1},
                {RequestPriceStatus.Running,2},
                {RequestPriceStatus.Aborted,3},
                {RequestPriceStatus.Finished,4},
                {RequestPriceStatus.Void,-1}
            });

            // 变价单变价类型
            EnumCodeMapper.AddMap<RequestPriceType>(new Dictionary<RequestPriceType, int> 
            {
                {RequestPriceType.PurchasePrice,0},
                {RequestPriceType.SalePrice,1}
            });
        }
    }
}
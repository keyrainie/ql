using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace IPP.MktToolMgmt.GroupBuyingJob.AppEnum
{
    public enum CustomerRank : int
    {
        [Description("ChuJi")]
        ChuJi=1,
        [Description("QingTong")]
        QingTong=2,
        [Description("BaiYin")]
        BaiYin=3,
        [Description("HuangJin")]
        HuangJin=4,
        [Description("ZuanShi")]
        ZuanShi=5,
        [Description("HuangGuan")]
        HuangGuan=6,
        [Description("ZhiZun")]
        ZhiZun=7
    }

    public enum FPStatus : int
    {
        [Description("Normal")]
        Normal = 0,
        [Description("Suspect")]
        Suspect = 1,
        [Description("Block")]
        Block = 2,
    }

      public enum AuditType : int
        {
            [Description("Auto")]
            Auto=0,
            [Description("Manual")]
            Manual=1
        }

        public enum BiStatus
        {
            [Description("InValid")]
            InValid = -1,
            [Description("Valid")]
            Valid = 0,
        }

        public enum SMSPriority
        {
            [Description("Low")]
            Low = 0,
            [Description("Normal")]
            Normal = 5,
            [Description("High")]
            High = 10,
        }

        public enum SMSContentType
        {
            [Description("订单审核")]
            OrderAudit = 0,
            [Description("订单出库")]
            OrderOutStock = 1,
        }

        #region 销售单状态
        public enum SOStatus : int
        {
            [Description("系统自动作废")]
            SystemCancel = -4,
            [Description("主管作废")]
            ManagerCancel = -3,
            [Description("客户作废")]
            CustomerCancel = -2,
            [Description("新蛋作废")]
            EmployeeCancel = -1,
            [Description("待审核")]
            Origin = 0,
            [Description("待出库")]
            WaitingOutStock = 1,
            [Description("待支付")]
            WaitingPay = 2,
            [Description("待主管审")]
            WaitingManagerAudit = 3,
            [Description("已出库")]
            OutStock = 4
        }
        #endregion

        #region 销售单Email类型
        public enum SOEmailType : int
        {
            [Description("销售单生成")]
            CreateSO = 0,
            [Description("销售单审核")]
            AuditSO = 1,
            [Description("销售单出库")]
            OutStock = 2,
            [Description("销售单加分")]
            AddDelayPoint = 3
        }
        #endregion

        //操作日志名称
        #region Log Type Enum
        //===========================================
        public enum LogType : int
        {
            [Description("财务NetPay Verify")]
            Finance_NetPay_Verify = 301311,
            [Description("销售单审核")]
            Sale_SO_Audit = 600602,
        }
        #endregion

        #region 财务销售 网上支付状态
        public enum NetPayStatus : int
        {
            [Description("Abandon")]
            Abandon = -1,
            [Description("Origin")]
            Origin = 0,
            [Description("Verified")]
            Verified = 1
        }
        #endregion

        #region 财务销售收款单 单据类型
        public enum SOIncomeOrderType : int
        {
            [Description("SO")]
            SO = 1,
            [Description("RO")]
            RO = 2,
            [Description("AO")]
            AO = 3,
            [Description("RO_Balance")]
            RO_Balance = 4,
            [Description("多付款退款单")]
            多付款退款单 = 5
        }
        #endregion

        #region 财务销售收款单 收款类型
        public enum SOIncomeStyle : int
        {
            [Description("Normal")]
            Normal = 0,
            [Description("Advanced")]
            Advanced = 1,
            [Description("RO")]
            RO = 2,
            [Description("RO_Balance")]
            RO_Balance = 3
        }
        #endregion

        #region 财务销售收款单 状态
        public enum SOIncomeStatus : int
        {
            [Description("Abandon")]
            Abandon = -1,
            [Description("Origin")]
            Origin = 0,
            [Description("Confirmed")]
            Confirmed = 1
        }
        #endregion

        //三值状态
        //请登记使用者
        #region TriStatus point, email, sms，RMA退货入库入其他产品时的审核状态。
        //===========================================
        public enum TriStatus : int
        {
            [Description("Abandon")]
            Abandon = -1,
            [Description("Origin")]
            Origin = 0,
            [Description("Handled")]
            Handled = 1

        }
        #endregion
    }


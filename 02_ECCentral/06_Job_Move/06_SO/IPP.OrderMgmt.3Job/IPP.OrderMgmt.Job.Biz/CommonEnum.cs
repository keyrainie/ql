using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.OrderMgmt.Job.Biz
{
    public class CommonEnum
    {
        public enum SpecialSOType
        {
            Normal = 0,
            AcerPromotion = 1,
            AZ = 2,
            Ricoh = 3
        }

        public enum SOStatus
        {
            SystemCancel = -4,
            ManagerCancel = -3,
            CustomerCancel = -2,
            EmployeeCancel = -1,
            Origin = 0,
            WaitingOutStock = 1,
            WaitingPay = 2,
            WaitingManagerAudit = 3,
            OutStock = 4
        }

        public enum SOEmailType
        {
            CreateSO = 0,
            AuditSO = 1,
            OutStock = 2,
            AddDelayPoint = 3
        }

        public enum TriStatus
        {
            Abandon = -1,
            Origin = 0,
            Handled = 1
        }

        public enum ComplainStatus
        {
            Abandoned = -1,
            Orgin = 0,
            Replied = 1,
            Handled = 2,
        }

        public enum SOIncomeStatus
        {
            Abandon = -1,
            Origin = 0,
            Confirmed = 1,
        }

        public enum ShipTypeEnum
        {
            NeweggShip = 0,
            IncityBySelf = 1,
            WareHouseBySelf = 2,
            ThirdParty = 3,
        }

        public enum YNStatus
        {
            No = 0,
            Yes = 1,
        }

        public enum SOIncomeOrderType
        {
            SO = 1,
            RO = 2,
            AO = 3,
            RO_Balance = 4,
            多付款退款单 = 5,
        }

        public enum PostIncomeOrderStatus : int
        {
            No = 0,
            Yes = 1
        }

        /// <summary>
        /// 邮局收款单确认状态
        /// </summary>
        public enum SOPostIncomeStatus : int
        {
            Abandon = -1,
            Origin = 0,
            Confirmed = 1
        }

        public enum ShipType
        {
            Default = 0,
            Urgency = 1,
            Normal = 1
        }

        public enum SMSPriority
        {
            Low = 0,
            Normal = 5,
            High = 10
        }
    }
}

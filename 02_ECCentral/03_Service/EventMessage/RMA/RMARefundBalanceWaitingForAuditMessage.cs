/*********************************************************************************************
// Copyright (c) 2013, Newegg (Chengdu) Co., Ltd. All rights reserved.
// Created by ViCTor.W.Ye at 6/4/2013 11:38:54 PM.
// E-Mail: Victor.W.Ye@newegg.com
// Class Name : RMA_RefundBalanceWaitingForAuditMessage
// CLR Version : 4.0.30319.17929
// Target Framework : 4.0
// Description :
//
//*********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.RMA
{
    public class RMACreateRefundBalanceWaitingForAuditMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_CreateRefundBalance_Audited";
            }
        }

        public int CurrentUserSysNo { get; set; }
        public int RefundBalanceSysNo { get; set; }
        public int RefundSysNo { get; set; }
    }

    public class RMACompleteRefundBalanceWaitingForAuditMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_CompleteRefundBalance_Audited";
            }
        }
        public int CurrentUserSysNo { get; set; }
        public int RefundBalanceSysNo { get; set; }
        public int RefundSysNo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    public class EnumCodeMapRegister : IStartup
    {
        public void Start()
        {
            EnumCodeMapper.AddMap<CustomerType>(new Dictionary<CustomerType, int>{
                { CustomerType.Personal, 0 },
                { CustomerType.Enterprise, 1 },
                { CustomerType.Campus, 2 },
                { CustomerType.Media,3 },
                { CustomerType.Internal,4 }
            });


            EnumCodeMapper.AddMap<AvtarShowStatus>(new Dictionary<AvtarShowStatus, string>(){
                {AvtarShowStatus.NotSet,"S"}, 
                {AvtarShowStatus.Show,"A"},
                {AvtarShowStatus.NotShow,"D"}
            });
            //兼容数据库中头像状态为S的情况
            EnumCodeMapper.AddExtraCodeMap<AvtarShowStatus>(AvtarShowStatus.NotSet, "0");//前台注册时为0，其实

            EnumCodeMapper.AddExtraCodeMap<CustomerStatus>(CustomerStatus.InValid, -1);//前台注册时为0，其实

            EnumCodeMapper.AddMap<OrderCheckStatus>(new Dictionary<OrderCheckStatus, int>(){
                {OrderCheckStatus.Invalid,1}, 
                {OrderCheckStatus.Valid,0}
            });

            EnumCodeMapper.AddMap<RefundRequestType>(new Dictionary<RefundRequestType, string>(){
                {RefundRequestType.SO,"S"}, 
                {RefundRequestType.Balance,"C"}
            });

            EnumCodeMapper.AddMap<RefundRequestStatus>(new Dictionary<RefundRequestStatus, string>(){
                {RefundRequestStatus.A,"A"}, 
                {RefundRequestStatus.O,"O"},
                {RefundRequestStatus.R,"R"}
            });

            EnumCodeMapper.AddMap<PrepayStatus>(new Dictionary<PrepayStatus, string>(){
                {PrepayStatus.Valid,"A"},
                {PrepayStatus.InValid,"D"}
            });

            //EnumCodeMapper.AddMap<CompanyCustomer>(new Dictionary<CompanyCustomer, int>()
            //{
            //    {CompanyCustomer.Newegg,0},
            //    {CompanyCustomer.AstraZeneca,1}
            //});
            EnumCodeMapper.AddMap<RefundAdjustStatus>(new Dictionary<RefundAdjustStatus, int>()
                {
                    {RefundAdjustStatus.Abandon,-1},
                    {RefundAdjustStatus.Initial,0},
                    {RefundAdjustStatus.WaitingAudit,1},
                    {RefundAdjustStatus.AuditRefuesed,2},
                    {RefundAdjustStatus.Audited,3},
                    {RefundAdjustStatus.Refunded,4}
                });
            EnumCodeMapper.AddMap<RefundAdjustType>(new Dictionary<RefundAdjustType, int>()
                {
                    {RefundAdjustType.ShippingAdjust,1},
                    {RefundAdjustType.Other,2}
                });
        }
    }
}

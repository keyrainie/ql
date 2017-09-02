using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ECCentral.BizEntity.Customer;
using IPP.EcommerceMgmt.SendCustomerGuidePoints.DataAccess;
using IPP.EcommerceMgmt.SendCustomerGuidePoints.Entities;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.ServiceConsole.Client;

namespace IPP.EcommerceMgmt.SendCustomerGuidePoints.BizProcess
{
    public class SendCustomerGuidePointsBP
    {
        private int TestCustomerGuideSysNo;

        public void Process()
        {
            TestCustomerGuideSysNo = 0;
            int.TryParse(ConfigurationManager.AppSettings["TestCustomerGuideSysNo"], out TestCustomerGuideSysNo);

            List<CustomerGuideEntity> customerGuideList = SendCustomerGuidePointsDA.GetAllCustomerGuideList();
            customerGuideList = FilterByTestSysNo(customerGuideList);
            List<CustomerGuideEntity> success1List = SendCommentPoint(customerGuideList);
            UpdateObtainPoint(success1List);

        }

        private List<CustomerGuideEntity> FilterByTestSysNo(List<CustomerGuideEntity> customerGuideList)
        {
            List<CustomerGuideEntity> result = new List<CustomerGuideEntity>();
            if (TestCustomerGuideSysNo > 0)
            {
                List<CustomerGuideEntity> testList = customerGuideList.Where(p => p.SysNo == TestCustomerGuideSysNo).ToList();
                foreach (CustomerGuideEntity guide in testList)
                {
                    result.Add(guide);
                }
            }
            else
            {
                result = customerGuideList;
            }
            return result;
        }

        private static void UpdateObtainPoint(List<CustomerGuideEntity> success1List)
        {
            foreach (CustomerGuideEntity item in success1List)
            {
                SendCustomerGuidePointsDA.UpdateObtainPoint(item);
            }
        }

        public List<CustomerGuideEntity> SendCommentPoint(List<CustomerGuideEntity> commentList)
        {
            if (commentList == null || commentList.Count == 0)
            {
                return new List<CustomerGuideEntity>();
            }
            List<CustomerPointsAddRequest> body = new List<CustomerPointsAddRequest>();
            foreach (var item in commentList)
            {
                //新蛋网商品（vendortype=0）评论经验积分从MKT-Topic账户发放。
                //商家商品(vendortype=1)评论相关发放积分从Seller –topic中发放。
                CustomerPointsAddRequest aPR = new CustomerPointsAddRequest
                {
                    CustomerSysNo = item.CustomerSysNo,
                    Point = item.CustomerPoint,
                    Memo = item.SysNo.ToString(),
                    Source = "ECommerceMgmt",
                    SysNo = item.SysNo
                };
                body.Add(aPR);
            }

            List<CustomerGuideEntity> successList = new List<CustomerGuideEntity>();
            if (body == null || body.Count == 0)
            {
                return new List<CustomerGuideEntity>();
            }
            foreach (CustomerPointsAddRequest item in body)
            {
                successList.Add(new CustomerGuideEntity()
                {
                    SysNo = Convert.ToInt32(item.SysNo),
                    CustomerPoint = Convert.ToInt32(item.Point),
                    CustomerSysNo = Convert.ToInt32(item.CustomerSysNo)
                });
            }
            return successList;
        }
    }
}

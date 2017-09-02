using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using GiveERPCustomerPoint.DA;
using GiveERPCustomerPoint.Entities;
using GiveERPCustomerPoint.Biz;

namespace GiveERPCustomerPoint
{
   public class ReturnCrmPointActionBiz
    {
       static ILog log = LogerManger.GetLoger();

        public ReturnCrmPointActionBiz()
        {

        }
        private JobContext context = null;
        public ReturnCrmPointActionBiz(JobContext _context)
        {
            context = _context;
        }

        public void ReturnPoint()
        {
            CustomerScoreLogDA da=new CustomerScoreLogDA();
            List<CustomerScoreEntity> list = da.GetWaitReturnSOIDs();
            if (list != null && list.Count > 0)
            {
                foreach (CustomerScoreEntity item in list)
                {

                    if (string.IsNullOrWhiteSpace(item.CrmMemberID))
                    {
                        string errorMark = "CrmMemberID为空";
                        string msg = string.Format("CRM退还积分失败：编号-{0}，积分-{1}，顾客-{2}，CRM顾客编号-，原因-{3}", item.SysNo
                            , item.ValidScore, item.CustomerSysNo, errorMark);
                        da.GivePointFaild(item.SysNo, msg);
                        Writelog(msg);
                        continue;
                    }
                    string message = string.Empty;
                    bool success = CRMExternalService.CrmReturn(item, out message);
                    if (success)
                    {
                        string msg = string.Format("CRM退还积分成功：编号-{0}，积分-{1}，顾客-{2}，CRM顾客编号-，原因-{3}", item.SysNo
                             , item.ValidScore, item.CustomerSysNo, message);
                        da.ReturnPiontSuccess(item.SysNo);
                        Writelog(msg);
                    }
                    else
                    {
                        if (message == "noneed")
                        {
                            //:该订单没有退款的商品
                        }
                        else
                        {
                            string msg = string.Format("CRM退还积分失败：编号-{0}，积分-{1}，顾客-{2}，CRM顾客编号-，原因-{3}", item.SysNo
                                                       , item.ValidScore, item.CustomerSysNo, message);
                            da.GivePointFaild(item.SysNo, msg);
                            Writelog(msg);
                        }
                    }
                }
            }
        }
        public void Writelog(string msg)
        {
            string msgStr = string.Format("{0}：{1}{2}", DateTime.Now.ToString(), msg, Environment.NewLine);

            if (this.context != null)
            {
                context.Message += msgStr;
            }
            log.WriteLog(msgStr);
        }
    }
}

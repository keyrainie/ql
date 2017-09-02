using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiveERPCustomerPoint.DA;
using GiveERPCustomerPoint.Entities;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace GiveERPCustomerPoint.Biz
{
    public class GiveERPCustomerPointBiz
    {
        static ILog log = LogerManger.GetLoger();

        public GiveERPCustomerPointBiz()
        {

        }

        public GiveERPCustomerPointBiz(JobContext _context)
        {
            context = _context;
        }

        private JobContext context = null;

        public  void GivePoint()
        {
            CustomerScoreLogDA da=new CustomerScoreLogDA();
            List<CustomerScoreEntity> list = da.LoadWaitGivePointList();
            if (list != null && list.Count > 0)
            {
                foreach (CustomerScoreEntity  item in list)
                {

                    if (string.IsNullOrWhiteSpace(item.CrmMemberID))
                    {
                        string errorMark = "CrmMemberID为空";
                        da.GivePointFaild(item.SysNo, errorMark);
                        string msg = string.Format("CRM赠送积分失败：编号-{0}，积分-{1}，顾客-{2}，CRM顾客编号-，原因-{3}", item.SysNo
                            , item.ValidScore, item.CustomerSysNo, errorMark);
                        Writelog(msg);
                        continue;
                    }
                    string message = string.Empty;
                    List<CRMLuckDrawLog> luckList = CRMExternalService.CrmTradeConfirm(item, out message);
                    if (string.IsNullOrEmpty(message))
                    {
                        da.GivePointSuccess(item.SysNo);
                        string msg = string.Format("CRM赠送积分成功：编号-{0}，积分-{1}，顾客-{2}，CRM顾客编号-{3}", item.SysNo
                            , item.ValidScore, item.CustomerSysNo, item.CrmMemberID);
                        //插入抽奖信息
                        if (luckList != null && luckList.Count > 0)
                        {
                            foreach (var entity in luckList)
                            {
                                da.InsertCRMLuckDrawLog(entity);
                            }
                        }
                        Writelog(msg);
                    }
                    else
                    {
                        string errorMark = CRMExternalService.CrmOpClaUtilInStance.RMsg;
                        da.GivePointFaild(item.SysNo, errorMark);
                        string msg = string.Format("CRM赠送积分失败：编号-{0}，积分-{1}，顾客-{2}，CRM顾客编号-{3}，原因-{4}", item.SysNo
                            , item.ValidScore, item.CustomerSysNo, item.CrmMemberID, errorMark);
                        Writelog(msg);
                    }
                }
            }
        }

        public  void Writelog(string msg)
        {
            string msgStr = string.Format("{0}：{1}{2}", DateTime.Now.ToString(), msg,Environment.NewLine);

            if (this.context != null)
            {
                context.Message += msgStr  ;
            }
            log.WriteLog(msgStr);
        }
    }
}

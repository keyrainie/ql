using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ECCentral.Service.RMA.IDataAccess
{
    public interface IOutBoundDA
    {
        /// <summary>
        /// 更新OutBoundItem中邮件发送次数
        /// </summary>
        /// <param name="OutboundSysNo">OutBound系统编号</param>
        /// <param name="RegisterSysNo">单件编号</param>
        /// <param name="SendMailCount">发送邮件次数</param>
        bool UpdateOutboundItemSendEmailCount(int outboundSysNo, int registerSysNo, int sendMailCount);

        DataRow GetOutboundBySysNo(int outboundSysNo);


        /// <summary>
        /// 通过单件号获取送修单编号
        /// </summary>
        /// <typeparam name="?"></typeparam>
        /// <param name="?"></param>
        /// <returns></returns>
        List<int> GetOutBoundSysNoListByRegisterSysNoList(string registerSysNoList);

        /// <summary>
        /// PO更新送修单
        /// </summary>
        /// <param name="outBoundList"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        bool UpdateOutBounds(string outBoundList);
    }
}

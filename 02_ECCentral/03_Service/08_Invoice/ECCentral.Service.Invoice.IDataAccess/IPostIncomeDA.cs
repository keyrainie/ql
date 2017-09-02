using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IPostIncomeDA
    {
        /// <summary>
        /// 创建电汇邮局收款单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        PostIncomeInfo Create(PostIncomeInfo entity);

        /// <summary>
        /// 更新电汇邮局收款单
        /// </summary>
        /// <param name="entity"></param>
        void Update(PostIncomeInfo entity);

        /// <summary>
        /// 处理电汇邮局收款单
        /// </summary>
        /// <param name="entity"></param>
        void Handle(PostIncomeInfo entity);

        /// <summary>
        /// 更新电汇邮局收款单状态
        /// 用于收款单的确认、取消确认、作废、取消作废操作
        /// </summary>
        /// <param name="sysNo">电汇邮局收款单系统编号</param>
        /// <param name="status">要更新到的目标状态</param>
        void UpdateConfirmStatus(int sysNo, PostIncomeStatus status);

        /// <summary>
        /// 根据电汇邮局收款单系统编号加载收款单信息
        /// </summary>
        /// <param name="postIncomeSysNo">电汇邮局收款单系统编号</param>
        /// <returns></returns>
        PostIncomeInfo LoadBySysNo(int postIncomeSysNo);

        /// <summary>
        /// 根据订单系统编号取得已和订单关联的PostIncome列表
        /// </summary>
        /// <param name="confirmedSOSysNo">订单系统编号字符串，多个订单编号之间用逗号（,）隔开</param>
        /// <returns></returns>
        List<PostIncomeInfo> GetListByConfirmedSOSysNo(string confirmedSOSysNo);

        /// <summary>
        /// 通过电汇邮局收款单系统编号取得订单确认关联信息列表
        /// </summary>
        /// <param name="postIncomeSysNo">电汇邮局收款单系统编号</param>
        /// <returns></returns>
        List<PostIncomeConfirmInfo> GetConfirmListByPostIncomeSysNo(int postIncomeSysNo);

        /// <summary>
        /// 通过订单系统编号(多个订单系统编号通过逗号分隔)取得订单确认关联信息列表
        /// </summary>
        /// <param name="SOSysNos">订单系统编号(多个订单系统编号通过逗号分隔)</param>
        /// <returns></returns>
        List<PostIncomeConfirmInfo> GetConfirmedListBySOSysNo(string SOSysNos);

        /// <summary>
        /// 通过通过电汇邮局收款单系统编号更新订单关联信息状态
        /// </summary>
        /// <param name="postIncomeSysNo">通过电汇邮局收款单系统编号</param>
        /// <param name="status">需要更新到的状态</param>
        void UpdatePostIncomeConfirmStatus(int postIncomeSysNo, PostIncomeConfirmStatus status);

        /// <summary>
        /// 创建订单确认关联信息
        /// </summary>
        /// <param name="entity"></param>
        void CreatePostIncomeConfirm(PostIncomeConfirmInfo entity);

        #region [For SO Domain]

        /// <summary>
        /// 取消拆分PostIncome
        /// </summary>
        /// <param name="entity"></param>
        void AbandonSplitForSO(PostIncomeInfo entity);

        #endregion [For SO Domain]
    }
}
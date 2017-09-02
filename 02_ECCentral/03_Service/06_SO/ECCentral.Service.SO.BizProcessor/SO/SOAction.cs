using System;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.SO.BizProcessor
{
    /// <summary>
    /// 订单的所有行为都继承此类，此类在实现类上加上Attribute：[VersionExport(typeof(SOAction), new string[] { "订单类字符串:SOType.ToString(),默认添加为:General", "操作命令字符串:SOAction.SOCommand.ToString()" })]，如类：SOCreater，SOUpdater
    /// </summary> 
    public abstract class SOAction
    {
        /// <summary>
        /// 要操作的订单信息
        /// </summary>
        public SOInfo CurrentSO { get; set; }

        /// <summary>
        /// 根据操作自定义参数，
        /// </summary>
        internal protected object[] Parameter { get; set; }

        protected object GetParameterByIndex(int index)
        {
            object p = null;
            if (Parameter != null && Parameter.Length > index)
            {
                p = Parameter[index];
            }
            return p;
        }
        protected T GetParameterByIndex<T>(int index, T defaultValue)
        {
            object p = GetParameterByIndex(index);
            return p == null ? defaultValue : (T)p;
        }
        protected T GetParameterByIndex<T>(int index)
        {
            object p = GetParameterByIndex(index);
            return p == null ? default(T) : (T)p;
        }

        public abstract void Do();

        public virtual void WriteLog(BizLogType OperationType, string operationName)
        {
            ObjectFactory<SOLogProcessor>.Instance.WriteSOLog(OperationType, operationName, CurrentSO);
        }

        public enum SOCommand
        {
            /// <summary>
            /// 创建
            /// </summary>
            Create,
            /// <summary>
            /// 修改
            /// </summary>
            Update,
            /// <summary>
            /// 拆分
            /// </summary>
            Split,
            /// <summary>
            /// 取消拆分
            /// </summary>
            CancelSplit,
            /// <summary>
            /// 审核
            /// </summary>
            Audit,
            /// <summary>
            /// 取消审核
            /// </summary>
            CancelAudit,
            /// <summary>
            /// 出库，订单出库目前只处理SellerPortal发来请求
            /// </summary>
            OutStock,
            /// <summary>
            /// 移仓在途
            /// </summary>
            Shipping,
            /// <summary>
            /// 完成
            /// </summary>
            Complete,
            /// <summary>
            /// 作废
            /// </summary>
            Abandon,
            /// <summary>
            /// 锁定
            /// </summary>
            Hold,
            /// <summary>
            /// 解锁
            /// </summary>
            Unhold,
            /// <summary>
            /// Job相关操作
            /// </summary>
            Job
        }
        [VersionExport(typeof(SOCommandInfo))]
        public class SOCommandInfo
        {
            /// <summary>
            /// 订单信息
            /// </summary>
            public SOInfo SOInfo
            {
                get;
                set;
            }

            /// <summary>
            /// 操作命令参数。与Parameter属性共同作用
            /// 使用说明如下：
            /// 如果Command == SOCommand.Create，同时SOInfo为赠品订单（SOInfo.BaseInfo.SOType == SOType.Gift）,则
            /// Parameter[0] : int? , 主订单编号。
            /// 如果Command== SOCommand.Abandon，则
            /// Parameter[0] : bool , 表示订单作废后订单中商品是否立即返还库存,默认为false；
            /// Parameter[1] : bool , 如果有有效的收款记录是否生成负收款单后再作废订单；
            /// Parameter[2] : ECCentral.BizEntity.Invoice.SOIncomeRefundInfo , 负收款信息。
            /// 如果Command== SOCommand.Audit， 则 
            /// Parameter[0] : bool , 表示是否强制审核订单,默认为false；
            /// Parameter[1] : bool , 表示是否是主管审核,默认为false；
            /// Parameter[2] : bool , 表示是否要审核网上支付,默认为false；
            /// 如果Command== SOCommand.Split，则
            /// 属性 Parameter 说明:
            /// Parameter[0] : bool , 是否自动拆分订单,默认为true；
            /// </summary>
            public SOCommand Command
            {
                get;
                set;
            }

            /// <summary>
            /// 此为动态参数，与Command属性共同用户，不同的Command有不同的意义。
            /// 使用说明如下：
            /// 如果Command == SOCommand.Create，同时SOInfo为赠品订单（SOInfo.BaseInfo.SOType == SOType.Gift）,则
            /// Parameter[0] : int? , 主订单编号。
            /// 如果Command== SOCommand.Abandon，则
            /// Parameter[0] : bool , 表示订单作废后订单中商品是否立即返还库存,默认为false；
            /// Parameter[1] : bool , 如果有有效的收款记录是否生成负收款单后再作废订单；
            /// Parameter[2] : ECCentral.BizEntity.Invoice.SOIncomeRefundInfo , 负收款信息。
            /// 如果Command== SOCommand.Audit，则
            /// Parameter[0] : bool , 表示是否强制审核订单,默认为false；
            /// Parameter[1] : bool , 表示是否是主管审核,默认为false；
            /// Parameter[2] : bool , 表示是否要审核网上支付,默认为false；
            /// 如果Command== SOCommand.Split，则
            /// 属性 Parameter 说明:
            /// Parameter[0] : bool , 是否自动拆分订单,默认为true；
            /// </summary>
            public object[] Parameter { get; set; }
        }
    }


}
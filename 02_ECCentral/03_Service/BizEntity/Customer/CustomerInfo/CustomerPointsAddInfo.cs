using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 手动给顾客加积分申请
    /// </summary>
    public class CustomerPointsAddRequest : AdjustPointRequest
    {
        public CustomerPointsAddRequest()
        {
            PointsItemList = new List<CustomerPointsAddRequestItem>();
        }
        /// <summary>
        /// 顾客姓名
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 顾客ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        //public string Note { get; set; }
        /// <summary>
        /// 审核通过的用户系统编号
        /// </summary>
        public int ConfirmUserSysNo { get; set; }
        /// <summary>
        /// 审核通过的时间
        /// </summary>
        public DateTime? ConfirmTime { get; set; }
        /// <summary>
        /// 加积分申请的状态
        /// </summary>
        public CustomerPointsAddRequestStatus? Status { get; set; }
        /// <summary>
        /// 审核备注
        /// </summary>
        public string ConfirmNote { get; set; }
        /// <summary>
        /// 补偿性积分时，对应的责任部门
        /// </summary>
        public string OwnByDepartment { get; set; }
        /// <summary>
        /// PM组系统编号
        /// </summary>
        public string PMGroupSysNos { get; set; }
        /// <summary>
        /// PM组名称
        /// </summary>
        public string PMGroupNames { get; set; }
        /// <summary>
        /// 创建人编号，当审核不通过的时候会给这个用户发送邮件
        /// </summary>
        public int CreateUserSysNo { get; set; }
        /// <summary>
        /// 系统账户（给用户发积分的系统账户）
        /// </summary>
        public string NewEggAccount { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 给顾客加积分时对应订单的明细
        /// </summary>
        public List<CustomerPointsAddRequestItem> PointsItemList { get; set; }

        public string ProductID { get; set; }
    }
    /// <summary>
    /// 给顾客加积分时对应订单的明细
    /// </summary>
    public class CustomerPointsAddRequestItem
    {
        /// <summary>
        /// 加积分申请的系统编号
        /// </summary>
        public int PointsAddRequestSysNo { get; set; }
        /// <summary>
        /// 订单中产品编号
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 产品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 产品数量
        /// </summary>
        public int? Quantity { get; set; }
        /// <summary>
        /// 加积分涉及的订单编号
        /// </summary>
        public int? SOSysNo { get; set; }
        /// <summary>
        /// 积分数量
        /// </summary>
        public int? Point { get; set; }
    }
}

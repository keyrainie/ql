using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// RMA单件信息
    /// </summary>
    public class RMARegisterInfo : IIdentity, IWebChannel
    {
        private int? sysNo;

        public RMARegisterInfo()
        {
            this.BasicInfo = new RegisterBasicInfo();
            this.CheckInfo = new RegisterCheckInfo();
            this.ResponseInfo = new RegisterResponseInfo();
            this.RevertInfo = new RegisterRevertInfo();
            this.SellerInfo = new SellerRelatedInfo();
            this.ReturnInfo = new RegisterReturnInfo();
            this.InternalMemos = new List<InternalMemoInfo>();
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                this.sysNo = value;
                this.BasicInfo.SysNo = value;
                this.CheckInfo.SysNo = value;
                this.RevertInfo.SysNo = value;
                this.ResponseInfo.SysNo = value;
            }
        }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public WebChannel WebChannel { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 申请类型
        /// </summary>
        public RMARequestType? RequestType { get; set; }

        /// <summary>
        /// 基本信息
        /// </summary>
        public RegisterBasicInfo BasicInfo { get; set; }

        /// <summary>
        /// 检测信息
        /// </summary>
        public RegisterCheckInfo CheckInfo { get; set; }

        /// <summary>
        /// 送修返还信息
        /// </summary>
        public RegisterResponseInfo ResponseInfo { get; set; }       

        /// <summary>
        /// 发货信息
        /// </summary>
        public RegisterRevertInfo RevertInfo { get; set; }

        /// <summary>
        /// 退还信息
        /// </summary>
        public RegisterReturnInfo ReturnInfo { get; set; }

        /// <summary>
        /// 商家相关信息
        /// </summary>
        public SellerRelatedInfo SellerInfo { get; set; }

        /// <summary>
        /// RMA跟进日志
        /// </summary>
        public List<InternalMemoInfo> InternalMemos { get; set; }        
    }
}
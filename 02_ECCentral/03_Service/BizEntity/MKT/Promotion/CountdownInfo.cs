using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.MKT.Promotion;


namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 限时销售：限时抢购，限时促销，秒杀
    /// </summary>
    public class CountdownInfo : IIdentity, IWebChannel, ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 是否促销计划 1是, 0否-限时抢购
        /// </summary>
        public bool? IsPromotionSchedule { get; set; }

        /// <summary>
        /// 是否是秒杀字段，为DC就是秒杀，否则为Null
        /// </summary>
        public bool? IsSecondKill { get; set; }

        /// <summary>
        /// 是否是团购
        /// </summary>
        public bool? IsGroupOn { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CountdownStatus? Status { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 促销计划标题
        /// </summary>
        public string PromotionTitle { get; set; }

        #region 促销活动显示位置相关配置

        /// <summary>
        /// 是否在限时抢购专区限时
        /// </summary>
        public bool? IsCountDownAreaShow { get; set; }

        /// <summary>
        /// 在限时抢购专区限时的优先级
        /// </summary>
        public int? AreaShowPriority { get; set; }

        /// <summary>
        /// 是否在首页限时抢购位置显示
        /// </summary>
        public bool? IsHomePageShow { get; set; }

        /// <summary>
        /// 在首页限时抢购位置显示的优先级
        /// </summary>
        public int? HomePagePriority { get; set; }

        /// <summary>
        /// 是否在今日特价位置显示
        /// </summary>
        public bool? IsTodaySpecials { get; set; }

        /// <summary>
        /// 是否在一级分类页面显示
        /// </summary>
        public bool? IsC1Show { get; set; }

        /// <summary>
        /// 是否在二级分类页面显示 
        /// </summary>
        public bool? IsC2Show { get; set; }

        /// <summary>
        /// 是否在24小时预告中显示
        /// </summary>
        public bool? Is24hNotice { get; set; }

        /// <summary>
        /// 在24小时预告中显示时是否显示商品的促销价格
        /// </summary>
        public bool? IsShowPriceInNotice { get; set; }

        #endregion

        #region 商品相关的设置


        /// <summary>
        /// 活动商品编码
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 活动商品ID
        /// </summary>
        public string ProductID { get; set; }



        #region 产品数量相关

        /// <summary>
        /// 拿出来做活动的产品总数量
        /// </summary>
        public int? CountDownQty { get; set; }

        /// <summary>
        /// 是否限量发售
        /// </summary>
        public bool? IsLimitedQty { get; set; }

        /// <summary>
        /// 限量发售的时候，如果限制的数量卖完了，是否需要结束这个促销
        /// </summary>
        public bool? IsEndIfNoQty { get; set; }

        /// <summary>
        /// 安全库存预警,-1时不预警，
        /// </summary>        
        public int? BaseLine { get; set; }

        /// <summary>
        /// 是否预留库存
        /// </summary>
        public bool? IsReservedQty { get; set; }

        /// <summary>
        /// 每单限购数量
        /// </summary>
        public int? MaxPerOrder { get; set; }

        /// <summary>
        /// 此次活动涉及到所有仓库的库存量明细
        /// </summary>
        public List<StockQtySetting> AffectedStockList { get; set; }

        /// <summary>
        /// 此次活动涉及到虚库的库存
        /// </summary>
        public int? AffectedVirtualQty { get; set; }

        /// <summary>
        /// 活动开始时涉及到的个仓库的虚库快照
        /// </summary>
        public string SnapShotCurrentVirtualQty { get; set; }

        #endregion

        #region 价格和优惠

        /// <summary>
        /// 促销时商品的卖价
        /// </summary>
        public decimal? CountDownCurrentPrice { get; set; }

        /// <summary>
        /// 促销时商品折扣
        /// </summary>
        public decimal? CountDownCashRebate { get; set; }

        /// <summary>
        /// 促销活动赠送礼券数量
        /// </summary>
        public decimal? CouponRebate { get; set; }

        /// <summary>
        /// 促销活动赠送积分数量
        /// </summary>
        public int? CountDownPoint { get; set; }


        /// <summary>
        /// 快照价格（活动开始时的商品实际售价）
        /// </summary>
        public decimal? SnapShotCurrentPrice { get; set; }

        /// <summary>
        ///  快照礼券数量（活动开始时的商品实际送礼券数量）
        /// </summary>
        public decimal? SnapShotCashRebate { get; set; }

        /// <summary>
        /// 快照积分数量（活动开始时的商品实际送积分数量）
        /// </summary>
        public int? SnapShotPoint { get; set; }

        #endregion

        /// <summary>
        /// 开展此次促销活动的理由
        /// </summary>
        public string Reasons { get; set; }

        /// <summary>
        /// 审核批注
        /// </summary>
        public string VerifyMemo { get; set; }


        #endregion

        #region dto
        /// <summary>
        /// 创建用户系统编号
        /// </summary>
        public int? CreateUserSysNo { get; set; }
        /// <summary>
        /// 是否提交审核
        /// </summary>
        public bool? IsSubmitAudit { get; set; }

        #endregion
        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }


        /// <summary>
        /// 兼容数据库做的字段，业务中使用AffectedStockList， 数据库中是拼接的字符串
        /// </summary>
        public string AffectedStock
        {
            get
            {
                string affectedStockStr = string.Empty;
                if (AffectedStockList != null)
                {
                    AffectedStockList.ForEach(item =>
                    {
                        affectedStockStr += string.Format("{0}:{1};", item.StockSysNo, item.Qty);
                    });
                }
                return affectedStockStr;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    List<StockQtySetting> _AffectedStockList = new List<StockQtySetting>();
                    foreach (var item in value.Split(';'))
                    {
                        if (!string.IsNullOrEmpty(item) && item.Split(':').Count() == 2)
                        {
                            _AffectedStockList.Add(new StockQtySetting()
                            {
                                StockSysNo = int.Parse(item.Split(':')[0]),
                                Qty = int.Parse(string.IsNullOrEmpty(item.Split(':')[1]) ? "0" : item.Split(':')[1])
                            });
                        }
                    }
                    AffectedStockList = _AffectedStockList;
                }
            }
        }

        /// <summary>
        /// 所属商家编号
        /// </summary>
        public int VendorSysNo { get; set; }
        /// <summary>
        /// 所属商家
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// Seller Portal请求编号
        /// </summary>
        public int RequestSysNo { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public int PMRole { get; set; }

        /// <summary>
        /// 导入的文件服务器端地址
        /// </summary>
        public string FileIdentity { get; set; }
    }


}

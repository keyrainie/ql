using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECommerce.Enums;
using ECommerce.Entity.Product;

namespace ECommerce.Entity.Promotion
{  
    /// <summary>
    ///促销模板
    /// </summary>
    [Serializable]
    [DataContract]
    public class SaleAdvertisement
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }
        /// <summary>
        /// 页面名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 页面头部
        /// </summary>
        [DataMember]
        public string Header { get; set; }
        /// <summary>
        /// 页面底部
        /// </summary>
        [DataMember]
        public string Footer { get; set; }
        /// <summary>
        /// 样式路径
        /// </summary>
        [DataMember]
        public string CssPath { get; set; }
        /// <summary>
        /// 状态 无效=  -1 有效=  0
        /// </summary>
        [DataMember]
        public int Status { get; set; }

        /// <summary>
        /// 是否分组显示  0=否 1=是
        /// </summary>
        [DataMember]
        public int IsGroupByCategory { get; set; }
        /// <summary>
        /// 分组类型  0=中类 1=小类 2=自定义
        /// </summary>
        [DataMember]
        public int GroupType { get; set; }

        /// <summary>
        ///是否展示评论  0=否 1=是
        /// </summary>
        [DataMember]
        public int EnableComment { get; set; }

        /// <summary>
        /// 回复等级
        /// </summary>
        [DataMember]
        public int EnableReplyRank { get; set; }


        [DataMember]
        public string IsHold { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]

        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string JumpAdvertising { get; set; }

        public List<SaleAdvertisementGroup> SaleAdvertisementGroupList { get; set; }

    }


    /// <summary>
    /// 促销模板组
    /// </summary>
    [Serializable]
    [DataContract]
    public class SaleAdvertisementGroup 
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        [DataMember]
        public string GroupName { get; set; }

        /// <summary>
        /// 展示开始日期
        /// </summary>
        [DataMember]
        public DateTime ShowStartDate { get; set; }

        /// <summary>
        /// 展示结束日期
        /// </summary>
        [DataMember]
        public DateTime ShowEndDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
          
        [DataMember]
        public string  Status { get; set; }

        /// <summary>
        /// 展示优先级
        /// </summary>

        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// 分组更多链接
        /// </summary>

        [DataMember]
        public string OtherGroupLink { get; set; }

        /// <summary>
        /// 定位锚ID
        /// </summary>

        [DataMember]
        public string GroupIDForAnchor { get; set; }

        /// <summary>
        /// 分组Banner（HTML）
        /// </summary>

        [DataMember]
        public string GroupBannerHTML { get; set; }

        /// <summary>
        /// 组广告图片资源地址
        /// </summary>

        [DataMember]
        public string GroupImgResourceAddr { get; set; }

        /// <summary>
        /// 组广告图片资源链接
        /// </summary>

        [DataMember]
        public string GroupImgResourceLink { get; set; }

        /// <summary>
        /// 边框色
        /// </summary>

        [DataMember]
        public string BorderColor { get; set; }

        /// <summary>
        /// 标题字体色
        /// </summary>

        [DataMember]
        public string TitleForeColor { get; set; }
        
        /// <summary>
        /// 标题背景色
        /// </summary>

        [DataMember]
        public string TitleBackColor { get; set; }
        
        /// <summary>
        /// 组商品推荐方式
        /// </summary>
        [DataMember]
        public GroupReCommandType IsRecommend { get; set; }

        public List<SaleAdvertisementItem> SaleAdvertisementItemList { get; set; }

    }

    /// <summary>
    /// 促销模板项
    /// </summary>
    [Serializable]
    [DataContract]
    public class SaleAdvertisementItem
    {
        /// <summary>
        /// 系统编号
        /// </summary>

        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// 促销模板编号
        /// </summary>

        [DataMember]
        public int SaleAdvSysNo { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>

        [DataMember]
        public int Priority { get; set; }
        /// <summary>
        /// 推荐方式
        /// </summary>

        [DataMember]
        public int IsRecommend { get; set; }
        /// <summary>
        /// 促销模板组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 促销模板组编号
        /// </summary>

        [DataMember]
        public int GroupSysNo { get; set; }
        /// <summary>
        /// 状态
        /// </summary>

        [DataMember]
        public string Status { get; set; }
        /// <summary>
        /// Icon地址
        /// </summary>

        [DataMember]
        public string IconAddr { get; set; }

        #region  商品信息
        
        /// <summary>
        /// 商品编号
        /// </summary>

        [DataMember]
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 商品Code
        /// </summary>

        [DataMember]
        public string ProductCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>

        [DataMember]
        public ProductStatus ProductStatus { get; set; }
        /// <summary>
        /// 类型
        /// </summary>

        [DataMember]
        public ProductType ProductType { get; set; }
        /// <summary>
        ///默认图片
        /// </summary>

        [DataMember]
        public string DefaultImage { get; set; }
        /// <summary>
        /// 促销标题
        /// </summary>

        [DataMember]
        public string PromotionTitle { get; set; }
        /// <summary>
        /// 商品标题
        /// </summary>

        [DataMember]
        public string ProductTitle { get; set; }
        /// <summary>
        /// 当前销售价
        /// </summary>

        [DataMember]
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 返现
        /// </summary>

        [DataMember]
        public decimal CashRebate { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>

        [DataMember]
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 税率
        /// </summary>

        [DataMember]
        public decimal TariffRate { get; set; }


        /// <summary>
        /// 是否包含赠品
        /// </summary>

        public bool IsHaveValidGift { get; set; }

        /// <summary>
        /// 进口税
        /// </summary>
        public decimal ProductTariffAmt
        {
            get {
                return this.CurrentPrice * this.TariffRate;
            }
        }

        #endregion

    }
}

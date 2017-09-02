using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.MemberService
{
    /// <summary>
    /// 企业(商家)基本信息
    /// </summary>
    public class StoreBasicInfoModel
    {
        #region  企业(商家)基本信息
        /// <summary>
        ///系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        ///商家(卖家)系统编号
        /// </summary>
        public int? SellerSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///企业名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        ///公司地址
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        ///公司网址
        /// </summary>
        public string Site
        {
            get;
            set;
        }

        /// <summary>
        ///联系人
        /// </summary>
        public string ContactName
        {
            get;
            set;
        }

        /// <summary>
        ///手机
        /// </summary>
        public string Mobile
        {
            get;
            set;
        }

        /// <summary>
        ///固话
        /// </summary>
        public string Phone
        {
            get;
            set;
        }

        /// <summary>
        ///电子邮箱
        /// </summary>
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        ///QQ号码
        /// </summary>
        public string QQ
        {
            get;
            set;
        }

        /// <summary>
        ///是否有电商经验
        /// </summary>
        public int? HaveECExp
        {
            get;
            set;
        }

        /// <summary>
        ///已有年经验
        /// </summary>
        public int? ECExpValue
        {
            get;
            set;
        }

        /// <summary>
        ///现有电商渠道
        /// </summary>
        public string CurrentECChannel
        {
            get;
            set;
        }

        /// <summary>
        ///是否有进出口经验
        /// </summary>
        public int? HaveExportExp
        {
            get;
            set;
        }

        /// <summary>
        ///已有年经验
        /// </summary>
        public int? ExportExpValue
        {
            get;
            set;
        }

        /// <summary>
        ///主营商品品类
        /// </summary>
        public string MainProductCategory
        {
            get;
            set;
        }

        /// <summary>
        ///主要品牌列表
        /// </summary>
        public string MainBrand
        {
            get;
            set;
        }

        /// <summary>
        ///品牌授权情况
        /// </summary>
        public string BrandAuthorize
        {
            get;
            set;
        }

        /// <summary>
        ///意向合作模式
        /// </summary>
        public string CooperationMode
        {
            get;
            set;
        }

        /// <summary>
        ///其他补充介绍
        /// </summary>
        public string Remark
        {
            get;
            set;
        }

        /// <summary>
        ///1:可用,0:不可用
        /// </summary>
        public int? Status
        {
            get;
            set;
        }

        /// <summary>
        ///创建者系统编号
        /// </summary>
        public int? InUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///创建者显示名
        /// </summary>
        public string InUserName
        {
            get;
            set;
        }

        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime? InDate
        {
            get;
            set;
        }

        /// <summary>
        ///最后修改人系统编号
        /// </summary>
        public int? EditUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///最后修改人显示名
        /// </summary>
        public string EditUserName
        {
            get;
            set;
        }

        /// <summary>
        ///最后修改时间
        /// </summary>
        public DateTime? EditDate
        {
            get;
            set;
        }

        public string StoreName
        {
            get;
            set;
        }

        public string LogoURL
        {
            get;
            set;
        }

        public DateTime ValidDate { get; set; }
        #endregion

        /// <summary>
        /// 是否被收藏
        /// </summary>
        public bool StoreIsWished { get; set; }
    }
}
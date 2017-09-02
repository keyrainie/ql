using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store
{
    /// <summary>
    ///企业(商家)基本信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class StoreBasicInfo
    {

        /// <summary>
        ///系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        ///商家(卖家)系统编号
        /// </summary>
        [DataMember]
        public int? SellerSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///店铺名称
        /// </summary>
        [DataMember]
        public string StoreName
        {
            get;
            set;
        }

        /// <summary>
        ///LogoURL
        /// </summary>
        [DataMember]
        public string LogoURL
        {
            get;
            set;
        }

        /// <summary>
        ///企业名称
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        ///公司地址
        /// </summary>
        [DataMember]
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        ///公司网址
        /// </summary>
        [DataMember]
        public string Site
        {
            get;
            set;
        }

        /// <summary>
        ///联系人
        /// </summary>
        [DataMember]
        public string ContactName
        {
            get;
            set;
        }

        /// <summary>
        ///手机
        /// </summary>
        [DataMember]
        public string Mobile
        {
            get;
            set;
        }

        /// <summary>
        ///固话
        /// </summary>
        [DataMember]
        public string Phone
        {
            get;
            set;
        }

        /// <summary>
        ///电子邮箱
        /// </summary>
        [DataMember]
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        ///QQ号码
        /// </summary>
        [DataMember]
        public string QQ
        {
            get;
            set;
        }

        /// <summary>
        ///是否有电商经验
        /// </summary>
        [DataMember]
        public int? HaveECExp
        {
            get;
            set;
        }

        /// <summary>
        ///已有年经验
        /// </summary>
        [DataMember]
        public int? ECExpValue
        {
            get;
            set;
        }

        /// <summary>
        ///现有电商渠道
        /// </summary>
        [DataMember]
        public string CurrentECChannel
        {
            get;
            set;
        }

        /// <summary>
        ///是否有进出口经验
        /// </summary>
        [DataMember]
        public int? HaveExportExp
        {
            get;
            set;
        }

        /// <summary>
        ///已有年经验
        /// </summary>
        [DataMember]
        public int? ExportExpValue
        {
            get;
            set;
        }

        /// <summary>
        ///主营商品品类
        /// </summary>
        [DataMember]
        public string MainProductCategory
        {
            get;
            set;
        }

        /// <summary>
        ///主要品牌列表
        /// </summary>
        [DataMember]
        public string MainBrand
        {
            get;
            set;
        }

        /// <summary>
        ///品牌授权情况
        /// </summary>
        [DataMember]
        public string BrandAuthorize
        {
            get;
            set;
        }

        /// <summary>
        ///意向合作模式
        /// </summary>
        [DataMember]
        public string CooperationMode
        {
            get;
            set;
        }

        /// <summary>
        ///其他补充介绍
        /// </summary>
        [DataMember]
        public string Remark
        {
            get;
            set;
        }

        /// <summary>
        ///1:可用,0:不可用
        /// </summary>
        [DataMember]
        public int? Status
        {
            get;
            set;
        }

        /// <summary>
        ///创建者系统编号
        /// </summary>
        [DataMember]
        public int? InUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///创建者显示名
        /// </summary>
        [DataMember]
        public string InUserName
        {
            get;
            set;
        }

        /// <summary>
        ///创建时间
        /// </summary>
        [DataMember]
        public DateTime? InDate
        {
            get;
            set;
        }

        /// <summary>
        ///最后修改人系统编号
        /// </summary>
        [DataMember]
        public int? EditUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///最后修改人显示名
        /// </summary>
        [DataMember]
        public string EditUserName
        {
            get;
            set;
        }

        /// <summary>
        ///最后修改时间
        /// </summary>
        [DataMember]
        public DateTime? EditDate
        {
            get;
            set;
        }


    }
}

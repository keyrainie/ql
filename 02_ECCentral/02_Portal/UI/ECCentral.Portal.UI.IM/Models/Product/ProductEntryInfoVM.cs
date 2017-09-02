using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.IM.Models.Product
{
    public class ProductEntryInfoVM : ModelBase
    {
        public ProductEntryInfoVM()
        {
            this.EntryBizTypeList = EnumConverter.GetKeyValuePairs<EntryBizType>(EnumConverter.EnumAppendItemType.Select);
            this.StoreTypeList = EnumConverter.GetKeyValuePairs<StoreType>(EnumConverter.EnumAppendItemType.Select);
            this.WhetherList = new List<KeyValuePair<int, string>>();
            this.WhetherList.Add(new KeyValuePair<int, string>(0, "否"));
            this.WhetherList.Add(new KeyValuePair<int, string>(1, "是"));
        }

        public List<KeyValuePair<int, string>> WhetherList { get; set; }

        /// <summary>
        /// 是否需效期
        /// </summary>
        private int? _NeedValid;
        public int? NeedValid
        {
            get { return _NeedValid; }
            set { SetValue("NeedValid", ref _NeedValid, value); }
        }

        /// <summary>
        /// 是否需黏贴中文标签
        /// </summary>
        private int? _NeedLabel;
        public int? NeedLabel
        {
            get { return _NeedLabel; }
            set { SetValue("NeedLabel", ref _NeedLabel, value); }
        }

        /// <summary>
        /// 产品不属于我国禁止进境物
        /// </summary>
        private int? _NotProhibitedEntry;
        public int? NotProhibitedEntry
        {
            get { return _NotProhibitedEntry; }
            set { SetValue("NotProhibitedEntry", ref _NotProhibitedEntry, value); }
        }

        /// <summary>
        /// 产品不在1712号公告名录内
        /// </summary>
        private int? _NotInNotice1712;
        public int? NotInNotice1712
        {
            get { return _NotInNotice1712; }
            set { SetValue("NotInNotice1712", ref _NotInNotice1712, value); }
        }

        /// <summary>
        /// 商品不属于转基因产品
        /// </summary>
        private int? _NotTransgenic;
        public int? NotTransgenic
        {
            get { return _NotTransgenic; }
            set { SetValue("NotTransgenic", ref _NotTransgenic, value); }
        }

        /// <summary>
        /// HSCode
        /// </summary>
        private string _HSCode;
        public string HSCode
        {
            get { return _HSCode; }
            set { SetValue("HSCode", ref _HSCode, value); }
        }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        private int? _productSysNo;
        public int? ProductSysNo
        {
            get { return _productSysNo; }
            set { SetValue("ProductSysNo", ref _productSysNo, value); }
        }


        /// <summary>
        /// 系统编号
        /// </summary>
        private int? _SysNo;
        public int? SysNo
        {
            get { return _SysNo; }
            set { SetValue("SysNo", ref _SysNo, value); }
        }

        /// <summary>
        /// 商品英文名称
        /// </summary>
        private string _ProductName_EN;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength,60)]
        public string ProductName_EN
        {
            get { return _ProductName_EN; }
            set { SetValue("ProductName_EN", ref _ProductName_EN, value); }
        }

        /// <summary>
        /// 规则
        /// </summary>
        private string _Specifications;
        [Validate(ValidateType.Required)]
        public string Specifications
        {
            get { return _Specifications; }
            set { SetValue("Specifications", ref _Specifications, value); }
        }

        /// <summary>
        /// 功能
        /// </summary>
        private string _Functions;
        [Validate(ValidateType.Required)]
        public string Functions
        {
            get { return _Functions; }
            set { SetValue("Functions", ref _Functions, value); }
        }


        /// <summary>
        /// 成份
        /// </summary>
        private string _Component;
        [Validate(ValidateType.Required)]
        public string Component
        {
            get { return _Component; }
            set { SetValue("Component", ref _Component, value); }
        }

        /// <summary>
        /// 产地
        /// </summary>
        private string _Origin;
        [Validate(ValidateType.Required)]
        public string Origin
        {
            get { return _Origin; }
            set { SetValue("Origin", ref _Origin, value); }
        }

        /// <summary>
        /// 用途
        /// </summary>
        private string _Purpose;
        [Validate(ValidateType.Required)]
        public string Purpose
        {
            get { return _Purpose; }
            set { SetValue("Purpose", ref _Purpose, value); }
        }

        /// <summary>
        /// 计税单位数量
        /// </summary>
        private string _TaxQty;
        //[Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength,10)]
        public string TaxQty
        {
            get { return _TaxQty; }
            set { SetValue("TaxQty", ref _TaxQty, value); }
        }

        /// <summary>
        /// 计税单位
        /// </summary>
        private string _TaxUnit;
        //[Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength,3)]
        public string TaxUnit
        {
            get { return _TaxUnit; }
            set { SetValue("TaxUnit", ref _TaxUnit, value); }
        }

        /// <summary>
        /// 申报单位
        /// </summary>
        private string _ApplyUnit;
        [Validate(ValidateType.MaxLength, 3)]
        public string ApplyUnit
        {
            get { return _ApplyUnit; }
            set { SetValue("ApplyUnit", ref _ApplyUnit, value); }
        }

        /// <summary>
        ///  毛重
        /// </summary>
        private string _GrossWeight;
        public string GrossWeight
        {
            get { return _GrossWeight; }
            set { SetValue("GrossWeight", ref _GrossWeight, value); }
        }

        /// <summary>
        /// 业务类型
        /// </summary>
        private EntryBizType? _BizType;
        [Validate(ValidateType.Required)]
        public EntryBizType? BizType
        {
            get { return _BizType; }
            set { SetValue("BizType", ref _BizType, value); }
        }

        /// <summary>
        /// 物资序号	自贸专区商品完成自贸区货物备案后，填写该值
        /// </summary>
        private string _Supplies_Serial_No;
        [Validate(ValidateType.MaxLength, 20)]
        public string Supplies_Serial_No
        {
            get { return _Supplies_Serial_No; }
            set { SetValue("Supplies_Serial_No", ref _Supplies_Serial_No, value); }
        }

        /// <summary>
        /// 货号	自贸专区商品完成自贸区货物备案后，归并关系中的企业内部货号
        /// </summary>
        private string _Product_SKUNO;
         [Validate(ValidateType.MaxLength, 60)]
        public string Product_SKUNO
        {
            get { return _Product_SKUNO; }
            set { SetValue("Product_SKUNO", ref _Product_SKUNO, value); }
        }

        /// <summary>
        /// 净重
        /// </summary>
        private string _SuttleWeight;
        public string SuttleWeight
        {
            get { return _SuttleWeight; }
            set { SetValue("SuttleWeight", ref _SuttleWeight, value); }
        }

        /// <summary>
        /// 申报数量
        /// </summary>
        private string _ApplyQty;
        [Validate(ValidateType.Interger)]
        public string ApplyQty
        {
            get { return _ApplyQty; }
            set { SetValue("ApplyQty", ref _ApplyQty, value); }
        }

        /// <summary>
        /// 其他备注
        /// </summary>
        private string _Note;
        public string Note
        {
            get { return _Note; }
            set { SetValue("Note", ref _Note, value); }
        }

        private string _TariffCode;
        public string TariffCode
        {
            get { return _TariffCode; }
            set { SetValue("TariffCode", ref _TariffCode, value); }
        }

        private string _Unit;
        public string Unit
        {
            get { return _Unit; }
            set { SetValue("Unit", ref _Unit, value); }
        }

        private string _TariffPrice;
        public string TariffPrice
        {
            get { return _TariffPrice; }
            set { SetValue("TariffPrice", ref _TariffPrice, value); }
        }

        private string _TariffRate;
        public string TariffRate
        {
            get { return _TariffRate; }
            set { SetValue("TariffRate", ref _TariffRate, value); }
        }



        /// <summary>
        /// 业务类型
        /// </summary>
        private StoreType? _StoreType;
        [Validate(ValidateType.Required)]
        public StoreType? StoreType
        {
            get { return _StoreType; }
            set { SetValue("StoreType", ref _StoreType, value); }
        }

        public List<KeyValuePair<EntryBizType?, string>> EntryBizTypeList { get; set; }

        public List<KeyValuePair<StoreType?, string>> StoreTypeList { get; set; }

        private string _Supplies_Serial_No_1;
         [Validate(ValidateType.MaxLength, 60)]
        public string Supplies_Serial_No_1
        {
            get { return _Supplies_Serial_No_1; }
            set { SetValue("Supplies_Serial_No_1", ref _Supplies_Serial_No_1, value); }
        }

        /// <summary>
        /// •	进口检疫审批许可证确认（自贸）
        /// </summary>
        private string _Remark1;
         [Validate(ValidateType.MaxLength, 200)]
        public string Remark1
        {
            get { return _Remark1; }
            set { SetValue("Remark1", ref _Remark1, value); }
        }

        /// <summary>
        /// 输出国或地区官方出具检疫证书确认（自贸）
        /// </summary>
        private string _Remark2;
        [Validate(ValidateType.MaxLength, 200)]
        public string Remark2
        {
            get { return _Remark2; }
            set { SetValue("Remark2", ref _Remark2, value); }
        }

        /// <summary>
        /// 原产地证明确认（自贸）
        /// </summary>
        private string _Remark3;
        [Validate(ValidateType.MaxLength, 200)]
        public string Remark3
        {
            get { return _Remark3; }
            set { SetValue("Remark3", ref _Remark3, value); }
        }

        /// <summary>
        /// 	品牌方授权确认（直邮）
        /// </summary>
        private string _Remark4;
        [Validate(ValidateType.MaxLength, 200)]
        public string Remark4
        {
            get { return _Remark4; }
            set { SetValue("Remark4", ref _Remark4, value); }
        }

        /// <summary>
        /// 	产地
        /// </summary>
        private string _OrginName;
        public string OrginName
        {
            get { return _OrginName; }
            set { SetValue("OrginName", ref _OrginName, value); }
        }

        /// <summary>
        /// 	其他名称
        /// </summary>
        private string _ProductOthterName;
        public string ProductOthterName
        {
            get { return _ProductOthterName; }
            set { SetValue("ProductOthterName", ref _ProductOthterName, value); }
        }

        /// <summary>
        /// 	出厂日期
        /// </summary>
        private DateTime? _ManufactureDate;
        public DateTime? ManufactureDate
        {
            get { return _ManufactureDate; }
            set { SetValue("ManufactureDate", ref _ManufactureDate, value); }
        }

        /// <summary>
        /// 	备案号
        /// </summary>
        private string _EntryCode;
         [Validate(ValidateType.MaxLength, 20)]
        public string EntryCode
        {
            get { return _EntryCode; }
            set { SetValue("EntryCode", ref _EntryCode, value); }
        }
        /// <summary>
        /// 默认备货天数
        /// </summary>
        private string _DefaultLeadTimeDays;
        [Validate(ValidateType.MaxLength, 8)]
        public string DefaultLeadTimeDays
        {
            get { return _DefaultLeadTimeDays; }
            set { SetValue("DefaultLeadTimeDays", ref _DefaultLeadTimeDays, value); }
        }

        /// <summary>
        /// 贸易类型
        /// </summary>
        private TradeType _ProductTradeType;
        public TradeType ProductTradeType
        {
            get { return _ProductTradeType; }
            set { SetValue("ProductTradeType", ref _ProductTradeType, value); }
        }

        /// <summary>
        /// 备案状态
        /// </summary>
        public ProductEntryStatus EntryStatus { get; set; }

        /// <summary>
        /// 备案状态
        /// </summary>
        private string _EntryStatusDisplay;
        public string EntryStatusDisplay
        {
            get {
                switch (EntryStatus)
                {
                    case ProductEntryStatus.Origin:
                        return "初始化";
                    case ProductEntryStatus.AuditFail:
                        return "审核失败";
                    case ProductEntryStatus.EntryFail:
                        return "备案失败";
                    case ProductEntryStatus.WaitingAudit:
                        return "待审核";
                    case ProductEntryStatus.AuditSucess:
                        return "审核成功";
                    case ProductEntryStatus.Entry:
                        return "备案中";
                    case ProductEntryStatus.EntrySuccess:
                        return "备案成功";
                }
                return _EntryStatusDisplay; 
            }
            set { SetValue("EntryStatusDisplay", ref _EntryStatusDisplay, value); }
        }

        /// <summary>
        /// 备案扩展状态
        /// </summary>
        public ProductEntryStatusEx? EntryStatusEx { get; set; }

        /// <summary>
        /// 备案扩展状态
        /// </summary>
        private string _EntryStatusExDisplay;
        public string EntryStatusExDisplay
        {
            get
            {
                switch (EntryStatusEx)
                {
                    case ProductEntryStatusEx.CustomsFail:
                        return "报关失败";
                    case ProductEntryStatusEx.InspectionFail:
                        return "商检失败";
                    case ProductEntryStatusEx.Inspection:
                        return "待商检";
                    case ProductEntryStatusEx.InspectionSucess:
                        return "商检成功";
                    case ProductEntryStatusEx.Customs:
                        return "待报关";
                    case ProductEntryStatusEx.CustomsSuccess:
                        return "报关成功";
                }
                return _EntryStatusDisplay;
            }
            set { SetValue("EntryStatusExDisplay", ref _EntryStatusExDisplay, value); }
        }
    }
}

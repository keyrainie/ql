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
using System.Collections.Generic;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOShippingInfoVM : ModelBase
    {
        private Int32? m_SOSysNo;
        public Int32? SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private Int32? m_StockSysNo;
        public Int32? StockSysNo
        {
            get { return this.m_StockSysNo; }
            set { this.SetValue("StockSysNo", ref m_StockSysNo, value); }
        }

        private Int32? m_ShipTypeSysNo;
        [Validate(ValidateType.Required)]
        public Int32? ShipTypeSysNo
        {
            get { return this.m_ShipTypeSysNo; }
            set { this.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value); }
        }

        private Int32? m_AllocatedManSysNo;
        public Int32? AllocatedManSysNo
        {
            get { return this.m_AllocatedManSysNo; }
            set { this.SetValue("AllocatedManSysNo", ref m_AllocatedManSysNo, value); }
        }

        private Int32? m_FreightUserSysNo;
        public Int32? FreightUserSysNo
        {
            get { return this.m_FreightUserSysNo; }
            set { this.SetValue("FreightUserSysNo", ref m_FreightUserSysNo, value); }
        }

        private String m_PackageID;
        public String PackageID
        {
            get { return this.m_PackageID; }
            set { this.SetValue("PackageID", ref m_PackageID, value); }
        }

        private Int32? m_OutUserSysNo;
        public Int32? OutUserSysNo
        {
            get { return this.m_OutUserSysNo; }
            set { this.SetValue("OutUserSysNo", ref m_OutUserSysNo, value); }
        }

        private DateTime? m_OutTime;
        public DateTime? OutTime
        {
            get { return this.m_OutTime; }
            set { this.SetValue("OutTime", ref m_OutTime, value); }
        }

        private String m_DeliveryMemo;
        public String DeliveryMemo
        {
            get { return this.m_DeliveryMemo; }
            set { this.SetValue("DeliveryMemo", ref m_DeliveryMemo, value); }
        }

        private DateTime? m_DeliveryDate;
        public DateTime? DeliveryDate
        {
            get { return this.m_DeliveryDate; }
            set { this.SetValue("DeliveryDate", ref m_DeliveryDate, value); }
        }

        private Int32? m_DeliveryTimeRange;
        public Int32? DeliveryTimeRange
        {
            get { return this.m_DeliveryTimeRange; }
            set { this.SetValue("DeliveryTimeRange", ref m_DeliveryTimeRange, value); }
        }

        private String m_RingOutShipType;
        public String RingOutShipType
        {
            get { return this.m_RingOutShipType; }
            set { this.SetValue("RingOutShipType", ref m_RingOutShipType, value); }
        }

        private String m_DeliverySection;
        public String DeliverySection
        {
            get { return this.m_DeliverySection; }
            set { this.SetValue("DeliverySection", ref m_DeliverySection, value); }
        }

        private SODeliveryPromise m_DeliveryPromise;
        public SODeliveryPromise DeliveryPromise
        {
            get { return this.m_DeliveryPromise; }
            set { this.SetValue("DeliveryPromise", ref m_DeliveryPromise, value); }
        }

        private Decimal? m_Weight;
        public Decimal? Weight
        {
            get { return this.m_Weight; }
            set { this.SetValue("Weight", ref m_Weight, value); }
        }

        private Decimal? m_ShippingFee;
        public Decimal? ShippingFee
        {
            get { return this.m_ShippingFee; }
            set { this.SetValue("ShippingFee", ref m_ShippingFee, value); }
        }

        private Decimal? m_PackageFee;
        public Decimal? PackageFee
        {
            get { return this.m_PackageFee; }
            set { this.SetValue("PackageFee", ref m_PackageFee, value); }
        }

        private Decimal? m_RegisteredFee;
        public Decimal? RegisteredFee
        {
            get { return this.m_RegisteredFee; }
            set { this.SetValue("RegisteredFee", ref m_RegisteredFee, value); }
        }

        private Int32? m_Weight3PL;
        public Int32? Weight3PL
        {
            get { return this.m_Weight3PL; }
            set { this.SetValue("Weight3PL", ref m_Weight3PL, value); }
        }

        private Decimal? m_ShipCost;
        public Decimal? ShipCost
        {
            get { return this.m_ShipCost; }
            set { this.SetValue("ShipCost", ref m_ShipCost, value); }
        }

        private Decimal? m_ShipCost3PL;
        public Decimal? ShipCost3PL
        {
            get { return this.m_ShipCost3PL; }
            set { this.SetValue("ShipCost3PL", ref m_ShipCost3PL, value); }
        }

        private Decimal? m_OriginShipPrice;
        public Decimal? OriginShipPrice
        {
            get { return this.m_OriginShipPrice; }
            set { this.SetValue("OriginShipPrice", ref m_OriginShipPrice, value); }
        }

        private BizEntity.PO.VirtualPurchaseOrderStatus? m_VPOStatus;
        public BizEntity.PO.VirtualPurchaseOrderStatus? VPOStatus
        {
            get { return this.m_VPOStatus; }
            set { this.SetValue("VPOStatus", ref m_VPOStatus, value); }
        }

        private Boolean? m_IsCombine;
        public Boolean? IsCombine
        {
            get { return this.m_IsCombine; }
            set { this.SetValue("IsCombine", ref m_IsCombine, value); }
        }

        private Boolean? m_IsMergeComplete;
        public Boolean? IsMergeComplete
        {
            get { return this.m_IsMergeComplete; }
            set { this.SetValue("IsMergeComplete", ref m_IsMergeComplete, value); }
        }

        private DateTime? m_MergeCompleteTime;
        public DateTime? MergeCompleteTime
        {
            get { return this.m_MergeCompleteTime; }
            set { this.SetValue("MergeCompleteTime", ref m_MergeCompleteTime, value); }
        }

        private DateTime? m_MergeOutTime;
        public DateTime? MergeOutTime
        {
            get { return this.m_MergeOutTime; }
            set { this.SetValue("MergeOutTime", ref m_MergeOutTime, value); }
        }

        private Int32? m_DestWarehouseNumber;
        public Int32? DestWarehouseNumber
        {
            get { return this.m_DestWarehouseNumber; }
            set { this.SetValue("DestWarehouseNumber", ref m_DestWarehouseNumber, value); }
        }

        private Int32? m_StockStatus;
        public Int32? StockStatus
        {
            get { return this.m_StockStatus; }
            set { this.SetValue("StockStatus", ref m_StockStatus, value); }
        }

        private Int32? m_LocalWHSysNo;
        public Int32? LocalWHSysNo
        {
            get { return this.m_LocalWHSysNo; }
            set { this.SetValue("LocalWHSysNo", ref m_LocalWHSysNo, value); }
        }

        private StockType? m_StockType;
        public StockType? StockType
        {
            get { return this.m_StockType; }
            set { this.SetValue("StockType", ref m_StockType, value); }
        }

        private string m_TrackingNumberStr;
        public string TrackingNumberStr
        {
            get { return this.m_TrackingNumberStr; }
            set { this.SetValue("TrackingNumberStr", ref m_TrackingNumberStr, value); }
        }

        private string m_OfficialWebsite;
        public string OfficialWebsite
        {
            get { return this.m_OfficialWebsite; }
            set { this.SetValue("OfficialWebsite", ref m_OfficialWebsite, value); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECCentral.Job.Invoice.AutoRunSOFreight.Biz;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.Log;

namespace ECCentral.Job.Invoice.AutoRunSOFreight
{
    public class Processor : IJobAction
    {
        public void Run(JobContext context)
        {
            var soList = FreightDA.GetSOList();
            foreach (var so in soList)
            {
                var tracking = FreightDA.GetTrackingNumber(so.SOSysNo, int.Parse(so.LocalWHSysNo));
                if (tracking != null && !string.IsNullOrEmpty(tracking.TrackingNumber))
                {
                    //订单运单号和实际重量
                    so.TrackingNumber = tracking.TrackingNumber;
                    so.RealWeight = tracking.RealWeight;

                    //订单的实际支出运费
                    #region 【 开始计算运费 】
                    List<ShippingFeeQueryInfo> qryList = new List<ShippingFeeQueryInfo>();
                    var qry = new ShippingFeeQueryInfo();
                    qry.TransID = so.SOSysNo.ToString();
                    qry.SoAmount = so.SoAmount;
                    qry.SoTotalWeight = Convert.ToInt32(so.RealWeight.ToString("F0")) * 1000;
                    qry.SOSingleMaxWeight = so.SOSingleMaxWeight;
                    qry.AreaId = so.ReceiveAreaSysNo;
                    qry.CustomerSysNo = so.CustomerSysNo;
                    qry.IsUseDiscount = 0;
                    qry.SubShipTypeList = so.ShipTypeSysNo.ToString();

                    VendorInfo vendorInfo = FreightDA.GetVendorInfo(so.MerchantSysNo);
                    qry.SellType = vendorInfo == null ? 0 : Convert.ToInt32(vendorInfo.SellerType);
                    qry.MerchantSysNo = so.MerchantSysNo;

                    qry.ShipTypeId = 0;
                    qryList.Add(qry);

                    List<ShippingInfo> shipFeeList = FreightDA.GetAllShippingFee(qryList);
                    ShippingInfo curShippingInfo = null;

                    curShippingInfo = shipFeeList.Find(x => x.TransID == so.SOSysNo.ToString() && x.ShippingTypeID.ToString() == so.ShipTypeSysNo.ToString());
                    if (curShippingInfo != null)
                    {
                        so.RealPayFreight = curShippingInfo.ShippingPrice;
                    }
                    #endregion

                    //创建统计订单运费
                    if (FreightDA.GetSOFreightStat(so.SOSysNo) == null)
                    {
                        FreightDA.CreateSOFreightStat(so);
                    }
                }
            }
        }
    }
}

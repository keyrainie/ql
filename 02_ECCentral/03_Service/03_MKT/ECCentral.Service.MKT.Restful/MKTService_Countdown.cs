using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService.Promotion;
using ECCentral.Service.MKT.Restful.ResponseMsg;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;


namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        [WebGet(UriTemplate = "/GetAllCountdownCreateUser/{channleID}")]
        public virtual List<UserInfo> GetAllCountdownCreateUser(string channleID)
        {
            return ObjectFactory<CountdownAppService>.Instance.GetAllCountdownCreateUser(channleID);
        }
        /// <summary>
        /// 团购分页查询服务
        /// </summary>
        [WebInvoke(UriTemplate = "/Countdown/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryCountdown(CountdownQueryFilter msg)
        {
            int totalCount = 0;
            DataTable dt = ObjectFactory<ICountdownQueryDA>.Instance.Query(msg, out totalCount);

            return new QueryResult() { Data = dt, TotalCount = totalCount };
        }

        [WebGet(UriTemplate = "/CountdownInfo/{sysNo}")]
        public virtual CountdownInfo LoadCountdown(string sysNo)
        {
            int id = 0;
            if (!int.TryParse(sysNo, out id))
            {
                throw new BizException("请传入有效的活动编号！");
            }

            return ObjectFactory<CountdownAppService>.Instance.Load(id);
        }

        [WebGet(UriTemplate = "/Countdown/GetQuickTimes")]
        public virtual List<CodeNamePair> GetQuickTimes()
        {
            return ObjectFactory<CountdownAppService>.Instance.GetQuickTimes();
        }

        [WebInvoke(UriTemplate = "/Countdown/Create", Method = "POST")]
        public CountdownInfo CreateCountdown(CountdownInfo info)
        {
            //检查是否有创建权限
            if (info.PMRole == 0)
            {
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_SorryYouNotHaveAuthority"));
            }
            return ObjectFactory<CountdownAppService>.Instance.CreateCountdown(info);
        }

        [WebInvoke(UriTemplate = "/Countdown/Update", Method = "PUT")]
        public CountdownInfo UpdateCountdown(CountdownInfo info)
        {
            return ObjectFactory<CountdownAppService>.Instance.UpdateCountdown(info);
        }


        [WebInvoke(UriTemplate = "/Countdown/Abandon", Method = "PUT")]
        public CountdownInfo AbandonCountdown(CountdownInfo info)
        {
            return ObjectFactory<CountdownAppService>.Instance.AbandonCountdown(info);
        }

        [WebInvoke(UriTemplate = "/Countdown/Interrupt", Method = "PUT")]
        public CountdownInfo InterruptCountdown(CountdownInfo info)
        {
            return ObjectFactory<CountdownAppService>.Instance.InterruptCountdown(info);
        }

        [WebInvoke(UriTemplate = "/Countdown/Verify", Method = "PUT")]
        public void VerifyCountdown(CountdownInfo info)
        {
            ObjectFactory<CountdownAppService>.Instance.VerifyCountdown(info);
        }


        [WebInvoke(UriTemplate = "/Countdown/CheckOptionalAccessoriesInfoMsg", Method = "PUT")]
        public string CheckOptionalAccessoriesInfoMsg(CountdownInfo info)
        {
            return ObjectFactory<CountdownAppService>.Instance.CheckOptionalAccessoriesInfoMsg(info);
        }

        [WebInvoke(UriTemplate = "/MKTProductDetailMsg/{sysNo}", Method = "GET")]
        public virtual MKTProductDetailMsg LoadMKTProductDetail(string sysNo)
        {
            int id = Convert.ToInt32(sysNo);
            MKTProductDetailMsg msg = new MKTProductDetailMsg();
            ProductInfo product = ObjectFactory<CountdownAppService>.Instance.GetProductInfo(id);
            ProductInventoryInfo inventory = ObjectFactory<CountdownAppService>.Instance.GetProductTotalInventoryInfo(id);
            DateTime? lastPoDate = ObjectFactory<CountdownAppService>.Instance.GetLastPoDate(id);
            ProductSalesTrendInfo salesTrendInfo = ObjectFactory<CountdownAppService>.Instance.GetProductTotalSalesTrendInfo(id);
            decimal grossMarginRate = ObjectFactory<CountdownAppService>.Instance.GetProductCurrentMarginRate(product);
            if (product != null)
            {
                msg.ProductSysNo = product.SysNo;
                msg.ProductName = product.ProductName;
                msg.UnitCost = product.ProductPriceInfo.UnitCost;
                msg.Price = product.ProductPriceInfo.CurrentPrice;
                msg.Point = product.ProductPriceInfo.Point;
            }
            if (inventory != null)
            {
                msg.AvailableQty = inventory.AvailableQty + inventory.VirtualQty + inventory.ConsignQty;//q4s
                msg.AccountQty = inventory.AccountQty;
                msg.ReservedQty = inventory.ReservedQty;
                msg.CurrentReservedQty = 0;
                msg.GrossMarginRate = grossMarginRate;
                msg.JDPrice = 0.00m;
            }
            if (salesTrendInfo != null)
            {
                msg.W1 = salesTrendInfo.W1.Value;
                msg.W2 = salesTrendInfo.W2.Value;
                msg.W3 = salesTrendInfo.W3.Value;
                msg.W4 = salesTrendInfo.W4.Value;
                msg.M1 = salesTrendInfo.M1.Value;
                msg.M2 = salesTrendInfo.M2.Value;
                msg.M3 = salesTrendInfo.M3.Value;
            }
            msg.LastPurchaseDate = lastPoDate;
            return msg;
        }

        [WebInvoke(UriTemplate = "/Countdown/GetGrossMargin", Method = "POST")]
        public virtual GrossMarginMsg GetGrossMargin(CountdownInfo entity)
        {
            GrossMarginMsg msg = new GrossMarginMsg();
            decimal GrossMargin = 0;
            decimal GrossMarginWithOutPointAndGift = 0;
            decimal GrossMarginRate = 0;
            decimal GrossMarginRateWithOutPointAndGift = 0;
            var giftSysNo = 0;
            var couponSysNo = 0;
            //获取蛋劵和赠品编号
            if (entity.ProductSysNo.HasValue)
            {
                ObjectFactory<ICountdownQueryDA>.Instance.GetGiftAndCouponSysNo(entity.ProductSysNo.Value, out giftSysNo, out couponSysNo);
                if (giftSysNo != -1)
                {
                    msg.GiftSysNo = giftSysNo;
                }
                if (couponSysNo != -1)
                {
                    msg.CouponSysNo = couponSysNo;
                }
            }
            ObjectFactory<CountdownAppService>.Instance.GetGrossMargin(entity, out GrossMargin, out GrossMarginWithOutPointAndGift, out GrossMarginRate, out GrossMarginRateWithOutPointAndGift);
            msg.GrossMargin = GrossMargin;
            msg.GrossMarginWithOutPointAndGift = GrossMarginWithOutPointAndGift;
            msg.GrossMarginRate = GrossMarginRate;
            msg.GrossMarginRateWithOutPointAndGift = GrossMarginRateWithOutPointAndGift;
            msg.CountDownMargin = Convert.ToDecimal(AppSettingManager.GetSetting("MKT", "CountDownMargin"));
            msg.CountDownMarginRate = Convert.ToDecimal(AppSettingManager.GetSetting("MKT", "CountDownMarginRate"));
            return msg;
        }

        [WebInvoke(UriTemplate = "/Countdown/GetVendorList", Method = "GET")]
        public virtual List<ECCentral.BizEntity.PO.VendorInfo> GetCountdownVendorList() 
        {
            return ObjectFactory<CountdownAppService>.Instance.GetCountdownVendorList();
        }
        /// <summary>
        /// 导入限时抢购
        /// </summary>
        /// <param name="fileIdentity"></param>
        [WebInvoke(UriTemplate = "/Countdown/BatchImportCountDown", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual void BatchImportCountDown(CountdownInfo entity)
        {

            ObjectFactory<CountdownAppService>.Instance.ImportCountInfo(entity.FileIdentity, entity.PMRole, false);
        }
        /// <summary>
        /// 导入促销计划
        /// </summary>
        /// <param name="fileIdentity"></param>
        [WebInvoke(UriTemplate = "/Countdown/BatchImportSchedule", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual void BatchImportSchedule(CountdownInfo entity)
        {

            ObjectFactory<CountdownAppService>.Instance.ImportCountInfo(entity.FileIdentity, entity.PMRole, true);
        }

        [WebGet(UriTemplate = "/GetPMByProductSysNo/{sysNo}")]
        public virtual string GetPMByProductSysNo(string sysNo)
        {
            return ObjectFactory<ICountdownQueryDA>.Instance.GetPMByProductSysNo(sysNo);
        }
    }
}

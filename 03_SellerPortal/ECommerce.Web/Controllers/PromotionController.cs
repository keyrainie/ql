using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Promotion;
using ECommerce.Service.Promotion;
using ECommerce.Enums;
using ECommerce.WebFramework;
using ECommerce.Entity.Common;
using ECommerce.Service.Common;
using ECommerce.Utility;
using System.Text;

namespace ECommerce.Web.Controllers
{
    public partial class PromotionController : SSLControllerBase
    {
        #region [赠品促销 相关:]

        public ActionResult GiftPromotionList()
        {
            return View();
        }
        [HttpPost]

        public JsonResult GetGiftPromotionList()
        {
            GiftPromotionListQueryFilter queryFilter = BuildQueryFilterEntity<GiftPromotionListQueryFilter>((s) =>
            {
                s.VendorSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
                s.CompanyCode = UserAuthHelper.GetCurrentUser().CompanyCode;
            });

            var result = GiftPromotionService.QueryGiftPromotionActivityList(queryFilter);
            return AjaxGridJson(result);
        }


        public ActionResult GiftPromotionEdit(int? sysNo)
        {
            if (!sysNo.HasValue)
            {
                //新建赠品促销活动:
                return View();
            }
            else
            {
                //编辑赠品促销活动:
                var editPromotionInfo = GiftPromotionService.LoadGiftPromotionInfo(sysNo.Value);
                return View(editPromotionInfo);
            }
        }


        [HttpPost]
        public JsonResult SaveGiftPromotion()
        {
            var result = 0;
            if (!string.IsNullOrEmpty(Request["giftInfo"]))
            {
                SalesGiftInfo giftInfo = SerializationUtility.JsonDeserialize2<SalesGiftInfo>(Request["giftInfo"]);
                if (null != giftInfo)
                {
                    giftInfo.SellerSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
                    giftInfo.InUserSysNo = UserAuthHelper.GetCurrentUser().UserSysNo;
                    giftInfo.InUserName = UserAuthHelper.GetCurrentUser().UserDisplayName;
                    giftInfo.EditUserSysNo = UserAuthHelper.GetCurrentUser().UserSysNo;
                    giftInfo.EditUser = giftInfo.EditUserName = UserAuthHelper.GetCurrentUser().UserDisplayName;
                }
                result = GiftPromotionService.SaveGiftPromotionInfo(giftInfo, UserAuthHelper.GetCurrentUser().UserDisplayName);
            }
            return Json(new { Data = result });
        }
        [HttpPost]
        public JsonResult PublishGiftPromotion(int giftPromotionSysNo)
        {
            GiftPromotionService.SubmitAudit(giftPromotionSysNo, UserAuthHelper.GetCurrentUser().UserDisplayName);
            return Json(new { Data = true });
        }
        [HttpPost]
        public JsonResult AbandonGiftPromotion(int giftPromotionSysNo)
        {
            GiftPromotionService.Void(giftPromotionSysNo, UserAuthHelper.GetCurrentUser().UserDisplayName);
            return Json(new { Data = true });
        }
        [HttpPost]
        public JsonResult TerminateGiftPromotion(int giftPromotionSysNo)
        {
            GiftPromotionService.Stop(giftPromotionSysNo, UserAuthHelper.GetCurrentUser().UserDisplayName);
            return Json(new { Data = true });
        }
        [HttpPost]
        public JsonResult AjaxBatchPublish()
        {
            List<int> sysnoList = SerializationUtility.JsonDeserialize2<List<int>>(Request.Params["sysNoList"]);
            var batchResult = GiftPromotionService.BatchSubmitAudit(sysnoList, UserAuthHelper.GetCurrentUser().UserDisplayName);
            return Json(new { successCount = batchResult.Item1, failCount = batchResult.Item2, resultMsg = batchResult.Item3 });
        }
        [HttpPost]
        public JsonResult AjaxBatchAbandon()
        {
            List<int> sysnoList = SerializationUtility.JsonDeserialize2<List<int>>(Request.Params["sysNoList"]);
            var batchResult = GiftPromotionService.BatchAbandon(sysnoList, UserAuthHelper.GetCurrentUser().UserDisplayName);
            return Json(new { successCount = batchResult.Item1, failCount = batchResult.Item2, resultMsg = batchResult.Item3 });
        }
        [HttpPost]
        public JsonResult AjaxBatchTerminate()
        {
            List<int> sysnoList = SerializationUtility.JsonDeserialize2<List<int>>(Request.Params["sysNoList"]);
            var batchResult = GiftPromotionService.BatchStop(sysnoList, UserAuthHelper.GetCurrentUser().UserDisplayName);
            return Json(new { successCount = batchResult.Item1, failCount = batchResult.Item2, resultMsg = batchResult.Item3 });
        }

        #endregion

        #region [优惠券 相关:]

        public ActionResult CouponList()
        {
            return View();
        }

        public ActionResult CouponCodeCustomerLogQuery()
        {
            return View();
        }

        public ActionResult CouponCodeRedeemLogQuery()
        {
            return View();
        }

        public ActionResult CouponEdit()
        {
            return View();
        }

        [HttpPost]
        [ActionName("QueryCoupon")]
        public ActionResult AjaxCouponQuery()
        {
            CouponQueryFilter filter = BuildQueryFilterEntity<CouponQueryFilter>();
            filter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var result = CouponService.QueryCouponList(filter);
            return AjaxGridJson(result);
        }

        [HttpGet]
        [ActionName("LoadCoupon")]
        public JsonResult AjaxLoadCoupon()
        {
            string CouponSysNoStr = Request["CouponSysNo"];
            int CouponSysNo = 0;
            int.TryParse(CouponSysNoStr, out CouponSysNo);
            if (CouponSysNo < 1)
            {
                throw new ECommerce.Utility.BusinessException(LanguageHelper.GetText("数据无效"));
            }
            UserAuthVM user = UserAuthHelper.GetCurrentUser();
            if (user == null)
            {
                throw new ECommerce.Utility.BusinessException();
            }
            Coupon info = CouponService.Load(CouponSysNo);
            if (info != null)
            {
                if (info.MerchantSysNo != user.SellerSysNo)
                {
                    throw new ECommerce.Utility.BusinessException(LanguageHelper.GetText("您没有权限访问该数据"));
                }
            }

            return Json(info);
        }

        [HttpPost]
        [ActionName("VoidCoupon")]
        public JsonResult AjaxVoidCoupon(int CouponSysNo)
        {
            UserAuthVM user = UserAuthHelper.GetCurrentUser();
            if (user == null)
            {
                throw new ECommerce.Utility.BusinessException();
            }
            CouponService.VoidCoupon(CouponSysNo, user.SellerSysNo, user.UserID);

            return Json(new { Data = true });
        }

        [HttpPost]
        [ActionName("StopCoupon")]
        public JsonResult AjaxStopCoupon(int CouponSysNo)
        {
            UserAuthVM user = UserAuthHelper.GetCurrentUser();
            if (user == null)
            {
                throw new ECommerce.Utility.BusinessException();
            }
            CouponService.StopCoupon(CouponSysNo, user.SellerSysNo, user.UserID);

            return Json(new { Data = true });
        }

        [HttpPost]
        [ActionName("SaveCoupon")]
        public JsonResult AjaxSaveCoupon()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            Coupon info = ECommerce.Utility.SerializationUtility.JsonDeserialize2<Coupon>(dataString);
            SetBizEntityUserInfo(info, info.SysNo.HasValue ? false : true);

            UserAuthVM user = UserAuthHelper.GetCurrentUser();
            if (user == null)
            {
                throw new ECommerce.Utility.BusinessException();
            }
            info.Status = CouponStatus.Init;
            info.MerchantSysNo = user.SellerSysNo;
            info.EditUser = user.UserID;
            info.InUser = user.UserID;
            if (info.SysNo.HasValue && info.SysNo > 0)
            {

                info = CouponService.Update(info);
            }
            else
            {
                info = CouponService.Create(info);
            }

            return Json(new { Data = info });
        }

        [HttpPost]
        [ActionName("SubmitCoupon")]
        public JsonResult AjaxSubmitCoupon()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            Coupon info = ECommerce.Utility.SerializationUtility.JsonDeserialize2<Coupon>(dataString);
            SetBizEntityUserInfo(info, info.SysNo.HasValue ? false : true);

            UserAuthVM user = UserAuthHelper.GetCurrentUser();
            if (user == null)
            {
                throw new ECommerce.Utility.BusinessException();
            }
            info.Status = CouponStatus.WaitingAudit;
            info.MerchantSysNo = user.SellerSysNo;
            info.EditUser = user.UserID;
            info.InUser = user.UserID;
            if (info.SysNo.HasValue && info.SysNo > 0)
            {
                info = CouponService.Update(info);
            }
            else
            {
                info = CouponService.Create(info);
            }

            return Json(new { Data = info });
        }

        [HttpPost]
        [ActionName("CreateCouponCode")]
        public JsonResult AjaxCreateCouponCode(int CouponSysNo)
        {
            UserAuthVM user = UserAuthHelper.GetCurrentUser();
            if (user == null)
            {
                throw new ECommerce.Utility.BusinessException();
            }
            string code = CouponService.GenerateRandomCode(10);

            return Json(code);
        }
        /// <summary>
        /// 批量生成通用优惠券代码
        /// </summary>
        /// <param name="Num">批量生成数量</param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("BitchCreateCouponCode")]
        public JsonResult AjaxBitchCreateCouponCode(int Num)
        {
            string code = "";
            UserAuthVM user = UserAuthHelper.GetCurrentUser();
            if (user == null)
            {
                throw new ECommerce.Utility.BusinessException();
            }
            for (int i = 0; i < Num; i++)
            {
                code += CouponService.GenerateRandomCode(10) + "\n";
            }
            return Json(code);
        }

        [HttpPost]
        [ActionName("QueryCustomers")]
        public ActionResult AjaxCustomerQuery()
        {
            CustomerQueryFilter filter = BuildQueryFilterEntity<CustomerQueryFilter>();
            //filter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var result = CommonService.QueryCustomers(filter);
            return AjaxGridJson(result);
        }

        [HttpPost]
        [ActionName("QueryCouponCode")]
        public ActionResult AjaxCouponCodeQuery()
        {
            CouponCodeQueryFilter filter = BuildQueryFilterEntity<CouponCodeQueryFilter>();
            filter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var result = CouponService.QueryCouponCodeList(filter);
            return AjaxGridJson(result);
        }

        [HttpPost]
        [ActionName("QueryCouponCodeRedeemLog")]
        public ActionResult AjaxCouponCodeRedeemLogQuery()
        {
            CouponCodeRedeemLogFilter filter = BuildQueryFilterEntity<CouponCodeRedeemLogFilter>();
            filter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var result = CouponService.QueryCouponCodeRedeemLog(filter);
            return AjaxGridJson(result);
        }

        [HttpPost]
        [ActionName("QueryCouponCodeCustomerLog")]
        public ActionResult AjaxCouponCodeCustomerLogQuery()
        {
            CouponCodeCustomerLogFilter filter = BuildQueryFilterEntity<CouponCodeCustomerLogFilter>();
            filter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var result = CouponService.QueryCouponCodeCustomerLog(filter);
            return AjaxGridJson(result);
        }

        [HttpPost]
        [ActionName("BatchVoidCoupon")]
        public JsonResult AjaxBatchVoidCoupon()
        {
            UserAuthVM user = UserAuthHelper.GetCurrentUser();
            if (user == null)
            {
                throw new ECommerce.Utility.BusinessException();
            }
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            var CouponSysNos = ECommerce.Utility.SerializationUtility.JsonDeserialize2<int[]>(dataString);
            {
                int merchantSysNo = user.SellerSysNo;
                string userName = user.UserID;
                if (CouponSysNos == null || CouponSysNos.Length < 1)
                {
                    throw new BusinessException(LanguageHelper.GetText("批量操作数据不能为空"));
                }
                StringBuilder sb = new StringBuilder();
                int errorCount = 0;
                int successCount = 0;
                foreach (int CouponSysNo in CouponSysNos)
                {
                    try
                    {
                        CouponService.VoidCoupon(CouponSysNo, merchantSysNo, userName);
                        successCount++;
                    }
                    catch (BusinessException ex)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动编号：{0} {1}<br/>"), CouponSysNo, ex.Message));
                        errorCount++;
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动编号：{0} {1}<br/>"), CouponSysNo, ex.Message));
                        errorCount++;
                    }
                }
                if (sb.Length > 0)
                {

                    StringBuilder exMessage = new StringBuilder();
                    exMessage.AppendLine(string.Format(LanguageHelper.GetText("操作已完成！成功{0}条，失败{1}条<br/>"), successCount, errorCount));
                    exMessage.AppendLine(sb.ToString());

                    throw new BusinessException(exMessage.ToString());
                }
            }
            return Json(new { Data = true });
        }

        [HttpPost]
        [ActionName("BatchStopCoupon")]
        public JsonResult AjaxBatchStopCoupon()
        {
            UserAuthVM user = UserAuthHelper.GetCurrentUser();
            if (user == null)
            {
                throw new ECommerce.Utility.BusinessException();
            }
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            var CouponSysNos = ECommerce.Utility.SerializationUtility.JsonDeserialize2<int[]>(dataString);
            {
                int merchantSysNo = user.SellerSysNo;
                string userName = user.UserID;
                if (CouponSysNos == null || CouponSysNos.Length < 1)
                {
                    throw new BusinessException(LanguageHelper.GetText("批量操作数据不能为空"));
                }
                StringBuilder sb = new StringBuilder();
                int errorCount = 0;
                int successCount = 0;
                foreach (int CouponSysNo in CouponSysNos)
                {
                    try
                    {
                        CouponService.StopCoupon(CouponSysNo, merchantSysNo, userName);
                        successCount++;
                    }
                    catch (BusinessException ex)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动编号：{0} {1}<br/>"), CouponSysNo, ex.Message));
                        errorCount++;
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动编号：{0} {1}<br/>"), CouponSysNo, ex.Message));
                        errorCount++;
                    }
                }
                if (sb.Length > 0)
                {

                    StringBuilder exMessage = new StringBuilder();
                    exMessage.AppendLine(string.Format(LanguageHelper.GetText("操作已完成！成功{0}条，失败{1}条<br/>"), successCount, errorCount));
                    exMessage.AppendLine(sb.ToString());

                    throw new BusinessException(exMessage.ToString());
                }
            }
            return Json(new { Data = true });
        }


        [HttpPost]
        [ActionName("BatchSubmitCoupon")]
        public JsonResult AjaxBatchSubmitCoupon()
        {
            UserAuthVM user = UserAuthHelper.GetCurrentUser();
            if (user == null)
            {
                throw new ECommerce.Utility.BusinessException();
            }
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            var CouponSysNos = ECommerce.Utility.SerializationUtility.JsonDeserialize2<int[]>(dataString);
            {
                int merchantSysNo = user.SellerSysNo;
                string userName = user.UserID;
                if (CouponSysNos == null || CouponSysNos.Length < 1)
                {
                    throw new BusinessException(LanguageHelper.GetText("批量操作数据不能为空"));
                }
                StringBuilder sb = new StringBuilder();
                int errorCount = 0;
                int successCount = 0;
                foreach (int CouponSysNo in CouponSysNos)
                {
                    try
                    {
                        Coupon info = CouponService.Load(CouponSysNo);
                        if (merchantSysNo != info.MerchantSysNo)
                        {
                            throw new BusinessException(LanguageHelper.GetText("您没有权限操作该数据"));
                        }
                        if (info.Status != CouponStatus.Init)
                        {
                            throw new ECommerce.Utility.BusinessException(LanguageHelper.GetText("只有初始化状态的活动才能提交！"));
                        }
                        CouponService.CheckCoupon(info);
                        CouponService.UpdateStatus(CouponSysNo, CouponStatus.WaitingAudit, userName);
                        successCount++;
                    }
                    catch (BusinessException ex)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动编号：{0} {1}<br/>"), CouponSysNo, ex.Message));
                        errorCount++;
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动编号：{0} {1}<br/>"), CouponSysNo, ex.Message));
                        errorCount++;
                    }
                }
                if (sb.Length > 0)
                {

                    StringBuilder exMessage = new StringBuilder();
                    exMessage.AppendLine(string.Format(LanguageHelper.GetText("操作已完成！成功{0}条，失败{1}条<br/>"), successCount, errorCount));
                    exMessage.AppendLine(sb.ToString());

                    throw new BusinessException(exMessage.ToString());
                }
            }
            return Json(new { Data = true });
        }
        #endregion

    }
}

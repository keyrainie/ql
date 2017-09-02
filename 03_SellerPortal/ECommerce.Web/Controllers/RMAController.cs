using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity;
using ECommerce.Entity.Common;
using ECommerce.Entity.RMA;
using ECommerce.Enums;
using ECommerce.Service.RMA;
using ECommerce.Utility;

namespace ECommerce.Web.Controllers
{
    public class RMAController : SSLControllerBase
    {
        //
        // GET: /Service/
        
        public ActionResult RequestOrderQuery()
        {
            return View();
        }

        [HttpPost]
        [ActionName("RequestOrderQuery")]
        public ActionResult AjaxRequestOrderQuery()
        {
            RMARequestQueryFilter filter = BuildQueryFilterEntity<RMARequestQueryFilter>();
            filter.SellerSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var result = RMARequestService.RMARequestOrderQuery(filter);

            return AjaxGridJson(result);
        }

        
        public ActionResult RequestOrderDetail()
        {
            return View();
        }

        [HttpPost]
        [ActionName("RequestValid")]
        public ActionResult AjaxValidRequest()
        {
            var sysnoData = Request["SysNo"];
            int sysno;
            if (!int.TryParse(sysnoData, out sysno) || sysno <= 0)
            {
                throw new BusinessException(L("退换货申请单编号不正确"));
            }

            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());
            RMARequestInfo data = RMARequestService.Valid(sysno, user);
            return Json(data);
        }

        [HttpPost]
        [ActionName("RequestReject")]
        public ActionResult AjaxRejectRequest()
        {
            var sysnoData = Request["SysNo"];
            int sysno;
            if (!int.TryParse(sysnoData, out sysno) || sysno <= 0)
            {
                throw new BusinessException(L("退换货申请单编号不正确"));
            }
            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());
            RMARequestInfo data = RMARequestService.Reject(sysno, user);
            return Json(data);
        }

        [HttpPost]
        [ActionName("RequestReceive")]
        public ActionResult AjaxReceiveRequest()
        {
            var sysnoData = Request["SysNo"];
            int sysno;
            if (!int.TryParse(sysnoData, out sysno) || sysno <= 0)
            {
                throw new BusinessException(L("退换货申请单编号不正确"));
            }

            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());
            RMARequestInfo data = RMARequestService.Receive(sysno, user);
            return Json(data);
        }

        [HttpPost]
        [ActionName("RequestAbandon")]
        public ActionResult AjaxAbandonRequest()
        {
            var sysnoData = Request["SysNo"];
            int sysno;
            if (!int.TryParse(sysnoData, out sysno) || sysno <= 0)
            {
                throw new BusinessException(L("退换货申请单编号不正确"));
            }
            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());
            RMARequestInfo data = RMARequestService.Abandon(sysno, user);
            return Json(data);
        }

        [HttpPost]
        [ActionName("RequestRevert")]
        public ActionResult AjaxRevertRequest()
        {
            var sysnoData = Request["SysNo"];
            int sysno;
            if (!int.TryParse(sysnoData, out sysno) || sysno <= 0)
            {
                throw new BusinessException(L("退换货申请单编号不正确"));
            }
            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());
            RMARequestInfo data = RMARequestService.Revert(sysno, user);
            return Json(data);
        }

        /// <summary>
        /// 生成退款单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("RequestRefund")]
        public ActionResult AjaxRefundRequest()
        {
            var sysnoData = Request["SysNo"];
            int sysno;
            if (!int.TryParse(sysnoData, out sysno) || sysno <= 0)
            {
                throw new BusinessException(L("退换货申请单编号不正确"));
            }
            var refundData = Request["RefundInfo"];
            RMARefundInfo refundInfo = SerializationUtility.JsonDeserialize<RMARefundInfo>(refundData);
            if (string.IsNullOrWhiteSpace(refundData) || refundInfo == null)
            {
                throw new BusinessException(L("退款信息为空"));
            }
            //退款方式为“网关直接退款”
            refundInfo.RefundPayType = RefundPayType.NetWorkRefund;

            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());
            RMARequestInfo data = RMARequestService.Refund(sysno, refundInfo, user);
            return Json(data);
        }

        [HttpPost]
        [ActionName("RequestBatchValid")]
        public JsonResult AjaxBatchValidRequest()
        {
            var sysnoListData = Request["SysNoList"];
            List<int> sysNoList = SerializationUtility.JsonDeserialize<List<int>>(sysnoListData);
            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());

            var data = DoBatchAction(sysNoList, user, RMARequestService.Valid);
            return Json(data);
        }

        [HttpPost]
        [ActionName("RequestBatchReject")]
        public JsonResult AjaxBatchRejectRequest()
        {
            var sysnoListData = Request["SysNoList"];
            List<int> sysNoList = SerializationUtility.JsonDeserialize<List<int>>(sysnoListData);
            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());

            var data = DoBatchAction(sysNoList, user, RMARequestService.Reject);
            return Json(data);
        }

        [HttpPost]
        [ActionName("RequestBatchReceive")]
        public JsonResult AjaxBatchReceiveRequest()
        {
            var sysnoListData = Request["SysNoList"];
            List<int> sysNoList = SerializationUtility.JsonDeserialize<List<int>>(sysnoListData);
            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());

            var data = DoBatchAction(sysNoList, user, RMARequestService.Receive);
            return Json(data);
        }

        [HttpPost]
        [ActionName("RequestBatchAbandon")]
        public JsonResult AjaxBatchAbandonRequest()
        {
            var sysnoListData = Request["SysNoList"];
            List<int> sysNoList = SerializationUtility.JsonDeserialize<List<int>>(sysnoListData);
            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());

            var data = DoBatchAction(sysNoList, user, RMARequestService.Abandon);
            return Json(data);
        }

        [NonAction]
        private static dynamic DoBatchAction(List<int> items, LoginUser user, Func<int, LoginUser, EntityBase> action)
        {
            if (items == null || items.Count <= 0)
            {
                return L("请选择要操作的记录");
            }
            StringBuilder info = new StringBuilder();
            StringBuilder error = new StringBuilder();
            int successRecords = 0, failRecords = 0;
            foreach (int item in items)
            {
                try
                {
                    action(item, user);
                    successRecords++;
                }
                catch (BusinessException ex)
                {
                    error.AppendLine(L("编号{0}：{1}<br />", item, ex.Message));
                    failRecords++;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            info.AppendLine(L("成功{0}条，失败{1}条<br />", successRecords, failRecords));
            if (failRecords > 0)
            {
                info.Append(error);
            }
            return new { Data = info.ToString() };
        }

        
        public ActionResult RefundOrderQuery()
        {
            return View();
        }

        [HttpGet]
        [ActionName("RefundInfo")]
        public ActionResult LoadRefundWithSysNo(int refundSysNo)
        {
            RMARefundInfo refundInfo = RMARefundService.LoadWithRefundSysNo(refundSysNo, UserAuthHelper.GetCurrentUser().SellerSysNo);
            return PartialView("_RefundOrderPop", refundInfo ?? new RMARefundInfo());
        }

        [HttpPost]
        [ActionName("RefundOrderQuery")]
        public ActionResult AjaxRefundOrderQuery()
        {
            RMARefundQueryFilter filter = BuildQueryFilterEntity<RMARefundQueryFilter>();
            filter.SellerSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var result = RMARefundService.RMARefundOrderQuery(filter);

            return AjaxGridJson(result);
        }
        /// <summary>
        /// 退款单--审核通过
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("RefundBatchValid")]
        public JsonResult AjaxBatchRejectRefund()
        {
            var sysnoListData = Request["SysNoList"];
            List<int> sysNoList = SerializationUtility.JsonDeserialize<List<int>>(sysnoListData);
            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());

            var data = DoBatchAction(sysNoList, user, RMARefundService.Valid);
            return Json(data);
        }
        /// <summary>
        /// 退款单--审核拒绝
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("RefundBatchReject")]
        public JsonResult AjaxRefundBatchReject()
        {
            var sysnoListData = Request["SysNoList"];
            List<int> sysNoList = SerializationUtility.JsonDeserialize<List<int>>(sysnoListData);
            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());

            var data = DoBatchAction(sysNoList, user, RMARefundService.Reject);
            return Json(data);
        }
        /// <summary>
        /// 退款单--确认退款
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("RefundBatchConfirm")]
        public JsonResult AjaxRefundBatchConfirm()
        {
            var sysnoListData = Request["SysNoList"];
            List<int> sysNoList = SerializationUtility.JsonDeserialize<List<int>>(sysnoListData);
            LoginUser user = EntityConverter<UserAuthVM, LoginUser>.Convert(UserAuthHelper.GetCurrentUser());

            var data = DoBatchAction(sysNoList, user, RMARefundService.Confirm);
            return Json(data);
        }


        private static string L(string key, params object[] args)
        {
            string multiLangText = ECommerce.WebFramework.LanguageHelper.GetText(key);
            return string.Format(multiLangText, args);
        }
    }
}

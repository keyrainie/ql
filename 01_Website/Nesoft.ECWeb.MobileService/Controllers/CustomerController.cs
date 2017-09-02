using System.Web.Mvc;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.MobileService.Models.Member;
using Nesoft.ECWeb.UI;
using Nesoft.ECWeb.Entity.Shipping;
using System.Collections.Generic;
using Nesoft.ECWeb.Entity;
using System.Threading;
using Nesoft.ECWeb.MobileService.Models.Order;
using Nesoft.ECWeb.MobileService.Models.RMA;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class CustomerController : BaseApiController
    {

        #region [基本信息:]
        /// <summary>
        /// 获取用户基本信息:
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult CustomerInfo()
        {
            bool success = true;
            var data = CustomerManager.GetCustomerInfo(UserMgr.ReadUserInfo());

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Data = data,
            };
            return result;
        }
        /// <summary>
        /// 修改用户基本信息
        /// </summary>
        /// <param name="customerInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireAuthorize]
        public JsonResult EditCustomerInfo(CustomerInfoViewModel customerInfo)
        {
            string errorMsg;
            bool success = CustomerManager.UpdateCustomerPersonInfo(UserMgr.ReadUserInfo(), customerInfo, out errorMsg);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Message = errorMsg,
            };
            return result;
        }
        [HttpPost]
        [RequireAuthorize]
        public JsonResult UpdateAvatar()
        {
            string errorMsg;
            string data;
            bool success = CustomerManager.UpdateCustomerAvatar(UserMgr.ReadUserInfo(), System.Web.HttpContext.Current, out data, out errorMsg);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Message = errorMsg,
                Data = data,
            };
            return result;
        }
        #endregion

        #region [订单相关:]
        /// <summary>
        /// 我的订单列表:
        /// </summary>
        /// <param name="soSearchType"></param>
        /// <param name="soType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult OrderList(int soSearchType, int pageIndex, int pageSize,string SearchKey)
        {
            SOSearchType getSOSearchType = SOSearchType.ALL;
            switch (soSearchType)
            {
                case 14:
                    getSOSearchType = SOSearchType.ALL;
                    break;
                case 15:
                    getSOSearchType = SOSearchType.LastThreeMonths;
                    break;
                case 18:
                    getSOSearchType = SOSearchType.ThreeMonthsBefore;
                    break;
                default:
                    break;
            }
            return Json(new AjaxResult() { Success = true, Data = CustomerManager.GetCustomerOrderList(null, getSOSearchType, pageIndex, pageSize, SearchKey) });
        }

        /// <summary>
        /// 待付款订单列表
        /// </summary>
        /// <param name="soSearchType">待支付13,已付款20,已出库21</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="SearchKey"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult UnpayedOrderList(int soSearchType, int pageIndex, int pageSize, string SearchKey)
        {
            SOPaymentStatus getSOPaymentStatus = SOPaymentStatus.All;
            switch (soSearchType)
            {
                case 13:
                    getSOPaymentStatus = SOPaymentStatus.NoPay;
                    break;
                case 20:
                    getSOPaymentStatus = SOPaymentStatus.HasPay;
                    break;
                case 21:
                    getSOPaymentStatus = SOPaymentStatus.All;
                    break;
                default:
                    break;
            }
            return Json(new AjaxResult() { Success = true, Data = CustomerManager.GetCustomerOrderList(getSOPaymentStatus, SOSearchType.ALL, pageIndex, pageSize, SearchKey) });

        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="queryType">从QueryDB获取订单详情信息 = 0,从CentreDB获取订单详情(适用于刚下单跳转到订单详情的情况)=1</param>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult OrderDetail(int queryType, int soSysNo)
        {
            return Json(new AjaxResult() { Success = true, Data = CustomerManager.GetCustomerOrderDetail(queryType, soSysNo) });
        }

        /// <summary>
        /// 查询订单跟踪记录
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult OrderDetailLogs(int soSysNo)
        {
            return Json(new AjaxResult() { Success = true, Data = CustomerManager.GetOrderDetailLogs(soSysNo) });
        }

        /// <summary>
        /// 获取订单作废原因
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult VoidOrderReasons()
        {
            return Json(new AjaxResult() { Success = true, Data = CustomerManager.GetVoidOrderReasons() });
        }

        /// <summary>
        /// 作废订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireAuthorize]
        public JsonResult VoidOrder(VoidOrderRequestViewModel request)
        {
            string result = CustomerManager.VoidOrder(request);
            return Json(new AjaxResult() { Success = result.Length <= 0 ? true : false, Data = result.Length <= 0 ? "作废订单成功!" : result, Code = result.Length <= 0 ? 0 : -1 });
        }
        #endregion

        #region [到货通知:]

        /// <summary>
        /// 到货通知列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult ProductNotifyList(int pageIndex, int pageSize)
        {
            return Json(new AjaxResult() { Success = true, Data = CustomerManager.GetProductNotifyList(pageIndex + 1, pageSize) });
        }
        /// <summary>
        /// 删除到货通知
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireAuthorize]
        public JsonResult DeleteProductNotifyInfo(DeleteProductNotifyInfoRequestModel request)
        {
            try
            {
                CustomerManager.DeleteProductNofityInfo(request.NotifySysNo);
                return Json(new AjaxResult() { Success = true, Data = "删除到货通知成功!", Code = 0 });
            }
            catch
            {
                return Json(new AjaxResult() { Success = false, Data = "删除到货通知失败，请稍后再试!", Code = -1 });
            }

        }

        /// <summary>
        /// 添加到货通知
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireAuthorize]
        public JsonResult AddProductNotifyInfo(AddProductNotifyInfoRequestModel request)
        {

            CustomerManager.AddProductNofityInfo(request.ProductSysNo, request.Email);
            return Json(new AjaxResult() { Success = true, Data = "添加到货通知成功" });
        }

        #endregion

        #region [降价通知:]

        /// <summary>
        /// 降价通知列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult ProductPriceNotifyList(int pageIndex, int pageSize)
        {
            return Json(new AjaxResult() { Success = true, Data = CustomerManager.GetProductPriceNotifyList(pageIndex, pageSize) });
        }
        /// <summary>
        /// 删除降价通知
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireAuthorize]
        public JsonResult DeleteProductPriceNotifyInfo(DeleteProductPriceNotifyInfoViewModel request)
        {
            try
            {
                CustomerManager.DeleteProductPriceNofityInfo(request.PriceNofitySysNo);
                return Json(new AjaxResult() { Success = true, Data = "删除降价通知成功!", Code = 0 });
            }
            catch
            {
                return Json(new AjaxResult() { Success = false, Data = "删除降价通知失败，请稍后再试!", Code = -1 });
            }

        }

        /// <summary>
        /// 添加降价通知
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireAuthorize]
        public JsonResult AddProductPriceNotifyInfo(AddProductPriceNotifyInfoViewModel request)
        {
            CustomerManager.AddProductPriceNofityInfo(request.ProductSysNo, request.ExpectedPrice, request.InstantPrice);
            return Json(new AjaxResult() { Success = true, Data = "添加降价通知成功!" });
        }

        #endregion

        #region [收藏]

        [HttpGet]
        [RequireAuthorize]
        public JsonResult FavoriteInfoList(int pageIndex, int pageSize)
        {
            return Json(new AjaxResult() { Success = true, Data = CustomerManager.GetProductFavoriteList(pageIndex + 1, pageSize) });
        }

        [HttpPost]
        [RequireAuthorize]
        public JsonResult DeleteFavoriteInfo(DeleteProductFavoriteInfoViewModel request)
        {

            try
            {
                CustomerManager.DeleteProductFavoriteInfo(request.SysNo);
                return Json(new AjaxResult() { Success = true, Data = "删除收藏成功!", Code = 0 });
            }
            catch
            {
                return Json(new AjaxResult() { Success = false, Data = "删除收藏失败，请稍后再试!", Code = -1 });
            }
        }

        #endregion

        #region [评论:]
        /// <summary>
        /// 获取用户评论列表
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="status"></param>
        /// <param name="scoreType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ReviewList(int reviewStatus, int pageIndex, int pageSize)
        {
            return Json(new AjaxResult() { Success = true, Data = CustomerManager.GetReviewList(0, reviewStatus, new List<ReviewScoreType>(), pageIndex, pageSize) });
        }
        #endregion

        #region [咨询:]
        /// <summary>
        /// 账户中心获取我的咨询列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult ConsultList(int pageIndex, int pageSize)
        {
            return Json(new AjaxResult() { Success = true, Data = CustomerManager.GetConsultList(pageIndex, pageSize) });
        }
        #endregion

        #region 修改密码
        [HttpPost]
        [RequireAuthorize]
        public JsonResult ChangePassword(ChangePasswordViewModel changePasswordInfo)
        {
            string errorMsg;
            bool success = CustomerManager.UpdateCustomerPassword(UserMgr.ReadUserInfo(), changePasswordInfo, out errorMsg);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Message = errorMsg
            };
            return result;
        }
        #endregion

        #region 收货地址管理
        /// <summary>
        /// 获取当前用户的收货地址列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult ShippingAddress()
        {
            bool success = true;
            //客户端操作数据后，如果延迟比较大，可以改为直接读取centerdb数据
            var data = CustomerManager.GetCustomerShippingAddressList(UserMgr.ReadUserInfo());

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Data = data,
            };
            return result;
        }

        /// <summary>
        /// 获取用户收货地址详细信息
        /// </summary>
        /// <param name="shippingAddressSysNo"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult ShippingAddressDetail(int shippingAddressSysNo)
        {
            var data = CustomerManager.GetCustomerShippingAddressDetail(shippingAddressSysNo);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = true,
                Data = data,
            };
            return result;
        }


        /// <summary>
        /// 新增或者更新当前用户的收货地址信息
        /// 当shippingAddressSysNo 大于0时，为新增
        /// </summary>
        /// <param name="shippingAddress"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireAuthorize]
        public JsonResult EditShippingAddress(ShippingContactInfo shippingAddress)
        {
            int sysNo;
            bool success = true;
            CustomerManager.EditCustomerShippingAddress(UserMgr.ReadUserInfo(), shippingAddress, out sysNo);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Data = sysNo,
            };
            return result;
        }
        /// <summary>
        /// 删除当前用户的收货地址信息
        /// </summary>
        /// <param name="shippingAddressSysNo"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult DeleteShippingAddress(int shippingAddressSysNo)
        {
            bool success = true;
            CustomerManager.DeleteCustomerShippingAddress(UserMgr.ReadUserInfo(), shippingAddressSysNo);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
            };
            return result;
        }

        #endregion

        #region 我的积分
        /// <summary>
        /// 积分获得记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult PointObtainList(int pageIndex, int pageSize)
        {
            PointObtainQueryResult queryResult = CustomerManager.GetPointObtainList(pageIndex, pageSize);
            var result = new AjaxResult
            {
                Success = true,
                Data = queryResult
            };
            return Json(result);
        }

        /// <summary>
        /// 积分使用记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult PointConsumeList(int pageIndex, int pageSize)
        {
            QueryResult<PointConsumeViewModel> queryResult = CustomerManager.GetPointConsumeList(pageIndex, pageSize);
            var result = new AjaxResult
            {
                Success = true,
                Data = queryResult
            };
            return Json(result);
        }

        #endregion

        #region 我的优惠券
        /// <summary>
        /// 获取当前用户的优惠券列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult Coupon(int pageIndex, int pageSize)
        {
            bool success = true;
            var data = CustomerManager.GetCustomerCouponCode(UserMgr.ReadUserInfo(), pageIndex, pageSize);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Data = data,
            };
            return result;
        }
        #endregion

        #region 手机验证
        [HttpPost]
        public JsonResult SendCellphoneCode(ValidateCellphoneViewModel validateCellphone)
        {
            string errorMsg;
            bool success = CustomerManager.SendCellphoneCode(UserMgr.ReadUserInfo(), validateCellphone, out errorMsg);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Message = errorMsg,
            };
            return result;
        }
        [HttpPost]
        [RequireAuthorize]
        public JsonResult ValidateCellphoneCode(ValidateCellphoneViewModel validateCellphone)
        {
            string errorMsg;
            bool success = CustomerManager.ValidateCellphoneCode(UserMgr.ReadUserInfo(), validateCellphone, out errorMsg);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Message = errorMsg,
            };
            return result;
        }
        #endregion

        #region 邮箱验证
        [HttpPost]
        [RequireAuthorize]
        public JsonResult SendValidateEmail(ValidateEmailViewModel validateEmail)
        {
            string errorMsg;
            bool success = true;
            var data = CustomerManager.SendValidateEmail(UserMgr.ReadUserInfo(), validateEmail, out errorMsg);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Message = errorMsg,
            };
            return result;
        }
        #endregion

        #region 找回密码
        /// <summary>
        /// 找回密码类型
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindPasswordType(string customerID)
        {
            string errorMsg;
            FindPasswordTypeViewModel data = CustomerManager.FindPasswordType(customerID, out errorMsg);
            bool success = data.Type != Models.Member.FindPasswordType.None;

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Message = errorMsg,
                Data = data,
            };
            return result;
        }
        /// <summary>
        /// 发送找回密码短信
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendFindPasswordCellphoneCode(string customerID)
        {
            string errorMsg;
            string data;
            bool success = CustomerManager.SendFindPasswordCellphoneCode(customerID, out data, out errorMsg);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Message = errorMsg,
                Data = data,
            };
            return result;
        }
        /// <summary>
        /// 验证找回密码短信并返回token
        /// </summary>
        /// <param name="validateCellphone"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ValidateFindPasswordCellphoneCode(ValidateCellphoneViewModel validateCellphone)
        {
            string errorMsg;
            string data;
            int customerSysNo;
            bool success = CustomerManager.ValidateFindPasswordCellphoneCode(validateCellphone, out data, out customerSysNo, out errorMsg);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Message = errorMsg,
                Data = data,
            };
            return result;
        }
        /// <summary>
        /// 通过得到得token重置密码
        /// </summary>
        /// <param name="findResetPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindResetPassword(FindResetPasswordViewModel findResetPassword)
        {
            findResetPassword.CustomerSysNo = 0;
            string errorMsg;
            bool success = CustomerManager.FindResetPassword(findResetPassword, out errorMsg);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Message = errorMsg,
            };
            return result;
        }
        /// <summary>
        /// 验证短信内容并重置密码
        /// </summary>
        /// <param name="findResetPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindValidateCellphoneCodeAndResetPassword(FindResetPasswordViewModel findResetPassword)
        {
            findResetPassword.CustomerSysNo = 0;
            string errorMsg;
            bool success;
            int customerSysNo;
            string token;
            //验证短信内容
            success = CustomerManager.ValidateFindPasswordCellphoneCode(new ValidateCellphoneViewModel()
            {
                CustomerID = findResetPassword.CustomerID,
                SMSCode = findResetPassword.SMSCode
            }, out token, out customerSysNo, out errorMsg);
            if (success)
            {
                //重置密码
                findResetPassword.Token = token;
                findResetPassword.CustomerSysNo = customerSysNo;
                success = CustomerManager.FindResetPassword(findResetPassword, out errorMsg);
            }
            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Message = errorMsg,
            };
            return result;
        }
        /// <summary>
        /// 发送找回密码邮件
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendFindPasswordEmail(string customerID)
        {
            string errorMsg;
            bool success = CustomerManager.SendFindPasswordEmail(customerID, out errorMsg);

            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = success,
                Message = errorMsg,
            };
            return result;
        }
        #endregion

        #region 实名认证

        /// <summary>
        /// 实名认证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveAuthenticationInfo(CustomerAuthenticationInfoModel model)
        {
            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = true,
                Message = null,
                Data = CustomerManager.SaveAuthenticationInfo(model)
            };
            return result;
        }

        #endregion

        #region 售后服务

        /// <summary>
        /// 查询售后List
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="search_ordertype">按哪种查询：值为requestsysno按售后单号，值为sosysno按订单编号</param>
        /// <param name="txtSearch">售后单号或者订单编号</param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult RMAQuery(int pageIndex, int pageSize)
        {
            CustomerManager manager = new CustomerManager();
            var data = manager.RMAQuery(pageIndex + 1, pageSize);
            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                 Success=true,
                 Data = data
            };
            return result;
        }

        /// <summary>
        /// 售后申请单详情
        /// </summary>
        /// <param name="sysno">售后单系统编号</param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult RMARequestDetail(int sysNo)
        {
            CustomerManager manager = new CustomerManager();
            var data = manager.RMARequestDetail(sysNo);
            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = true,
                Data = data
            };
            return result;
        }

        /// <summary>
        /// 售后明细
        /// </summary>
        /// <param name="registSysNo">明细系统编号</param>
        /// <param name="requestSysNo">售后单系统编号</param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult RMARegisterDetail(int registSysNo, int requestSysNo)
        {
            CustomerManager manager = new CustomerManager();
            var data = manager.RMARegisterDetail(registSysNo, requestSysNo);
            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = true,
                Data = data
            };
            return result;
        }

        /// <summary>
        /// 获取需要申请售后订单的商品列表
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult RMANewRequestProductList(int soSysNo)
        {
            CustomerManager manager = new CustomerManager();
            var data = manager.RMANewRequestProductList(soSysNo);
            JsonResult result = new JsonResult();
            result.Data = new AjaxResult()
            {
                Success = true,
                Data = data
            };
            return result;
        }

        /// <summary>
        /// 能进行售后申请的订单列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult QueryCanRequestOrders(int pageIndex, int pageSize)
        {
            return Json(new AjaxResult() { Success = true, Data = CustomerManager.QueryCanRequestOrders(pageIndex + 1, pageSize) });
        }

        /// <summary>
        /// 创建售后申请
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireAuthorize]
        public JsonResult CreateRMARequestInfo(RMARequestInfoModel request)
        {
            AjaxResult ajaxResult = CustomerManager.CreateRMARequestInfo(request);
            JsonResult result = new JsonResult();
            result.Data = ajaxResult;

            return result;
        }
        #endregion
    }
}

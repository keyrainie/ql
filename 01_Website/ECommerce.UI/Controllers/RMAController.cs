using ECommerce.Entity.Shipping;
using ECommerce.Facade.Shipping;
using ECommerce.Facade.Shopping;
using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerce.UI.Controllers
{
    public class RMAController : SSLControllerBase
    {
        //
        // GET: /RMA/

        /// <summary>
        /// 售后申请
        /// </summary>
        /// <returns></returns>
        public ActionResult RMARequest()
        {
            return View();
        }

        /// <summary>
        /// 售后查询
        /// </summary>
        /// <returns></returns>
        public ActionResult RMAQuery()
        {
            return View();
        }

        /// <summary>
        /// 新增售后
        /// </summary>
        /// <returns></returns>
        public ActionResult RMANewRequest()
        {
            return View();
        }

        /// <summary>
        /// 新增售后
        /// </summary>
        /// <returns></returns>
        public ActionResult RMANewRequestSuccess()
        {
            return View();
        }

        /// <summary>
        /// 售后详情
        /// </summary>
        /// <returns></returns>
        public ActionResult RMARequestDetail()
        {
            return View();
        }

        /// <summary>
        /// 还未提交的售后详情
        /// </summary>
        /// <returns></returns>
        public ActionResult RMARequestDetailNotSubmit()
        {
            return View();
        }

        /// <summary>
        /// 售后详情
        /// </summary>
        /// <returns></returns>
        public ActionResult RMARegisterDetail()
        {
            return View();
        }

        /// <summary>
        /// 获得收货地址列表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetShippingAddressList(CheckOutContext context)
        {
            if (context == null)
            {
                throw new BusinessException("无效的请求");
            }

            CheckOutResult result = new CheckOutResult();
            result.ShippingAddressList = CustomerShippingAddresssFacade.GetCustomerShippingAddressList(CurrUser.UserSysNo);
            if (result.ShippingAddressList != null)
            {
                result.SelShippingAddress = result.ShippingAddressList.Find(x => x.SysNo == context.ShippingAddressID);
                if (result.SelShippingAddress == null)
                {
                    result.SelShippingAddress = result.ShippingAddressList.Find(x => x.IsDefault);
                }
            }
            return PartialView("_RMAShippingAddressPanel", result);
        }

        /// <summary>
        /// 修改收货地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditShippingAddress()
        {
            if (Request["cmd"] == null)
            {
                throw new BusinessException("无效的请求");
            }

            int contactAddressSysNo;
            string command = Request["cmd"].Trim().ToUpper();
            switch (command)
            {
                case "GET":

                    if (Request["id"] == null || !int.TryParse(Request["id"], out contactAddressSysNo))
                    {
                        throw new BusinessException("无效的请求");
                    }
                    else
                    {
                        ShippingContactInfo customerShippingAddressInfo = CustomerShippingAddresssFacade.GetCustomerShippingAddress(contactAddressSysNo, CurrUser.UserSysNo);
                        if (customerShippingAddressInfo == null)
                        {
                            throw new BusinessException("无效的收货地址");
                        }
                        return PartialView("_RMAShippingAddressEditPanel", customerShippingAddressInfo);
                    }

                case "DEL":

                    if (Request["id"] == null || !int.TryParse(Request["id"], out contactAddressSysNo))
                    {
                        throw new BusinessException("无效的请求");
                    }
                    else
                    {
                        CustomerShippingAddresssFacade.DeleteCustomerContactInfo(contactAddressSysNo, CurrUser.UserSysNo);
                        return Json(new { sysno = contactAddressSysNo });
                    }

                case "DEFAULT":
                    if (Request["id"] == null || !int.TryParse(Request["id"], out contactAddressSysNo))
                    {
                        throw new BusinessException("无效的请求");
                    }
                    else
                    {
                        CustomerShippingAddresssFacade.SetCustomerContactAsDefault(contactAddressSysNo, CurrUser.UserSysNo);
                        return Content("y");
                    }

                default:
                    break;
            }

            return Json("");
        }

        /// <summary>
        /// 保存收货地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubmitShippingAddress()
        {
            ShippingContactInfo shippingAddressInfo = new ShippingContactInfo()
            {
                IsDefault = true
            };
            if (TryUpdateModel(shippingAddressInfo))
            {
                int receiveAreaIdD;
                if (int.TryParse(Request["District"], out receiveAreaIdD) && receiveAreaIdD > 0)
                {
                    shippingAddressInfo.ReceiveAreaSysNo = receiveAreaIdD;
                    //checkout页面“收货人”和“联系人”写成同一个值
                    shippingAddressInfo.ReceiveContact = shippingAddressInfo.ReceiveName;
                    shippingAddressInfo = CustomerShippingAddresssFacade.EditCustomerContactInfo(shippingAddressInfo, CurrUser.UserSysNo);
                    return Json(shippingAddressInfo);
                }
                throw new BusinessException("收货区域不能为空");
            }
            throw new BusinessException("无效的请求");
        }
    }
}

using Nesoft.ECWeb.Entity.Payment;
using Nesoft.ECWeb.Facade.Payment;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.MobileService.Models.AliPay;
using Nesoft.ECWeb.MobileService.Models.Order;
using Nesoft.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class AliPayController : BaseApiController
    {
        /// <summary>
        /// 同步 
        /// </summary>
        /// <returns></returns>
        [RequireAuthorize]
        [HttpGet]
        public ActionResult FrontPay(string resultInfo)
        {
            //Nesoft.Utility.Logger.WriteLog(resultInfo, "AliPay", "FrontPay");
            NameValueCollection sPara = new NameValueCollection();//GetRequestGet();
            resultInfo = System.Web.HttpUtility.UrlDecode(resultInfo, System.Text.Encoding.GetEncoding("GB2312"));
            Regex regex = new Regex(@"(^|&)?(?<key>\w+)=(?<value>[^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection matches = regex.Matches(resultInfo);
            foreach (Match item in matches)
            {
                if (item.Success)
                {
                    if (item.Groups["key"].Value == "success" && item.Groups["value"].Value == "\"true\"")
                    {
                        sPara.Add("trade_status", "TRADE_SUCCESS");
                    }
                    if (item.Groups["key"].Value == "success" && item.Groups["value"].Value != "\"true\"")
                    {
                        sPara.Add("trade_status", "fail");
                    }
                    sPara.Add(item.Groups["key"].Value, item.Groups["value"].Value.Replace("\"",""));
                    
                }
            }
            string out_trade_no = sPara["out_trade_no"];
            //int SOSysNo = int.Parse(out_trade_no);
            Nesoft.Utility.Logger.WriteLog(resultInfo, "AliPay", "FrontPay");
            JsonResult result = BulidJsonResult(OrderManager.AliOnlinePay(out_trade_no, sPara));
            System.Threading.Thread.Sleep(1000);
            return result;
        }
        /// <summary>
        /// 处理ios提交过来的支付信息
        /// </summary>
        /// <returns></returns>
        [RequireAuthorize]
        [HttpGet]
        public ActionResult IosFrontPay()
        {
            SortedDictionary<string, string> sPara = GetRequestGet();
            NameValueCollection sParaios = new NameValueCollection();
            foreach (string key in sPara.Keys)
            {
                sParaios.Add(key, sPara[key].Replace("\"", ""));
            }

            string out_trade_no = sParaios["out_trade_no"];
           // int SOSysNo = int.Parse(out_trade_no);
            JsonResult result = BulidJsonResult(OrderManager.AliOnlinePay(out_trade_no, sParaios));
            System.Threading.Thread.Sleep(1000);
            return result;
        }

        [RequireAuthorize]
        [HttpGet]
        public JsonResult changeOrderStates()
        {

            AjaxResult ajaxResult = new AjaxResult();
            SortedDictionary<string, string> sPara = GetRequestGet();
            string msg = string.Empty;
            if (sPara.Count > 0 && !string.IsNullOrEmpty(Request.QueryString["out_trade_no"]))//判断是否有带返回参数
            {
                string out_trade_no = Request.QueryString["out_trade_no"];
                //int SOSysNo = 0;
                if (!string.IsNullOrEmpty(out_trade_no) && Request.QueryString["success"] == "true")
                {
                    //SOSysNo = int.Parse(out_trade_no);
                    PaymentService Pay = new PaymentService();
                    int CheckNumble = Pay.AliPayCheck(out_trade_no);
                    if (CheckNumble == 1000)
                    {
                        msg = "订单不存在！";
                        ajaxResult.Data = msg;
                        ajaxResult.Success = true;
                        ajaxResult.Code = 0;
                    }
                    else if (CheckNumble == 3000)
                    {
                        msg = "订单已支付！";
                        ajaxResult.Data = msg;
                        ajaxResult.Success = true;
                        ajaxResult.Code = 9000;
                    }
                    else if (CheckNumble == 4000)
                    {
                        CallbackContext context = null;
                        msg = (new PaymentService()).AliPayCallback(112, Request.QueryString, out context);
                        ajaxResult.Data = msg;
                        ajaxResult.Success = true;
                        ajaxResult.Code = 9001;
                    }
                }
                return BulidJsonResult(ajaxResult);
            }
            else
            {
                ajaxResult.Data = ("无参数带入");
                ajaxResult.Success = false;
                ajaxResult.Code = 0;
                return BulidJsonResult(ajaxResult);
            }
        }
        /// <summary>
        /// 异步
        /// </summary>
        /// <returns></returns>
        public void BackPay()
        {
            
            SortedDictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);
                //verifyResult = false;
                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                    //商户订单号

                    string out_trade_no = Request.Form["out_trade_no"];

                    //支付宝交易号

                    string trade_no = Request.Form["trade_no"];

                    //交易状态
                    string trade_status = Request.Form["trade_status"];


                    if (Request.Form["trade_status"] == "TRADE_FINISHED")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //该种交易状态只在两种情况下出现
                        //1、开通了普通即时到账，买家付款成功后。
                        //2、开通了高级即时到账，从该笔交易成功时间算起，过了签约时的可退款时限（如：三个月以内可退款、一年以内可退款等）后。
                    }
                    else if (Request.Form["trade_status"] == "TRADE_SUCCESS")
                    {
                        CallbackContext context = null;
                        Response.Write((new PaymentService()).AliPayCallback(112, Request.Form, out context));
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //该种交易状态只在一种情况下出现——开通了高级即时到账，买家付款成功后。
                    }
                    else
                    {
                    }

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    Response.Write("success");  //请不要修改或删除

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    Response.Write("fail");
                }
            }
            else
            {
                Response.Write("无通知参数");
            }
        }
        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;
            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }
        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }



        private JsonResult BulidJsonResult(AjaxResult ajaxResult)
        {
            JsonResult result = new JsonResult();
            result.Data = ajaxResult;

            return result;
        }
    }
}

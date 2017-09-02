using System;
using System.Data;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.RMA.BizProcessor
{
    /// <summary>
    /// 支付接口工具类
    /// </summary>
    public class ChinaPayUtils
    {
        private static readonly string PriKeyPath = AppSettingManager.GetSetting("Invoice", "priKeyPath");
        private static string PubKeyPath = AppSettingManager.GetSetting("Invoice", "pubKeyPath");
        private static readonly string MerId = AppSettingManager.GetSetting("Invoice", "MerId");
        private static readonly string RefundUrl = AppSettingManager.GetSetting("Invoice", "RefundUrl");
        private static readonly string Version = AppSettingManager.GetSetting("Invoice", "version");
        private static readonly string TransType = AppSettingManager.GetSetting("Invoice", "TransType");
        private static readonly string ReturnURL = AppSettingManager.GetSetting("Invoice", "ReturnURL");
        private RefundPostData refundPostData = null;
        private NetPay netPay = null;

        public ChinaPayUtils()
        {
            refundPostData = new RefundPostData
                {
                    MerID = MerId,
                    Version = Version,
                    TransType = TransType
                };
            netPay = new NetPay();
            //设置密钥文件地址
            netPay.buildKey(MerId, 0, PriKeyPath);
        }

        /// <summary>
        /// 签名数据
        /// </summary>
        /// <param name="merId">供应商ID</param>
        /// <param name="plain">原始文本</param>
        /// <returns></returns>
        private string SignData(string rawText)
        {
            //byte[] StrRes = Encoding.GetEncoding("utf-8").GetBytes(rawText);
            //rawText = Convert.ToBase64String(StrRes);
            // 对一段字符串的签名
            return netPay.Sign(rawText);

        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="entity">退款实体信息</param>
        /// <returns>退款结果</returns>
        public ResponseResult Refund(ChinaPayEntity entity)
        {
            var result = new ResponseResult();
            refundPostData.OrderId = entity.SOSysNo.ToString().PadLeft(16, '0');
            //退款金额单位"分"
            refundPostData.RefundAmount = (Math.Round(entity.RefundAmt * 100)).ToString().PadLeft(12, '0'); ;
            refundPostData.Priv1 = entity.RefundSysNo.ToString();
            refundPostData.TransDate = String.Format("{0:yyyyMMdd}", entity.OrderDate);
            refundPostData.ChkValue = SignData(refundPostData.GetRawChkValue());


            byte[] data = Encoding.UTF8.GetBytes(refundPostData.ToString());

            string rawResult = PostData(data);

            //响应日志
            result = new ResponseResult(rawResult);
            string message = result.Message;
            if (result.Exception != null)
            {
                message = result.Exception.Message;
            }
            string resultNote = string.Format("用户[{0}]对订单号：{1} 调用了退款接口.调用结果;{2} 调用返回信息：{3} PostUrl:{4} ", ServiceContext.Current.UserSysNo, entity.SOSysNo, result.Result, message, RefundUrl);
            ExternalDomainBroker.CreateOperationLog(resultNote, BizLogType.RMA_Refund_Refund, entity.RefundSysNo, entity.CompanyCode);
            return result;
        }

        /// <summary>
        /// 发送HttpRequest请求
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns>返回结果</returns>
        private string PostData(byte[] requestData)
        {

            var request = (HttpWebRequest)WebRequest.Create(RefundUrl);
            HttpWebResponse response = null;
            string result = string.Empty;
            try
            {
                request.Method = "Post";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = requestData.Length;

                using (Stream stream = request.GetRequestStream())
                {
                    //发送数据   
                    stream.Write(requestData, 0, requestData.Length);
                    stream.Close();

                    response = (HttpWebResponse)request.GetResponse();
                    var receiveStream = response.GetResponseStream();
                    var sb = new StringBuilder();
                    if (receiveStream != null)
                    {
                        using (var readStream = new StreamReader(receiveStream, Encoding.Default))
                        {
                            var read = new Char[256];
                            int count = readStream.Read(read, 0, 256);

                            while (count > 0)
                            {
                                var readstr = new String(read, 0, count);
                                sb.Append(readstr);
                                count = readStream.Read(read, 0, 256);
                            }
                        }

                    }
                    return sb.ToString();
                }

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

        }

    }

    /// <summary>
    /// 退款PostData
    /// </summary>
    public class RefundPostData
    {
        /// <summary>
        /// MerId为ChinaPay统一分配给商户的商户号
        /// </summary>
        public string MerID;

        /// <summary>
        /// 交易类型：0002为退款请求
        /// </summary>
        public string TransType;

        /// <summary>
        /// 原支付交易的订单号
        /// </summary>
        public string OrderId = string.Empty;

        /// <summary>
        /// 退款金额
        /// </summary>
        public string RefundAmount;

        /// <summary>
        /// 原交易日期，8位长度 20070801
        /// </summary>
        public string TransDate;
        /// <summary>
        /// 单笔退款接口版本号
        /// </summary>
        public string Version;

        /// <summary>
        /// 退款状态接收URL，可选，长度不要超过80个字节
        /// </summary>
        public string ReturnURL;

        /// <summary>
        /// 商户私有域，必输，长度不要超过40个字节,不能重复
        /// </summary>
        public string Priv1;
        /// <summary>
        /// 256字节长的ASCII码，必填
        /// </summary>
        public string ChkValue;

        public string GetRawNameValueString()
        {
            const string addChar = "&";
            var sb = new StringBuilder();
            sb.AppendFormat("MerID={0}", MerID);
            sb.Append(addChar);
            sb.AppendFormat("TransType={0}", TransType);
            sb.Append(addChar);
            sb.AppendFormat("OrderId={0}", OrderId);
            sb.Append(addChar);
            sb.AppendFormat("RefundAmount={0}", RefundAmount);
            sb.Append(addChar);
            sb.AppendFormat("TransDate={0}", TransDate);
            sb.Append(addChar);
            sb.AppendFormat("Version={0}", Version);
            sb.Append(addChar);
            sb.AppendFormat("ReturnURL={0}", ReturnURL);
            sb.Append(addChar);
            sb.AppendFormat("Priv1={0}", Priv1);
            return sb.ToString();

        }

        public string GetRawChkValue()
        {
            return (MerID + TransDate + TransType + OrderId + RefundAmount + Priv1);
        }

        public override string ToString()
        {
            return string.Format("{0}&ChkValue={1}", GetRawNameValueString(), ChkValue);
        }



    }

    /// <summary>
    /// 退款返回信息
    /// </summary>
    public class ResponseResult
    {

        public bool Result
        {
            get;
            set;
        }
        public string Message
        {
            get;
            set;
        }
        public ResponseResult()
        {
            Result = false;
            Message = string.Empty;
        }


        public Exception Exception
        {
            get;
            set;
        }


        public ResponseResult(bool result, string message)
        {
            Result = result;
            Message = message;

        }

        public ResponseResult(bool result, string message, Exception ex)
        {
            Result = result;
            Message = message;
            Exception = ex;
        }

        /// <summary>
        /// 处理银联返回的信息
        /// </summary>
        /// <param name="rawRefundResult"></param>
        public ResponseResult(string rawRefundResult)
        {
            int startTagPosition = rawRefundResult.IndexOf("<body>");
            int endTagPosition = rawRefundResult.IndexOf("</body>");
            string result = rawRefundResult.Substring(startTagPosition + 6, endTagPosition - startTagPosition - 6);
            string[] strArray = result.Split('&');
            if (strArray.Length >= 2)
            {
                string[] arrayResponseCode = strArray[0].Split('=');
                //当ResponseCode的值为0时后续的字段需要参与签名验证后才能表示交易处理成功，
                //失败时ResponseCode为其它错误码
                if (arrayResponseCode[1] != "0")
                {
                    Result = false;
                    string[] arrayMessage = strArray[1].Split('=');
                    if (arrayMessage.Length >= 2)
                    {
                        Message = arrayMessage[1];
                    }
                }
                else
                {
                    string[] arrayStatus = strArray[7].Split('=');
                    if (arrayStatus[1] == "1")
                    {
                        Result = true;
                        //Message = "退款提交成功";
                        Message = ResouceManager.GetMessageString("Invoice.NePay", "NetPay_RefundSubmitSuccess");
                    }
                    else if (arrayStatus[1] == "3")
                    {
                        Result = true;
                        //Message = "退款成功";
                        Message = ResouceManager.GetMessageString("Invoice.NePay", "NetPay_RefundSuccess");
                    }
                    else if (arrayStatus[1] == "8")
                    {
                        Result = false;
                        //Message = "退款失败";
                        Message = ResouceManager.GetMessageString("Invoice.NePay", "NetPay_RefundFailed");
                    }
                }
            }
            else
            {
                Result = false;
                //Message = "提交退款应答失败!";
                Message = ResouceManager.GetMessageString("Invoice.NePay", "NetPay_SubmitRefundReplayFailed");
            }
        }
    }

    public class ChinaPayEntity
    {
        /// <summary>
        /// 订单sosysNO
        /// </summary>
        public int SOSysNo { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        /// <example>
        /// 退款金额的单位是分。
        /// 如果退款金额 103.21 元,那么退款金额就是10321
        /// </example>
        public decimal RefundAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 退款单单号
        /// </summary>
        public int RefundSysNo
        {
            get;
            set;
        }

        public string CompanyCode { get; set; }

        public DateTime OrderDate { get; set; }
    }

}

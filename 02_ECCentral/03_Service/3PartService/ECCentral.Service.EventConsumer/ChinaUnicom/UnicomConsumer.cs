using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using ECCentral.Service.EventMessage;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventConsumer
{
    public class UnicomConsumer : IConsumer<AbandonUnicomMessage>
    {
        private const string ABANDONSERVER = "WEBSERVICE0006";

        /// <summary>
        /// 获取指定SKU的商品信息(Quantity,Price,Description等)
        /// </summary>
        /// <param name="request">IngramRequest</param>
        /// <returns>IngramResponse</returns>
        public void HandleEvent(AbandonUnicomMessage eventMessage)
        {
            AbandonUnicomRequestMessage request = InitRequest(eventMessage);
            string strURL = AppSettingHelper.UnicomURL;
            string reqXML = BuildRequest(request);
            string rspXML = null;
            string orderSysNo = eventMessage.OrderNumber;
            try
            {
                WriteLog(reqXML, orderSysNo);

                //访问Ingram服务获取批定SKU的Item信息
                rspXML = GetResponse(reqXML, strURL);

                UnicomResponseMessage response = BuildUnicomResponseMessage(rspXML);
                ValidataResponse(response);

                WriteLog(rspXML, orderSysNo);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, orderSysNo);
            }
        }

        private void WriteLog(string content, string referencekey)
        {
            //Logger.WriteLog(new LogEntry
            //{
            //    Category = "联通服务调用日志",
            //    Content = content,
            //    GlobalName = "IPP",
            //    LocalName = "ThirdPart",
            //    ReferenceKey = referencekey
            //});
        }

        private AbandonUnicomRequestMessage InitRequest(AbandonUnicomMessage message)
        {
            AbandonUnicomRequestMessage request = new AbandonUnicomRequestMessage
            {
                Param = message
            };
            if (string.IsNullOrEmpty(request.AcctountID))
            {
                request.AcctountID = AppSettingHelper.UnicomAccountID;
            }
            if (string.IsNullOrEmpty(request.Password))
            {
                request.Password = AppSettingHelper.UnicomPassword;
            }
            if (string.IsNullOrEmpty(request.SeqNo))
            {
                request.SeqNo = BuildSeqNo();
            }
            if (string.IsNullOrEmpty(request.AgentKey))
            {
                request.AgentKey = AppSettingHelper.UnicomAgentKey;
            }
            request.ServerName = ABANDONSERVER;
            return request;
        }

        private string BuildRequest(AbandonUnicomRequestMessage request)
        {
            XmlSerializer xs = new XmlSerializer(typeof(AbandonUnicomRequestMessage));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            xs.Serialize(ms, request, ns);

            string xmlString = System.Text.UnicodeEncoding.UTF8.GetString(ms.ToArray());
            return xmlString;
        }

        private UnicomResponseMessage BuildUnicomResponseMessage(string response)
        {
            UnicomResponseMessage ingramResponse;

            XmlSerializer xs = new XmlSerializer(typeof(UnicomResponseMessage));
            MemoryStream memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(response));
            ingramResponse = (UnicomResponseMessage)xs.Deserialize(memoryStream);

            return ingramResponse;
        }

        private void ValidataResponse(UnicomResponseMessage response)
        {
            Dictionary<string, string> errorCodeList = new Dictionary<string, string>();
            errorCodeList.Add("100000", "请求串不合法");
            errorCodeList.Add("100001", "服务器名称不正确");
            errorCodeList.Add("100002", "用户名与密码不符");
            errorCodeList.Add("100003", "代理商不正确");
            errorCodeList.Add("100004", "WebServiceKey不正确");
            errorCodeList.Add("100005", "号码查询失败");
            errorCodeList.Add("100006", "号码预占失败");
            errorCodeList.Add("100007", "号码取消预占失败");
            errorCodeList.Add("100008", "套餐查询失败");
            errorCodeList.Add("100009", "下单失败");
            errorCodeList.Add("100010", "订单取消失败");
            errorCodeList.Add("100011", "合作方订单号查询失败");
            errorCodeList.Add("100012", "订单状态查询失败");
            errorCodeList.Add("100013", "sim卡绑定失败");
            errorCodeList.Add("100014", "无可选择的sim卡");
            errorCodeList.Add("100015", "通知开户失败");
            errorCodeList.Add("100016", "订单状态修改失败");
            errorCodeList.Add("999999", "未知异常");

            if (response != null
                && response.MsgInfo != null
                && !string.IsNullOrEmpty(response.MsgInfo.MsgCode))
            {
                foreach (var errorCode in errorCodeList)
                {
                    if (response.MsgInfo.MsgCode == errorCode.Key)
                    {
                        ThrowBizException(errorCode.Value);
                    }
                }
            }
        }

        private void ThrowBizException(string message)
        {
            throw new ThirdPartBizException(message);
        }

        /// <summary>
        /// 构造请求编号
        /// </summary>
        /// <returns></returns>
        private string BuildSeqNo()
        {
            return DateTime.Now.ToString("yyyyMMddhhmmss");
        }

        /// <summary>
        /// 联通访问
        /// </summary>
        /// <param name="reqParamters"></param>
        /// <param name="strURL"></param>
        /// <returns></returns>
        private static string GetResponse(string reqParamters, string strURL)
        {
            string request = string.Format("{0}?test={1}", strURL, reqParamters);
            string response = WebRequestHelper.GetResponse(request, Encoding.Default);
            return response;
        }


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}

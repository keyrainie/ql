using Nesoft.Job.WMS.Common.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Nesoft.Utility;
using System.Diagnostics;

namespace Nesoft.Job.WMS.Common
{
    public class RestfulResult<T>
    {
        /// <summary>
        /// 业务状态码
        /// </summary>
        [JsonProperty("code")]
        public RestfulCode Code { get; set; }
        public T Data { get; set; }
        /// <summary>
        /// 业务消息
        /// </summary>
        [JsonProperty("desc")]
        public string Message { get; set; }
    }

    public static class RestfulClient
    {
        private static string Sign(List<string> parameter,  string secretKey)
        {
            return SignatureUtil.Build(string.Join("&", parameter.ToArray()), secretKey);
        }

        public static RestfulResult<T> PostJson<T>(string url, string method, object data, Action<string> beforeRequest)
        {
            BaseEntity baseData = EntityConverter<object, BaseEntity>.Convert(data);
            List<string> parameter = new List<string>();
            parameter.Add("method=" + HttpUtility.UrlEncode(method, Encoding.UTF8));
            parameter.Add("orderData=" + HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(data), Encoding.UTF8));
            parameter.Add("version=" + HttpUtility.UrlEncode("1.0", Encoding.UTF8));
            parameter.Add("nonce=" + HttpUtility.UrlEncode(new Random().NextDouble().ToString(), Encoding.UTF8));
            parameter.Add("appId=" + HttpUtility.UrlEncode(baseData.AppId, Encoding.UTF8));
            parameter.Add("timestamp=" + HttpUtility.UrlEncode(DateTime.Now.ToString("yyyyMMddhhmmss"), Encoding.UTF8));
            parameter.Add("sign=" + HttpUtility.UrlEncode(Sign(parameter, baseData.AppSecret), Encoding.UTF8));

            string logContent = "Request:" + string.Join("&", parameter.ToArray());
            Logger.WriteLog(logContent, "Http");
            Debug.WriteLine(logContent);
            if (beforeRequest != null)
            {
                beforeRequest(logContent);
            }
            var httpContent = new StringContent(string.Join("&", parameter.ToArray()));
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            //var httpContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(data));
            //httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var httpClient = new HttpClient();
            try
            {
                var task = httpClient.PostAsync(new Uri(url), httpContent);
                task.Wait();

                logContent = "Response:"+task.Result.Content.ReadAsStringAsync().Result;
                Logger.WriteLog(logContent, "Http");
                Debug.WriteLine(logContent);

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<RestfulResult<T>>(task.Result.Content.ReadAsStringAsync().Result);
                return result;
            }
            catch (Exception ex)
            {
                RestfulResult<T> result = new RestfulResult<T>();
                result.Message = ex.Message;
                if (ex.InnerException != null)
                {
                    result.Message += ex.InnerException.Message;
                }
                result.Data = default(T);
                result.Code = RestfulCode.SendDataFailed;
                return result;
            }
        }
    }


    public enum RestfulCode
    {
        #region 标准错误码
        /// <summary>
        /// 访问超过限制
        /// </summary>
        [Description("访问超过限制")]
        MoreThanLimit = -5,
        /// <summary>
        /// IP校验错误
        /// </summary>
        [Description("IP校验错误")]
        IPError = -4,
        /// <summary>
        /// 无API访问权限
        /// </summary>
        [Description("无API访问权限")]
        WithoutPermission = -3,
        /// <summary>
        /// 签名校验错误
        /// </summary>
        [Description("签名校验错误")]
        SiginError = -2,
        /// <summary>
        /// 请求参数错误
        /// </summary>
        [Description("请求参数错误")]
        RequestParamError = -1,
        #endregion
        #region 通用返回码
        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功")]
        Success = 200,
        /// <summary>
        /// 操作失败
        /// </summary>
        [Description("操作失败")]
        Failed = 201,
        /// <summary>
        /// 请求方式非法
        /// </summary>
        [Description("请求方式非法")]
        BadRequest = 202,
        /// <summary>
        /// 系统错误
        /// </summary>
        [Description("系统错误")]
        SystemError = 203,
        #endregion
        #region 数据库异常
        /// <summary>
        /// 查询数据异常
        /// </summary>
        [Description("查询数据异常")]
        QueryDataError = 301,
        /// <summary>
        /// 插入数据异常
        /// </summary>
        [Description("插入数据异常")]
        AddDataError = 302,
        /// <summary>
        /// 修改数据异常
        /// </summary>
        [Description("修改数据异常")]
        UpdateDataError = 303,
        /// <summary>
        /// 删除数据异常
        /// </summary>
        [Description("删除数据异常")]
        DelDataError = 304,
        /// <summary>
        /// 过程执行异常
        /// </summary>
        [Description("过程执行异常")]
        ProcessError = 305,
        #endregion
        #region 业务逻辑错误
        /// <summary>
        /// 关键数据项不能为空
        /// </summary>
        [Description("关键数据项不能为空")]
        CriticalDataNotEmpty = 401,
        /// <summary>
        /// 业务数据内容不存在
        /// </summary>
        [Description("业务数据内容不存在")]
        BusinessDataNotExists = 402,
        /// <summary>
        /// 数据格式不正确
        /// </summary>
        [Description("数据格式不正确")]
        BadDataFormat = 403,
        /// <summary>
        /// 业务逻辑执行错误
        /// </summary>
        [Description("业务逻辑执行错误")]
        BusinessLogicError = 404,
        #endregion
        #region 网络通讯错误
        /// <summary>
        /// 连接服务异常
        /// </summary>
        [Description("连接服务异常")]
        ConnectServerError = 601,
        /// <summary>
        /// 断开服务异常
        /// </summary>
        [Description("断开服务异常")]
        DisConnectServerError = 602,
        /// <summary>
        /// 发送数据失败
        /// </summary>
        [Description("发送数据失败")]
        SendDataFailed = 603,
        /// <summary>
        /// 接收数据失败
        /// </summary>
        [Description("接收数据失败")]
        RecieveDataFailed = 604,
        #endregion
    }
}

using Nesoft.ECWeb.Facade.Passport.Partner;
using Nesoft.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Nesoft.ECWeb.Facade.Payment
{
    class TLYHPoint
    {
        public int ReadTimeout { get; set; }
        public int Conntimeout { get; set; }
        public string Url { get; set; }
        public string Uid { get; set; }

        /// <summary>
        /// 初始化参数
        /// </summary>
        public TLYHPoint()
        {
            ReadTimeout = 60000;
            Conntimeout = 5000;
            Url = "http://10.4.16.171:38080/EEMAIL";
            Uid = "EEMAIL";
        }

        /// <summary>
        /// 获取网银用户积分
        /// </summary>
        /// <returns></returns>
        public double GetBankUserPoint(string customerCode)
        {
            double result = 0;

            //构造请求数据
            NameValueCollection postData = new NameValueCollection();
            //添加公共请求头部
            AddCommonParameter(postData);

            //积分查询请求参数
            postData.Add("CLIENT_NO", customerCode);
            string praraStr = Partners.BuildStringFromNameValueCollection(postData);

            //发送请求
            string responseData = Partners.HttpPostRequest(Url, praraStr, "application/x-www-form-urlencoded", "UTF-8");

            //解析响应数据
            BankPointResponse data = SerializationUtility.JsonDeserialize<BankPointResponse>(responseData);

            result = data.POINT;


            return result;
        }

        /// <summary>
        /// 网银积分冲正
        /// </summary>
        /// <param name="customerCode"></param>
        /// <param name="mechNo"></param>
        /// <param name="thirdSeqNo"></param>
        /// <param name="oriThirdSeqNo"></param>
        /// <param name="point"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public string RushTrading(string customerCode, string mechNo, string thirdSeqNo, string oriThirdSeqNo, double point, string description)
        {
            string result = string.Empty;

            //构造请求数据
            NameValueCollection postData = new NameValueCollection();
            //添加公共请求头部
            AddCommonParameter(postData);
            //冲正请求参数
            postData.Add("CLIENT_NO", customerCode);//客户号
            postData.Add("MECH_NO", mechNo);//商户号
            postData.Add("THIRD_SEQ_NO", thirdSeqNo);//第三方流水号
            postData.Add("ORI_THIRD_SEQ_NO", oriThirdSeqNo);//原第三方流水号
            postData.Add("POINT", point.ToString());//积分
            postData.Add("DESCRIPTION", description);//说明 
            string praraStr = Partners.BuildStringFromNameValueCollection(postData);

            //发送请求
            string responseData = Partners.HttpPostRequest(Url, praraStr, "application/x-www-form-urlencoded", "UTF-8");

            //解析响应数据
            BankPointRushTradingResponse data = SerializationUtility.JsonDeserialize<BankPointRushTradingResponse>(responseData);

            result = data.SERV_SEQ_NO;//返回服务处理流水号

            return result;
        }

        /// <summary>
        /// 增加公共参数
        /// </summary>
        /// <param name="postData"></param>
        private void AddCommonParameter(NameValueCollection postData)
        {
            postData.Add("SERVICE_CODE", "11002000003"); //服务码
            postData.Add("SERVICE_SCENE", "01");  //服务场景
            postData.Add("CONSUMER_SEQ_NO", "20101020130624160243100002"); //交易流水号
            postData.Add("TRAN_DATE", "20150811");//交易日期
            postData.Add("TRAN_TIMESTAMP", "111501");//交易时间
            postData.Add("CONSUMER_ID", "123456");//请求系统编号
            postData.Add("PROVIDER_ID", "123456"); //响应系统编号

            postData.Add("BUSS_SEQ_NO", "20101020130624160243100005");//业务流水号
        }
    }

    /// <summary>
    /// 获取用户积分response
    /// </summary>
    public class BankPointResponse
    {
        public string POINT_TYPE { get; set; }
        public double POINT { get; set; }
        public double EXPIRE_POINT { get; set; }
        public string STATUS { get; set; }
    }
    /// <summary>
    /// 获取积分冲账交易response
    /// </summary>
    public class BankPointRushTradingResponse
    {
        public string SERV_SEQ_NO { get; set; }
    }
}

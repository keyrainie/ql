using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ECommerce.Entity
{
    public class ConstValue
    {

        /// <summary>
        /// 登录验证码类别(0:图形验证码 1:手机短信在线验证)
        /// </summary>
        public static string LoginValidCodeType = ConfigurationSettings.AppSettings["LoginValidCodeType"];
        /// <summary>
        /// 批量生成通用优惠券代码默认数量
        /// </summary>
        public static string BitchDefaultGeneralCodeNumber = ConfigurationSettings.AppSettings["BitchDefaultGeneralCodeNumber"];
        /// <summary>
        /// 中国
        /// </summary>
        public static string regionCountry = ConfigurationSettings.AppSettings["-1"];
        /// <summary>
        /// 华东地区编号
        /// </summary>
        public static string regionHuadong = ConfigurationSettings.AppSettings["-2"];
        /// <summary>
        /// 华南地区编号
        /// </summary>
        public static string regionHuanan = ConfigurationSettings.AppSettings["-3"];
        /// <summary>
        /// 华北地区编号
        /// </summary>
        public static string regionHuabei = ConfigurationSettings.AppSettings["-4"];
        /// <summary>
        /// 华中地区编号
        /// </summary>
        public static string regionHuazhong = ConfigurationSettings.AppSettings["-5"];
        /// <summary>
        /// 西南地区编号
        /// </summary>
        public static string regionXinan = ConfigurationSettings.AppSettings["-6"];
        /// <summary>
        /// 西北地区编号
        /// </summary>
        public static string regionXibei = ConfigurationSettings.AppSettings["-7"];
        /// <summary>
        /// 东北地区编号
        /// </summary>
        public static string regionDongbei = ConfigurationSettings.AppSettings["-8"];
    }
}

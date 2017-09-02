using System;
using System.Collections.Generic;
using System.Configuration;

/// <summary>
/// Configuration Settings class for this App.
/// </summary>
public sealed class Settings
{
    /// <summary>
    /// 公司代码
    /// </summary>
    public static string CompanyCode
    {
        get
        {
            return ConfigurationManager.AppSettings["CompanyCode"];
        }
    }

    /// <summary>
    /// 获取Email发送地址
    /// </summary>
    public static string EmailAddress
    {
        get
        {
            return ConfigurationManager.AppSettings["EmailAddress"];
        }
    }

    /// <summary>
    /// 获取日志文件名称
    /// </summary>
    public static string LogFileName
    {
        get
        {
            return ConfigurationManager.AppSettings["LogFileName"];
        }
    }

    /// <summary>
    /// 报表模版文件夹路径
    /// </summary>
    public static string ReportConfigPath
    {
        get
        {
            return ConfigurationManager.AppSettings["ReportConfigPath"];
        }
    }
    /// <summary>
    /// 每次更新数据数量
    /// </summary>
    public static DateTime SettledDate
    {
        get
        {
            string dateStr = ConfigurationManager.AppSettings["SettledMonth"];
            if (string.IsNullOrEmpty(dateStr))
            {
                return DateTime.Now;
            }
            else
            {
                if (SettledDay.HasValue)
                {
                    return Convert.ToDateTime(dateStr + "-" + SettledDay.Value);
                }
                else
                {
                    return Convert.ToDateTime(dateStr + "-20");
                }
            }
            
        }
    }

    public static int? SettledDay
    {
        get
        {
            string dateStr = ConfigurationManager.AppSettings["SettledDay"];
            if (string.IsNullOrEmpty(dateStr))
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(dateStr);
            }
            
        }
    }
    
    
    

    
}


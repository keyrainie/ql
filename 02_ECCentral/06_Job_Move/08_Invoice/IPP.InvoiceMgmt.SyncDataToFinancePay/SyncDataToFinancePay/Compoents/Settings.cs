using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


/// <summary>
/// Configuration Settings class for this App.
/// </summary>
public sealed class Settings
{
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

    public static int SyncDataCount
    {
        get
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["SyncDataCount"]);
        }
    }
    /// <summary>
    /// 获取需要调用的同步存储过程名称
    /// </summary>
    public static List<string> SyncDataProcessList
    {
        get
        {
            return new List<string>(ConfigurationManager.AppSettings["SyncDataProcessList"].Split(','));
        }
    }
}


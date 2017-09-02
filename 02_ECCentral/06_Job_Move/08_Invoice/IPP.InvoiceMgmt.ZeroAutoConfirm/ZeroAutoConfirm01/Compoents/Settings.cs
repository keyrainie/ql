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
    public static string LongDateFormat = "yyyy-MM-dd HH:mm:ss";

    public static string ShortDateFormat = "yyyy-MM-dd";

    public static string Decimal = "#0.00";

    public static string EmailAddress
    {
        get
        {
            return ConfigurationManager.AppSettings["EmailAddress"];
        }
    }

    public static string LogFileName
    {
        get
        {
            return ConfigurationManager.AppSettings["LogFileName"];
        }
    }

    public static string CompanyCode
    {
        get
        {
            return ConfigurationManager.AppSettings["CompanyCode"];
        }
    }

    public static string UserDisplayName
    {
        get
        {
            return ConfigurationManager.AppSettings["UserDisplayName"];
        }
    }

    public static string UserLoginName
    {
        get
        {
            return ConfigurationManager.AppSettings["UserLoginName"];
        }
    }

    public static string StoreSourceDirectoryKey
    {
        get
        {
            return ConfigurationManager.AppSettings["StoreSourceDirectoryKey"];
        }
    }

    public static string EmailModel
    {
        get
        {
            return ConfigurationManager.AppSettings["EmailModel"];
        }
    }

    public static DateTime InitialDate
    {
        get
        {
            return Convert.ToDateTime(ConfigurationManager.AppSettings["InitialDate"]);
        }
    }

    public static string PrePayConfirmID
    {
        get
        {
            return ConfigurationManager.AppSettings["PrePayConfirmID"];
        }
    }

    public static string GiftCardConfirmID
    {
        get
        {
            return ConfigurationManager.AppSettings["GiftCardConfirmID"];
        }
    }

    public static string PointConfirmID
    {
        get
        {
            return ConfigurationManager.AppSettings["PointConfirmID"];
        }
    }

    public static string EmailFrom
    {
        get
        {
            return ConfigurationManager.AppSettings["EmailFrom"];
        }
    }

    public static string EmailSubject
    {
        get
        {
            return ConfigurationManager.AppSettings["EmailSubject"];
        }
    }

}


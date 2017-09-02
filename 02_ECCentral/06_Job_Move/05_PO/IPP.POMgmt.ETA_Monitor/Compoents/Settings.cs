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
    #region Constructors

    private Settings()
    {
    }

    #endregion


    public static string MailAddress
    {
        get
        {
            return ConfigurationManager.AppSettings["MailAddress"];
        }
    }

    /// <summary>
    /// LogFileName
    /// </summary>
    public static string LogFileName
    {
        get
        {
            return ConfigurationManager.AppSettings["LogFileName"];
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
    public static string CompanyCode
    {
        get
        {
            return ConfigurationManager.AppSettings["CompanyCode"];
        }
    }
    public static string TicketType
    {
        get
        {
            return ConfigurationManager.AppSettings["TicketType"];
        }
    }
    public static string StoreCompanyCode
    {
        get
        {
            return ConfigurationManager.AppSettings["StoreCompanyCode"];
        }
    }
    public static string StoreSourceDirectoryKey
    {
        get
        {
            return ConfigurationManager.AppSettings["StoreSourceDirectoryKey"];
        }
    }
    public static string ConsumerName
    {
        get
        {
            return ConfigurationManager.AppSettings["ConsumerName"];
        }
    }
    public static string EIMSLocationName
    {
        get
        {
            return ConfigurationManager.AppSettings["EIMSLocationName"];
        }
    }

    public static string GiftCardIDList
    {
        get
        {
            return ConfigurationManager.AppSettings["GiftCardIDList"];
        }
    }


    public static string LanguageCode
    {
        get
        {
            return ConfigurationManager.AppSettings["LanguageCode"];
        }
    }
}


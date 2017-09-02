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
    public static string LanguageCode
    {
        get
        {
            return ConfigurationManager.AppSettings["LanguageCode"];
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


    public static string RunDay
    {

        get
        {
            return ConfigurationManager.AppSettings["RunDay"];
        }
    }
    public static string UserSysNo
    {

        get
        {
            return ConfigurationManager.AppSettings["UserSysNo"];
        }
    }


    
}


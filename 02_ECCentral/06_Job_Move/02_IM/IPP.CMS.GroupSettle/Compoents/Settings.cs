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

    /// <summary>
    /// 周结编号
    /// </summary>
    public static int WeeksPaytermsNo
    {
        get
        {
            int temp = 0;
            if (int.TryParse(ConfigurationManager.AppSettings["WeeksPaytermsNo"], out temp))
            {
                temp = int.Parse(ConfigurationManager.AppSettings["WeeksPaytermsNo"]);
            }
            return temp;
        }
    }
    //周结First
    public static int WeeksDay_One
    {
        get
        {
            int temp = 0;
            if (int.TryParse(ConfigurationManager.AppSettings["WeeksDay_One"], out temp))
            {
                temp = int.Parse(ConfigurationManager.AppSettings["WeeksDay_One"]);
            }
            return temp;
        }
    }

    //周结Two
    public static int WeeksDay_Two
    {
        get
        {
            int temp = 0;
            if (int.TryParse(ConfigurationManager.AppSettings["WeeksDay_Two"], out temp))
            {
                temp = int.Parse(ConfigurationManager.AppSettings["WeeksDay_Two"]);
            }
            return temp;
        }
    }
    //周结Three
    public static int WeeksDay_Three
    {
        get
        {
            int temp = 0;
            if (int.TryParse(ConfigurationManager.AppSettings["WeeksDay_Three"], out temp))
            {
                temp = int.Parse(ConfigurationManager.AppSettings["WeeksDay_Three"]);
            }
            return temp;
        }
    }

    

    /// <summary>
    /// 月结编号
    /// </summary>
    public static int MonthsPaytermsNo
    {
        get
        {
            int temp = 0;
            if (int.TryParse(ConfigurationManager.AppSettings["MonthsPaytermsNo"], out temp))
            {
                temp = int.Parse(ConfigurationManager.AppSettings["MonthsPaytermsNo"]);
            }
            return temp;
        }
    }

    /// <summary>
    /// 月结First
    /// </summary>
    public static int MonthsDay_One
    {
        get
        {
            int temp = 0;
            if (int.TryParse(ConfigurationManager.AppSettings["MonthsDay_One"], out temp))
            {
                temp = int.Parse(ConfigurationManager.AppSettings["MonthsDay_One"]);
            }
            return temp;
        }
    }

    /// <summary>
    /// 月结Two
    /// </summary>
    public static int MonthsDay_Two
    {
        get
        {
            int temp = 0;
            if (int.TryParse(ConfigurationManager.AppSettings["MonthsDay_Two"], out temp))
            {
                temp = int.Parse(ConfigurationManager.AppSettings["MonthsDay_Two"]);
            }
            return temp;
        }
    }
    
}


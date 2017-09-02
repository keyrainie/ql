using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public sealed class LogerManger
{
    #region Member Variables
    private static ILog loger;
    private static readonly object _lock = new object();
    #endregion

    #region Constructors

    private LogerManger()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get the current Loger instanct
    /// </summary>
    /// <returns></returns>
    public static ILog GetLoger()
    {
        if (loger == null)
        {
            lock (_lock)
            {
                if (loger == null)
                {
                    string logFilePath = Settings.LogFileName;
                    loger = new TxtFileLoger(logFilePath);
                }
            }
        }

        return loger;
    }

    #endregion
}


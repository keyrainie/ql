using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POASNMgmt.AutoCreateVendorSettle.Compoents.Logger
{
    public static class LogManager
    {
        #region Member Variables

        private static ILog logger;
        private static readonly object _lock = new object();

        #endregion

        #region Methods

        /// <summary>
        /// Get the current Loger instanct
        /// </summary>
        /// <returns></returns>
        public static ILog GetLogger()
        {
            if (logger == null)
            {
                lock (_lock)
                {
                    if (logger == null)
                    {
                        string logFilePath = GlobalSettings.LogFileName;
                        logger = new TxtFileLoger(logFilePath);
                    }
                }
            }

            return logger;
        }

        #endregion
    }
}


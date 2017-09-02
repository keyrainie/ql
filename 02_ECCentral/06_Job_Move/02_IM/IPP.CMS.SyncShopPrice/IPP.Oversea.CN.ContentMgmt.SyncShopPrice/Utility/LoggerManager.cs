using System.Configuration;


namespace IPP.ContentMgmt.SyncShopPrice.Utility
{
    public sealed class LoggerManager
    {
        #region Member Variables

        private static ILog logger;
        private static readonly object synRoot = new object();

        #endregion

        #region Constructors

        private LoggerManager()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the current Loger instanct
        /// </summary>
        /// <returns>ILog的实例</returns>
        public static ILog GetLogger()
        {
            if (logger == null)
            {
                lock (synRoot)
                {
                    if (logger == null)
                    {
                        string logFilePath = ConfigurationManager.AppSettings["LogFileName"];
                        logger = new TxtFileLoger(logFilePath);
                    }
                }
            }

            return logger;
        }

        #endregion
    }
}


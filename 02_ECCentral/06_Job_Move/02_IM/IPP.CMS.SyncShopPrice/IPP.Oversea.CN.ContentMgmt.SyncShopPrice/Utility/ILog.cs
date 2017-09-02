using System;

namespace IPP.ContentMgmt.SyncShopPrice.Utility
{
    /// <summary>
    /// Interface of Log
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Write Log content to log file
        /// </summary>
        /// <param name="content">log content</param>
        void WriteLog(string content);

        /// <summary>
        /// Write exception content to log file
        /// </summary>
        /// <param name="exception">error</param>
        void WriteLog(Exception exception);
    }
}


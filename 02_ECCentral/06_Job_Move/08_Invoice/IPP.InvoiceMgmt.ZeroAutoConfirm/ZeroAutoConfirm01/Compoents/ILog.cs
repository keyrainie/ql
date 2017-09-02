using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroAutoConfirm.Compoents
{
    public interface ILog
    {
        /// <summary>
        /// Write Log content to log file
        /// </summary>
        /// <param name="content"></param>
        void WriteLog(string content);
        /// <summary>
        /// Write exception content to log file
        /// </summary>
        /// <param name="exception"></param>
        void WriteLog(Exception exception);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;

namespace ECCentral.Service.RMA.BizProcessor
{
    internal static class CommonCheck
    {
        private static void ThrowRequiredException(string varName)
        {
            string msg = ResouceManager.GetMessageString("RMA.Request", "ObjectIsRequired");
            msg = string.Format(msg, varName);
            throw new BizException(msg);
        }

        public static bool VerifyNotNull(string varName, DateTime? dt)
        {
            if (!dt.HasValue)
            {
                ThrowRequiredException(varName);
            }
            return true;
        }       

        public static bool VerifyNotNull(string varName, int? val)
        {
            if (!val.HasValue)
            {
                ThrowRequiredException(varName);
            }
            return true;
        }

        public static bool VerifyNotNull(string varName, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                ThrowRequiredException(varName);
            }
            return true;
        }

        public static bool VerifyLength(string varName, string str, int maxlength)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > maxlength)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "StringTooLong");
                msg = string.Format(msg, varName, maxlength);
                throw new BizException(msg);
            }
            return true;
        }
    }
}

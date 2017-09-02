using System;
using Nesoft.Utility;

namespace Nesoft.ECWeb.M
{
    public static class ServiceHelper
    {
        public static T GetService<T>() where T : class, new()
        {
            try
            {
                T t = new T();
                return t;
            }
            catch(Exception ex)
            {
                Logger.WriteLog(string.Format("Can't find the implementation service class type:{0}",ex.StackTrace));
                throw new ApplicationException(string.Format("Can't find the implementation service class type:{0}", ex.Message));
            }

        }
 
    }
}

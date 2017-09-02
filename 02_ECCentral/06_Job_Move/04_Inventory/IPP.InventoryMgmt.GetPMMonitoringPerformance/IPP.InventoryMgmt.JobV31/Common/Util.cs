using System;
using System.Text;
using Newegg.Oversea.Framework.Contract;

namespace IPP.InventoryMgmt.JobV31.Common
{
    public static class Util
    {
        /// <summary>
        /// 用于基本的数据类型的转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T[] Converts<T, TSource>(params TSource[] arr)
        {
            if (arr == null)
            {
                return null;
            }
            T[] result = new T[arr.Length];
            Type tType = typeof(T);
            for (int i = 0; i < arr.Length; i++)
            {
                result[i] = (T)Convert.ChangeType(arr[i], tType);
            }
            return result;
        }

        /// <summary>
        /// 将数据通过标识符连接成字符串(仅适用于基本数据类型的转换)
        /// </summary>
        /// <param name="partner"></param>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string Contract<T>(string partner, params T[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                return string.Empty;
            }
            string[] result = Converts<string, T>(arr);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(partner);
                }
                sb.Append(result[i]);
            }

            return sb.ToString();
        }

        public static MessageHeader CreateServiceHeader()
        {
            return CreateServiceHeader(null);
        }

        public static MessageHeader CreateServiceHeader(CommonConst Common)
        {
            Common = Common ?? new CommonConst();      
            MessageHeader header = new MessageHeader();
            header.CompanyCode = Common.CompanyCode;
            header.StoreCompanyCode = Common.StoreCompanyCode;
            header.OperationUser = new OperationUser(
                Common.UserDisplayName
                , Common.UserLoginName
                , Common.StoreSourceDirectoryKey
                , Common.CompanyCode
            );
            return header;
        }
    }
}

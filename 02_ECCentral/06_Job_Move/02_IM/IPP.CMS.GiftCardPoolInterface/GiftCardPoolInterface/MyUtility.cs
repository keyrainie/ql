using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ContentMgmt.GiftCardPoolInterface
{
    public static class MyUtility
    {
        private static readonly int initialCodeValue = ParseCodeValue(ConstValues.InitialCode);
        private static readonly char ZeroChar = ConstValues.AvailableCode[0];

        public static int InitialCodeValue
        {
            get
            {
                return initialCodeValue;
            }
        }

        public static int ParseCodeValue(string code)
        {
            int result = 0;
            foreach (char c in code)
            {
                result *= ConstValues.CodeDimension;
                result += ConstValues.AvailableCode.IndexOf(c);
            }
            return result;
        }

        public static string GenerateCode(int number, int length)
        {
            StringBuilder builder = new StringBuilder();
            while (number > 0)
            {
                builder.Insert(0, ConstValues.AvailableCode[number % ConstValues.CodeDimension]);
                number /= ConstValues.CodeDimension;
            }
            if(builder.Length < length)
            {
                builder.Insert(0, new string(ZeroChar, length - builder.Length));
            }
            return builder.ToString();
        }

        public static void WriteLog(string prefix, int total, int current)
        {
            string message = string.Format("{0} - {1} -> {2}"
                , prefix, total.ToString(), current.ToString());
            WriteLog(message);
        }

        public static void WriteLog(string message)
        {
            Console.WriteLine("{0}: {1}"
                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")
                , message);
        }
    }
}

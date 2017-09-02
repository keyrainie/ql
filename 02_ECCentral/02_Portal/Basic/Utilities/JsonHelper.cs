using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace ECCentral.Portal.Basic.Utilities
{
    public class JsonHelper
    {
        public static string JsonSerializer<T>(T t) where T : class
        {
            if (t == null) return "";
            var ser = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream();
            ser.WriteObject(ms, t);
            var data = ms.ToArray();
            string jsonString = Encoding.UTF8.GetString(data, 0, data.Length);
            ms.Close();
            //替换Json的Date字符串  
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            var matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            var reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }

        private static string ConvertJsonDateToDateString(Match m)
        {
            var dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            var result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        public static T JsonDeserialize<T>(string jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式  
            var p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            var matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            var reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            var ser = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var obj = (T)ser.ReadObject(ms);
            return obj;
        }

        private static string ConvertDateStringToJsonDate(Match m)
        {
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            string result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }

    }

}

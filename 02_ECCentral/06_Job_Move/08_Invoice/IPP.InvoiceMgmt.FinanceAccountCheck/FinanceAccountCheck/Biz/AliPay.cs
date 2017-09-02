using System.Web;
using System.Text;
using System.Security.Cryptography;
/// <summary>
/// New Interface for AliPay
/// </summary>
namespace IPPOversea.Invoicemgmt.ZFBAccountCheck.Biz
{
    public class AliPay
    {
        /// <summary>
        /// 与ASP兼容的MD5加密算法
        /// </summary>
        public static string GetMD5(string s, string _input_charset)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(s));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 冒泡排序法
        /// 按照字母序列从a到z的顺序排列
        /// </summary>
        public static string[] BubbleSort(string[] r)
        {
            int i, j; //交换标志 
            string temp;

            bool exchange;

            for (i = 0; i < r.Length; i++) //最多做R.Length-1趟排序 
            {
                exchange = false; //本趟排序开始前，交换标志应为假

                for (j = r.Length - 2; j >= i; j--)
                {//交换条件
                    if (System.String.CompareOrdinal(r[j + 1], r[j]) < 0)
                    {
                        temp = r[j + 1];
                        r[j + 1] = r[j];
                        r[j] = temp;

                        exchange = true; //发生了交换，故将交换标志置为真 
                    }
                }

                if (!exchange) //本趟排序未发生交换，提前终止算法 
                {
                    break;
                }
            }
            return r;
        }

        public static string CreatUrl(
            string gateway,
            string service,
            string partner,
            string gmt_create_start,
            string gmt_create_end,
            string trans_code,
            string sign_type,
            string key,
            string _input_charset
            )
        {
            int i;

            //构造数组；
            //以下数组即是参与加密的参数，若参数的值不允许为空，若该参数为空，则不要成为该数组的元素
            string[] Oristr ={ 
                "service="+service, 
                "partner=" + partner,
                "gmt_create_start=" + gmt_create_start,
                "gmt_create_end=" + gmt_create_end,
                "trans_code=" + trans_code,
                "_input_charset="+_input_charset,
                "no_coupon=y"
                };

            //进行排序；
            string[] Sortedstr = BubbleSort(Oristr);


            //构造待md5摘要字符串 ；

            StringBuilder prestr = new StringBuilder();

            for (i = 0; i < Sortedstr.Length; i++)
            {
                if (i == Sortedstr.Length - 1)
                {
                    prestr.Append(Sortedstr[i]);

                }
                else
                {
                    prestr.Append(Sortedstr[i] + "&");
                }

            }

            prestr.Append(key);

            //生成Md5摘要；
            string sign = GetMD5(prestr.ToString(), _input_charset);

            //构造支付Url；
            StringBuilder parameter = new StringBuilder();
            parameter.Append(gateway);
            for (i = 0; i < Sortedstr.Length; i++)
            {
                parameter.Append(Sortedstr[i] + "&");
            }

            parameter.Append("sign=" + sign + "&sign_type=" + sign_type);

            //返回支付Url；
            return parameter.ToString();
        }

    }
}
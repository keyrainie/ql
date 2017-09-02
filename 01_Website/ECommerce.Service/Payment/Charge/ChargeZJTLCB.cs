using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using Nesoft.ECWeb.Entity.Payment;
using Nesoft.Utility;

namespace Nesoft.ECWeb.Facade.Payment.Charge
{
    /// <summary>
    /// 泰隆银行网上支付接口
    /// </summary>
    public class ChargeZJTLCB : Charges
    {
        #region 网关动态调用dll方法 - 旧的，暂时不用
        ///// <summary>
        ///// The DLL name
        ///// </summary>
        //private const string DLLFileName = "zjtl_sign_verifySign.dll";

        ///// <summary>
        ///// The sign method name
        ///// </summary>
        //private const string SignMethodEntryPoint = "zjtl_pkcs7Sign";

        ///// <summary>
        ///// The verify sign method name
        ///// </summary>
        //private const string VerifySignMethodEntryPoint = "zjtl_pkcs7VerifySign";

        ///// <summary>
        ///// The DLL load path
        ///// </summary>
        //private static string dllLoadPath = HttpContext.Current.Server.MapPath(@"~/Bin/" + DLLFileName.TrimStart('/'));

        ///// <summary>
        ///// SignDelegate  功能：对原文数据进行pkcs7 attached签名
        /////
        /////	参数说明：
        /////
        /////	strPfxFile：  用户pfx格式签名证书的完整路径和文件名。
        /////	strPfxPwd：   用户pfx格式证书的保护口令（不能为空）。
        /////	originData:   原文数据。
        /////	outContext:   base64编码后的签名数据，如果为NULL，则函数返回签名数据的长度。
        /////
        /////	返回值：  > 0	 成功返回签名数据的长度；< 0 失败，返回错误码。
        ///// </summary>
        ///// <param name="strPfxFile">The string PFX file.</param>
        ///// <param name="strPfxPwd">The string PFX password.</param>
        ///// <param name="originData">The origin data.</param>
        ///// <param name="outContext">The out context.</param>
        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate int SignDelegate(char[] strPfxFile, char[] strPfxPwd, char[] originData, StringBuilder outContext);

        ///// <summary>
        ///// VerifySignDelegate  功能：对原文数据验证pkcs7 attach签名
        /////
        /////	参数说明：
        /////
        /////	pOriginData: 原文数据。
        /////	pb64SignData:  base64编码的签名数据。
        /////
        /////	返回值：> 0	成功   返回签名证书的长度；< 0 	失败，返回错误码。
        ///// </summary>
        ///// <param name="pOriginData">原文数据.</param>
        ///// <param name="pb64SignData">base64编码的签名数据.</param>
        ///// <returns></returns>
        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate int VerifySignDelegate(char[] pOriginData, char[] pSignData);

        ///// <summary>
        ///// Tries to sign data.
        ///// </summary>
        ///// <param name="strPfxFile">The string PFX file.</param>
        ///// <param name="strPfxPwd">The string PFX password.</param>
        ///// <param name="context">The context.</param>
        ///// <param name="outContext">The out context.</param>
        ///// <returns></returns>
        //private int TryToSignData(string strPfxFile, string strPfxPwd, string context, out string outContext)
        //{
        //    int result = 0;
        //    outContext = string.Empty;

        //    FunctionLoader loader = new FunctionLoader(dllLoadPath);
        //    SignDelegate signDataDelete = (SignDelegate)loader.LoadFunction<SignDelegate>(SignMethodEntryPoint);

        //    char[] file = (strPfxFile.ToCharArray());
        //    char[] pwd = (strPfxPwd.ToCharArray());
        //    char[] con = (context.ToCharArray());

        //    StringBuilder sb = new StringBuilder(1024 * 2);
        //    result = signDataDelete(file, pwd, con, sb);
        //    outContext = sb.ToString();

        //    return result;
        //}

        ///// <summary>
        ///// Try to verify sign data.
        ///// </summary>
        ///// <param name="pOriginData">The p origin data.</param>
        ///// <param name="pSignData">The p sign data.</param>
        ///// <returns></returns>
        //private int TryToVerifySignData(string pOriginData, string pSignData)
        //{
        //    FunctionLoader loader = new FunctionLoader(dllLoadPath);
        //    VerifySignDelegate verifySignDelete = (VerifySignDelegate)loader.LoadFunction<VerifySignDelegate>(VerifySignMethodEntryPoint);

        //    char[] oData = (pOriginData.ToCharArray());
        //    char[] pData = (pSignData.ToCharArray());

        //    return verifySignDelete(oData, pData);
        //}
        #endregion

        //[DllImport(@"C:\zjtl_sign_verifySign\zjtl_sign_verifySign.dll"
        //           , CallingConvention = CallingConvention.Cdecl
        //           , EntryPoint = "zjtl_pkcs7Sign"
        //           , ThrowOnUnmappableChar = true
        //           , SetLastError = true
        //           , PreserveSig = true
        //           , CharSet = CharSet.Ansi)]
        //static extern int TryToSignData(char[] strPfxFile, char[] strPfxPwd, char[] context, StringBuilder outContext);

        //[DllImport(@"C:\zjtl_sign_verifySign\zjtl_sign_verifySign.dll"
        //           , CallingConvention = CallingConvention.Cdecl
        //           , EntryPoint = "zjtl_pkcs7VerifySign"
        //           , ThrowOnUnmappableChar = true
        //           , SetLastError = true
        //           , PreserveSig = true
        //           , CharSet = CharSet.Ansi)]
        //static extern int TryToVerifySignData(char[] pOriginData, char[] pSignData);

        /// <summary>
        /// Tries to sign data.
        /// </summary>
        /// <param name="strPfxFile">The string PFX file.</param>
        /// <param name="strPfxPwd">The string PFX password.</param>
        /// <param name="context">The context.</param>
        /// <param name="outContext">The out context.</param>
        /// <returns></returns>
        private string TryToSignData(string strPfxFile, string strPfxPwd, string context)
        {
            //int result = 0;
            //outContext = string.Empty;
            //char[] file = (strPfxFile.ToCharArray());
            //char[] pwd = (strPfxPwd.ToCharArray());
            //char[] con = (context.ToCharArray());
            //StringBuilder retpara = new StringBuilder(1024 * 2);
            //result = TryToSignData(file, pwd, con, retpara);
            //outContext = retpara.ToString();
            return "";// MerchantUtil.sign(strPfxFile, strPfxPwd, context);
        }
        //private int TryToSignData(string strPfxFile, string strPfxPwd, string context, out string outContext)
        //{
        //    int result = 0;
        //    //outContext = string.Empty;
        //    //char[] file = (strPfxFile.ToCharArray());
        //    //char[] pwd = (strPfxPwd.ToCharArray());
        //    //char[] con = (context.ToCharArray());
        //    StringBuilder retpara = new StringBuilder(1024 * 2);
        //    //result = TryToSignData(file, pwd, con, retpara);
        //    outContext = retpara.ToString();

        //    return result;
        //}

        /// <summary>
        /// Try to verify sign data.
        /// </summary>
        /// <param name="pOriginData">The p origin data.</param>
        /// <param name="pSignData">The p sign data.</param>
        /// <returns></returns>
        private int TryToVerifySignData(string pOriginData, string pSignData)
        {
            //char[] oData = (pOriginData.ToCharArray());
            //char[] pData = (pSignData.ToCharArray());
            //return TryToVerifySignData(oData, pData);
            return 0;// MerchantUtil.verifyZjtlcbSign(pOriginData, pSignData);
        }

        /// <summary>
        /// Sets the request form.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void SetRequestForm(Entity.Payment.ChargeContext context)
        {
            context.RequestForm["merchantNo"]   = context.PaymentInfo.PaymentMode.BankCert;
            context.RequestForm["orderNo"]      = context.SOInfo.SoSysNo.ToString();
            context.RequestForm["orderAmt"]     = context.SOInfo.RealPayAmt.ToString("F2");
            context.RequestForm["orderDate"]    = context.SOInfo.OrderDate.ToString("yyyyMMdd");
            context.RequestForm["orderTime"]    = context.SOInfo.OrderDate.ToString("HHmmss");
            context.RequestForm["currencyType"] = "01";
            context.RequestForm["ifAutoJump"]   = "1";
            context.RequestForm["merURL"]       = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl,
                context.PaymentInfo.PaymentMode.PaymentCallbackUrl);
            context.RequestForm["ifInfoMer"]    = "1";
            context.RequestForm["infoMerURL"]   = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl,
                context.PaymentInfo.PaymentMode.PaymentBgCallbackUrl);
            context.RequestForm["payUse"]       = "0";

            context.RequestForm["merSignMsg"]   = SignData(context);
            Logger.WriteLog(string.Format("中间日志,merURL:{0},infoMerURL:{1}", context.RequestForm["merURL"], context.RequestForm["infoMerURL"]), "ChargeZJTLCB", "RequestForm");
        }

        /// <summary>
        /// Verifies the sign.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override bool VerifySign(Entity.Payment.CallbackContext context)
        {
            return context.PaymentInfo.PaymentMode.BankCert == context.ResponseForm["merchantNo"]
                && context.ResponseForm["signMsgBank"].Length > 0
                && VerifyCallbackData(context);
        }

        /// <summary>
        /// Gets the pay result.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override bool GetPayResult(Entity.Payment.CallbackContext context)
        {
            return context.ResponseForm["tranResult"].Equals("20");
        }

        /// <summary>
        /// Gets the pay amount.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override decimal GetPayAmount(Entity.Payment.CallbackContext context)
        {
            return decimal.Parse(context.ResponseForm["orderAmt"]);
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override string GetSerialNumber(Entity.Payment.CallbackContext context)
        {
            return context.ResponseForm["payFlowNo"];
        }

        /// <summary>
        /// Gets the so system no.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override int GetSOSysNo(Entity.Payment.CallbackContext context)
        {
            return int.Parse(context.ResponseForm["orderNo"]);
        }

        /// <summary>
        /// Gets the pay process time.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override string GetPayProcessTime(Entity.Payment.CallbackContext context)
        {
            return context.ResponseForm["transferTime"];
        }

        /// <summary>
        /// Signs the data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private string SignData(ChargeContext context)
        {
            string result         = string.Empty;

            string strPfxFile     = string.Empty;
            string strPfxPwd      = context.PaymentInfo.PaymentMode.CustomConfigs["PfxPwd"];
            string pOriginData    = string.Empty;

            if (!Path.IsPathRooted(context.PaymentInfo.PaymentMode.BankCertKey))
            {
                strPfxFile = HttpContext.Current.Server.MapPath(@"~/bin/" + context.PaymentInfo.PaymentMode.BankCertKey.TrimStart('/').TrimStart('\\'));
            }
            else
            {
                strPfxFile = context.PaymentInfo.PaymentMode.BankCertKey;
            }

            StringBuilder sb      = new StringBuilder();

            sb.Append(context.RequestForm["merchantNo"]);
            sb.Append("|");
            sb.Append(context.RequestForm["orderNo"]);
            sb.Append("|");
            sb.Append(context.RequestForm["orderAmt"]);
            sb.Append("|");
            sb.Append(context.RequestForm["orderDate"]);
            sb.Append("|");
            sb.Append(context.RequestForm["orderTime"]);
            sb.Append("|");
            sb.Append(context.RequestForm["currencyType"]);

            pOriginData = sb.ToString();

            Logger.WriteLog(string.Format("中间日志,strPfxFile:{0},strPfxPwd:{1},pOriginData:{2}", strPfxFile, strPfxPwd, pOriginData), "ChargeZJTLCB", "SignData");
            string code = TryToSignData(strPfxFile, strPfxPwd, pOriginData);


            if (!string.IsNullOrWhiteSpace(code))
            {
                return code;
            }
            else
            {
                Logger.WriteLog(string.Format("签名失败，错误代码：{0}，", code), "ChargeZJTLCB", "SignData");

                return string.Empty;
            }

        }

        /// <summary>
        /// Signs the data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private bool VerifyCallbackData(CallbackContext context)
        {
            string sign  = context.ResponseForm["signMsgBank"];
            string pOriginData = string.Empty;

            StringBuilder sb   = new StringBuilder();

            sb.Append(context.ResponseForm["merchantNo"]);
            sb.Append("|");
            sb.Append(context.ResponseForm["orderNo"]);
            sb.Append("|");
            sb.Append(context.ResponseForm["orderAmt"]);
            sb.Append("|");
            sb.Append(context.ResponseForm["tranResult"]);
            sb.Append("|");
            sb.Append(context.ResponseForm["chargeFee"]);
            sb.Append("|");
            sb.Append(context.ResponseForm["transferTime"]);
            sb.Append("|");
            sb.Append(context.ResponseForm["payFlowNo"]);

            pOriginData = sb.ToString();
            Logger.WriteLog(string.Format("中间日志,tranResult:{0},pOriginData:{1},sign{2}", context.ResponseForm["tranResult"], pOriginData, sign), "ChargeZJTLCB", "VerifyCallbackData");
            int code = TryToVerifySignData(pOriginData, sign);
            //if (code > 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    Logger.WriteLog(string.Format("验证签名失败，错误代码：{0}，", code), "ChargeZJTLCB", "VerifyCallbackData");
            //    return false;
            //}

            if (code == 0)
            {
                return true;
            }
            else if (code == 1)
            {
                Logger.WriteLog(string.Format("验证签名失败，错误代码：{0}，签名证书不正确", code), "ChargeZJTLCB", "VerifyCallbackData");
                return false;
            }
            else if (code == 2)
            {
                Logger.WriteLog(string.Format("验证签名失败，错误代码：{0}", code), "ChargeZJTLCB", "VerifyCallbackData");
                return false;
            }
            else
            {
                Logger.WriteLog(string.Format("异常，错误代码：{0}，", code), "ChargeZJTLCB", "VerifyCallbackData");
                return false;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class FunctionLoader
    {
        /// <summary>
        /// The pointer
        /// </summary>
        private IntPtr pointer;

        /// <summary>
        /// Load the library.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        [DllImport("Kernel32.dll")]
        private static extern IntPtr LoadLibrary(string path);

        /// <summary>
        /// Free the library.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr pointer);

        /// <summary>
        /// Gets the proc address.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="procName">Name of the proc.</param>
        /// <returns></returns>
        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr pointer, string procName);

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionLoader"/> class.
        /// </summary>
        /// <param name="path">The DLL path.</param>
        public FunctionLoader(String path)
        {
            pointer = LoadLibrary(path);
        }

         /// <summary>
         /// Finalizes an instance of the <see cref="FunctionLoader"/> class.
         /// </summary>
         ~FunctionLoader()
         {
             FreeLibrary(pointer);
         }

         /// <summary>
         /// Loads the function.
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="functionName">Name of the function.</param>
         /// <returns></returns>
        public Delegate LoadFunction<T>(string functionName)
        {
            IntPtr functionAddress = GetProcAddress(pointer, functionName);
            return Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(T));
        }
    }
}

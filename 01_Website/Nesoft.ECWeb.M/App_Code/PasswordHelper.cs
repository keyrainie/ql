using System;
using System.Security.Cryptography;
using System.Text;
using Nesoft.ECWeb.Entity.Member;
using Nesoft.ECWeb.Enums;

namespace Nesoft.ECWeb.M.App_Code
{
    public class PasswordHelper
    {
        ///// <summary>
        ///// 对字符串进行加密
        ///// </summary>
        ///// <param name="oldstring"></param>
        ///// <returns></returns>
        //public static string GetEncryptedPassword(string oldstring)
        //{
        //    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
        //    byte[] source = Encoding.UTF8.GetBytes(oldstring);
        //    byte[] destination = sha1.ComputeHash(source);
        //    sha1.Clear();
        //    (sha1 as IDisposable).Dispose();
        //    return Convert.ToBase64String(destination);
        //}

        // [2014/12/22 by Swika]增加支持第三方系统导入的账号的密码验证
        /// <summary>
        /// 对字符串进行加密
        /// </summary>
        /// <param name="passwordPlaintext">待加密的密码明文</param>
        /// <param name="encryptMeta">加密使用方式元信息</param>
        /// <returns></returns>
        public static string GetEncryptedPassword(string passwordPlaintext, EncryptMetaInfo encryptMeta)
        {
            string sourcePlaintext = passwordPlaintext + encryptMeta.PasswordSalt;

            switch (encryptMeta.EncryptMode)
            {
                case EncryptMode.SHA1:
                    {
                        SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                        byte[] source = Encoding.UTF8.GetBytes(sourcePlaintext);
                        byte[] destination = sha1.ComputeHash(source);
                        sha1.Clear();
                        (sha1 as IDisposable).Dispose();
                        return Convert.ToBase64String(destination);
                    }
                case EncryptMode.MD5:
                    {
                        byte[] destination = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(sourcePlaintext));
                        return Convert.ToBase64String(destination);
                    }
                default:
                    throw new NotSupportedException();
            }
        }


        public static void GetNewPasswordAndSalt(ref string newPassword, ref string encryptPassword, ref string passwordSalt)
        {
            passwordSalt = Guid.NewGuid().ToString("N");
            //EncryptType encryptionStatus = EncryptType.Off;
            //encryptPassword = GetEncryptedPassword(newPassword.Trim() + passwordSalt);
            encryptPassword = GetEncryptedPassword(newPassword.Trim(), new EncryptMetaInfo(passwordSalt));
            newPassword = string.Empty;
        }
    }
}
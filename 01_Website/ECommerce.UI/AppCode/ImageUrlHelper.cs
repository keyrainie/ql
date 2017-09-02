using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using ECommerce.Enums;

namespace ECommerce.UI
{
    public class ImageUrlHelper
    {
        /// <summary>
        /// 适用于内容图片
        /// </summary>
        /// <param name="imgRelPath"></param>
        /// <param name="imagesize"></param>
        /// <returns></returns>
        public static string BuildTopicImageSrc(string imgRelPath, EImageSize imagesize)
        {
            if (string.IsNullOrWhiteSpace(imgRelPath))
            {
                return string.Empty;
            }

            string src = ConfigurationManager.AppSettings["FileBaseUrl"].ToString().Trim().TrimEnd("/".ToCharArray());
            src += "/Content/";
            if (imagesize == EImageSize.Icon)
            {
                src += "100/";
            }
            else if (imagesize == EImageSize.Small)
            {
                src += "200/";
            }
            else if (imagesize == EImageSize.Middle)
            {
                src += "300/";
            }
            else if (imagesize == EImageSize.Big)
            {
                src += "600/";
            }
            else
            {
                src += "source/";
            }
            src += imgRelPath;
            return src;

        }

        public static string BuildProductImageSrc(string imgRelPath, EImageSize imagesize)
        {
            if (string.IsNullOrWhiteSpace(imgRelPath))
            {
                return string.Empty;
            }

            string src = ConfigurationManager.AppSettings["FileBaseUrl"].ToString().Trim().TrimEnd("/".ToCharArray());
            src += "/Product/";
            if (imagesize == EImageSize.Icon)
            {
                src += "160/";
            }
            else if (imagesize == EImageSize.Small)
            {
                src += "320/";
            }
            else if (imagesize == EImageSize.Middle)
            {
                src += "480/";
            }
            else if (imagesize == EImageSize.Big)
            {
                src += "960/";
            }
            else
            {
                src += "960/";
            }
            src += imgRelPath;
            return src;

        }

         
        /// <summary>
        /// 非图片的资源文件
        /// </summary>
        /// <param name="fileRelPath"></param>
        /// <returns></returns>
        public static string BuildFileSrc(string fileRelPath)
        {
            if (string.IsNullOrWhiteSpace(fileRelPath))
            {
                return string.Empty;
            }
            if (fileRelPath.ToUpper().StartsWith("HTTP"))
            {
                return fileRelPath;
            }
            string src = ConfigurationManager.AppSettings["FileBaseUrl"].ToString().Trim().TrimEnd("/".ToCharArray());

            return src + "/Content/File/" + fileRelPath;
        }
    }
}
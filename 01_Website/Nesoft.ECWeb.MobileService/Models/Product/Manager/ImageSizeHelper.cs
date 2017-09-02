using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Nesoft.ECWeb.MobileService.Models.Product.Config;
using Nesoft.Utility;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Facade.Product;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public class ImageUrlHelper
    {
        private static ImageSizeConfig GetImageSizeConfig() 
        { 
            var configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration/ImageSize.config");
            return CacheManager.ReadXmlFileWithLocalCache<ImageSizeConfig>(configPath);
        }

        public static ImageSize GetImageSize(ImageType imageType)
        {
            ImageSize imageSize;
            
            string clientType = HeaderHelper.GetClientType().ToString();
            ImageSizeConfig imageSizeConfig = GetImageSizeConfig();
            var imageSettings = imageSizeConfig.Settings.Where(item => string.Compare(item.ImageResolution, clientType,true) == 0).SingleOrDefault();
            var deviceSettings = imageSettings.ImageSizes.Where(item => string.Compare(imageType.ToString(), item.ImageType, true) == 0).SingleOrDefault();
            Enum.TryParse<ImageSize>(deviceSettings.ImageSize, out imageSize);

            return imageSize;
        }
    }
}
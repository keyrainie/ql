using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
   public class ProductImage
    {
       /// <summary>
       /// 图片编号
       /// </summary>
       public int ImageSysNo { get; set; }
       /// <summary>
       /// 图片资源地址
       /// </summary>
       public string ResourceUrl { get; set; }
       /// <summary>
       /// 图片类型
       /// I 普通图片
       /// D 360图片
       /// V Video
       /// </summary>
       public string ImageType { get; set; }
       /// <summary>
       /// 图片版本号
       /// </summary>
       public string ImageVersion { get; set; }

       public ProductImageType ImageTypeFormat
       {
           get
           {
               ProductImageType tmpImageType;
               switch (this.ImageType)
               {
                   case "V":
                       tmpImageType= ProductImageType.Video;
                       break;
                   default:
                       tmpImageType= ProductImageType.Image;
                       break;
               }
               return tmpImageType;
           }
           
       }
    }
}

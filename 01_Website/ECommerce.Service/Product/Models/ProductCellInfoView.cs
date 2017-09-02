using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Product;
using ECommerce.Enums;

namespace ECommerce.Facade.Product.Models
{
    /// <summary>
    /// 商品控件VIIE
    /// </summary>
   public class ProductCellInfoView
    {
       public ProductCellInfoView()
       {
           this.ProductInfo = new ProductItemInfo();
       }
       /// <summary>
       /// 商品信息
       /// </summary>
       public ProductItemInfo ProductInfo { get; set; }


        #region 自定义信息

       /// <summary>
       /// 图片尺寸(默认P220)
       /// </summary>
       private ImageSize productImageSize=ImageSize.P220;
       public ImageSize ProductImageSize 
       {
           get{return this.productImageSize;}
           set{this.productImageSize=value;}
       }

       /// <summary>
       /// 是否显示促销(默认false)
       /// </summary>
       private bool isShowPromotion=false;
       public bool IsShowPromotion 
       {
           get{return isShowPromotion;}
           set{this.isShowPromotion=value;}
       }

       /// <summary>
       /// 是否显示市场价(默认false)
       /// </summary>
       private bool isShowBasicPrice = false;
       public bool IsShowBasicPrice
       {
           get { return this.isShowBasicPrice; }
           set { this.isShowBasicPrice = value; }
       }


        #endregion
    }
}

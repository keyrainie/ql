using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductRelatedVM : ModelBase
    {
        /// <summary>
        /// 主商品ID
        /// </summary>

          private string productSysNo;
          [Validate(ValidateType.Required)]
          public string ProductSysNo
          {

              get { return productSysNo; }
              set { SetValue("ProductSysNo", ref productSysNo, value); }
          }

        /// <summary>
        /// 相关商品ID
        /// </summary>

          private string relatedProductSysNo;
          [Validate(ValidateType.Required)]
          public string RelatedProductSysNo
          {
              get { return relatedProductSysNo; }
              set
              {
                  SetValue("RelatedProductSysNo", ref relatedProductSysNo, value);
              }
          }

        /// <summary>
        /// 优先级
        /// </summary>


          private string priority;
          [Validate(ValidateType.Regex, @"^[0-9]*[1-9][0-9]*$", ErrorMessageResourceType=typeof(ResBrandQuery),ErrorMessageResourceName="Error_ValidateIntHint")]
          [Validate(ValidateType.Required)]
          public string Priority {
              get { return priority; }
              set
              {
                  SetValue("Priority", ref priority, value);
              }
          }

        /// <summary>
        /// 是否相互相关
        /// </summary>
        public bool IsMutual { get; set; }

        public int SysNo { get; set; }

        /// <summary>
        /// 主商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 相关商品ID
        /// </summary>
        public string RelatedProductID { get; set; }
    }
}

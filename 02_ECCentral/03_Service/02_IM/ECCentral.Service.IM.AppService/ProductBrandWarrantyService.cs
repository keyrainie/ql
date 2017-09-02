using System.Collections.Generic;
using System.Data;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductBrandWarrantyService))]
    public class ProductBrandWarrantyService
    {
        #region private
         private ProductBrandWarrantyProcessor Biz =  
             ObjectFactory<ProductBrandWarrantyProcessor>.Instance;
        #endregion

         public void BrandWarrantyInfoByAddOrUpdate(ProductBrandWarrantyInfo ProductBrandWarranty)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                Biz.BrandWarrantyInfoByAddOrUpdate(ProductBrandWarranty);
            }
            catch (BizException ex)
            {
                result.AppendLine(string.Format(ResouceManager.GetMessageString("IM.Category", "FailReason") + "：{0}", ex.Message));
                throw new BizException(result.ToString());
            }
            //result.AppendLine(string.Format("提交成功！"));
            //throw new BizException(result.ToString());
        }

         public void DelBrandWarrantyInfoBySysNos(List<ProductBrandWarrantyInfo> ProductBrandWarrantyInfos)
         {
             StringBuilder result = new StringBuilder();
             try
             {
                 Biz.DelBrandWarrantyInfoBySysNos(ProductBrandWarrantyInfos);
             }
             catch (BizException ex)
             {
                 result.AppendLine(string.Format(ResouceManager.GetMessageString("IM.Category", "FailReason") + "：{0}", ex.Message));
                 throw new BizException(result.ToString());
             }
             //result.AppendLine(string.Format("提交成功！"));
             //throw new BizException(result.ToString());
         }

         public void UpdateBrandWarrantyInfoBySysNo(ProductBrandWarrantyInfo productBrandWarranty)
         {
             StringBuilder result = new StringBuilder();
             try
             {
                 Biz.UpdateBrandWarrantyInfoBySysNo(productBrandWarranty);
             }
             catch (BizException ex)
             {
                 result.AppendLine(string.Format(ResouceManager.GetMessageString("IM.Category", "FailReason") + "：{0}", ex.Message));
                 throw new BizException(result.ToString());
             }
             //result.AppendLine(string.Format("提交成功！"));
             //throw new BizException(result.ToString());
         }
    }
}

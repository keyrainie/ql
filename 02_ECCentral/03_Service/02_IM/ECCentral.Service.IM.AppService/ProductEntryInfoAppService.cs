using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Service.IM.AppService
{
     [VersionExport(typeof(ProductEntryInfoAppService))]
    public class ProductEntryInfoAppService
    {
         //public bool InsertEntryInfo(ProductEntryInfo entity)
         //{
         //    return ObjectFactory<ProductEntryInfoProcessor>.Instance.InsertEntryInfo(entity);
         //}

         public bool UpdateEntryInfo(ProductEntryInfo entity)
         {
             return ObjectFactory<ProductEntryInfoProcessor>.Instance.UpdateEntryInfo(entity);
         }

         public ProductEntryInfo LoadProductEntryInfo(int productSysNo)
         {
             return ObjectFactory<ProductEntryInfoProcessor>.Instance.LoadProductEntryInfo(productSysNo);
         }

         /// <summary>
         /// 审核通过
         /// </summary>
         /// <param name="sysNo"></param>
         /// <returns></returns>
         public bool AuditSucess(ProductEntryInfo info)
         {
             return ObjectFactory<ProductEntryInfoProcessor>.Instance.AuditSucess(info);
         }

         /// <summary>
         /// 审核失败
         /// </summary>
         /// <param name="sysNo"></param>
         /// <returns></returns>
         public bool AuditFail(ProductEntryInfo info)
         {
             return ObjectFactory<ProductEntryInfoProcessor>.Instance.AuditFail(info);
         }

         /// <summary>
         /// 提交商检
         /// </summary>
         /// <param name="sysNo"></param>
         /// <returns></returns>
         public bool ToInspection(ProductEntryInfo info)
         {
             return ObjectFactory<ProductEntryInfoProcessor>.Instance.ToInspection(info);
         }

         /// <summary>
         /// 商检通过
         /// </summary>
         /// <param name="sysNo"></param>
         /// <returns></returns>
         public bool InspectionSucess(ProductEntryInfo info)
         {
             return ObjectFactory<ProductEntryInfoProcessor>.Instance.InspectionSucess(info);
         }

         /// <summary>
         /// 商检失败
         /// </summary>
         /// <param name="sysNo"></param>
         /// <returns></returns>
         public bool InspectionFail(ProductEntryInfo info)
         {
             return ObjectFactory<ProductEntryInfoProcessor>.Instance.InspectionFail(info);
         }

         /// <summary>
         /// 提交报关
         /// </summary>
         /// <param name="sysNo"></param>
         /// <returns></returns>
         public bool ToCustoms(ProductEntryInfo info)
         {
             return ObjectFactory<ProductEntryInfoProcessor>.Instance.ToCustoms(info);
         }

         /// <summary>
         /// 报关成功
         /// </summary>
         /// <param name="sysNo"></param>
         /// <returns></returns>
         public bool CustomsSuccess(ProductEntryInfo info)
         {
             return ObjectFactory<ProductEntryInfoProcessor>.Instance.CustomsSuccess(info);
         }

         /// <summary>
         /// 报关失败
         /// </summary>
         /// <param name="sysNo"></param>
         /// <returns></returns>
         public bool CustomsFail(ProductEntryInfo info)
         {
             return ObjectFactory<ProductEntryInfoProcessor>.Instance.CustomsFail(info);
         }
    }
}

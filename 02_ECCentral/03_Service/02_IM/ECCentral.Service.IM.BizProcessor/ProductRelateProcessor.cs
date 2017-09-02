using System;
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductRelateProcessor))]
    public class ProductRelateProcessor
    {
        private readonly IProductRelatedDA productRelatedDA = ObjectFactory<IProductRelatedDA>.Instance;

        /// <summary>
        /// 创建相关商品
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual ProductRelatedInfo CreateItemRelated(ProductRelatedInfo info)
        {
            int result = productRelatedDA.CreateProductRelate(info);
            if (result == -1)
            {
                //主商品必须是show(1)状态！（主商品）
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedResult1"));
            }
            else if (result == 1)
            {
                //相关商品必须是show状态！（从商品）
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedResult2"));
            }
            else if (result == -2)
            {
                //相关商品所属类别必须是Valid(0)状态！（主商品）
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedResult3"));
            }
            else if (result == 2)
            {
                //相关商品所属类别必须是Valid状态！（从商品）
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedResult4"));
            }
            else if (result == 3)
            {
                //相关商品所属类别必须是相关类别！
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedResult5"));
            }
            else if (result == 4)
            {
                //相关商品已存在！
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedResult6"));
            }

            return info;
        }
        public virtual void DeleteItemRelated(List<string> list)
        {
            foreach (var item in list)
            {
                productRelatedDA.DeleteProductRelate(item);
            }
        }
        public void UpdateProductRelatePriority(List<ProductRelatedInfo> list)
        {
            foreach (var item in list)
            {
                productRelatedDA.UpdateProductRelatePriority(item);
            }
        }
        /// <summary>
        /// 批量设置相关商品
        /// </summary>
        /// <param name="listInfo"></param>
        public void CreateItemRelatedByList(List<ProductRelatedInfo> listInfo)
        {

            //异常的集合
            Dictionary<int, List<ProductRelatedInfo>> dicError = new Dictionary<int, List<ProductRelatedInfo>>();
            bool flag = false;
            foreach (var info in listInfo)
            {
                int result = productRelatedDA.CreateProductRelate(info);
                if (dicError.ContainsKey(result))
                {
                    if (result != 0)
                    {
                        dicError[result].Add(info);
                    }
                   
                }
                else
                {
                    if (result != 0)
                    {
                        dicError.Add(result, new List<ProductRelatedInfo>() { info });
                    }
                }
            }
            if (dicError.Count > 0)
            {
                string ErrorMessage = "";
                foreach (var item in dicError.Keys)
                {
                    if (item == -1)
                    {
                        foreach (var info in dicError[item])
                        {
                            ErrorMessage = ErrorMessage + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult1") + info.ProductID + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult5");
                        }
                       
                    }
                    if (item == 1)
                    {
                        foreach (var info in dicError[item])
                        {
                            ErrorMessage = ErrorMessage + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult2") + info.RelatedProductID + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult5");
                        }
                       
                    }
                    if (item == -2)
                    {
                        foreach (var info in dicError[item])
                        {
                            ErrorMessage = ErrorMessage + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult1") + info.ProductID + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult6");
                        }
                    }
                    if (item == -2)
                    {
                        foreach (var info in dicError[item])
                        {
                            ErrorMessage = ErrorMessage + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult2") + info.RelatedProductID + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult6");
                        }
                    }
                    if (item == 3)
                    {
                        foreach (var info in dicError[item])
                        {
                            ErrorMessage = ErrorMessage + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult3") + info.ProductID + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult4") + info.RelatedProductID + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult7");
                        }
                       
                    }
                    if (item == 4)
                    {
                        foreach (var info in dicError[item])
                        {
                            ErrorMessage = ErrorMessage + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult3") + info.ProductID + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult4") + info.RelatedProductID + ResouceManager.GetMessageString("IM.Product", "CreateItemRelatedByListResult8");
                        }
                    }
                }
                throw new BizException(ErrorMessage);
            }

        }
    }
}



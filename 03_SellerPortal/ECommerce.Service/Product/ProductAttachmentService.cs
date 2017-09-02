using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Product;
using ECommerce.Entity.Common;
using ECommerce.Entity.Product;
using ECommerce.Utility;

namespace ECommerce.Service.Product
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProductAttachmentService
    {
        /// <summary>
        /// Queries the product attachment.
        /// </summary>
        /// <param name="queryCriteria">The query criteria.</param>
        /// <returns></returns>
        public static QueryResult<ProductAttachmentQueryBasicInfo> QueryProductAttachment(ProductAttachmentQueryFilter queryCriteria)
        {
            int totalCount = 0;
            QueryResult<ProductAttachmentQueryBasicInfo> result = new QueryResult<ProductAttachmentQueryBasicInfo>();

            List<ProductAttachmentQueryBasicInfo> list =
                ProductAttachmentDA.QueryProductAttachment(queryCriteria, out totalCount);

            result.ResultList = list;
            result.PageInfo = new PageInfo
            {
                PageIndex = queryCriteria.PageIndex,
                PageSize = queryCriteria.PageSize,
                TotalCount = totalCount,
            };

            return result;
        }

        /// <summary>
        /// Deletes the attachment by product system no.
        /// </summary>
        /// <param name="producySysNo">The producy system no.</param>
        public static void DeleteAttachmentByProductSysNo(int producySysNo)
        {
            CheckIsExistProduct(producySysNo);
            ProductAttachmentDA.DeleteAttachmentByProductSysNo(producySysNo);
        }

        /// <summary>
        /// Gets the product attachment list.
        /// </summary>
        /// <param name="productSysNo">The product system no.</param>
        /// <returns></returns>
        public static QueryResult<ProductAttachmentInfo> GetProductAttachmentList(int productSysNo ,int? sellerSysNo)
        {
            QueryResult<ProductAttachmentInfo> result = new QueryResult<ProductAttachmentInfo>();

            CheckIsExistProductForEdit(productSysNo);

            var list = ProductAttachmentDA.GetProductAttachmentList(productSysNo, sellerSysNo);

            result.ResultList = list;
            result.PageInfo = new PageInfo
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
                TotalCount = list == null ? 0 : list.Count,
            };

            return result;
        }

        /// <summary>
        /// Creates the product attachment.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public static ProductAttachmentStatus CreateProductAttachment(ProductAttachmentInfo entity)
        {
            //CheckTheProductStatus(entity.ProductSysNo);
            ProductAttachmentStatus productStatus = CheckTheProductStatus(entity.ProductID);
            entity.ProductSysNo = productStatus.ProductSysNo.Value;

            //CheckTheAttachmentStatus(entity.AttachmentSysNo);
            ProductAttachmentStatus attachmentStatus = CheckTheAttachmentStatus(entity.AttachmentID);
            entity.AttachmentSysNo = attachmentStatus.ProductSysNo.Value;

            if (ProductAttachmentDA.IsExistProductAttachment(entity.ProductSysNo, entity.AttachmentSysNo))
            {
                throw new BusinessException("该商品的相同附件已存在，不能重复添加");
            }

            ProductAttachmentDA.InsertAttachment(entity);

            ProductAttachmentStatus status = ProductAttachmentDA.GetTheProductAttachmentStatusByProductSysNo(entity.ProductSysNo);

            return status;
        }

        /// <summary>
        /// Deletes the attachment system no.
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        public static void DeleteSingleAttachment(int producySysNo, int sysNo)
        {
            CheckIsExistProduct(producySysNo);
            ProductAttachmentDA.DeleteAttachmentBySysNo(sysNo);            
        }

        #region [ Private Methods ]

        /// <summary>
        /// Checks the is exist product.
        /// </summary>
        /// <param name="productSysNo">The product system no.</param>
        /// <exception cref="ECommerce.Utility.BusinessException">商品编号不能为空</exception>
        private static void CheckIsExistProduct(int productSysNo)
        {
            if (productSysNo <= 0)
            {
                throw new BusinessException("商品编号不能为空");
            }
            ProductAttachmentStatus status = ProductAttachmentDA.GetTheProductAttachmentStatusByProductSysNo(productSysNo);
            if (!status.ProductSysNo.HasValue || status.ProductSysNo <= 0)
            {
                throw new BusinessException("商品不存在");
            }
            if (status.ProductStatus.HasValue && status.ProductStatus == 1)
            {
                throw new BusinessException("商品不能是上架状态");
            }
        }

        /// <summary>
        /// Checks the is exist product for edit.
        /// </summary>
        /// <param name="productSysNo">The product system no.</param>
        /// <exception cref="ECommerce.Utility.BusinessException">
        /// 商品编号不能为空
        /// or
        /// 商品不存在
        /// </exception>
        private static void CheckIsExistProductForEdit(int productSysNo)
        {
            if (productSysNo <= 0)
            {
                throw new BusinessException("商品编号不能为空");
            }
            ProductAttachmentStatus status = ProductAttachmentDA.GetTheProductAttachmentStatusByProductSysNo(productSysNo);
            if (!status.ProductSysNo.HasValue || status.ProductSysNo <= 0)
            {
                throw new BusinessException("商品不存在");
            }          
        }

        /// <summary>
        /// Checks the product status.
        /// </summary>
        /// <param name="productID">The product identifier.</param>
        /// <exception cref="ECommerce.Utility.BusinessException">商品不存在
        /// or
        /// 商品不能是上架状态
        /// or
        /// 商品附件不能超过3个</exception>
        private static ProductAttachmentStatus CheckTheProductStatus(string productID)
        {
            ProductAttachmentStatus status = ProductAttachmentDA.GetTheProductAttachmentStatusByProductID(productID);
            if (!status.ProductSysNo.HasValue || status.ProductSysNo <= 0)
            {
                throw new BusinessException("商品不存在");
            }
            if (status.ProductStatus.HasValue && status.ProductStatus == 1)
            {
                throw new BusinessException("商品不能是上架状态");
            }
            if (status.AttachmentCount.HasValue && status.AttachmentCount >= 3)
            {
                throw new BusinessException("商品附件不能超过3个");
            }

            return status;
        }

        /// <summary>
        /// Checks the product status for edit.
        /// </summary>
        /// <param name="productSysNo">The product system no.</param>
        /// <returns></returns>
        /// <exception cref="ECommerce.Utility.BusinessException">
        /// 商品不存在
        /// or
        /// 商品不能是上架状态
        /// </exception>
        public static ProductAttachmentStatus CheckTheProductStatusForEdit(int productSysNo)
        {
            ProductAttachmentStatus status = ProductAttachmentDA.GetTheProductAttachmentStatusByProductSysNo(productSysNo);
            if (!status.ProductSysNo.HasValue || status.ProductSysNo <= 0)
            {
                throw new BusinessException("商品不存在");
            }         
            return status;
        }

        /// <summary>
        /// Checks the product status.
        /// </summary>
        /// <param name="productSysNo">The product system no.</param>
        /// <exception cref="ECommerce.Utility.BusinessException">商品不存在
        /// or
        /// 商品不能是上架状态
        /// or
        /// 商品附件不能超过3个</exception>
        private static ProductAttachmentStatus CheckTheProductStatus(int productSysNo)
        {
            ProductAttachmentStatus status = ProductAttachmentDA.GetTheProductAttachmentStatusByProductSysNo(productSysNo);
            if (!status.ProductSysNo.HasValue || status.ProductSysNo <= 0)
            {
                throw new BusinessException("商品不存在");
            }
            if (status.ProductStatus.HasValue && status.ProductStatus == 1)
            {
                throw new BusinessException("商品不能是上架状态");
            }
            if (status.AttachmentCount.HasValue && status.AttachmentCount >= 3)
            {
                throw new BusinessException("商品附件不能超过3个");
            }

            return status;
        }


        /// <summary>
        /// Checks the attachment status.
        /// </summary>
        /// <param name="productID">The product identifier.</param>
        /// <exception cref="ECommerce.Utility.BusinessException">
        /// 附件不存在
        /// or
        /// 附件不能是上架状态
        /// </exception>
        private static ProductAttachmentStatus CheckTheAttachmentStatus(string productID)
        {
            ProductAttachmentStatus status = ProductAttachmentDA.GetTheProductAttachmentStatusByProductID(productID);
            if (!status.ProductSysNo.HasValue || status.ProductSysNo <= 0)
            {
                throw new BusinessException("附件不存在");
            }
            if (status.ProductStatus.HasValue && status.ProductStatus == 1)
            {
                throw new BusinessException("附件不能是上架状态");
            }

            return status;
        }

        /// <summary>
        /// Checks the attachment status.
        /// </summary>
        /// <param name="productSysNo">The product system no.</param>
        /// <exception cref="ECommerce.Utility.BusinessException">
        /// 附件不存在
        /// or
        /// 附件不能是上架状态
        /// </exception>
        private static ProductAttachmentStatus CheckTheAttachmentStatus(int productSysNo)
        {
            ProductAttachmentStatus status = ProductAttachmentDA.GetTheProductAttachmentStatusByProductSysNo(productSysNo);
            if (!status.ProductSysNo.HasValue || status.ProductSysNo <= 0)
            {
                throw new BusinessException("附件不存在");
            }
            if (status.ProductStatus.HasValue && status.ProductStatus == 1)
            {
                throw new BusinessException("附件不能是上架状态");
            }

            return status;
        }

        #endregion
    }
}

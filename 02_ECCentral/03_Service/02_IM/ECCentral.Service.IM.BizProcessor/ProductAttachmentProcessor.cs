using System;
using System.Collections.Generic;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductAttachmentProcessor))]
    public class ProductAttachmentProcessor
    {
        private readonly IProductAttachmentDA _attachmentDA = ObjectFactory<IProductAttachmentDA>.Instance;

        #region 商品附件业务处理方法

        /// <summary>
        /// 创建商品附件
        /// </summary>
        /// <param name="mainProductEntity"></param>
        /// <returns></returns>
        public void CreateProductAttachment(ProductInfo mainProductEntity)
        {
            CheckProductInfo(mainProductEntity);
            IsExistProductAttachmentByProductSysNo(mainProductEntity.SysNo);
            OperatoreProductAttachment(mainProductEntity);
        }


        /// <summary>
        /// 修改商品附件
        /// </summary>
        /// <param name="mainProductEntity"></param>
        /// <returns></returns>
        public void ModifyProductAttachment(ProductInfo mainProductEntity)
        {
            CheckProductInfo(mainProductEntity);
            OperatoreProductAttachment(mainProductEntity);
        }

        /// <summary>
        /// 根据商品SysNo获取商品附件信息组
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public List<ProductAttachmentInfo> GetProductAttachmentList(int productSysNo)
        {
            IsExistProduct(productSysNo);
            var result = _attachmentDA.GetProductAttachmentList(productSysNo);
            return result;
        }

        /// <summary>
        /// 操作商品附件
        /// </summary>
        /// <param name="mainProductEntity"></param>
        private void OperatoreProductAttachment(ProductInfo mainProductEntity)
        {
            using (var tran = new TransactionScope())
            {
                _attachmentDA.DeleteAttachmentByProductSysNo(mainProductEntity.SysNo);
                foreach (var item in mainProductEntity.ProductAttachmentList)
                {
                    _attachmentDA.InsertAttachment(mainProductEntity.SysNo, item);
                }
                tran.Complete();
            }
        }

        /// <summary>
        /// 删除附件信息
        /// </summary>
        /// <param name="productSysNo"></param>
        public void DeleteAttachmentByProductSysNo(int productSysNo)
        {
            _attachmentDA.DeleteAttachmentByProductSysNo(productSysNo);
        }

        /// <summary>
        /// 判断商品是否是附件，返回True-是附件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public bool CheckIsAttachment(int productSysNo)
        {
            return _attachmentDA.IsProductAttachment(productSysNo);
        }
        #endregion

        #region 商品附件信息检查逻辑

        private void CheckProductInfo(ProductInfo mainProductEntity)
        {
            if (mainProductEntity == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", "ProductInfoIsNull"));
            }
            if (mainProductEntity.SysNo <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", "ProductSysNoIsNull"));
            }
            IsExistProduct(mainProductEntity.SysNo);
            if (mainProductEntity.ProductAttachmentList == null || mainProductEntity.ProductAttachmentList.Count == 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", "ProductAttachmentCountIsInvalid"));
            }
            if (mainProductEntity.ProductAttachmentList.Count > 3)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", "ProductAttachmentCountIsNull"));
            }
            mainProductEntity.ProductAttachmentList.ForEach(CheckProductAttachment);
        }

        private void CheckProductAttachment(ProductAttachmentInfo attachmentInfo)
        {
            if (attachmentInfo == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", "ProductAttachmentIsNull"));
            }
            if (attachmentInfo.AttachmentProduct.SysNo <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", "ProductAttachmentSysNOIsNull"));
            }
            IsExistProductAttachment(attachmentInfo.AttachmentProduct.SysNo);
        }

        /// <summary>
        /// 是否存在不展示的商品
        /// </summary>
        /// <param name="productSysNo"></param>
        private void IsExistProduct(int productSysNo)
        {
            if (productSysNo <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", "ProductSysNoIsNull"));
            }
            var product = ObjectFactory<IProductDA>.Instance;
            var tempProduct = product.GetProductInfoBySysNo(productSysNo);
            if (tempProduct == null || String.IsNullOrEmpty(tempProduct.ProductID))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", "ProductInfoIsInvalid"));
            }
            if (tempProduct.ProductStatus != ProductStatus.InActive_UnShow)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", "ProductStatusIsInvalid"));
            }
        }

        /// <summary>
        /// 是否存在不展示的附件
        /// </summary>
        /// <param name="productSysNo"></param>
        private void IsExistProductAttachment(int productSysNo)
        {
            var product = ObjectFactory<IProductDA>.Instance;
            var tempProduct = product.GetProductInfoBySysNo(productSysNo);
            if (tempProduct == null || String.IsNullOrEmpty(tempProduct.ProductID))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", "ProductAttachmentIsInvalid"));
            }
            if (tempProduct.ProductStatus != ProductStatus.InActive_UnShow)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", "ProductAttachmentStatusIsInvalid"));
            }
        }

        /// <summary>
        /// 某个商品下有附件
        /// </summary>
        /// <param name="productSysNo"></param>
        private void IsExistProductAttachmentByProductSysNo(int productSysNo)
        {
            var result = _attachmentDA.IsExistProductAttachment(productSysNo);
            if (result)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", " IsExistsProductAttachment"));
            }

        }


        #endregion

        
    }
}

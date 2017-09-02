using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductAttachmentDA))]
    public class ProductAttachmentDA : IProductAttachmentDA
    {
        /// <summary>
        /// 创建商品附件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="attachmentEntity"></param>
        /// <returns></returns>
        public ProductAttachmentInfo InsertAttachment(int productSysNo, ProductAttachmentInfo attachmentEntity)
        {
            var cmd = DataCommandManager.GetDataCommand("InsertAttachment");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@ProductAttachmentSysNo", attachmentEntity.AttachmentProduct.SysNo);
            cmd.SetParameterValue("@Quantity", attachmentEntity.Quantity);
            cmd.SetParameterValue("@InUser",attachmentEntity.InUser.UserName);
            cmd.SetParameterValue("@EditUser", attachmentEntity.EditUser!=null?attachmentEntity.EditUser.UserName:null);
            cmd.SetParameterValue("@EditDate", attachmentEntity.EditDate);
            cmd.SetParameterValue("@InDate", attachmentEntity.InDate);
            cmd.ExecuteNonQuery();
            attachmentEntity.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return attachmentEntity;
        }

        public bool IsExistProductAttachment(int productSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("IsExistProductAttachment");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        /// <summary>
        /// 是否为配件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public bool IsProductAttachment(int productSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("IsProductAttachment");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        /// <summary>
        /// 删除商品附件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public void DeleteAttachmentByProductSysNo(int productSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("DeleteAttachmentByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.ExecuteNonQuery();
            return;
        }

        /// <summary>
        /// 根据商品SysNo获取商品附件信息组
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public List<ProductAttachmentInfo> GetProductAttachmentList(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductAttachmentList");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            var sourceEntity = cmd.ExecuteEntityList<ProductAttachmentInfo>() ??
                              new List<ProductAttachmentInfo>();
            return sourceEntity;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using ECommerce.Utility.DataAccess;
using ECommerce.Entity.Product;

namespace ECommerce.DataAccess.Product
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProductAttachmentDA
    {
        /// <summary>
        /// 查询商品附件
        /// </summary>
        /// <returns></returns>
        public static List<ProductAttachmentQueryBasicInfo> QueryProductAttachment(ProductAttachmentQueryFilter queryCriteria, out int totalCount)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductAttachment");
            var pagingInfo = new PagingInfoEntity
            {
                StartRowIndex = queryCriteria.PageIndex * queryCriteria.PageSize,
                MaximumRows = queryCriteria.PageSize,
                SortField = queryCriteria.SortFields,
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, string.IsNullOrEmpty(pagingInfo.SortField) ? pagingInfo.SortField : "P.ProductSysNo DESC"))
            {
                if (!String.IsNullOrEmpty(queryCriteria.ProductID))
                {
                    dataCommand.AddInputParameter("@ProductID", DbType.String, queryCriteria.ProductID);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.ProductID",
                        DbType.String, "@ProductID",
                        QueryConditionOperatorType.Like,
                        queryCriteria.ProductID);
                }
                if (!String.IsNullOrEmpty(queryCriteria.ProductName))
                {
                    dataCommand.AddInputParameter("@ProductName", DbType.String, queryCriteria.ProductName);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.ProductName",
                        DbType.String, "@ProductName",
                        QueryConditionOperatorType.Like,
                        queryCriteria.ProductName);
                }
                if (!String.IsNullOrEmpty(queryCriteria.AttachmentID))
                {
                    dataCommand.AddInputParameter("@AttachmentID", DbType.String, queryCriteria.AttachmentID);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.AttachmentSysNo",
                        DbType.String, "@AttachmentID",
                        QueryConditionOperatorType.Like,
                        queryCriteria.AttachmentID);
                }
                if (!String.IsNullOrEmpty(queryCriteria.AttachmentName))
                {
                    dataCommand.AddInputParameter("@AttachmentName", DbType.String, queryCriteria.AttachmentName);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.AttachmentName",
                        DbType.String, "@AttachmentName",
                        QueryConditionOperatorType.Like,
                        queryCriteria.AttachmentName);
                }
                if (!String.IsNullOrEmpty(queryCriteria.EditUser))
                {
                    dataCommand.AddInputParameter("@EditUser", DbType.String, queryCriteria.EditUser);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.InUser",
                        DbType.String, "@EditUser",
                        QueryConditionOperatorType.Like,
                        queryCriteria.EditUser);
                }
                //sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "P.InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.LessThanOrEqual, QueryConditionOperatorType.MoreThanOrEqual, queryCriteria.InDateEnd, queryCriteria.InDateStart);
                if (queryCriteria.InDateStart != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                        QueryConditionRelationType.AND,
                                        "P.InDate",
                                        DbType.DateTime,
                                        "@InDateStart",
                                        QueryConditionOperatorType.MoreThanOrEqual,
                                        queryCriteria.InDateStart
                                   );
                }
                if (queryCriteria.InDateEnd != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                  QueryConditionRelationType.AND,
                                  "P.InDate",
                                  DbType.DateTime,
                                  "@InDateEnd",
                                  QueryConditionOperatorType.LessThanOrEqual,
                                  queryCriteria.InDateEnd
                             );
                }
                if (queryCriteria.SellerSysNo.HasValue)
                {
                    dataCommand.AddInputParameter("@SellerSysNo", DbType.String, queryCriteria.SellerSysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "PP.MerchantSysNo",
                        DbType.String, "@SellerSysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.SellerSysNo);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                List<ProductAttachmentQueryBasicInfo> list = dataCommand.ExecuteEntityList<ProductAttachmentQueryBasicInfo>();
                
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                
                return list;
            }
        }

        /// <summary>
        /// 创建商品附件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="attachmentEntity"></param>
        /// <returns></returns>
        public static ProductAttachmentInfo InsertAttachment(ProductAttachmentInfo attachmentEntity)
        {
            var cmd = DataCommandManager.GetDataCommand("InsertAttachment");
            cmd.SetParameterValue("@ProductSysNo", attachmentEntity.ProductSysNo);
            cmd.SetParameterValue("@ProductAttachmentSysNo", attachmentEntity.AttachmentSysNo);
            cmd.SetParameterValue("@Quantity", attachmentEntity.Quantity);
            cmd.SetParameterValue("@InUser", attachmentEntity.InUserName);
            cmd.SetParameterValue("@EditUser", attachmentEntity.EditUserName);
            cmd.SetParameterValue("@EditDate", attachmentEntity.EditDate);
            cmd.SetParameterValue("@InDate", DateTime.Now);
            cmd.ExecuteNonQuery();
            attachmentEntity.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return attachmentEntity;
        }

        /// <summary>
        /// Determines whether [is exist product attachment] [the specified product system no].
        /// </summary>
        /// <param name="productSysNo">The product system no.</param>
        /// <param name="attachmentSysNo">The attachment system no.</param>
        /// <returns></returns>
        public static bool IsExistProductAttachment(int productSysNo,int attachmentSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("IsExistProductAttachment");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@ProductAttachmentSysNo", attachmentSysNo);
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
        public static bool IsProductAttachment(int productSysNo)
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
        public static void DeleteAttachmentByProductSysNo(int productSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("DeleteAttachmentByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.ExecuteNonQuery();
            return;
        }

        /// <summary>
        /// Deletes the attachment by system no.
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        public static void DeleteAttachmentBySysNo(int sysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("DeleteAttachmentBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.ExecuteNonQuery();
            return;
        }

        /// <summary>
        /// 根据商品SysNo获取商品附件信息组
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<ProductAttachmentInfo> GetProductAttachmentList(int productSysNo, int? sellerSysNo = null)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductAttachmentList");

            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);

            var sourceEntity = cmd.ExecuteEntityList<ProductAttachmentInfo>() ??
                              new List<ProductAttachmentInfo>();
            return sourceEntity;
        }


        /// <summary>
        /// Gets the product attachment status by product identifier.
        /// </summary>
        /// <param name="productID">The product identifier.</param>
        /// <returns></returns>
        public static ProductAttachmentStatus GetTheProductAttachmentStatusByProductID(string productID)
        {
            List<string> result = new List<string>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetTheProductAttachmentStatusByProductID");

            cmd.SetParameterValue("@ProductID", productID);

            return cmd.ExecuteEntity<ProductAttachmentStatus>();

        }

        /// <summary>
        /// Gets the product attachment status by product system no.
        /// </summary>
        /// <param name="productSysNo">The product system no.</param>
        /// <returns></returns>
        public static ProductAttachmentStatus GetTheProductAttachmentStatusByProductSysNo(int productSysNo)
        {
            List<string> result = new List<string>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetTheProductAttachmentStatusByProductSysNo");

            cmd.SetParameterValue("@ProductSysNo", productSysNo);

            return cmd.ExecuteEntity<ProductAttachmentStatus>();

        }
    }
}

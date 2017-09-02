using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.SqlDataAccess
{

    [VersionExport(typeof(IProductReviewDA))]
    public class ProductReviewDA : IProductReviewDA
    {
        #region 产品评论
        /// <summary>
        /// 产品评论的批量审核状态
        /// </summary>
        /// <param name="items"></param>
        public void BatchSetProductReviewStatus(List<int> items, string status)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_BatchUpdateProductReviewStatus");//ProductReview_BatchUpdateStatus
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", status);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }

        public ProductReview CreateProductReview(ProductReview entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_CreateProductReview");
            dc.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            dc.SetParameterValue("@CustomerSysNo", entity.CustomerSysNo);
            dc.SetParameterValue("@Service", entity.Service);
            dc.SetParameterValue("@Title", entity.Title);
            dc.SetParameterValue("@Prons", entity.Prons);
            dc.SetParameterValue("@Cons", entity.Cons);
            dc.SetParameterValue("@Score1", entity.Score1);
            dc.SetParameterValue("@Score2", entity.Score2);
            dc.SetParameterValue("@Score3", entity.Score3);
            dc.SetParameterValue("@Score4", entity.Score4);
            dc.SetParameterValue("@Score", entity.Score);
            dc.SetParameterValueAsCurrentUserSysNo("@InUser");            
            entity.SysNo = dc.ExecuteScalar<int>();            
            return entity;
        }

        /// <summary>
        /// 更新评论相关的信息
        /// </summary>
        /// <param name="item"></param>
        public void UpdateProductReview(ProductReview item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_UpdateProductReview");//
            dc.SetParameterValue<ProductReview>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新评论之后更新Homepage中的记录
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="type"></param>
        public void UpdateHomepageForProductReview(int sysNo, string type)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_UpdateHomepageSetting");
            dc.SetParameterValue("@SysNo", sysNo);
            dc.SetParameterValue("@Type", type);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新评论之后删除Homepage中的记录
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="type"></param>
        public void DeleteHomepageForProductReview(int sysNo, string type)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_DeleteHomepageSetting");
            dc.SetParameterValue("@SysNo", sysNo);
            dc.SetParameterValue("@Type", type);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除评论相关图片
        /// </summary>
        /// <param name="image"></param>
        public void DeleteProductReviewImage(string image)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_DeleteProductReviewImage");
            string[] param = image.Split('!');
            dc.SetParameterValue("@Image", param[0]);
            dc.SetParameterValue("@SysNo", param[1]);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据评论编号，加载相应的评论
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public ProductReview LoadProductReview(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_GetProductReviewInfo");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();
            return DataMapper.GetEntity<ProductReview>(dt.Rows[0]);
        }

        ///<summary>
        /// 加载商品评论
        /// </summary>
        /// <returns></returns>
        public List<ProductReview> GetProductReview()
        {
            throw new BizException("");
        }

        public ProductReview GetProductReview(int productID)
        {
            throw new BizException("");
        }

        public void AuditApproveProductReview(int commentID)
        {
            throw new BizException("");
        }

        /// <summary>
        /// 作废产品评论
        /// </summary>
        /// <param name="commentID"></param>
        public void AuditRefuseProductReview(int commentID)
        {
            throw new BizException("");
        }

        /// <summary>
        /// 提交CS处理
        /// </summary>
        /// <param name="commentID"></param>
        public void SubmitProductReview(int commentID)
        {
            throw new BizException("");
        }
        #endregion

        #region 产品评论—回复


        /// <summary>
        /// 根据讨论论编号，加载相应的讨论回复
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public List<ProductReviewReply> GetProductReviewReplyList(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_GetProductReviewReplyList");
            dc.SetParameterValue("@ReviewSysNo", sysNo);
            //if(!string.IsNullOrEmpty(type))
                //dc.SetParameterValue("@Type", type);
            
            CodeNamePairColumnList pairList = new CodeNamePairColumnList();
            pairList.Add("Status", "MKT", "ReplyStatus");//评论状态
            pairList.Add("Type", "MKT", "ReplySource");//回复类型
            DataTable dt = dc.ExecuteDataTable(pairList);
            List<ProductReviewReply> list = new List<ProductReviewReply>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(DataMapper.GetEntity<ProductReviewReply>(dr));
            }
            return list;
        }

        /// <summary>
        /// 根据评论编号加载相应的厂商评论回复
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public List<ProductReviewReply> GetProductReviewFactoryReply(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_GetProductReviewFactoryReply");
            dc.SetParameterValue("@ReviewSysNo", sysNo);
            CodeNamePairColumnList pairList = new CodeNamePairColumnList();
            //pairList.Add("Status", "MKT", "ReplyStatus");//评论状态
            pairList.Add("Type", "MKT", "ReplySource");//回复类型
            pairList.Add("Status", "MKT", "FactoryReplyStatus");
            DataTable dt = dc.ExecuteDataTable(pairList);
            List<ProductReviewReply> list = new List<ProductReviewReply>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(DataMapper.GetEntity<ProductReviewReply>(dr));
            }
            return list;
        }

        /// <summary>
        /// 批量设置产品评论的状态
        /// </summary>
        /// <param name="items"></param>
        /// <param name="status"></param>
        public void BatchSetProductReviewReplyStatus(List<int> items, string status)
        {
            foreach (int sysNo in items)
            {
                DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_UpdateProductReviewStatusForUpdateReplyStatus");
                dc.SetParameterValue("@SysNo", sysNo);
                dc.SetParameterValue("@Status", status);
                dc.SetParameterValueAsCurrentUserAcct("EditUser");
                dc.ExecuteNonQuery();
            }
            //StringBuilder message = new StringBuilder();
            //foreach (var i in items)
            //{
            //    message.Append(i.ToString() + ",");
            //}

            //DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_BatchUpdateProductReviewReplyStatus");
            //dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            //dc.SetParameterValue("@Status", status);
            //dc.SetParameterValueAsCurrentUserAcct("EditUser");
            //dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 添加产品评论回复:添加产品评论回复有3种方式：
        ///1.	网友回复，需通过审核才展示。
        ///2.	厂商回复（通过Seller Portal），需通过审核才展示。
        ///3.	IPP系统中回复，默认直接展示。
        /// </summary>
        public void AddProductReviewReply(ProductReviewReply item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_CreateProductReviewReply");
            dc.SetParameterValue<ProductReviewReply>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 厂商回复的批量发布与拒绝
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateProductReviewVendorReplyStatus(ProductReviewReply item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductReview_UpdateVendorStatus");
            //dc.SetParameterValue<ProductReviewReply>(item);
            dc.SetParameterValue("@SysNo", item.SysNo.Value);
            dc.SetParameterValue("@ReviewSysNo", item.ReviewSysNo.Value);
            dc.SetParameterValue("@Content", item.Content);
            dc.SetParameterValue("@Status", item.Status);
            dc.SetParameterValue("@CompanyCode", item.CompanyCode);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
            return Convert.ToInt32(dc.GetParameterValue("@IsSuccess"));
        }

        

        #endregion

    }
}

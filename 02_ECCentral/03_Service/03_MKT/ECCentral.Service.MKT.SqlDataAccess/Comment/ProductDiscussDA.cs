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

    [VersionExport(typeof(IProductDiscussDA))]
    public class ProductDiscussDA : IProductDiscussDA
    {

        #region 产品讨论


        /// <summary>
        /// 加载产品讨论主信息
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual ProductDiscussDetail LoadProductDiscuss(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductDiscuss_GetProductDiscussInfo");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();
            return DataMapper.GetEntity<ProductDiscussDetail>(dt.Rows[0]);
        }


        /// <summary>
        /// 只有在网站登录账户且有权限才能发表
        /// </summary>
        /// <param name="item"></param>
        public void CreateProductDiscuss(ProductDiscussDetail item)
        {
            throw new BizException("");
        }

        /// <summary>
        /// 加载产品讨论
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public List<ProductDiscussDetail> GetProductDiscuss(int productID)
        {
            throw new BizException("");
        }

        /// <summary>
        /// 设置产品讨论的状态
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="status"></param>
        public void BatchSetProductDiscussStatus(List<int> items, string status)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }

            DataCommand dc = DataCommandManager.GetDataCommand("ProductDiscuss_BatchUpdateProductDiscussStatus");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", status);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }


        /// <summary>
        /// 编辑产品讨论
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditProductDiscuss(ProductDiscussDetail item)
        {
            throw new BizException("");
        }
        #endregion

        #region 产品讨论—回复（ProductDiscussReply）


        /// <summary>
        /// 根据讨论论编号，加载相应的讨论回复
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public List<ProductDiscussReply> GetProductDiscussReply(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductDiscuss_GetProductDiscussReplyList");
            dc.SetParameterValue("@DiscussSysNo", sysNo);
            CodeNamePairColumnList pairList = new CodeNamePairColumnList();
            pairList.Add("Status", "MKT", "ReplyStatus");//评论状态
            DataTable dt = dc.ExecuteDataTable(pairList);
            List<ProductDiscussReply> list = new List<ProductDiscussReply>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(DataMapper.GetEntity<ProductDiscussReply>(dr));
            }
            return list;
        }

        /// <summary>
        /// 讨论回复加1，并设置相关状态
        /// </summary>
        /// <param name="discussSysNo"></param>
        public void UpdateProductDiscussReplyCount(int discussSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductDiscuss_UpdateProductDiscussCount");
            dc.SetParameterValue("@DiscussSysNo", discussSysNo);
            dc.SetParameterValueAsCurrentUserAcct("InUser");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 添加产品讨论回复    1.	在网站登录账户且有权限才能发表。需要审核才能展示在网页中。2.	IPP系统中发表回复。
        /// </summary>
        /// <param name="item"></param>
        public void AddProductDiscussReply(ProductDiscussReply item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductDiscuss_CreateProductDiscussReply");
            dc.SetParameterValue<ProductDiscussReply>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 设置产品讨论回复的状态
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="status"></param>
        public void BatchSetProductDiscussReplyStatus(List<int> items, string status)
        {
            foreach (int sysNo in items)
            {
                DataCommand dc = DataCommandManager.GetDataCommand("ProductDiscuss_UpdateProductDiscussStatusForUpdateReplyStatus");
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

            //DataCommand dc = DataCommandManager.GetDataCommand("ProductDiscuss_BatchUpdateProductDiscussReplyStatus");
            //dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            //dc.SetParameterValue("@Status", status);
            //dc.SetParameterValueAsCurrentUserAcct("EditUser");
            //dc.ExecuteNonQuery();
        }




        #endregion
    }
}

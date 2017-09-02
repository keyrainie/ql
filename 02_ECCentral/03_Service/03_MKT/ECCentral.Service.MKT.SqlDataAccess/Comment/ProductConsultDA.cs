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

    [VersionExport(typeof(IProductConsultDA))]
    public class ProductConsultDA : IProductConsultDA
    {
        #region 咨询管理（ProductConsult）


        /// <summary>
        /// 添加或更新回复,并更新咨询的回复次数
        /// </summary>
        /// <param name="item"></param>
        public void UpdateProductConsultDetailReply(ProductConsultReply item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_UpdateProductConsultDetailReply");
            dc.SetParameterValue<ProductConsultReply>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载购物咨询
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public ProductConsult LoadProductConsult(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_GetProductConsultDetailEditView");
            dc.SetParameterValue("@SysNo", sysNo);
            //EnumColumnList enumList = new EnumColumnList();
            //enumList.Add("Status", typeof(ECCentral.BizEntity.MKT.ReplyStatus));//前台展示状态
            //enumList.Add("Type", typeof(ECCentral.BizEntity.MKT.NYNStatus));//是否存在回复

            CodeNamePairColumnList pairList = new CodeNamePairColumnList();
            //pairList.Add("Status", "MKT", "ReplyStatus");
            //pairList.Add("Type", "MKT", "ConsultCategory");

            DataTable dt = dc.ExecuteDataTable(pairList);
            return DataMapper.GetEntity<ProductConsult>(dt.Rows[0]);
        }

        /// <summary>
        /// 批量购物咨询的审核状态
        /// </summary>
        /// <param name="items"></param>
        public void BatchSetProductConsultStatus(List<int> items, string status)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_BatchUpdateProductConsultStatus");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", status);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }

        #endregion

        #region  产品咨询回复（ProductConsultReply）

        //public int ProductConsult_UpdateFactoryActiveStatus(ProductConsultReply item)
        //{
        //    DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_UpdateFactoryActiveStatus");
        //    dc.SetParameterValue<ProductConsultReply>(item);
        //    dc.ExecuteNonQuery();
        //    return Convert.ToInt32(dc.GetParameterValue("@IsSuccess"));
        //}


        /// <summary>
        /// 咨询回复之批准回复
        /// </summary>
        /// <param name="item"></param>
        public int ApproveProductConsultRelease(ProductConsultReply item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_UpdateFactoryActiveStatus");
            //dc.SetParameterValue<ProductConsultReply>(item);

            dc.SetParameterValue("@SysNo", item.SysNo.Value);
            dc.SetParameterValue("@ConsultSysNo", item.ConsultSysNo.Value);
            dc.SetParameterValue("@CompanyCode", item.CompanyCode);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
            return Convert.ToInt32(dc.GetParameterValue("@IsSuccess"));
        }

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int SendSSBForApproveProductConsultRelease(ProductConsultReply item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_UP_VP_Reply");
            dc.SetParameterValue("@SysNo", item.SysNo.Value);
            dc.SetParameterValue("@VPSysNo", item.RefSysNo);
            //调用存储过程，需要把A状态修改为C状态
            if (item.Status == "A")
                item.Status = "C";
            dc.SetParameterValue("@Status", item.Status);
            dc.SetParameterValue("@Type", "C");
            dc.SetParameterValue("@Memo", item.ReplyContent);
            dc.ExecuteNonQuery();
            return Convert.ToInt32(dc.GetParameterValue("@IsSuccess"));
        }

        /// <summary>
        /// 咨询回复之批准拒绝
        /// </summary>
        /// <param name="item"></param>
        public int RejectProductConsultRelease(ProductConsultReply item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_UpdateFactoryRefuseStatus");
            dc.SetParameterValue("@SysNo", item.SysNo.Value);
            dc.SetParameterValue("@CompanyCode", item.CompanyCode);

            dc.ExecuteNonQuery();
            return Convert.ToInt32(dc.GetParameterValue("@IsSuccess"));
        }

        /// <summary>
        /// 检查是否存在厂商回复  可能没用到
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool CheckProductConsultFactoryReply(int sysNo, string status)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductConsult_CheckFactoryReply");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@Status", sysNo);
            return cmd.ExecuteScalar() != null;
        }

        /// <summary>
        /// 检查是否已经存在该咨询的回复
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <returns></returns>
        public bool CheckProductConsultReply(int consultSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductConsult_GetProductConsultReplyByConsultSysNo");
            cmd.SetParameterValue("@ConsultSysNo", consultSysNo);
            return cmd.ExecuteScalar() != null;
        }

        /// <summary>
        /// 新建咨询回复
        /// </summary>
        /// <param name="item"></param>
        public void CreateProductConsultReply(ProductConsultReply item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_InsertProductConsultReply");
            dc.SetParameterValue<ProductConsultReply>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新咨询管理回复    可能不用
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void UpdateProductConsultReply(ProductConsultReply item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_UpdateProductConsultDetailReply");
            dc.SetParameterValue<ProductConsultReply>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新咨询管理人回复数量
        /// </summary>
        /// <param name="item"></param>
        /// <param name="bMore"></param>
        public void UpdateProductConsultReplyCount(int sysNo, bool bMore)
        {
            if (bMore)
            {
                DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_UpdateProductConsultReplyCountMore");
                dc.SetParameterValue("@SysNo", sysNo);
                dc.SetParameterValueAsCurrentUserAcct("EditUser");
                dc.ExecuteNonQuery();
            }
            else
            {
                DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_UpdateProductConsultReplyCountLess");
                dc.SetParameterValue("@SysNo", sysNo);
                dc.SetParameterValueAsCurrentUserAcct("EditUser");
                dc.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取关于咨询的所有回复，除去厂商回复
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <returns></returns>
        public List<ProductConsultReply> GetProductConsultReplyList(int consultSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_GetProductConsultReplyList");
            dc.SetParameterValue("@ConsultSysNo", consultSysNo);

            //CodeNamePairColumnList pairList = new CodeNamePairColumnList();
            //pairList.Add("Type", "MKT", "ConsultCategory");
            DataTable dt = dc.ExecuteDataTable();
            List<ProductConsultReply> list = new List<ProductConsultReply>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(DataMapper.GetEntity<ProductConsultReply>(dr));
                }
            }
            return list;
        }

        /// <summary>
        ///  获取厂商关于咨询的回复列表
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <returns></returns>
        public List<ProductConsultReply> GetProductConsultFactoryReplyList(int consultSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_GetProductConsultFactoryReplyList");

            dc.SetParameterValue("@ConsultSysNo", consultSysNo);
            //EnumColumnList enumList = new EnumColumnList();
            //enumList.Add("Status", typeof(ECCentral.BizEntity.MKT.ReplyStatus));//前台展示状态
            //enumList.Add("Type", typeof(ECCentral.BizEntity.MKT.NYNStatus));//是否存在回复

            CodeNamePairColumnList pairList = new CodeNamePairColumnList();
            pairList.Add("Type", "MKT", "ConsultCategory");
            pairList.Add("Status", "MKT", "FactoryReplyStatus");

            DataTable dt = dc.ExecuteDataTable(pairList);
            List<ProductConsultReply> list = new List<ProductConsultReply>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(DataMapper.GetEntity<ProductConsultReply>(dr));
                }
            }
            return list;
        }

        /// <summary>
        /// 批量设置咨询回复的状态及置顶情况 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="status"></param>
        public void BatchSetProductConsultReplyStatus(List<int> items, string status)
        {
            StringBuilder message = new StringBuilder();

            if (status == "Y" || status == "N")//置顶情况 
            {
                foreach (var i in items)
                {
                    message.Append(i.ToString() + ",");
                }
                DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_BatchUpdateProductConsultReplyIsTop");
                dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
                dc.SetParameterValue("@IsTop", status);
                dc.SetParameterValueAsCurrentUserAcct("EditUser");
                dc.ExecuteNonQuery();
            }
            else
            {
                foreach (int sysNo in items)
                {
                    DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_UpdateProductConsultStatusForUpdateReplyStatus");
                    dc.SetParameterValue("@SysNo", sysNo);
                    dc.SetParameterValue("@Status", status);
                    dc.SetParameterValueAsCurrentUserAcct("EditUser");
                    dc.ExecuteNonQuery();
                }
                //DataCommand dc = DataCommandManager.GetDataCommand("ProductConsult_BatchUpdateProductConsultReplyStatus");
                //dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
                //dc.SetParameterValue("@Status", status);
                //dc.SetParameterValueAsCurrentUserAcct("EditUser");
                //dc.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 根据咨询编号，加载相应的回复。
        /// </summary>
        /// <returns></returns>
        public List<ProductConsultReply> LoadProductConsultReply(int itemID)
        {
            throw new BizException("");
        }

        /// <summary>
        /// 拒绝发布厂商回复
        /// </summary>
        /// <param name="itemID"></param>
        public void AuditRefuseProductConsultReply(int itemID)
        {
            throw new BizException("");
        }

        /// <summary>
        /// 审核购物咨询回复，并在website中展示。
        /// </summary>
        /// <param name="itemID"></param>
        public void AuditApproveProductConsultReply(int itemID)
        {
            throw new BizException("");
        }

        /// <summary>
        /// 作废评论回复，不展示在website中。
        /// </summary>
        /// <param name="itemID"></param>
        public void VoidProductConsultReply(int itemID)
        {
            throw new BizException("");
        }

        #endregion

    }
}

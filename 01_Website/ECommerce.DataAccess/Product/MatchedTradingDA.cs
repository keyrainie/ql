using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ECommerce.Entity.Product;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity;

namespace ECommerce.DataAccess.Product
{
    public class MatchedTradingDA
    {
        private static void SetCommandDefaultParameters(DataCommand command)
        {
            command.SetParameterValue("@LanguageCode", "zh-cn");
            command.SetParameterValue("@CompanyCode", "8601");
            command.SetParameterValue("@StoreCompanyCode", "8601");
        }
        /// <summary>
        /// 一分钟检查
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static MatchedTradingInfo CheckProductMatchedTradingInfo(int customerSysNo)
        {

            DataCommand dataCommand = DataCommandManager.GetDataCommand("CheckProductMatchedTradingInfo");
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            //DAHelper.SetCommandDefaultParameters(dataCommand);
            SetCommandDefaultParameters(dataCommand);
            var consultation = dataCommand.ExecuteEntity<MatchedTradingInfo>();
            return consultation;
        }

        /// <summary>
        /// 根据商品唯一系统ID获取咨询
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns>咨询</returns>
        public static MatchedTradingInfo GetMatchedTradingListBySysNo(MatchedTradingQueryInfo queryInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetProductMatchedTradingInfoBySysNo");
            dataCommand.SetParameterValue("@SysNo", queryInfo.MatchedTradingSysNo);
            dataCommand.SetParameterValue("@PageSize", queryInfo.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageIndex", queryInfo.PagingInfo.PageIndex);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            DataSet result = dataCommand.ExecuteDataSet();
            DataTable masterTable = result.Tables[0];
            MatchedTradingInfo consultInfo = new MatchedTradingInfo();

            if (masterTable.Rows.Count > 0)
            {
                consultInfo = DataMapper.GetEntity<MatchedTradingInfo>(masterTable.Rows[0]);// EntityBuilder.BuildEntity<MatchedTradingInfo>(masterTable.Rows[0]);
                if (consultInfo == null)
                {
                    return null;
                }
                DataTable itemTable = result.Tables[1];
                List<ProductMatchedTradingReplyInfo> replyList =
                    DataMapper.GetEntityList<ProductMatchedTradingReplyInfo, List<ProductMatchedTradingReplyInfo>>(itemTable.Rows);// EntityBuilder.BuildEntityList<ProductMatchedTradingReplyInfo>(itemTable);
                if (itemTable.Rows.Count > 0)
                {
                    if (replyList.Count > 0)
                    {
                        //新蛋回复
                        List<ProductMatchedTradingReplyInfo> neweggReplyList = replyList.FindAll(
                            replyInfo => replyInfo.ReplyType == FeedbackReplyType.Newegg);

                        if (neweggReplyList.Count > 0)
                        {
                            neweggReplyList.Sort((a, b) => b.InDate.CompareTo(a.InDate));
                            consultInfo.NeweggReply = neweggReplyList[0];
                        }
                        //厂商回复

                        List<ProductMatchedTradingReplyInfo> manuReplyList = replyList.FindAll(
                            replyInfo => replyInfo.ReplyType == FeedbackReplyType.Manufacturer);
                        if (manuReplyList.Count > 0)
                        {
                            manuReplyList.Sort((a, b) => b.InDate.CompareTo(a.InDate));
                            consultInfo.ManufactureReply = manuReplyList[0];
                        }
                        //如果网友回复被置顶则作为厂商回复
                        List<ProductMatchedTradingReplyInfo> topReplyList = replyList.FindAll(
                            replyInfo =>
                                replyInfo.ReplyType == FeedbackReplyType.Web && replyInfo.IsTop.ToUpper() == "Y");
                        if (topReplyList.Count > 0)
                        {
                            topReplyList.Sort((a, b) => b.EditDate.CompareTo(a.EditDate));
                            consultInfo.ManufactureReply = topReplyList[0];
                            //网友回复列表（除去用来替换厂商回复的一条数据）
                            replyList.RemoveAll(replyInfo => replyInfo.SysNo == topReplyList[0].SysNo);
                        }
                        List<ProductMatchedTradingReplyInfo> userList = replyList.FindAll(
                            replyInfo => replyInfo.ReplyType == FeedbackReplyType.Web);
                        int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                        int pageIndex = queryInfo.PagingInfo.PageIndex;
                        if ((pageIndex * queryInfo.PagingInfo.PageSize) > totalCount)
                        {
                            if (totalCount != 0 && (totalCount % queryInfo.PagingInfo.PageSize) == 0)
                            {
                                pageIndex = totalCount / queryInfo.PagingInfo.PageSize;
                            }
                            else
                            {
                                pageIndex = totalCount / queryInfo.PagingInfo.PageSize + 1;
                            }
                        }
                        consultInfo.PagedReplyList = new PagedResult<ProductMatchedTradingReplyInfo>(totalCount, queryInfo.PagingInfo.PageSize, pageIndex, userList);
                    }
                }
            }
            return consultInfo;
        }

        /// <summary>
        /// 获取咨询列表
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static PagedResult<MatchedTradingInfo> GetProductDetailMatchedTradingList(MatchedTradingQueryInfo queryInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("FeedBack_MatchedTrading_GetProductDetailMatchedTradingList");
            dataCommand.SetParameterValue("@ProductGroupSysNo", queryInfo.ProductGroupSysNo);
            dataCommand.SetParameterValue("@PageSize", queryInfo.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            DataSet result = dataCommand.ExecuteDataSet();
            DataTable masterTable = result.Tables[0];


            var consultInfoList = new List<MatchedTradingInfo>();
            if (masterTable.Rows.Count > 0)
            {
                consultInfoList = DataMapper.GetEntityList<MatchedTradingInfo, List<MatchedTradingInfo>>(masterTable.Rows);
            }
            if (consultInfoList.Count > 0)
            {
                DataTable itemTable = result.Tables[1];
                List<ProductMatchedTradingReplyInfo> replyList = DataMapper.GetEntityList<ProductMatchedTradingReplyInfo, List<ProductMatchedTradingReplyInfo>>(itemTable.Rows);


                DataTable webReplyTable = result.Tables[2];
                List<ProductMatchedTradingReplyInfo> webReplyList = DataMapper.GetEntityList<ProductMatchedTradingReplyInfo, List<ProductMatchedTradingReplyInfo>>(webReplyTable.Rows);


                //厂商以及跨境通回复
                if (replyList.Count > 0)
                {
                    ProductMatchedTradingReplyInfo manufacutrerReply;

                    consultInfoList.ForEach(delegate(MatchedTradingInfo consultInfo)
                    {

                        //新蛋回复
                        consultInfo.NeweggReply = replyList.Find(
                            replyInfo =>
                                replyInfo.ReplyType == FeedbackReplyType.Newegg &&
                                replyInfo.MatchedTradingSysNo == consultInfo.SysNo);

                        //厂商回复
                        manufacutrerReply = replyList.Find(
                            replyInfo =>
                                replyInfo.ReplyType == FeedbackReplyType.Web &&
                                replyInfo.MatchedTradingSysNo == consultInfo.SysNo && replyInfo.IsTop.ToUpper() == "Y");

                        if (manufacutrerReply == null)
                        {
                            manufacutrerReply = replyList.Find(
                                replyInfo =>
                                    replyInfo.ReplyType == FeedbackReplyType.Manufacturer &&
                                    replyInfo.MatchedTradingSysNo == consultInfo.SysNo);
                        }
                        else
                        {
                            consultInfo.ReplyCount = consultInfo.ReplyCount - 1;
                        }
                        consultInfo.ManufactureReply = manufacutrerReply;

                    });
                }

                //网友回复

                if (webReplyList.Count > 0)
                {
                    consultInfoList.ForEach(delegate(MatchedTradingInfo consultInfo)
                    {
                        //网友回复
                        consultInfo.ReplyList = webReplyList.FindAll(
                           replyInfo =>
                               replyInfo.ReplyType == FeedbackReplyType.Web &&
                                replyInfo.MatchedTradingSysNo == consultInfo.SysNo);
                    });
                }
            }
            int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            //return new PagedList<MatchedTradingInfo>(consultInfoList, 0, 0, totalCount);

            PagedResult<MatchedTradingInfo> list = new PagedResult<MatchedTradingInfo>(totalCount, queryInfo.PagingInfo.PageSize, queryInfo.PagingInfo.PageIndex, consultInfoList);
            list.PageSize = queryInfo.PagingInfo.PageSize;

            if (list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (MatchedTradingInfo info in list)
                    sb.Append(info.ProductSysNo.ToString()).Append(",");

                if (sb.Length > 1)
                    sb.Remove(sb.Length - 1, 1);

                DataCommand dataCommand2 = DataCommandManager.GetDataCommand("FeedBack_GetProductGroupByProductSysnos");
                dataCommand2.SetParameterValue("@productSysnos", sb.ToString());

                List<GroupPropertyVendorInfo> groups = dataCommand2.ExecuteEntityList<GroupPropertyVendorInfo>();

                if (groups != null && groups.Count > 0)
                    foreach (MatchedTradingInfo info in list)
                        foreach (GroupPropertyVendorInfo g in groups)
                            if (g.ProductSysNo == info.ProductSysNo)
                            {
                                info.GroupPropertyInfo = g.GroupPropertyInfo;
                                info.VendorInfo = g.VendorInfo;
                                break;
                            }
            }
            return list;
        }

        /// <summary>
        /// 发表咨询
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool CreateProductMatchedTrading(MatchedTradingInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductMatchedTrading");
            cmd.SetParameterValue("@ProductSysNo", info.ProductSysNo);
            cmd.SetParameterValue("@CustomerSysNo", info.CustomerSysNo);
            cmd.SetParameterValue("@Content", info.Content);
            cmd.SetParameterValue("@Type", info.Type);
            cmd.SetParameterValue("@Status", info.Status);
            SetCommandDefaultParameters(cmd);
            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 取得咨询内容
        /// </summary>
        /// <param name="consultSysNo"></param>
        /// <returns></returns>
        public static MatchedTradingInfo GetProductMatchedTrading(int consultSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductMatchedTrading");
            cmd.SetParameterValue("@SysNo", consultSysNo);
            return cmd.ExecuteEntity<MatchedTradingInfo>();
        }

        /// <summary>
        /// 发表咨询回复
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool CreateProductMatchedTradingReply(ProductMatchedTradingReplyInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductMatchedTradingReply");
            cmd.SetParameterValue("@MatchedTradingSysNo", info.MatchedTradingSysNo);
            cmd.SetParameterValue("@CustomerSysNo", info.CustomerSysNo);
            cmd.SetParameterValue("@Content", info.Content);

            SetCommandDefaultParameters(cmd);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

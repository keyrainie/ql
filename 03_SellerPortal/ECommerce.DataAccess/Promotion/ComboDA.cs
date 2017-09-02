using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity.Promotion;
using ECommerce.Utility.DataAccess;
using ECommerce.Utility;
using ECommerce.Entity.Common;

namespace ECommerce.DataAccess.Promotion
{
    public class ComboDA
    {
        public virtual int CreateMaster(ComboInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCombo");
            cmd.SetParameterValue("@SaleRuleName", info.SaleRuleName);
            cmd.SetParameterValue("@SaleRuleType", 0);
            cmd.SetParameterValue("@CreateUserSysNo", info.InUserSysNo);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@IsShow", "Y");
            cmd.SetParameterValue("@Priority", 0);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@ReferenceSysNo", 0);
            cmd.SetParameterValue("@ReferenceType", 1);
            cmd.SetParameterValue("@Reason", "");
            cmd.SetParameterValue("@VendorSysNo", info.SellerSysNo);
            cmd.SetParameterValue("@CreateUserName", info.InUserName);

            cmd.ExecuteNonQuery();
            info.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return info.SysNo;
        }

        public virtual void UpdateMaster(ComboInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCombo");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@SaleRuleName", info.SaleRuleName);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@IsShow", "Y");
            cmd.SetParameterValue("@Priority", 0);
            cmd.SetParameterValue("@SaleRuleType", 0);
            cmd.SetParameterValue("@Reason", "");
            cmd.ExecuteNonQuery();
        }

        public virtual void UpdateStatus(int sysNo, int targetStatus,string editUser)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateStatus");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@Status", targetStatus);
            cmd.SetParameterValue("@EditUser", editUser);
            cmd.ExecuteNonQuery();
        }

        public virtual int AddComboItem(ComboItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AddComboItem");
            cmd.SetParameterValue("@SaleRuleSysNo", item.ComboSysNo);
            cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            cmd.SetParameterValue("@Quantity", item.Quantity);
            cmd.SetParameterValue("@Discount", item.Discount);
            cmd.SetParameterValue("@IsMasterItem", item.IsMasterItem ? 1 : 0);
            cmd.ExecuteNonQuery();
            item.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return item.SysNo;

        }

        public virtual void DeleteComboAllItem(int comboSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteComboAllItem");
            cmd.SetParameterValue("@ComboSysNo", comboSysNo);
            cmd.ExecuteNonQuery();
        }

        //public List<ComboInfo> GetComboListByProductSysNo(int productSysNo)
        //{
        //    DataCommand cmd = DataCommandManager.GetDataCommand("GetComboListByProductSysNo");
        //    cmd.SetParameterValue("@ProductSysNo", productSysNo);
        //    DataSet ds = cmd.ExecuteDataSet();
        //    List<ComboInfo> comboList = new List<ComboInfo>();
        //    DataTable dtMaster = ds.Tables[0];
        //    DataTable dtItems = ds.Tables[1];
        //    if (dtMaster != null && dtMaster.Rows.Count > 0)
        //    {
        //        comboList = DataMapper.GetEntityList<ComboInfo, List<ComboInfo>>(dtMaster.Rows);
        //        List<ComboItem> tempItemList = DataMapper.GetEntityList<ComboItem, List<ComboItem>>(dtItems.Rows, (row, entity) =>
        //        {
        //            entity.IsMasterItem = row["IsMasterItem"] == null ? false : (row["IsMasterItem"].ToString() == "1" ? true : false);
        //        });
        //        foreach (ComboInfo combo in comboList)
        //        {
        //            combo.Items = new List<ComboItem>();
        //            foreach (ComboItem item in tempItemList)
        //            {
        //                if (combo.SysNo == item.ComboSysNo)
        //                {
        //                    combo.Items.Add(item);
        //                }
        //            }
        //        }
        //    }
        //    return comboList;
        //}

        //public virtual int CheckComboExits(string comboName, int productSysNo)
        //{
        //    DataCommand dc = DataCommandManager.GetDataCommand("CheckSaleRuleExits");

        //    dc.SetParameterValue("@ProductSysNo", productSysNo);
        //    dc.SetParameterValue("@SaleRuleName", comboName);
        //    dc.SetParameterValue("@CreateTime", DateTime.Now.Date);

        //    return dc.ExecuteScalar<int>();
        //}


        /// <summary>
        /// 加载Combo所有信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ComboQueryResult Load(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadComboInfo");
            cmd.SetParameterValue("@SysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            DataTable dtMaster = ds.Tables[0];
            DataTable dtItem = ds.Tables[1];
            if (dtMaster == null || dtMaster.Rows.Count == 0)
            {
                return null;
            }

            var info = DataMapper.GetEntity<ComboQueryResult>(dtMaster.Rows[0]);

            var itemList = DataMapper.GetEntityList<ComboItemQueryResult, List<ComboItemQueryResult>>(dtItem.Rows);
            info.Items = new List<ComboItem>();
            foreach (var item in itemList.OrderByDescending(item=>item.IsMasterItem))
            {
                info.Items.Add(item);
            }

            return info;
        }

        public QueryResult<ComboQueryResult> Query(ComboQueryFilter filter)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.SortFields;
            pagingEntity.MaximumRows = filter.PageSize;
            pagingEntity.StartRowIndex = filter.PageIndex * filter.PageSize;
            if (!string.IsNullOrEmpty(pagingEntity.SortField) && pagingEntity.SortField.Contains("DiscountAmt"))
            {
                pagingEntity.SortField = pagingEntity.SortField.Replace("DiscountAmt", "Isnull(Sum(si.[Quantity] * si.[Discount]),0)");
            }
            //if (!string.IsNullOrEmpty(pagingEntity.SortField) && pagingEntity.SortField.Contains("PriceDiff"))
            //{
            //    pagingEntity.SortField = pagingEntity.SortField.Replace("PriceDiff", "Isnull(Sum(pr.[CurrentPrice]*si.[Quantity] - pr.UnitCost*si.[Quantity]+si.[Quantity]* si.[Discount]),0)");
            //}

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCombo");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "sm.[CreateTime] DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "sm.SysNo", DbType.Int32, "@SystemNumber", QueryConditionOperatorType.Equal, filter.SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sm.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);



                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sm.SaleRuleName", DbType.String,
                    "@SaleRuleName", QueryConditionOperatorType.Like, filter.Name);


                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "sm.VendorSysNo",
                   DbType.Int32,
                   "@MerchantSysNo",
                   QueryConditionOperatorType.Equal,
                   filter.SellerSysNo);


                cmd.CommandText = sqlBuilder.BuildQuerySql();

                //构造商品系统编号，商品编号查询参数
                string strWhere = "";
                if (filter.ProductSysNo.HasValue)
                {
                    strWhere += " and p.SysNo=@ProductSysNo ";
                    cmd.AddInputParameter("@ProductSysNo", DbType.Int32, filter.ProductSysNo);
                }
                if (!string.IsNullOrWhiteSpace(filter.ProductID))
                {
                    strWhere += " and p.ProductID=@ProductID";
                    cmd.AddInputParameter("@ProductID", DbType.AnsiString, filter.ProductID);
                }
                cmd.ReplaceParameterValue("#StrWhere_Product#", strWhere);

                var dataList = cmd.ExecuteEntityList<ComboQueryResult>();
                int totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                var result = new QueryResult<ComboQueryResult>();
                result.ResultList = dataList;
                result.PageInfo = new PageInfo()
                {
                    TotalCount = totalCount,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize
                };
                return result;
            }
        }
    }
}

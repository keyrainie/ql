using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.IDataAccess;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductAccessoriesDA))]
    public class ProductAccessoriesDA : IProductAccessoriesDA
    {

        #region "配件查询操作"
        
        
        /// <summary>
        /// 根据query得到配件查询信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetProductAccessoriesByQuery(ProductAccessoriesQueryFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductAccessoriesByQuery");
            cmd.SetParameterValue("@AccessoriesQueryName",query.AccessoriesQueryName);
            cmd.SetParameterValue("@Status", query.Status);
            cmd.SetParameterValue("@CreateUser", query.CreateUserName);
            cmd.SetParameterValue("@BeginCreateDate", query.CreateDateFrom);
            cmd.SetParameterValue("@EndCreateDate", query.CreateDateTo);
            cmd.SetParameterValue("@PageSize", query.PagingInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", query.PagingInfo.PageIndex);
            cmd.SetParameterValue("@SortField", query.PagingInfo.SortBy);
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable(3, typeof(ValidStatus));
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;

        }


        /// <summary>
        /// 创建查询功能
        /// </summary>
        /// <param name="info"></param>
        public void CreateAccessoriesQueryMaster(ProductAccessoriesInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateAccessoriesQueryMaster");
            cmd.SetParameterValue("@AccessoriesQueryName",info.AccessoriesQueryName);
            cmd.SetParameterValue("@BackPictureBigUrl", info.BackPictureBigUrl);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@CreateUser", info.User.UserName);
            cmd.SetParameterValue("@EditUser", info.User.UserName);
            cmd.SetParameterValue("@IsTreeQuery", info.IsTreeQuery);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", info.LanguageCode);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 检查是否存在该查询功能 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsExistsAccessoriesQuery(ProductAccessoriesInfo info)
        {
               DataCommand cmd ;
               if (info.SysNo > 0)
               {
                   cmd = DataCommandManager.GetDataCommand("IsExistsAccessoriesQueryByUpdate");
                   cmd.SetParameterValue("@SysNo", info.SysNo);
               }
               else
               {
                   cmd = DataCommandManager.GetDataCommand("IsExistsAccessoriesQueryByCreate");
               }
    
            cmd.SetParameterValue("@AccessoriesQueryName", info.AccessoriesQueryName);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@Flag")<0;
        }

        /// <summary>
        /// 修改配件查询功能信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateAccessoriesQueryMaster(ProductAccessoriesInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateAccessoriesQueryMaster");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@AccessoriesQueryName", info.AccessoriesQueryName);
            cmd.SetParameterValue("@BackPictureBigUrl", info.BackPictureBigUrl);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@EditUser", info.User.UserName);
            cmd.SetParameterValue("@IsTreeQuery", info.IsTreeQuery);
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region "查询条件操作"
        
       
        public DataTable GetAccessoriesQueryConditionBySysNo(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAccessoriesQueryConditionBySysNo");
            cmd.SetParameterValue("@SysNo", SysNo);
            return cmd.ExecuteDataTable();
        }

        /// <summary>
        /// 创建查询条件
        /// </summary>
        /// <param name="Info"></param>
        public void CreateAccessoriesQueryCondition(ProductAccessoriesQueryConditionInfo Info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateAccessoriesQueryCondition");
            cmd.SetParameterValue("@ConditionName",Info.Condition.ConditionName);
            cmd.SetParameterValue("@Level", Info.Condition.Priority);
            cmd.SetParameterValue("@ParentSysNo", Info.ParentCondition.SysNo);
            cmd.SetParameterValue("@MasterSysNo", Info.Condition.MasterSysNo);
            cmd.ExecuteNonQuery();
            
        }

        /// <summary>
        /// 修改查询条件
        /// </summary>
        /// <param name="Info"></param>
        public void UpdateAccessoriesQueryCondition(ProductAccessoriesQueryConditionInfo Info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateAccessoriesQueryCondition");
            cmd.SetParameterValue("@ConditionName", Info.Condition.ConditionName);
            cmd.SetParameterValue("@Level", Info.Condition.Priority);
            cmd.SetParameterValue("@ParentSysNo", Info.ParentCondition.SysNo);
            cmd.SetParameterValue("@MasterSysNo", Info.Condition.MasterSysNo);
            cmd.SetParameterValue("@SysNo", Info.Condition.SysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除查询条件
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteAccessoriesQueryCondition(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteAccessoriesQueryCondition");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 检查条件是否已存在
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsExistsAccessoriesQueryCondition(ProductAccessoriesQueryConditionInfo info)
        {
            DataCommand cmd;
            if (info.Condition.SysNo > 0)
            {
                cmd = DataCommandManager.GetDataCommand("IsExistsAccessoriesQueryConditionByUpdate");
                cmd.SetParameterValue("@SysNo",info.Condition.SysNo);
            }
            else
            {
                cmd = DataCommandManager.GetDataCommand("IsExistsAccessoriesQueryConditionByCreate");
            }
            cmd.SetParameterValue("@ConditionName", info.Condition.ConditionName);
            cmd.SetParameterValue("@MasterSysNo", info.Condition.MasterSysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@Flag")<0;
        }

        #endregion


        #region "条件选项值操作"



        /// <summary>
        /// 根据query获取选项值
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetProductAccessoriesConditionValueByQuery(ProductAccessoriesConditionValueQueryFilter Query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductAccessoriesConditionValueByQuery");
            cmd.SetParameterValue("@ConditionSysNo",Query.ConditionSysNo);
            cmd.SetParameterValue("@ConditionValue", Query.ConditionValue);
            cmd.SetParameterValue("@MasterSysNo", Query.MasterSysNo);
            cmd.SetParameterValue("@SortField", Query.PagingInfo.SortBy);
            cmd.SetParameterValue("@PageSize", Query.PagingInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", Query.PagingInfo.PageIndex);
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable();
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;
       }

        /// <summary>
        /// 得到该条件的父节点的所有选项值
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetProductAccessoriesConditionValueByConditionSysNo(ProductAccessoriesConditionValueQueryFilter query)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductAccessoriesConditionValueByConditionSysNo");
            cmd.SetParameterValue("@ConditionSysNo", query.ConditionSysNo);
            cmd.SetParameterValue("@MasterSysNo", query.MasterSysNo);
            return cmd.ExecuteDataTable();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="Info"></param>
        public void CreateProductAccessoriesQueryConditionValue(ProductAccessoriesQueryConditionValueInfo Info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductAccessoriesQueryConditionValue");
            cmd.SetParameterValue("@ConditionValue",Info.ConditionValue);
            cmd.SetParameterValue("@ConditionSysNo", Info.ConditionSysNo);
            cmd.SetParameterValue("@ParentSysNo", Info.ConditionValueParentSysNo);
            cmd.SetParameterValue("@MasterSysNo", Info.MasterSysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="Info"></param>
        public void UpdateProductAccessoriesQueryConditionValue(ProductAccessoriesQueryConditionValueInfo Info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductAccessoriesQueryConditionValue");
            cmd.SetParameterValue("@ConditionValue", Info.ConditionValue);
            cmd.SetParameterValue("@ConditionSysNo", Info.ConditionSysNo);
            cmd.SetParameterValue("@ParentSysNo", Info.ConditionValueParentSysNo);
            cmd.SetParameterValue("@MasterSysNo", Info.MasterSysNo);
            cmd.SetParameterValue("@SysNo", Info.SysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteProductAccessoriesQueryConditionValue(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteProductAccessoriesQueryConditionValue");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.ExecuteNonQuery();
        }

       /// <summary>
       /// 检查选项值是否存在
       /// </summary>
       /// <param name="info"></param>
       /// <returns></returns>
        public bool IsExistsAccessoriesQueryConditionValue(ProductAccessoriesQueryConditionValueInfo info)
        {
            DataCommand cmd;
            if (info.SysNo > 0)
            {
                cmd = DataCommandManager.GetDataCommand("IsExistsAccessoriesQueryConditionValueByUpdate");
                cmd.SetParameterValue("@SysNo", info.SysNo);
            }
            else
            {
                cmd = DataCommandManager.GetDataCommand("IsExistsAccessoriesQueryConditionValueByCreate");
            }
            cmd.SetParameterValue("@ConditionValue", info.ConditionValue);
            cmd.SetParameterValue("@ConditionSysNo", info.ConditionSysNo);
            cmd.SetParameterValue("@ParentSysNo", info.ConditionValueParentSysNo);
            cmd.SetParameterValue("@MasterSysNo", info.MasterSysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@Flag") < 0;
        }

        #endregion



        #region "查询效果操作"

        /// <summary>
        /// 得到某条件下的所有选项值
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetConditionValueByQuery(ProductAccessoriesConditionValueQueryFilter query)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetConditionValueByQuery");
            cmd.SetParameterValue("@ConditionSysNo", query.ConditionSysNo);
            cmd.SetParameterValue("@MasterSysNo", query.MasterSysNo);
            return cmd.ExecuteDataTable();
        }
        /// <summary>
        /// 得到商品和条件选项值bing的信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCoutn"></param>
        /// <returns></returns>
        public DataTable QueryAccessoriesQueryConditionBind(ProductAccessoriesQueryConditionPreViewQueryFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryAccessoriesQueryConditionBind");
            cmd.SetParameterValue("@MasterSysNo",query.MasterSysNo);
            cmd.SetParameterValue("@ProductID", query.ProductID);
            cmd.SetParameterValue("@ConditionValueSysNo1", query.ConditionValueSysNo1);
            cmd.SetParameterValue("@ConditionValueSysNo2", query.ConditionValueSysNo2);
            cmd.SetParameterValue("@ConditionValueSysNo3", query.ConditionValueSysNo3);
            cmd.SetParameterValue("@ConditionValueSysNo4", query.ConditionValueSysNo4);
            cmd.SetParameterValue("@Category1SysNo", query.Category1SysNo);
            cmd.SetParameterValue("@Category2SysNo", query.Category2SysNo);
            cmd.SetParameterValue("@Category3SysNo", query.Category3SysNo);
            cmd.SetParameterValue("@SortField", query.PagingInfo.SortBy);
            cmd.SetParameterValue("@PageSize", query.PagingInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", query.PagingInfo.PageIndex);
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable();
            totalCount = (int)cmd.GetParameterValue("@TotalCount");

            return dt;
        }

        /// <summary>
        /// 删除bing
        /// </summary>
        /// <param name="info"></param>
        public void DeleteAccessoriesQueryConditionBind(ProductAccessoriesQueryConditionPreViewInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteAccessoriesQueryConditionBind");
            cmd.SetParameterValue("@ProductSysNo", info.ProductSysNo);
            cmd.SetParameterValue("@ConditionValueSysNo", info.ConditionValueSysNo);
            cmd.SetParameterValue("@masterSysNo", info.masterSysNo);
            cmd.ExecuteNonQuery();

        }

        #endregion

        #region "导出"
        public DataTable GetAccessoriesQueryExcelOutput(ProductAccessoriesConditionValueQueryFilter query)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAccessoriesQueryExcelOutput");
            cmd.SetParameterValue("@MasterSysNo", query.MasterSysNo);
            cmd.SetParameterValue("@IsTreeQuery", query.IsTreeQuery);
            DataTable dt= cmd.ExecuteDataTable();
            if (dt.Rows.Count == 0)
            {
                
               var row= dt.NewRow();
               dt.Rows.Add(row);
               dt.Rows[0][0] = 0;
                dt.Rows[0][1] = "No data records.";
            }
            return dt;
            
        }
        #endregion







       
    }
    
}

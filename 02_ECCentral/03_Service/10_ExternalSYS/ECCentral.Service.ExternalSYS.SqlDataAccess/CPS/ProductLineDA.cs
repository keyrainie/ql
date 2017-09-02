using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess.CPS
{
    [VersionExport(typeof(IProductLineDA))]
    public class ProductLineDA : IProductLineDA
    {
        /// <summary>
        /// 根据query得到产品线信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns>DataTable</returns>
        public DataTable GetProductLineByQuery(ProductLineQueryFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductLineByQuery");
            cmd.SetParameterValue("@ProductLineName", query.ProductLineName);
            cmd.SetParameterValue("@PageIndex", query.PageInfo.PageIndex);
            cmd.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            cmd.SetParameterValue("@SortFiled", query.PageInfo.SortBy);
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable();
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;
        }
        /// <summary>
        /// 得到产品线分类
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllProductLineCategory()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllProductLineCategory");
            return cmd.ExecuteDataTable();
        }
        /// <summary>
        /// 创建产品线信息
        /// </summary>
        /// <param name="info"></param>
        public ProductLineInfo CreateProductLine(ProductLineInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductLine");
            cmd.SetParameterValue("@ProductLineName", info.ProductLineName);
            cmd.SetParameterValue("@ProductLineCategorySysNo", info.ProductLineCategorySysNo);
            cmd.SetParameterValue("@Priority", info.Priority);
            cmd.SetParameterValue("@UseScopeDescription", info.UseScopeDescription);
            cmd.SetParameterValue("@UserName", info.User.UserName);
            cmd.SetParameterValue("@ComPanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", info.LanguageCode);
            cmd.ExecuteNonQuery();
            info.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return info;
        }
        /// <summary>
        /// 更新产品线信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateProductLine(ProductLineInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductLine");
            cmd.SetParameterValue("@ProductLineName", info.ProductLineName);
            cmd.SetParameterValue("@ProductLineCategorySysNo", info.ProductLineCategorySysNo);
            cmd.SetParameterValue("@Priority", info.Priority);
            cmd.SetParameterValue("@UserName", info.User.UserName);
            cmd.SetParameterValue("@UseScopeDescription", info.UseScopeDescription);
            cmd.SetParameterValue("@SysNo", info.SysNo);
             cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 检查是否在同一个类别下,已存在该产品线
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsExistsProductLine(ProductLineInfo info)
        {
            DataCommand cmd;
            if (info.SysNo > 0)
            {
                cmd = DataCommandManager.GetDataCommand("IsExistsProductLineByUpdate");
                cmd.SetParameterValue("@SysNo", info.SysNo);
            }
            else
            {
                cmd = DataCommandManager.GetDataCommand("IsExistsProductLine");
            }
            cmd.SetParameterValue("@ProductLineName", info.ProductLineName);
            cmd.SetParameterValue("@ProductLineCategorySysNo", info.ProductLineCategorySysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@Flag") > 0;

        }
        /// <summary>
        /// 删除产品线信息
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteProductLine(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteProductLine");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 新增，更新时 都要把表中所有大于等于该记录优先级的数据优先级+1
        /// </summary>
        /// <param name="Priority"></param>
        public void UpdatePriority(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdatePriority");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.ExecuteNonQuery();
          }

        /// <summary>
        /// 根据产品线类别SysNo得到产品线
        /// </summary>
        /// <param name="CategorySysNo"></param>
        /// <returns>DataTable</returns>
        public DataTable GetProductLineByProductLineCategorySysNo(int CategorySysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductLineByProductLineCategorySysNo");
            cmd.SetParameterValue("@CategorySysNo", CategorySysNo);
            return  cmd.ExecuteDataTable();
        }
    }
}

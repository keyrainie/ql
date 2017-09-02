using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IProductUseCouponLimitDA))]
    public class ProductUseCouponLimitDA : IProductUseCouponLimitDA
    {
        /// <summary>
        /// 根据query获取特殊商品限制使用蛋卷信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetProductUseCouponLimitByQuery(ProductUseCouponLimitQueryFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductUseCouponLimitByQuery");
            cmd.SetParameterValue("@ProductSysNo", query.ProductSysNo);
            cmd.SetParameterValue("@Type", query.CouponLimitType);
            cmd.SetParameterValue("@Status", query.Status);
            cmd.SetParameterValue("@SortField", query.PageInfo.SortBy);
            cmd.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", query.PageInfo.PageIndex);
            EnumColumnList enumList = new EnumColumnList
                {
                    { "Status", typeof(ADStatus) },
                    { "Type", typeof(CouponLimitType) }
                   
                };
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable(enumList);
           totalCount= (int)cmd.GetParameterValue("@TotalCount");
           return dt;


        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="info"></param>
        public int CreateProductUseCouponLimit(ProductUseCouponLimitInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductUseCouponLimit");
            cmd.SetParameterValue("@ProductID", info.ProductId);
            cmd.SetParameterValue("@InUser", info.User.UserName);
            int result = cmd.ExecuteScalar<int>();
            return result; ;

        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteProductUseCouponLimit(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteProductUseCouponLimit");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="SysNo"></param>
        public void UpdateProductUseCouponLimitStatus(int SysNo, ADStatus status)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductUseCouponLimitStatus");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.SetParameterValue("@Status", status);
            cmd.ExecuteNonQuery();
        }

        #region Job
        public List<ProductJobLimitProductInfo> GetLimitProductList(string datacommandname)
        {
            DataCommand command = DataCommandManager.GetDataCommand(datacommandname);
            return command.ExecuteEntityList<ProductJobLimitProductInfo>();
        }
        public ProductJobLimitProductInfo GetLimitProductByProductSysNo(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetLimitProductByProductSysNo");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            return command.ExecuteEntity<ProductJobLimitProductInfo>();
        }
        public void CreateLimitProduct(ProductJobLimitProductInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateLimitProduct");
            cmd.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            cmd.SetParameterValue("@ReferenceSysNo", entity.ReferenceSysNo);
            cmd.SetParameterValue("@InUser", entity.InUser);
            cmd.SetParameterValue("@Type", entity.Type);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
            cmd.ExecuteNonQuery();
        }
        #endregion
    }
}

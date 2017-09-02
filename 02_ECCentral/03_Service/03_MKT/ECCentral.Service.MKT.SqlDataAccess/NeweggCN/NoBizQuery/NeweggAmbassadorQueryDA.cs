using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Enum;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(INeweggAmbassadorQueryDA))]
    public class NeweggAmbassadorQueryDA : INeweggAmbassadorQueryDA
    {

        #region INeweggAmbassadorQueryDA Members

        public DataSet QueryAmbassadorBasicInfo(QueryFilter.MKT.NeweggAmbassadorQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetNeweggAmbassadorList");

            cmd.SetParameterValue("@NeweggAmbassadorID", filter.AmbassadorID);
            cmd.SetParameterValue("@NeweggAmbassadorName",string.IsNullOrEmpty(filter.AmbassadorName)?"":string.Format("%{0}%", filter.AmbassadorName.Trim()));
            cmd.SetParameterValue("@CustomerMark", filter.Status);
            cmd.SetParameterValue("@AreaSysNo", filter.AreaSysNo);
            cmd.SetParameterValue("@BigProvinceSysNo", filter.BigAreaSysNo);

            if (!string.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                string[] sortInfo = filter.PagingInfo.SortBy.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (sortInfo.Length > 1)
                {
                    cmd.SetParameterValue("@SortField",sortInfo[0]);
                    cmd.SetParameterValue("@SortType", sortInfo[1]);
                }
                else
                {

                    cmd.SetParameterValue("@SortField", null);
                    cmd.SetParameterValue("@SortType", null);
                }
            }
            else
            {

            cmd.SetParameterValue("@SortField",null);
            cmd.SetParameterValue("@SortType", null);
            }
            cmd.SetParameterValue("@PageSize", filter.PagingInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", filter.PagingInfo.PageIndex);

            DataSet ds = cmd.ExecuteDataSet();
            EnumColumnList enumConfigNews = new EnumColumnList();
            enumConfigNews.Add("CustomerMark", typeof(AmbassadorStatus));

            if (Int32.TryParse(cmd.GetParameterValue("@TotalCount").ToString(), out totalCount))
            { }
            else
            {
                totalCount = 0;
            }

            cmd.ConvertEnumColumn(ds.Tables[0], enumConfigNews);

            return ds;
        }

        /// <summary>
        /// 查询代购订单信息。
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataSet QueryPurchaseOrderInfo(NeweggAmbassadorOrderQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetPanliOrderList");

            cmd.SetParameterValue("@CustomersType", filter.SelectedUserType);

            cmd.SetParameterValue("@NeweggAmbassadorID", filter.CustomerID);
            cmd.SetParameterValue("@SOID", filter.OrderSysNo);
            cmd.SetParameterValue("@OrderDateFrom", filter.OrderTimeFrom);
            cmd.SetParameterValue("@OrderDateTo", filter.OrderTimeTo);
            cmd.SetParameterValue("@PointCreateFrom", filter.PointTimeFrom);
            cmd.SetParameterValue("@PointCreateTo", filter.PointTimeTo);
            cmd.SetParameterValue("@SOStatus", filter.SelectedSOStatus);
            cmd.SetParameterValue("@PointStatus", filter.SelectedPointStatus);
            cmd.SetParameterValue("@BigProvinceSysNo", filter.BigAreaSysNo);

            

            if (!string.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                string[] sortInfo = filter.PagingInfo.SortBy.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (sortInfo.Length > 1)
                {
                    cmd.SetParameterValue("@SortField", sortInfo[0]);
                    cmd.SetParameterValue("@SortType", sortInfo[1]);
                }
                else
                {

                    cmd.SetParameterValue("@SortField", null);
                    cmd.SetParameterValue("@SortType", null);
                }
            }
            else
            {

                cmd.SetParameterValue("@SortField", null);
                cmd.SetParameterValue("@SortType", null);
            }
            cmd.SetParameterValue("@PageSize", filter.PagingInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", filter.PagingInfo.PageIndex);

            cmd.SetParameterValue("@CompanyCode", filter.CompanyCode);
            DataSet ds = cmd.ExecuteDataSet();

            EnumColumnList enumConfigNews = new EnumColumnList();
            enumConfigNews.Add("SOStatus", typeof(SOStatus));
            enumConfigNews.Add("RASysNo", typeof(NYNStatus));
            enumConfigNews.Add("IsAddPoint", typeof(PointStatus));
            enumConfigNews.Add("PayStatus", typeof(PayStatus));
            cmd.ConvertEnumColumn(ds.Tables[0], enumConfigNews);


            totalCount = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);

            return ds;
        }

        /// <summary>
        /// 查询推荐订单信息。
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataSet QueryRecommendOrderInfo(NeweggAmbassadorOrderQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetRecommendOrderList");

            cmd.SetParameterValue("@CreateSOCustomerID", filter.CreateSOCustomerID);

            cmd.SetParameterValue("@NeweggAmbassadorID", filter.CustomerID);
            cmd.SetParameterValue("@SOID", filter.OrderSysNo);
            cmd.SetParameterValue("@OrderDateFrom", filter.OrderTimeFrom);
            cmd.SetParameterValue("@OrderDateTo", filter.OrderTimeTo);
            cmd.SetParameterValue("@PointCreateFrom", filter.PointTimeFrom);
            cmd.SetParameterValue("@PointCreateTo", filter.PointTimeTo);
            cmd.SetParameterValue("@SOStatus", filter.SelectedSOStatus);
            cmd.SetParameterValue("@PointStatus", filter.SelectedPointStatus);
            cmd.SetParameterValue("@BigProvinceSysNo", filter.BigAreaSysNo);

            

            if (!string.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                string[] sortInfo = filter.PagingInfo.SortBy.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (sortInfo.Length > 1)
                {
                    cmd.SetParameterValue("@SortField", sortInfo[0]);
                    cmd.SetParameterValue("@SortType", sortInfo[1]);
                }
                else
                {

                    cmd.SetParameterValue("@SortField", null);
                    cmd.SetParameterValue("@SortType", null);
                }
            }
            else
            {

                cmd.SetParameterValue("@SortField", null);
                cmd.SetParameterValue("@SortType", null);
            }
            cmd.SetParameterValue("@PageSize", filter.PagingInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", filter.PagingInfo.PageIndex);

            cmd.SetParameterValue("@CompanyCode", filter.CompanyCode);
            DataSet ds = cmd.ExecuteDataSet();

            EnumColumnList enumConfigNews = new EnumColumnList();
            enumConfigNews.Add("SOStatus", typeof(SOStatus));
            enumConfigNews.Add("RASysNo", typeof(NYNStatus));
            enumConfigNews.Add("IsAddPoint", typeof(PointStatus));
            enumConfigNews.Add("PayStatus", typeof(PayStatus));
            cmd.ConvertEnumColumn(ds.Tables[0], enumConfigNews);


            totalCount = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);

            return ds;
        }

        /// <summary>
        /// 查询积分发放信息。
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataSet QueryPointInfo(NeweggAmbassadorOrderQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetPointList");

            cmd.SetParameterValue("@CustomersType", filter.SelectedUserType);

            cmd.SetParameterValue("@NeweggAmbassadorID", filter.CustomerID);
            cmd.SetParameterValue("@SOID", filter.OrderSysNo);
            cmd.SetParameterValue("@OrderDateFrom", filter.OrderTimeFrom);
            cmd.SetParameterValue("@OrderDateTo", filter.OrderTimeTo);
            cmd.SetParameterValue("@PointCreateFrom", filter.PointTimeFrom);
            cmd.SetParameterValue("@PointCreateTo", filter.PointTimeTo);
            cmd.SetParameterValue("@SOStatus", filter.SelectedSOStatus);
            cmd.SetParameterValue("@PointStatus", filter.SelectedPointStatus);
            cmd.SetParameterValue("@BigProvinceSysNo", filter.BigAreaSysNo);



            if (!string.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                string[] sortInfo = filter.PagingInfo.SortBy.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (sortInfo.Length > 1)
                {
                    cmd.SetParameterValue("@SortField", sortInfo[0]);
                    cmd.SetParameterValue("@SortType", sortInfo[1]);
                }
                else
                {

                    cmd.SetParameterValue("@SortField", null);
                    cmd.SetParameterValue("@SortType", null);
                }
            }
            else
            {

                cmd.SetParameterValue("@SortField", null);
                cmd.SetParameterValue("@SortType", null);
            }
            cmd.SetParameterValue("@PageSize", filter.PagingInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", filter.PagingInfo.PageIndex);

            cmd.SetParameterValue("@CompanyCode", filter.CompanyCode);
            DataSet ds = cmd.ExecuteDataSet();

            EnumColumnList enumConfigNews = new EnumColumnList();
            enumConfigNews.Add("SOStatus", typeof(SOStatus));
            enumConfigNews.Add("RASysNo", typeof(NYNStatus));
            enumConfigNews.Add("IsAddPoint", typeof(PointStatus));
            enumConfigNews.Add("PayStatus", typeof(PayStatus));
            cmd.ConvertEnumColumn(ds.Tables[0], enumConfigNews);


            totalCount = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);

            return ds;
        }

        #endregion
    }
}

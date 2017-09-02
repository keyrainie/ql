using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IGroupBuyingDA
    {
        /// <summary>
        /// 创建团购
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Create(GroupBuyingInfo entity);
        /// <summary>
        /// 创建团购阶梯价格
        /// </summary>
        /// <param name="ProductSysNo"></param>
        /// <param name="SellCount"></param>
        /// <param name="GroupBuyingPrice"></param>
        /// <returns></returns>
        int CreateProductGroupBuyingPrice(int ProductGroupBuyingSysNo, int? SellCount, decimal? GroupBuyingPrice, int? GroupBuyingPoint,decimal? costAmt);

        void CreateGroupBuyingActivityRel(int groupBuyingSysNo, int vendorStoreSysNo);

        void DeleteGroupBuyingActivityRel(int groupBuyingSysNo);

        /// <summary>
        /// 删除团购阶梯价格
        /// </summary>
        /// <param name="ProductGroupBuyingSysNo"></param>
        void DeleteProductGroupBuyingPrice(int ProductGroupBuyingSysNo);
        

        /// <summary>
        /// 更新团购
        /// </summary>
        /// <param name="entity"></param>
        void Update(GroupBuyingInfo entity);

        /// <summary>
        /// 加载团购信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        GroupBuyingInfo Load(int sysNo);

        List<ProductPromotionDiscountInfo> GetProductGroupBuyingPriceByProductSysNo(int productSysNo, GroupBuyingStatus gbStatus);

        Dictionary<int, string> GetGroupBuyingTypes();

        Dictionary<int, string> GetGroupBuyingAreas();

        Dictionary<int, string> GetGroupBuyingVendors();
        /// <summary>
        /// 更新团购状态
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="status"></param>
        /// <param name="userName"></param>
        void UpdataSatus(int sysNo, string status, string userName);

        /// <summary>
        /// 更新团购结束时间为当前时间，用于中止团购
        /// </summary>
        /// <param name="sysNo"></param>
        void UpdateGroupBuyingEndDate(int sysNo, string userName);

        /// <summary>
        /// 更新团购状态,用于审核流程
        /// </summary>
        /// <param name="entity"></param>
        void UpdateProductGroupBuyingStatus(GroupBuyingInfo entity);

        /// <summary>
        /// 同步Seller Portal团购状态
        /// </summary>
        /// <param name="entity"></param>
        void SyncGroupBuyingStatus(GroupBuyingInfo entity);

        /// <summary>
        /// 验证商品是否在时间段内有相冲突的团购活动
        /// </summary>
        /// <param name="excludeSysNo">排除的团购活动系统编号，比如排除当前编辑的记录的系统编号</param>
        /// <param name="productSysNos">商品系统编号列表，用IN语句实现查询</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>存在返回true,否则返回false</returns>
        bool CheckConflict(int? excludeSysNo, List<int> productSysNos, DateTime beginDate, DateTime endDate);

        #region Job 相关

        /// <summary>
        /// 取得没有处理的团购信息
        /// </summary>
        /// <param name="companyCode">如果为null,表示取得所有没有处理的团购信息</param>
        /// <returns></returns>
        List<GroupBuyingInfo> GetGroupBuyInfoForNeedProcess(string companyCode);
        /// <summary>
        /// 修改团购处理状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="settlementStatus"></param>
        void UpdateGroupBuySettlementStatus(int sysNo, GroupBuyingSettlementStatus settlementStatus);
        #endregion

        /// <summary>
        /// 根据商品编号获取正在参加团购的商品编号
        /// </summary>
        /// <param name="products">待验证的商品编号</param>
        /// <returns>正在参加团购的商品编号</returns>
        List<int> GetProductsOnGroupBuying(IEnumerable<int> products);

        /// <summary>
        /// 读取商品原价
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="isByGroup">是否根据组读取</param>
        /// <returns>商品原价</returns>
        List<object> GetProductOriginalPrice(int productSysNo, string isByGroup, string companyCode);


        #region  Job相关

        List<GroupBuyingInfo> GetGroupBuyingList(int groupBuyingSysNo, int companyCode);

        #endregion

        GroupBuyingCategoryInfo CreateGroupBuyingCategory(GroupBuyingCategoryInfo entity);

        GroupBuyingCategoryInfo UpdateGroupBuyingCategory(GroupBuyingCategoryInfo entity);

        List<GroupBuyingCategoryInfo> GetAllGroupBuyingCategory();

        bool CheckGroupBuyingCategoryNameExists(string categoryName, int excludeSysNo, GroupBuyingCategoryType groupBuyingType);

        GroupBuyingCategoryInfo GetGroupBuyingCategoryBySysNo(int sysNo);

        List<int> GetGroupBuyingVendorStores(int groupBuyingSysNo);

        void ReadGroupbuyingFeedback(int sysNo);

        void HandleGroupbuyingBusinessCooperation(int sysNo);

        GroupBuyingSettlementInfo LoadGroupBuyingSettleBySysNo(int sysNo);

        void UpdateGroupBuyingSettlementStatus(int sysNo, SettlementBillStatus status);

        GroupBuyingTicketInfo LoadGroupBuyingTicketBySysNo(int sysNo);

        void UpdateGroupBuyingTicketStatus(int sysNo, GroupBuyingTicketStatus groupBuyingTicketStatus);

        void UpdateGroupBuyingTicketRefundInfo(GroupBuyingTicketInfo item);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using System.Data;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface ICountdownDA
    {
        /// <summary>
        /// 验证商品是否在时间段内有相冲突的限时抢购活动
        /// </summary>
        /// <param name="excludeSysNo">排除的限时抢购活动系统编号，比如排除当前编辑的记录的系统编号</param>
        /// <param name="productSysNos">商品系统编号列表，用IN语句实现查询</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>存在返回true,否则返回false</returns>
        bool CheckConflict(int excludeSysNo, List<int> productSysNos, DateTime beginDate, DateTime endDate);
        /// <summary>
        /// 验证商品是否在时间段内有相冲突的团购活动
        /// </summary>
        /// <param name="productSysNos">商品系统编号列表，用IN语句实现查询</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>存在返回true,否则返回false</returns>
        bool CheckGroupBuyConflict(List<int> productSysNos, DateTime beginDate, DateTime endDate);

        List<CountdownInfo> GetCountDownByProductSysNo(int productSysNo);
 
        bool HasDuplicateProduct(CountdownInfo entity);

        void CreateCountdown(CountdownInfo entity);

        void CreatePromotionSchedule(CountdownInfo entity);

        bool CheckRunningLimitedItem(CountdownInfo entity);

        void MaintainPromotionSchedule(CountdownInfo entity);

        void MaintainCountdown(CountdownInfo entity);

        void MaintainCountdownStatus(CountdownInfo entity);

        List<CountdownInfo> CountItemHasReserveQtyNotRunning(CountdownInfo entity);

        void VerifyCountdown(CountdownInfo entity);

        void VerifyPromotionSchedule(CountdownInfo oldEntity);

        CountdownInfo Load(int? sysNo);

        List<BizEntity.Common.UserInfo> GetAllCountdownCreateUser(string channleID);

        void SyncCountdownStatus(int requestSysNo, int status, string reason);

        List<ECCentral.BizEntity.PO.VendorInfo> GetCountdownVendorList();
        
        void MainTainCountdownEndTime(CountdownInfo entity);

        bool CheckSaleRuleDiscount(CountdownInfo entity);

        /// <summary>
        /// 检测审核人和创建人是否相同
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        bool CheckUser(int sysNo,int userSysNo);

        /// <summary>
        /// 读取上传的Execl转换DataTable
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        DataTable ReadExcelFileToDataTable(string fileName);

        /// <summary>
        /// 检查限时抢购创建权限
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        bool CheckCreatePermissions(int productSysNo, int userSysNo);
    }
}

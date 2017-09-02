using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IHotSaleCategoryDA
    {
        void Insert(HotSaleCategory msg);

        void Update(HotSaleCategory msg);

        void Delete(int sysNo);

        HotSaleCategory Load(int sysNo);

        List<HotSaleCategory> GetByPosition(HotSaleCategory msg);

        //验证：同一位置有效记录的组名必须相同
        string GetExistsGroupNameByPosition(HotSaleCategory query);

         //验证同位置同组下是否存在重复的分类设置
        bool CheckDuplicateCategory(HotSaleCategory query);

        /// <summary>
        /// 获取同位置同组下其它的记录-
        /// </summary>
        /// <param name="relatedSysNo">参照记录的系统编号</param>
        /// <returns></returns>
        List<HotSaleCategory> GetSameGroupOtherRecords(int relatedSysNo);
    }
}

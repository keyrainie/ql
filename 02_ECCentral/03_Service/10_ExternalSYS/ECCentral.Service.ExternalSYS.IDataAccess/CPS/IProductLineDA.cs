using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.IDataAccess
{
   public interface IProductLineDA
    {
       /// <summary>
       /// 根据query得到产品线信息
       /// </summary>
       /// <param name="query"></param>
       /// <returns>DataTable</returns>
       DataTable GetProductLineByQuery(ProductLineQueryFilter query,out int totalCount);

       /// <summary>
       /// 得到产品线分类
       /// </summary>
       /// <returns></returns>
       DataTable GetAllProductLineCategory();

       /// <summary>
       /// 创建产品线信息
       /// </summary>
       /// <param name="info"></param>
       ProductLineInfo CreateProductLine(ProductLineInfo info);

       /// <summary>
       /// 更新产品线信息
       /// </summary>
       /// <param name="info"></param>
       void UpdateProductLine(ProductLineInfo info);

       /// <summary>
       /// 检查是否在同一个类别下,已存在该产品线
       /// </summary>
       /// <param name="info"></param>
       /// <returns></returns>
       bool IsExistsProductLine(ProductLineInfo info);

       /// <summary>
       /// 删除产品线信息
       /// </summary>
       /// <param name="SysNo"></param>
       void DeleteProductLine(int SysNo);

       /// <summary>
       /// 新增，更新时 都要把表中所有大于等于该记录优先级的数据优先级+1
       /// </summary>
       /// <param name="SysNo"></param>
       void UpdatePriority(int SysNo);

       /// <summary>
       /// 根据产品线类别SysNo得到产品线
       /// </summary>
       /// <param name="CategorySysNo"></param>
       /// <returns></returns>
       DataTable GetProductLineByProductLineCategorySysNo(int CategorySysNo);

       
    }
}

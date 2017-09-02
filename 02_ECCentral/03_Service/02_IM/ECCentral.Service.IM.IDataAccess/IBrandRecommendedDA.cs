//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
//using ECCentral.QueryFilter;
//using ECCentral.BizEntity.IM;

//namespace ECCentral.Service.IM.IDataAccess
//{
//   public  interface IBrandRecommendedDA
//    {
//       /// <summary>
//       /// 获取一级类别
//       /// </summary>
//       /// <returns></returns>
//       DataTable GetCategory1List();
//       /// <summary>
//       /// 获取二级类别
//       /// </summary>
//       /// <returns></returns>
//       DataTable GetCategory2List();

//       /// <summary>
//       /// 获取推荐品牌
//       /// </summary>
//       /// <param name="query"></param>
//       /// <returns></returns>
//       DataTable GetBrandRecommendedList(BrandRecommendedQueryFilter query,out int totalCount);

//       /// <summary>
//       /// 更新推荐品怕
//       /// </summary>
//       /// <param name="info"></param>
//       void UpdateBrandRecommended(BrandRecommendedInfo info);
//    }
//}

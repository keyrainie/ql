using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;

namespace ECCentral.Service.MKT.Restful.Job
{
    public partial class MKTService
    {
        /// <summary>
        /// 清理无效的推荐数据
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/MktJob/ClearTableOnLinelist", Method = "DELETE")]
        public void ClearTableOnLinelist(string day)
        {
            ObjectFactory<MKTJOBAppService>.Instance.ClearTableOnLinelist(day);
        }

        /// <summary>
        /// 获取所有一级类别信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/MktJob/GetAllC1SysNolist", Method = "POST")]
        public void GetAllC1SysNolist()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 标记为删除
        /// </summary>
        /// <param name="pagetype">页面类型</param>
        /// <param name="positionid">位置编号</param>
        /// <param name="C1SysNo">类别编号</param>
        public void UpdateInvalidData(int pagetype, int positionid, int C1SysNo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        public void DeleteOnlineListItem(int p1, int p2, int p3)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取位置编号
        /// </summary>
        /// <param name="pagetype"></param>
        /// <param name="positionid"></param>
        /// <param name="C1SysNo"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public void GetLocationSysno(int pagetype, int positionid, int C1SysNo, string p)
        {
            throw new NotImplementedException();
        }

        public void GetSaleHightItem(int diffnum, int C1SysNo, List<int> itemlist, List<int> groupSysNo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 添加推荐商品
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/MktJob/CreateOnlinelist", Method = "POST")]
        public void CreateOnlinelist(ProductRecommendInfo entity)
        {
            ObjectFactory<ProductRecommendAppService>.Instance.Create(entity);
        }
    }
}

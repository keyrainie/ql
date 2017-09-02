//************************************************************************
// 用户名				泰隆优选
// 系统名				品牌管理
// 子系统名		        品牌管理业务实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(BrandAppService))]
    public class BrandAppService
    {

        /// <summary>
        /// 根据SysNO获取品牌信息
        /// </summary>
        /// <param name="brandSysNo"></param>
        /// <returns></returns>
        public BrandInfo GetBrandInfoBySysNo(int brandSysNo)
        {
            var result = ObjectFactory<BrandProcessor>.Instance.GetBrandInfoBySysNo(brandSysNo);
            return result;
        }

        /// <summary>
        /// 获取所有有效的品牌
        /// </summary>
        /// <returns></returns>
        public List<BrandInfo> GetBrandInfoList()
        {
            var result = ObjectFactory<BrandProcessor>.Instance.GetBrandInfoList();
            return result;
        }

        /// <summary>
        /// 自动生成品牌Code
        /// </summary>
        /// <returns></returns>
        public string GetBrandCode()
        {
            return ObjectFactory<BrandProcessor>.Instance.GetBrandCode();
        }

        /// <summary>
        /// 创建品牌
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public BrandInfo CreateBrand(BrandInfo entity)
        {
            var result = ObjectFactory<BrandProcessor>.Instance.CreateBrand(entity);
            return result;
        }


        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public BrandInfo UpdateBrand(BrandInfo entity)
        {
            var result = ObjectFactory<BrandProcessor>.Instance.UpdateBrand(entity);
            return result;
        }

        /// <summary>
        /// 批量设置品牌置顶
        /// </summary>
        /// <param name="SysNos">SysNo集合</param>
        public void SetTopBrands(List<string> list)
        {
            ObjectFactory<BrandProcessor>.Instance.SetTopBrands(list);
        }
        public void UpdateBrandMasterByManufacturerSysNo(BrandInfo entity)
        {
            ObjectFactory<BrandProcessor>.Instance.UpdateBrandMasterByManufacturerSysNo(entity);

        }
        #region "授权牌操作"
        /// <summary>
        /// 批量删除授权牌
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteBrandAuthorized(List<int> list)
        {
            ObjectFactory<BrandProcessor>.Instance.DeleteBrandAuthorized(list);

        }

        /// <summary>
        /// 更新授权牌的状态
        /// </summary>
        /// <param name="info"></param>
        public void UpdateBrandAuthorized(List<BrandAuthorizedInfo> listInfo)
        {
            ObjectFactory<BrandProcessor>.Instance.UpdateBrandAuthorized(listInfo);
        }

        /// <summary>
        /// 插入授权信息
        /// </summary>
        /// <param name="info"></param>
        public void InsertBrandAuthorized(BrandAuthorizedInfo info)
        {
            ObjectFactory<BrandProcessor>.Instance.InsertBrandAuthorized(info);
        }
        /// <summary>
        /// 是否存在该授权信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsExistBrandAuthorized(BrandAuthorizedInfo info)
        {
            return ObjectFactory<BrandProcessor>.Instance.IsExistBrandAuthorized(info);
        }
        /// <summary>
        /// 检测图片地址是否有效
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool UrlIsExist(string url)
        {
            return ObjectFactory<BrandProcessor>.Instance.UrlIsExist(url);
        }
        #endregion

    }


}

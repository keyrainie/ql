//************************************************************************
// 用户名				泰隆优选
// 系统名				品牌管理
// 子系统名		        品牌管理业务逻辑实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Transactions;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(BrandProcessor))]
    public class BrandProcessor
    {
        private readonly IBrandDA _brandDA = ObjectFactory<IBrandDA>.Instance;

        #region 品牌业务方法
        /// <summary>
        /// 根据SysNo获取品牌信息
        /// </summary>
        /// <param name="brandSysNo"></param>
        /// <returns></returns>
        public virtual BrandInfo GetBrandInfoBySysNo(int brandSysNo)
        {
            CheckBrandProcessor.CheckBrandSysNo(brandSysNo);
            return _brandDA.GetBrandInfoBySysNo(brandSysNo);
        }

        /// <summary>
        /// 获取所有有效的品牌
        /// </summary>
        /// <returns></returns>
        public List<BrandInfo> GetBrandInfoList()
        {
            return _brandDA.GetBrandInfoList();
        }
        /// <summary>
        /// 创建品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual BrandInfo CreateBrand(BrandInfo entity)
        {
            CheckBrandProcessor.CheckBrandInfo(entity);
            return _brandDA.CreateBrand(entity);
        }

        /// <summary>
        /// 修改品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual BrandInfo UpdateBrand(BrandInfo entity)
        {
            if (entity != null)
            {
                CheckBrandProcessor.CheckBrandSysNo(entity.SysNo);
            }
            CheckBrandProcessor.CheckBrandInfo(entity);
            using (TransactionScope scope = new TransactionScope())
            {
                entity = _brandDA.UpdateBrand(entity);
                if (entity.Status == ValidStatus.DeActive)
                {
                    ObjectFactory<IProductLineDA>.Instance.DeleteByBrand(entity.SysNo.Value);
                }
                scope.Complete();
            }
            return entity;
        }
        /// <summary>
        /// 批量设置品牌置顶
        /// </summary>
        /// <param name="SysNos">SysNo集合</param>
        public void SetTopBrands(List<string> list)
        {
            string SysNos = "";
            if (list.Count == 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Brand", "NotSystemNo"));
            }
            foreach (var item in list)
            {
                SysNos = item + "," + SysNos;
            }
            SysNos = SysNos.Substring(0, SysNos.Length - 1);
            _brandDA.SetTopBrands(SysNos);
        }

        public void UpdateBrandMasterByManufacturerSysNo(BrandInfo entity)
        {

            _brandDA.UpdateBrandMasterByManufacturerSysNo(entity);
        }
        #endregion

        #region 检查品牌逻辑
        private static class CheckBrandProcessor
        {
            /// <summary>
            /// 检查品牌实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckBrandInfo(BrandInfo entity)
            {
                if (entity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandIsNull"));
                }
                /*
                if (entity.Manufacturer == null || entity.Manufacturer.SysNo < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandManufacturerSysNoIsNull"));
                }*/
                //var languageCode = Thread.CurrentThread.CurrentUICulture.Name;
                var localName = entity.BrandNameLocal.Content;
                if (String.IsNullOrEmpty(entity.BrandNameGlobal) && String.IsNullOrEmpty(localName))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandNameIsNull"));
                }
                CheckExistBrandName(entity.BrandNameGlobal, entity.SysNo, entity.Manufacturer.SysNo);
                if (entity.SysNo != null && entity.SysNo > 0 && entity.Status == ValidStatus.DeActive)
                {
                    CheckBrandInUsing(entity.SysNo);
                }

                CheckBrandCodeIsExit(entity.BrandCode, entity.SysNo);

            }

            /// <summary>
            /// 检查品牌编号
            /// </summary>
            /// <param name="brandSysNo"></param>
            public static void CheckBrandSysNo(int? brandSysNo)
            {

                if (brandSysNo == null || brandSysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandSysNOIsNull"));
                }
            }

            /// <summary>
            /// 是否存在某个同名的品牌
            /// </summary>
            /// <param name="name"></param>
            /// <param name="brandSysNo"></param>
            /// <param name="manufacturerSysNo"></param>
            /// <returns></returns>
            private static void CheckExistBrandName(string name, int? brandSysNo, int? manufacturerSysNo)
            {
                //if (string.IsNullOrEmpty(name))
                //{
                //    throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandNameIsNull"));
                //}
                if (brandSysNo == null)
                {
                    brandSysNo = 0;
                }
                if (manufacturerSysNo == null)
                {
                    manufacturerSysNo = 0;
                }
                if (brandSysNo.Value < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandSysNOIsNull"));
                }
                var brandDA = ObjectFactory<IBrandDA>.Instance;
                var isExist = brandSysNo == 0 ? brandDA.IsExistBrandName(name, manufacturerSysNo.Value) : brandDA.IsExistBrandName(name, brandSysNo.Value, manufacturerSysNo.Value);
                if (isExist)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Brand", "ExistBrandName"));
                }
            }

            /// <summary>
            /// 检查被商品使用的品牌
            /// </summary>
            /// <param name="brandSysNo"></param>
            /// <returns></returns>
            private static void CheckBrandInUsing(int? brandSysNo)
            {
                if (brandSysNo == null || brandSysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandSysNOIsNull"));
                }
                var brandDA = ObjectFactory<IBrandDA>.Instance;
                var result = brandDA.IsBrandInUsing(brandSysNo.Value);
                if (result)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandIsInvalid"));
                }
            }

            private static void CheckBrandCodeIsExit(string brandCode, int? brandSysNo)
            {
                if (string.IsNullOrEmpty(brandCode))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandCodeIsNull"));
                }
                var brandDA = ObjectFactory<IBrandDA>.Instance;
                var result = brandDA.CheckBrandCodeIsExit(brandCode, brandSysNo);
                if (result)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandCodeIsExit"));
                }
            }
        }
        #endregion

        #region "授权牌操作"
        /// <summary>
        /// 批量删除授权牌
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteBrandAuthorized(List<int> list)
        {
            foreach (var item in list)
            {
                _brandDA.DeleteBrandAuthorized(item);
            }

        }

        /// <summary>
        /// 更新授权牌的状态
        /// </summary>
        /// <param name="info"></param>
        public void UpdateBrandAuthorized(List<BrandAuthorizedInfo> listInfo)
        {
            foreach (var item in listInfo)
            {
                _brandDA.UpdateBrandAuthorized(item);
            }
        }

        /// <summary>
        /// 插入授权信息
        /// </summary>
        /// <param name="info"></param>
        public void InsertBrandAuthorized(BrandAuthorizedInfo info)
        {
            if (!_brandDA.CheckAuthorized(info))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (info.IsExist)
                    {
                        _brandDA.DeleteBrandAuthorizeBySysNoAndBrandSysNo(info);
                    }
                    _brandDA.InsertBrandAuthorized(info);
                    scope.Complete();
                }
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Brand", "DiffBrandNotUseBrand"));
            }

        }
        /// <summary>
        /// 是否存在该授权信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsExistBrandAuthorized(BrandAuthorizedInfo info)
        {
            return _brandDA.IsExistBrandAuthorized(info);
        }

        /// <summary>
        /// 检测图片地址是否有效
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool UrlIsExist(string url)
        {
            System.Uri u = null;
            try
            {
                u = new Uri(url);
            }
            catch { return false; }
            bool isExist = false;
            System.Net.HttpWebRequest r = System.Net.HttpWebRequest.Create(u) as System.Net.HttpWebRequest;
            r.Method = "HEAD";
            try
            {
                System.Net.HttpWebResponse s = r.GetResponse() as System.Net.HttpWebResponse;
                if (s.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    isExist = true;
                }
            }
            catch (System.Net.WebException x)
            {
                try
                {
                    isExist = ((x.Response as System.Net.HttpWebResponse).StatusCode != System.Net.HttpStatusCode.NotFound);
                }
                catch { isExist = (x.Status == System.Net.WebExceptionStatus.Success); }
            }
            return isExist;
        }
        #endregion

        /// <summary>
        /// 自动生成品牌Code
        /// </summary>
        /// <returns></returns>
        public string GetBrandCode()
        {
            return _brandDA.GetBrandCode();
        }
    }

}


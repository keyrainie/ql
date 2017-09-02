//************************************************************************
// 用户名				泰隆优选
// 系统名				厂商管理
// 子系统名		        厂商管理业务逻辑实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Collections.Generic;
using System.Transactions;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ManufacturerProcessor))]
    public class ManufacturerProcessor
    {
        #region 厂商业务逻辑
        private readonly IManufacturerDA _manufacturerDA = ObjectFactory<IManufacturerDA>.Instance;

        /// <summary>
        /// 根据SysNo获取生产商信息
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        public virtual ManufacturerInfo GetManufacturerInfoBySysNo(int manufacturerSysNo)
        {
            return _manufacturerDA.GetManufacturerInfoBySysNo(manufacturerSysNo);
        }

        /// <summary>
        /// 创建生产商信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ManufacturerInfo CreateManufacturer(ManufacturerInfo entity)
        {
            CheckManufacturerProcessor.CheckManufacturerInfo(entity);
            return _manufacturerDA.CreateManufacturer(entity);
        }

        /// <summary>
        /// 更新生产商信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ManufacturerInfo UpdateManufacturer(ManufacturerInfo entity)
        {
            if (entity != null)
            {
                CheckManufacturerProcessor.CheckManufacturerSysNo(entity.SysNo);
            }
            CheckManufacturerProcessor.CheckManufacturerInfo(entity);
            using (TransactionScope scope = new TransactionScope())
            {
                entity = _manufacturerDA.UpdateManufacturer(entity);
                if (entity.Status == ManufacturerStatus.DeActive)
                {
                    ObjectFactory<IProductLineDA>.Instance.DeleteByManufacturer(entity.SysNo.Value);
                }
                scope.Complete();
            }
            return entity;
        }

        public virtual List<ManufacturerInfo> GetAllManufacturer(string companyCode)
        {
            return _manufacturerDA.GetAllManufacturer(companyCode);
        }
        #endregion

        #region 检查厂商逻辑
        public static class CheckManufacturerProcessor
        {
            /// <summary>
            /// 检查品牌实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckManufacturerInfo(ManufacturerInfo entity)
            {
                if (entity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerIsNull"));
                }
                //var languageCode = Thread.CurrentThread.CurrentUICulture.Name;
                var localName = entity.ManufacturerNameLocal.Content;
                if (String.IsNullOrEmpty(entity.ManufacturerNameGlobal) && String.IsNullOrEmpty(localName))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerNameIsNull"));
                }
                //if (String.IsNullOrEmpty(entity.ManufacturerID))
                //{
                //    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerIDIsNull"));
                //}
                if (entity.SysNo != null && entity.SysNo > 0 && entity.Status == ManufacturerStatus.DeActive)
                {
                    CheckManufacturerInUsing(entity.SysNo);
                }
                if (ObjectFactory<IManufacturerDA>.Instance.IsExistManufacturerByUpdate(entity))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ExistNameManufacturer"));
                }

                //if (entity.SysNo != null && entity.SysNo > 0)
                 // CheckExistManufacturerName(entity.ManufacturerNameGlobal, entity.SysNo);
                //CheckExistManufacturerID(entity.ManufacturerID, entity.SysNo);

            }

            /// <summary>
            /// 检查厂商系统编号
            /// </summary>
            /// <param name="sysNo"></param>
            public static void CheckManufacturerSysNo(int? sysNo)
            {

                if (sysNo == null || sysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerSysNOIsNull"));
                }
            }

            /// <summary>
            /// 是否存在某个同名的厂商
            /// </summary>
            /// <param name="name">国际化名称</param>
            /// <param name="manufacturerSysNo"></param>
            /// <returns></returns>
            public static void CheckExistManufacturerName(string name, int? manufacturerSysNo)
            {
                //if (string.IsNullOrEmpty(name))
                //{
                //    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerNameIsNull"));
                //}

                if (manufacturerSysNo == null)
                {
                    manufacturerSysNo = 0;
                }
                if (manufacturerSysNo.Value < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerSysNOIsNull"));
                }
                var manufacturerDA = ObjectFactory<IManufacturerDA>.Instance;
                var isExist = manufacturerSysNo == 0 ? manufacturerDA.IsExistManufacturerName(name) : manufacturerDA.IsExistManufacturerName(name, manufacturerSysNo.Value)>0;
                if (isExist)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ExistManufacturerName"));
                }
            }

            /// <summary>
            /// 检查被商品使用的厂商
            /// </summary>
            /// <param name="manufacturerSysNo"></param>
            /// <returns></returns>
            private static void CheckManufacturerInUsing(int? manufacturerSysNo)
            {
                if (manufacturerSysNo == null || manufacturerSysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerSysNOIsNull"));
                }
                var manufacturerDA = ObjectFactory<IManufacturerDA>.Instance;
                var result = manufacturerDA.IsManufacturerInUsing(manufacturerSysNo.Value);
                if (result)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerIsInvalid"));
                }
            }


            /// <summary>
            /// 是否存在某个同名的厂商
            /// </summary>
            /// <param name="manufacturerID">生产商ID</param>
            /// <param name="manufacturerSysNo"></param>
            /// <returns></returns>
            private static void CheckExistManufacturerID(string manufacturerID, int? manufacturerSysNo)
            {
                if (string.IsNullOrEmpty(manufacturerID))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerNameIsNull"));
                }

                if (manufacturerSysNo == null)
                {
                    manufacturerSysNo = 0;
                }
                if (manufacturerSysNo.Value < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerSysNOIsNull"));
                }
                var manufacturerDA = ObjectFactory<IManufacturerDA>.Instance;
                var isExist = manufacturerSysNo == 0 ? manufacturerDA.IsExistManufacturerID(manufacturerID) : manufacturerDA.IsExistManufacturerID(manufacturerID, manufacturerSysNo.Value);
                if (isExist)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ExistManufacturerID"));
                }
            }
        }
        #endregion

        /// <summary>
        /// 删除旗舰店首页分类
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public void DeleteBrandShipCategory(int sysNo)
        {
            _manufacturerDA.DeleteBrandShipCategory(sysNo);
        }

        /// <summary>
        /// 创建旗舰店首页分类
        /// </summary>
        /// <param name="brandShipCategory"></param>
        /// <returns></returns>
        public void CreateBrandShipCategory(BrandShipCategory brandShipCategory)
        {
            int result = _manufacturerDA.CreateBrandShipCategory(brandShipCategory);
            string message = string.Empty;
            switch (result)
            {
                case -1:
                    message = ResouceManager.GetMessageString("IM.ResManufacturerIndexPageCategory", "Msg_CategoryIsNotExist");
                    throw new BizException(message);
                    break;
                case -2:
                    message = ResouceManager.GetMessageString("IM.ResManufacturerIndexPageCategory", "Msg_CategoryIsExist");
                    throw new BizException(message);
                    break;
                default:
                    break;
            }

        }
    }
}

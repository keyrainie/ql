using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using System.Transactions;
using ECCentral.BizEntity;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(BannerProcessor))]
    public class BannerProcessor
    {
        private IBannerDA _bannerDA = ObjectFactory<IBannerDA>.Instance;
        /// <summary>
        /// 创建广告信息
        /// </summary>
        /// <param name="bannerLocation">广告信息</param>
        public virtual void Create(BannerLocation bannerLocation)
        {
            if (!bannerLocation.BeginDate.HasValue) bannerLocation.BeginDate = DateTime.Now;          

            Validate(bannerLocation);
            using (TransactionScope ts = new TransactionScope())
            {
                //新建广告
                CreateBanner(bannerLocation);
                //处理三级分类扩展生效
                ProcessECCategory3Extend(bannerLocation);
                //TODO:写入创建日志
                ExternalDomainBroker.CreateOperationLog(
                    String.Format("{0}{1}SysNo:{2}",
                    DateTime.Now.ToString(),ResouceManager.GetMessageString("MKT.Banner", "Banner_Add")
                    , bannerLocation.SysNo)
                    , BizEntity.Common.BizLogType.Banner_Add
                    , bannerLocation.SysNo.Value, bannerLocation.CompanyCode);
                ts.Complete();
            }
        }

        private void CreateBanner(BannerLocation bannerLocation)
        {        
            _bannerDA.CreateBannerInfo(bannerLocation.Infos);
            _bannerDA.CreateBannerLocation(bannerLocation);
            //创建广告和主要投放区域之间的关系
            foreach (var area in bannerLocation.AreaShow)
            {
                ObjectFactory<IAreaRelationDA>.Instance.Create(area, bannerLocation.SysNo.Value, AreaRelationType.Banner);
            }
           
        }

        //通过前台3级类别找到对应的后台3级类别，
        //然后把与后台3级类别对用的所有前台3级类别找出来，所有类别都插入记录
        private void ProcessECCategory3Extend(BannerLocation bannerLocation)
        {
            if (bannerLocation.IsExtendValid && bannerLocation.Status == ADStatus.Active)
            {
                if (bannerLocation.PageID.HasValue)
                {
                    var relatedECCategory3List = ObjectFactory<IECCategoryDA>.Instance.GetRelatedECCategory3SysNo(bannerLocation.PageID.Value);
                    foreach (var c3 in relatedECCategory3List)
                    {
                        bannerLocation.PageID = c3.SysNo;
                        CreateBanner(bannerLocation);
                    }
                }
            }
        }

        /// <summary>
        /// 更新广告信息
        /// </summary>
        /// <param name="bannerLocation">广告信息</param>
        public virtual void Update(BannerLocation bannerLocation)
        {
            if (!bannerLocation.BeginDate.HasValue) bannerLocation.BeginDate = DateTime.Now;       

            Validate(bannerLocation);         

            using (TransactionScope ts = new TransactionScope())
            {
                //1.更新广告和主要投放区域之间的关系
                var orginal = Load(bannerLocation.SysNo.Value);
                var temp = bannerLocation.AreaShow.Intersect(orginal.AreaShow).ToList<int>();
                //不在交集中的就删除
                foreach (int r in orginal.AreaShow.Except(temp).ToList<int>())
                {
                    ObjectFactory<IAreaRelationDA>.Instance.Delete(r, bannerLocation.SysNo.Value, AreaRelationType.Banner);
                }
                //不在交集中的就新增
                foreach (int r in bannerLocation.AreaShow.Except(temp).ToList<int>())
                {
                    ObjectFactory<IAreaRelationDA>.Instance.Create(r, bannerLocation.SysNo.Value, AreaRelationType.Banner);
                }

                //2.持久化广告信息
                bannerLocation.Infos.Status = bannerLocation.Status;
                _bannerDA.UpdateBannerInfo(bannerLocation.Infos);
                _bannerDA.UpdateBannerLocation(bannerLocation);
               
                //TODO:写入修改日志
                ExternalDomainBroker.CreateOperationLog(
                   String.Format("{0}{1}SysNo:{2}| 广告类型:{3}| 广告标题:{4} | 资源地址:{5} | 链接地址:{6} | 广告脚本{7}| 状态:{8}",
                   DateTime.Now.ToString(), ResouceManager.GetMessageString("MKT.Banner", "Banner_Update")
                   , bannerLocation.SysNo, bannerLocation.Infos.BannerType
                   , bannerLocation.Infos.BannerTitle
                   , bannerLocation.Infos.BannerResourceUrl
                   , bannerLocation.Infos.BannerLink
                   , bannerLocation.Infos.BannerOnClick
                   , bannerLocation.Infos.Status == ADStatus.Active ? "有效" : "无效")
                   , BizEntity.Common.BizLogType.Banner_Update
                   , bannerLocation.SysNo.Value, bannerLocation.CompanyCode);
                ts.Complete();
            }
        }

        public virtual BannerLocation Load(int bannerLocationSysNo)
        {
            var result = _bannerDA.LoadBannerLocation(bannerLocationSysNo);
            if (result == null)
            {
                //throw new BizException("广告信息不存在。");
                throw new BizException(ResouceManager.GetMessageString("MKT.Banner", "Banner_NotExists"));
            }
            result.Infos = _bannerDA.LoadBannerInfo(result.BannerInfoSysNo);
            result.BannerDimension = ObjectFactory<IBannerDimensionDA>.Instance.LoadBannerDimension(result.BannerDimensionSysNo);
            result.AreaShow = ObjectFactory<IAreaRelationDA>.Instance.GetSelectedArea(bannerLocationSysNo, AreaRelationType.Banner);

            return result;
        }

        /// <summary>
        /// 作废banner
        /// </summary>
        /// <param name="bannerLocationSysNo">系统编号</param>
        public virtual void Delete(int bannerLocationSysNo)
        {
            var banner = Load(bannerLocationSysNo);
            using (TransactionScope ts = new TransactionScope())
            {
                _bannerDA.UpdateBannerLocationStatus(bannerLocationSysNo, ADStatus.Deactive);
                _bannerDA.UpdateBannerInfoStatus(banner.BannerInfoSysNo, ADStatus.Deactive);
                ts.Complete();
            }
            //[Mark][Alan.X.Luo 硬编码]
            ExternalDomainBroker.CreateOperationLog(string.Format("{0}作废", bannerLocationSysNo), BizEntity.Common.BizLogType.Banner_Canel, bannerLocationSysNo, "8601");
        }

         /// <summary>
        /// 检查页面上的Banner位上已有的有效Banner数量
        /// </summary>
        public virtual int CountBannerPosition(int bannerDimensionSysNo, int pageID, string companyCode, string channelID)
        {
            return _bannerDA.CountBannerPosition(bannerDimensionSysNo, pageID, companyCode, channelID);
        }

        private void Validate(BannerLocation loc)
        {
            //BeginDate不能大于EndDate
            if (loc.BeginDate.HasValue && loc.EndDate.HasValue
                && loc.BeginDate.Value > loc.EndDate.Value)
            {
                //throw new BizException("开始时间不能大于结束时间。");
                throw new BizException(ResouceManager.GetMessageString("MKT.Banner", "Banner_BeginDateEndDateInvalid"));
            }
            ////投放地区必须选一个
            //if (loc.AreaShow == null || loc.AreaShow.Count == 0)
            //{
            //    throw new BizException("主要投放区域至少选一个。");
            //}
            //验证广告位是否存在
            var bd = ObjectFactory<IBannerDimensionDA>.Instance.LoadBannerDimension(loc.BannerDimensionSysNo);
            if (bd == null)
            {
                //throw new BizException("广告对应的位置在系统中不存在。");
                throw new BizException(ResouceManager.GetMessageString("MKT.Banner", "Banner_BannerDimensionNotExists"));
            }
        }
    }
}

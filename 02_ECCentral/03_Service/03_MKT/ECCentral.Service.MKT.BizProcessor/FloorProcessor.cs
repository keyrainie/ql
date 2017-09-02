using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT.Floor;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(FloorProcessor))]
    public class FloorProcessor
    {
        private IFloorDA _floorDA = ObjectFactory<IFloorDA>.Instance;

        /// <summary>
        /// 获取所有有效的模板信息
        /// </summary>
        /// <returns></returns>
        public virtual List<FloorTemplate> GetFloorTemplateList()
        {
            return _floorDA.GetFloorTemplateList();
        }

        /// <summary>
        /// 获取所有楼层主信息，包括Active和Deactive
        /// </summary>
        /// <returns></returns>
        public virtual List<FloorMaster> GetAllFloorMasterList()
        {
            return _floorDA.GetAllFloorMasterList();
        }

        /// <summary>
        /// 获取所有楼层主信息，包括Active和Deactive
        /// </summary>
        /// <returns></returns>
        public virtual FloorMaster GetFloorMaster(string sysno)
        {
            return _floorDA.GetFloorMaster(sysno);
        }

        /// <summary>
        /// 创建楼层主信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int CreateFloorMaster(FloorMaster entity)
        {
            entity.InUserSysNo = ServiceContext.Current.UserSysNo;
            entity.InUserName = ServiceContext.Current.UserDisplayName;
            return _floorDA.CreateFloorMaster(entity);
        }

        /// <summary>
        /// 更新楼层主信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateFloorMaster(FloorMaster entity)
        {
            entity.EditUserSysNo = ServiceContext.Current.UserSysNo;
            entity.EditUserName = ServiceContext.Current.UserDisplayName;
            _floorDA.UpdateFloorMaster(entity);
        }

        /// <summary>
        /// 删除楼层，会删除楼层所有相关信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void DeleteFloor(string sysno)
        {
            _floorDA.DeleteFloor(sysno);
        }


        /// <summary>
        /// 获取当前楼层所有的Section list
        /// </summary>
        /// <param name="floorMasterSysNo"></param>
        /// <returns></returns>
        public virtual List<FloorSection> GetFloorSectionList(string floorMasterSysNo)
        {
            return _floorDA.GetFloorSectionList(floorMasterSysNo);
        }

        /// <summary>
        /// 获取当前指定编号的Section
        /// </summary>
        /// <param name="floorMasterSysNo"></param>
        /// <returns></returns>
        public virtual FloorSection GetFloorSection(string floorSectionSysNo)
        {
            return _floorDA.GetFloorSection(floorSectionSysNo);
        }

        /// <summary>
        /// 创建Section基础信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int CreateFloorSection(FloorSection entity)
        {
            return _floorDA.CreateFloorSection(entity);
        }

        /// <summary>
        /// 更新Section基础信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateFloorSection(FloorSection entity)
        {
            _floorDA.UpdateFloorSection(entity);
        }

        /// <summary>
        /// 删除Section，会删除Section所有相关信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void DeleteFloorSection(string sysno)
        {
            _floorDA.DeleteFloorSection(sysno);
        }

        /// <summary>
        /// 获取指定Section下Item的List
        /// </summary>
        /// <param name="floorSectionSysNo"></param>
        /// <returns></returns>
        public virtual List<FloorSectionItem> GetFloorSectionItemList(string floorSectionSysNo)
        {
            var result = _floorDA.GetFloorSectionItemList(floorSectionSysNo);
            result.ForEach(m =>
            {
                switch (m.ItemType)
                {
                    case BizEntity.MKT.FloorItemType.Banner:
                        m.ItemBanner = ECCentral.Service.Utility.SerializationUtility.XmlDeserialize<FloorItemBanner>(m.ItemValue);
                        break;
                    case BizEntity.MKT.FloorItemType.Brand:
                        m.ItemBrand = ECCentral.Service.Utility.SerializationUtility.XmlDeserialize<FloorItemBrand>(m.ItemValue);
                        break;
                    case BizEntity.MKT.FloorItemType.Product:
                        m.ItemProudct = ECCentral.Service.Utility.SerializationUtility.XmlDeserialize<FloorItemProduct>(m.ItemValue);
                        break;
                    case BizEntity.MKT.FloorItemType.TextLink:
                        m.ItemTextLink = ECCentral.Service.Utility.SerializationUtility.XmlDeserialize<FloorItemTextLink>(m.ItemValue);
                        break;                    
                }
                m.ItemValue = string.Empty;
            });
            return result;
        }

        /// <summary>
        /// 创建 Item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int CreateFloorSectionItem(FloorSectionItem entity)
        {
            switch (entity.ItemType)
            {
                case BizEntity.MKT.FloorItemType.Product:
                    entity.ItemValue = ECCentral.Service.Utility.SerializationUtility.XmlSerialize(entity.ItemProudct);
                    break;
                case BizEntity.MKT.FloorItemType.Brand:
                    entity.ItemValue = ECCentral.Service.Utility.SerializationUtility.XmlSerialize(entity.ItemBrand);
                    break;
                case BizEntity.MKT.FloorItemType.Banner:
                    entity.ItemValue = ECCentral.Service.Utility.SerializationUtility.XmlSerialize(entity.ItemBanner);
                    break;
                case BizEntity.MKT.FloorItemType.TextLink:
                    entity.ItemValue = ECCentral.Service.Utility.SerializationUtility.XmlSerialize(entity.ItemTextLink);
                    break;
            }
            return _floorDA.CreateFloorSectionItem(entity);
        }

        /// <summary>
        /// 更新 Item
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateFloorSectionItem(FloorSectionItem entity)
        {
            switch (entity.ItemType)
            {
                case BizEntity.MKT.FloorItemType.Product:
                    entity.ItemValue = ECCentral.Service.Utility.SerializationUtility.XmlSerialize(entity.ItemProudct);
                    break;
                case BizEntity.MKT.FloorItemType.Brand:
                    entity.ItemValue = ECCentral.Service.Utility.SerializationUtility.XmlSerialize(entity.ItemBrand);
                    break;
                case BizEntity.MKT.FloorItemType.Banner:
                    entity.ItemValue = ECCentral.Service.Utility.SerializationUtility.XmlSerialize(entity.ItemBanner);
                    break;
                case BizEntity.MKT.FloorItemType.TextLink:
                    entity.ItemValue = ECCentral.Service.Utility.SerializationUtility.XmlSerialize(entity.ItemTextLink);
                    break;
            }
            _floorDA.UpdateFloorSectionItem(entity);
        }

        /// <summary>
        /// 删除指定的Item
        /// </summary>
        /// <param name="sysno"></param>
        public virtual void DeleteFloorSectionItem(string sysno)
        {
            _floorDA.DeleteFloorSectionItem(sysno);
        }
    }
}

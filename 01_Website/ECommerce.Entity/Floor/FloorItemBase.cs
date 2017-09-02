using ECommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity
{
    /// <summary>
    /// 楼层元素 Item
    /// </summary>
    [Serializable]
    public class FloorItemBase
    {
        public FloorItemBase() { }

        /// <summary>
        /// 根据传入的实体 Clone 本身的属性值
        /// </summary>
        /// <param name="entity"></param>
        public void CloneFloorItemBase(FloorItemBase entity)
        {
            this.FloorSectionSysNo = entity.FloorSectionSysNo;
            this.Priority = entity.Priority;
            this.ItemType = entity.ItemType;
            this.ItemPosition = entity.ItemPosition;
            this.ItemValue = entity.ItemValue;
            this.IsSelfPage = entity.IsSelfPage;
        }
        #region DataMapping Filed
        /// <summary>
        /// 获取或设置所属的 Tab 编号
        /// </summary>
        public int FloorSectionSysNo { get; set; }

        /// <summary>
        /// 获取或设置优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 获取或设置类型
        /// </summary>
        public FloorItemType ItemType { get; set; }

        /// <summary>
        /// 获取或设置内容位置
        /// </summary>
        public int ItemPosition { get; set; }

        /// <summary>
        /// 获取或设置内容数据信息XML
        /// </summary>
        public string ItemValue { get; set; }

        /// <summary>
        /// 获取或设置是否是当前页面打开；【0=新打开页面，1=当前页面】
        /// </summary>
        public int IsSelfPage { get; set; }
        #endregion

        /// <summary>
        /// 填充数据实体的信息
        /// </summary>
        public virtual void FillEntityForXMLData() { }
    }
}

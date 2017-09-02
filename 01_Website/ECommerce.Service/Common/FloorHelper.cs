using ECommerce.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Category;

namespace ECommerce.Facade.Common
{
    /// <summary>
    /// 模板帮助类
    /// </summary>
    public class FloorHelper
    {
        #region 辅助方法
        /// <summary>
        /// 获取楼层信息
        /// </summary>
        /// <typeparam name="T">要获取的楼层信息类型</typeparam>
        /// <param name="floor">楼层数据</param>
        /// <returns></returns>
        public static List<T> GetFloorItem<T>(FloorEntity floor) where T : FloorItemBase, new()
        {
            List<T> result = new List<T>();
            if (floor == null || floor.FloorSectionItems == null)
            {
                return result;
            }

            List<FloorItemBase> brandList = floor.FloorSectionItems.FindAll(x => x is T);
            foreach (var entity in brandList)
            {
                result.Add(entity as T);
            }

            return result;
        }

        public static T GetFloorItemByPosID<T>(List<T> list, int positionID) where T : FloorItemBase, new()
        {
            T item = new T();
            if (list.Exists(x => x.ItemPosition == positionID) == true)
            {
                item = list.Find(x => x.ItemPosition == positionID);
            }
            return item;
        }

        public static T GetFloorItemByPosID<T>(List<T> list, int positionID, int tabSysNo) where T : FloorItemBase, new()
        {
            T item = new T();
            if (list.Exists(x => x.ItemPosition == positionID && x.FloorSectionSysNo == tabSysNo) == true)
            {
                item = list.Find(x => x.ItemPosition == positionID && x.FloorSectionSysNo == tabSysNo);
            }
            return item;
        }
        #endregion

        public static string GetProductDOMTitle(FloorItemProduct item)
        {
            if (string.IsNullOrWhiteSpace(item.PromotionTitle))
                return item.ProductTitle;
            return string.Format("{0}[{1}]", item.ProductTitle, item.PromotionTitle);
        }
        public static string GetProductDOMTitle(RecommendProduct item)
        {
            if (string.IsNullOrWhiteSpace(item.PromotionTitle))
                return item.ProductTitle;
            return string.Format("{0}[{1}]", item.ProductTitle, item.PromotionTitle);
        }
    }
}

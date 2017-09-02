using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Search
{
    public class SearchFilterModel
    {
        public SearchFilterModel()
        {
            this.Items = new List<SearchFilterItemModel>();
        }

        /// <summary> 
        /// 过滤条件ID
        /// Sale = 30, 促销类型
        /// Brand = 80000,品牌
        /// Price = 400062,价格
        /// Category = 100,中类
        /// SubCategory = 40000,小类
        /// Attribute = 999999,//属性
        /// </summary>
        public Int32 ID	{get;set;}

        /// <summary>
        /// 过滤条件名
        /// </summary>
        public String Name{get;set;}	

        /// <summary>
        /// 过滤条件选项
        /// </summary>
        public List<SearchFilterItemModel> Items{get;set;}
    }
}
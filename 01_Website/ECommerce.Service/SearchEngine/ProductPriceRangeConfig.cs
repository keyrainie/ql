using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility.DataAccess.SearchEngine;

namespace ECommerce.Facade.SearchEngine
{
    internal class ProductPriceRangeConfig
    {
        public static List<PriceRange> PriceRangeList { get; set; }

        static ProductPriceRangeConfig()
        {
            PriceRangeList = new List<PriceRange>()
            {
    new PriceRange(){ Key="400001",Text="0 - 100"},
	new PriceRange(){ Key="400002",Text="100 - 200"},
	new PriceRange(){ Key="400003",Text="200 - 300"},
	new PriceRange(){ Key="400004",Text="300 - 400"},
	new PriceRange(){ Key="400005",Text="400 - 800"},
	new PriceRange(){ Key="400006",Text="800 - 1600"},
	new PriceRange(){ Key="400007",Text="1600 - 3200"},
	new PriceRange(){ Key="400008",Text="3200 - 5600"},
	new PriceRange(){ Key="400009",Text="5600 - 8000"},
	new PriceRange(){ Key="400010",Text="8000以上"}
            };
        }
    }

    internal class PriceRange
    {
        public string Key { get; set; }
        public string Text { get; set; }
    }


    
    public class SortKeyValueMappingConfig
    {
        public static List<SortEntity> SortItemList { get; set; }
        static SortKeyValueMappingConfig()
        {
            SortItemList = new List<SortEntity>()
            {
                //默认排序
                new SortEntity(){ Key=10, Item=new SortItem(){ SortKey="p_sysno", SortType=SortOrderType.DESC}},
                //销量降序
                new SortEntity(){ Key=20, Item=new SortItem(){ SortKey="p_hotsort", SortType=SortOrderType.DESC}},
                //价格升序
                new SortEntity(){ Key=40, Item=new SortItem(){ SortKey="p_totalprice", SortType=SortOrderType.ASC}},
                //价格降序
                new SortEntity(){ Key=50, Item=new SortItem(){ SortKey="p_totalprice", SortType=SortOrderType.DESC}},
                //评论数降序
                new SortEntity(){ Key=60, Item=new SortItem(){ SortKey="p_reviewcount", SortType=SortOrderType.DESC}},
                //上架时间升序
                new SortEntity(){ Key=80, Item=new SortItem(){ SortKey="p_firstonlinetime", SortType=SortOrderType.ASC}},
                //上架时间降序
                new SortEntity(){ Key=90, Item=new SortItem(){ SortKey="p_firstonlinetime", SortType=SortOrderType.DESC}},
                //Solr关键词匹配度降序
                new SortEntity(){Key=9999,Item=new SortItem (){ SortKey="score",SortType=SortOrderType.DESC}},
            };
        }
    }
    public class SortEntity
    {
        public int Key { get; set; }
        public SortItem Item { get; set; }
    }
}

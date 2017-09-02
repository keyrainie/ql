//************************************************************************
// 用户名				泰隆优选
// 系统名				图片单据
// 子系统名		        图片
// 作成者				Tom.H.Li
// 改版日				2012.4.26
// 改版内容				新建
//************************************************************************
using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.Restful
{
    public class ProductResourceRequestMsg
    {
        public List<ProductResourceForNewegg> ProductResources { get; set; }
    }

  
}

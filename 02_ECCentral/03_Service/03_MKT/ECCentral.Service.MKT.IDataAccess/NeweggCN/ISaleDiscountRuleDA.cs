using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    /// <summary>
    /// 销售立减DA
    /// </summary>
    public interface ISaleDiscountRuleDA
    {
        SaleDiscountRule Load(int sysNo);

        void Insert(SaleDiscountRule data);

        void Update(SaleDiscountRule data);

        //限定分类
        bool CheckExistsProductScope_Category(int excludeSysNo, int c3SysNo);

        //限定品牌
        bool CheckExistsProductScope_Brand(int excludeSysNo, int brandSysNo);

        //限定分类+品牌
        bool CheckExistsProductScope_CategoryBrand(int excludeSysNo, int c3SysNo, int brandSysNo);

        //限定商品(包含商品组的概念)
        bool CheckExistsProductScope_Product(int excludeSysNo, params int[] productSysNos);

        //获取所有有效的销售立减规则
        List<SaleDiscountRule> GetAllValid();
    }
}

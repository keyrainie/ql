using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductBrandWarrantyProcessor))]
    public class ProductBrandWarrantyProcessor
    {
        #region Private
        private readonly IProductBrandWarrantyDA brandRequestDA = 
            ObjectFactory<IProductBrandWarrantyDA>.Instance;

        #endregion
        #region Method
        public void BrandWarrantyInfoByAddOrUpdate(ProductBrandWarrantyInfo productBrandWarrantyInfo)
        {
            //查找数据库书否存在记录
            //存在更新操作反之添加操作
            //记录日志
            //如果C1SysNo 为1的话 那所有的 子类下的都需要 添加
            using (var tran = new TransactionScope())
            {
                //找出所有三级类
                int sysNo=0;
                List<Int32> C3SysNos = GetC3SysNos(productBrandWarrantyInfo);
                List<ProductBrandWarrantyInfo> _brandWarrantyInfoAll = GetBrandWarrantyInfoByAll();
                foreach (int C3SysNo in C3SysNos)
                {
                    productBrandWarrantyInfo.C3SysNo = C3SysNo;
                    ProductBrandWarrantyInfo _newproductBrandWarrantyInfo = new ProductBrandWarrantyInfo();
                    ProductBrandWarrantyInfo _productBrandWarrantyInfo = _brandWarrantyInfoAll
                        .Where(p => p.C3SysNo.Equals(productBrandWarrantyInfo.C3SysNo))
                        .Where(p => p.BrandSysNo.Equals(productBrandWarrantyInfo.BrandSysNo)).FirstOrDefault();
                    if (_productBrandWarrantyInfo == null)
                        sysNo = brandRequestDA.InsertBrandWarrantyInfo(
                            productBrandWarrantyInfo);
                    else
                        brandRequestDA.UpdateBrandWarrantyInfoByBrandSysNoAndC3SysNo(productBrandWarrantyInfo);

                    string LogNote = _productBrandWarrantyInfo == null
                        ? String.Format(
                        "用户名：{0} 添加操作 BrandSysNo:{1} C1SysNo:{2} C2SysNo:{3} C3SysNo:{4} WarrantyDay:{5} WarrantyDesc{6}"
                        , productBrandWarrantyInfo.EditUser.UserDisplayName, productBrandWarrantyInfo.BrandSysNo
                        , productBrandWarrantyInfo.C1SysNo, productBrandWarrantyInfo.C2SysNo
                        , productBrandWarrantyInfo.C3SysNo, productBrandWarrantyInfo.WarrantyDay
                        , productBrandWarrantyInfo.WarrantyDesc)
                       : String.Format(
                        @"用户名：{0} 更新操作 原数据：BrandSysNo:{1} C1SysNo:{2} C2SysNo:{3} C3SysNo:{4} WarrantyDay:{5} WarrantyDesc{6} 
                    替换为 BrandSysNo:{7} C1SysNo:{8} C2SysNo:{9} C3SysNo:{10} WarrantyDay:{11} WarrantyDesc{12} "
                       , productBrandWarrantyInfo.EditUser.UserDisplayName, _productBrandWarrantyInfo.BrandSysNo
                       , _productBrandWarrantyInfo.C1SysNo, _productBrandWarrantyInfo.C2SysNo
                       , _productBrandWarrantyInfo.C3SysNo, _productBrandWarrantyInfo.WarrantyDay
                       , _productBrandWarrantyInfo.WarrantyDesc
                       , productBrandWarrantyInfo.BrandSysNo, productBrandWarrantyInfo.C1SysNo
                       , productBrandWarrantyInfo.C2SysNo, productBrandWarrantyInfo.C3SysNo
                       , productBrandWarrantyInfo.WarrantyDay, productBrandWarrantyInfo.WarrantyDesc);

                    //记录日志
                    ExternalDomainBroker.CreateOperationLog(LogNote
                        , _productBrandWarrantyInfo == null
                            ? BizLogType.IM_BrandWarranty_Add : BizLogType.IM_BrandWarranty_Edit
                        , _productBrandWarrantyInfo == null
                            ? sysNo : int.Parse(_productBrandWarrantyInfo.SysNo.ToString()), "8601");
                }
                tran.Complete();
            }
        }

        public void UpdateBrandWarrantyInfoBySysNo(ProductBrandWarrantyInfo productBrandWarrantyInfo)
        {
            using (var tran = new TransactionScope())
            {
                List<ProductBrandWarrantyInfo> _brandWarrantyInfoAll = GetBrandWarrantyInfoByAll();
                //更新日期和詳細信息
                foreach (int item in productBrandWarrantyInfo.SysNos)
                {
                    ProductBrandWarrantyInfo _oldProductBrandWarrantyInfo =
                        _brandWarrantyInfoAll.Where(p => p.SysNo == item).FirstOrDefault();
                    brandRequestDA.UpdateBrandWarrantyInfoBySysNo(new ProductBrandWarrantyInfo()
                    {
                        SysNo = item,
                        EditUser = productBrandWarrantyInfo.EditUser,
                        WarrantyDay = productBrandWarrantyInfo.WarrantyDay,
                        WarrantyDesc = productBrandWarrantyInfo.WarrantyDesc
                    });
                    string LogNote =  String.Format(
                        @"用户名：{0} 批量更新操作 原数据：BrandSysNo:{1} C1SysNo:{2} C2SysNo:{3} C3SysNo:{4} WarrantyDay:{5} WarrantyDesc{6} 
                    替换为 BrandSysNo:{7} C1SysNo:{8} C2SysNo:{9} C3SysNo:{10} WarrantyDay:{11} WarrantyDesc{12} "
                       , productBrandWarrantyInfo.EditUser.UserDisplayName, _oldProductBrandWarrantyInfo.BrandSysNo
                       , _oldProductBrandWarrantyInfo.C1SysNo, _oldProductBrandWarrantyInfo.C2SysNo
                       , _oldProductBrandWarrantyInfo.C3SysNo, _oldProductBrandWarrantyInfo.WarrantyDay
                       , _oldProductBrandWarrantyInfo.WarrantyDesc
                       , _oldProductBrandWarrantyInfo.BrandSysNo, _oldProductBrandWarrantyInfo.C1SysNo
                       , _oldProductBrandWarrantyInfo.C2SysNo, _oldProductBrandWarrantyInfo.C3SysNo
                       , productBrandWarrantyInfo.WarrantyDay, productBrandWarrantyInfo.WarrantyDesc);
                    ExternalDomainBroker.CreateOperationLog(LogNote,BizLogType.IM_BrandWarranty_Edit, item, "8601");
                }
                tran.Complete();
            }
        }

        //删除操作
        public void DelBrandWarrantyInfoBySysNos(List<ProductBrandWarrantyInfo> productBrandWarrantyInfos)
        {
            using (var tranDel = new TransactionScope())
            {
                List<String> errorlist = new List<String>();
                productBrandWarrantyInfos.ForEach(p =>
                {

                    ProductBrandWarrantyInfo _ProductBrandWarranty =
                        brandRequestDA.GetAllowDeleteBrandWarranty(
                        int.Parse(p.C3SysNo.ToString()), int.Parse(p.BrandSysNo.ToString()));
                    if (_ProductBrandWarranty != null)
                    {
                        errorlist.Add(String.Format("C3SysNo:{0} 品牌:{1} 有被商品使用，不能删除！"
                     , p.C3SysNo.ToString(), p.BrandSysNo.ToString()));
                    }
                    else
                    {
                        string LogNote =
                            String.Format("用户名：{0} 删除操作 品牌系统编号为{1}", p.EditUser.UserDisplayName, p.SysNo.ToString());
                        brandRequestDA.DelBrandWarrantyInfoBySysNo(int.Parse(p.SysNo.ToString()));
                        ExternalDomainBroker.CreateOperationLog(
                            LogNote, BizLogType.IM_BrandWarranty_Delete
                            , int.Parse(p.SysNo.ToString()), "8601");
                    }

                });
                tranDel.Complete();
                if (errorlist.Count > 0)
                {
                    throw new BizException(string.Format("失败{0}条，\n内容是：{1}"
                        , errorlist.Count, errorlist.Join(";")));
                }
            }
        }


        #endregion
        #region protected
        protected List<ProductBrandWarrantyInfo> GetBrandWarrantyInfoByAll()
        {
            return brandRequestDA.GetBrandWarrantyInfoByAll();
        }
        protected  List<Int32> GetC3SysNos(ProductBrandWarrantyInfo productBrandWarrantyInfo)
        {
            List<Int32> C3SysNos = new List<int>();
            if (productBrandWarrantyInfo.C3SysNo != null)
            {
                //c3
                C3SysNos.Add(int.Parse(productBrandWarrantyInfo.C3SysNo.ToString()));
            }
            else
            {
                if (productBrandWarrantyInfo.C2SysNo != null)
                {
                    //c2
                    brandRequestDA.GetC3SysNo(productBrandWarrantyInfo).ForEach(p =>
                    {
                        C3SysNos.Add(int.Parse(p.C3SysNo.ToString()));
                    });

                }
                else
                {
                    //c1
                    brandRequestDA.GetC3SysNo(productBrandWarrantyInfo).ForEach(p =>
                    {
                        C3SysNos.Add(int.Parse(p.C3SysNo.ToString()));
                    });
                }
            }
            return C3SysNos;
        }
        #endregion
    }
}

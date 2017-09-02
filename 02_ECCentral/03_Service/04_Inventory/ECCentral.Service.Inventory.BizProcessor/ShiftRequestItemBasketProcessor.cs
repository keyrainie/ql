using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.BizEntity;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(ShiftRequestItemBasketProcessor))]
    public class ShiftRequestItemBasketProcessor
    {
        public virtual void BatchCreateShiftBasket(ShiftRequestItemBasket basket)
        {
            foreach (ShiftRequestItemInfo item in basket.ShiftItemInfoList)
            {
                CreateShiftBasket(item);
            }
        }

        public virtual int CreateShiftBasket(ShiftRequestItemInfo item)
        {
            #region [创建之前的Check操作]

            if (item == null || item.ShiftProduct == null || item.ShiftProduct.SysNo < 1 || item.ShiftQuantity < 1 || item.SourceStock == null || item.TargetStock == null)
            {
                throw new BizException("添加移仓篮参数不完整！");
            }
            bool isExist = ObjectFactory<IShiftRequestItemBasketDA>.Instance.IsExistSourceAndTargetStockInBasket(item);
            if (isExist)
            {
                throw new BizException(
                    string.Format("商品{0}已经在移仓篮中存在移出仓[{1}]，移入仓[{2}]的记录，不能重复添加。",
                    item.ShiftProduct.SysNo,
                    item.SourceStock.StockName,
                   item.TargetStock.StockName
                    ));
            }
            //int StockAvailableQtyGroupByProductSysNo =
            //    ShiftBasketDA.GetStockAvailableQtyGroupByProductSysNo(entity);
            if (!item.InStockQuantity.HasValue)
            {
                throw new BizException("可移库存参数为空，添加失败！");
            }

            //int AllowShiftNum = item.InStockQuantity.Value;
            //int NowShiftQtyGroupByProductSysNo =
            //   ObjectFactory<IShiftRequestItemBasketDA>.Instance.GetNowShiftQtyGroupByProductSysNo(item);

            //if (NowShiftQtyGroupByProductSysNo + item.ShiftQuantity > AllowShiftNum)
            //{
            //    int availableShiftNum = AllowShiftNum -
            //    NowShiftQtyGroupByProductSysNo;
            //    if (availableShiftNum < 0)
            //    {
            //        availableShiftNum = 0;
            //    }
            //    throw new BizException(
            //       string.Format("同一商品在同一个移出仓的移仓数量之和必须满足小于等于移出仓可移库存！编号为{0}的商品在移出仓[{1}]可移库存为：{2}，移仓篮中已存在移仓数量：{3}，可移数量为：{4}。",
            //       item.ShiftProduct.SysNo,
            //       item.SourceStock.StockName,
            //       AllowShiftNum,
            //       NowShiftQtyGroupByProductSysNo,
            //       availableShiftNum
            //       ));
            //}

            #endregion

            //创建移仓篮记录 ：
            return ObjectFactory<IShiftRequestItemBasketDA>.Instance.CreateShiftBasket(item);
        }

        public virtual void BatchDeleteShiftBasket(List<ShiftRequestItemInfo> list)
        {
            list.ForEach(x =>
            {
                DeleteShiftBasket(x);
            });
        }

        public virtual int DeleteShiftBasket(ShiftRequestItemInfo item)
        {
            if (item == null || item.SysNo < 1)
            {
                throw new BizException("删除移仓篮参数不完整！");
            }
            int result = ObjectFactory<IShiftRequestItemBasketDA>.Instance.DeleteShiftBasket(item);

            #region write log
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(string.Format("用户\"{0}\"删除了商品编号为\"{1}\"的移仓篮商品", ServiceContext.Current.UserSysNo, item.ShiftProduct.SysNo), BizLogType.St_Shift_Master_Update, item.SysNo.Value, item.CompanyCode);

            #endregion

            return result;
        }

        public virtual void BatchUpdateShiftBasket(List<ShiftRequestItemInfo> itemList)
        {
            itemList.ForEach(x =>
            {
                UpdateShiftBasket(x);
            });
        }

        public virtual int UpdateShiftBasket(ShiftRequestItemInfo item)
        {
            if (item == null || item.SysNo < 1 || item.ShiftQuantity < 1)
            {
                throw new BizException("修改移仓篮参数不完整！");
            }

            //int StockAvailableQtyGroupByProductSysNo = ObjectFactory<IShiftRequestItemBasketDA>.Instance.GetStockAvailableQtyGroupByProductSysNo(item);
            //int NowShiftQtyGroupByProductSysNo = ObjectFactory<IShiftRequestItemBasketDA>.Instance.GetNowShiftQtyGroupByProductSysNo(item);
            //if (NowShiftQtyGroupByProductSysNo + item.ShiftQuantity > StockAvailableQtyGroupByProductSysNo)
            //{
            //    int availableShiftNum = StockAvailableQtyGroupByProductSysNo -
            //    NowShiftQtyGroupByProductSysNo;
            //    if (availableShiftNum < 0)
            //    {
            //        availableShiftNum = 0;
            //    }
            //    throw new BizException(
            //       string.Format("同一商品在同一个移出仓的移仓数量之和必须满足小于等于移出仓可移库存（可用库存+代销库存）！编号为{0}的商品在移出仓[{1}]可移库存为：{2}，移仓篮中已存在移仓数量：{3}，可用数量为：{4}。",
            //       item.ShiftProduct.SysNo,
            //       item.SourceStock.StockName,
            //       StockAvailableQtyGroupByProductSysNo,
            //       NowShiftQtyGroupByProductSysNo,
            //       availableShiftNum
            //       ));
            //}
            int result = ObjectFactory<IShiftRequestItemBasketDA>.Instance.UpdateShiftBasket(item);

            #region write log
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(string.Format("用户\"{0}\"修改了商品编号为\"{1}\"的移仓篮商品的数量为\"{2}\"", ServiceContext.Current.UserSysNo, item.ShiftProduct.SysNo, item.ShiftQuantity), BizLogType.St_Shift_Master_Update, item.SysNo.Value, item.CompanyCode);
            #endregion

            return result;
        }
    }
}

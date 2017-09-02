using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class UnmarketableInventoryQuery : PageBase
    {      
        #region 初始化加载
        public UnmarketableInventoryQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);         
            string getParam = this.Request.Param;
            if (!string.IsNullOrEmpty(getParam))
            {
                int ProductSysNo = Convert.ToInt32(getParam.Trim());             
                new InventoryQueryFacade(this).QueryUnmarketableInventoryInfo(ProductSysNo, (obj,args) => 
                {
                    if (!args.FaultsHandle() && args.Result != null && args.Result.Count>0)
                    {
                        UnmarketabelInventoryInfo unmarketablEntity = new UnmarketabelInventoryInfo();
                        List<UnmarketabelInventoryInfo> unmarketablEntityList = args.Result;

                        foreach (UnmarketabelInventoryInfo item in unmarketablEntityList)
                        {
                            unmarketablEntity.ProductID = item.ProductID;//产品编号
                            unmarketablEntity.ProductSysNo = item.ProductSysNo;//产品系统编号
                            if (item.MAXInStockDays >= 0 && item.MAXInStockDays <= 15)
                            {
                                unmarketablEntity.UnSale0015DaysQty = item.SUMQuantity;
                                unmarketablEntity.UnSale0015DaysPrice = Math.Round(item.SUMPrice, 2).ToString();
                            }
                            else if (item.MAXInStockDays >= 16 && item.MAXInStockDays <= 30)
                            {
                                unmarketablEntity.UnSale1630DaysQty = item.SUMQuantity;
                                unmarketablEntity.UnSale1630DaysPrice = Math.Round(item.SUMPrice, 2).ToString();
                            }
                            else if (item.MAXInStockDays >= 31 && item.MAXInStockDays <= 45)
                            {
                                unmarketablEntity.UnSale3145DaysQty = item.SUMQuantity;
                                unmarketablEntity.UnSale3145DaysPrice = Math.Round(item.SUMPrice, 2).ToString();
                            }
                            else if (item.MAXInStockDays >= 46 && item.MAXInStockDays <= 60)
                            {
                                unmarketablEntity.UnSale4660DaysQty = item.SUMQuantity;
                                unmarketablEntity.UnSale4660DaysPrice = Math.Round(item.SUMPrice, 2).ToString();
                            }
                            else if (item.MAXInStockDays >= 61 && item.MAXInStockDays <= 90)
                            {
                                unmarketablEntity.UnSale61900DaysQty = item.SUMQuantity;
                                unmarketablEntity.UnSale6190DaysPrice = Math.Round(item.SUMPrice,2).ToString();
                            }
                            else if (item.MAXInStockDays >= 91 && item.MAXInStockDays <= 120)
                            {
                                unmarketablEntity.UnSale91120DaysQty = item.SUMQuantity;
                                unmarketablEntity.UnSale91120DaysPrice = Math.Round(item.SUMPrice, 2).ToString();
                            }
                            else if (item.MAXInStockDays >= 121 && item.MAXInStockDays <= 150)
                            {
                                unmarketablEntity.UnSale121150DaysQty = item.SUMQuantity;
                                unmarketablEntity.UnSale121150DaysPrice = Math.Round(item.SUMPrice, 2).ToString();
                            }
                            else if (item.MAXInStockDays >= 151 && item.MAXInStockDays <= 180)
                            {
                                unmarketablEntity.UnSale151180DaysQty = item.SUMQuantity;
                                unmarketablEntity.UnSale151180DaysPrice = Math.Round(item.SUMPrice, 2).ToString();
                            }
                            else if (item.MAXInStockDays >= 181 && item.MAXInStockDays <= 360)
                            {
                                unmarketablEntity.UnSale181360DaysQty = item.SUMQuantity;
                                unmarketablEntity.UnSale181360DaysPrice = Math.Round(item.SUMPrice, 2).ToString();
                            }
                            else if (item.MAXInStockDays >= 361 && item.MAXInStockDays <= 720)
                            {
                                unmarketablEntity.UnSale361720DaysQty = item.SUMQuantity;
                                unmarketablEntity.UnSale361720DaysPrice = Math.Round(item.SUMPrice, 2).ToString();
                            }
                            else if (item.MAXInStockDays >= 721)
                            {
                                unmarketablEntity.UnSaleUP720DaysQty = item.SUMQuantity;
                                unmarketablEntity.UnSaleUP720DaysPrice = Math.Round(item.SUMPrice, 2).ToString();
                            }
                        }
                        //获取0-30天 数据
                        if (unmarketablEntity.UnSale0015DaysQty != null && unmarketablEntity.UnSale1630DaysQty != null)
                        {
                            unmarketablEntity.UnSale0030DaysQty = (Convert.ToInt32(unmarketablEntity.UnSale0015DaysQty) + Convert.ToInt32(unmarketablEntity.UnSale1630DaysQty)).ToString();
                            unmarketablEntity.UnSale0030DaysPrice = Math.Round((Convert.ToDecimal(unmarketablEntity.UnSale0015DaysPrice) + Convert.ToDecimal(unmarketablEntity.UnSale1630DaysPrice)),2).ToString();
                        }
                        else if (unmarketablEntity.UnSale0015DaysQty != null)
                        {
                            unmarketablEntity.UnSale0030DaysQty = Convert.ToInt32(unmarketablEntity.UnSale0015DaysQty).ToString();
                            unmarketablEntity.UnSale0030DaysPrice = Math.Round((Convert.ToDecimal(unmarketablEntity.UnSale0015DaysPrice)),2).ToString();
                        }
                        else if (unmarketablEntity.UnSale1630DaysQty != null)
                        {
                            unmarketablEntity.UnSale0030DaysQty = Convert.ToInt32(unmarketablEntity.UnSale1630DaysQty).ToString();
                            unmarketablEntity.UnSale0030DaysPrice = Math.Round((Convert.ToDecimal(unmarketablEntity.UnSale1630DaysPrice)),2).ToString();
                        }

                        //获取31-60天 数据
                        if (unmarketablEntity.UnSale3145DaysQty != null && unmarketablEntity.UnSale4660DaysQty != null)
                        {
                            unmarketablEntity.UnSale3160DaysQty = (Convert.ToInt32(unmarketablEntity.UnSale3145DaysQty) + Convert.ToInt32(unmarketablEntity.UnSale4660DaysQty)).ToString();
                            unmarketablEntity.UnSale3160DaysPrice = Math.Round(((Convert.ToDecimal(unmarketablEntity.UnSale3145DaysPrice) + Convert.ToDecimal(unmarketablEntity.UnSale4660DaysPrice))),2).ToString();
                        }
                        else if (unmarketablEntity.UnSale3145DaysQty != null)
                        {
                            unmarketablEntity.UnSale3160DaysQty = Convert.ToInt32(unmarketablEntity.UnSale3145DaysQty).ToString();
                            unmarketablEntity.UnSale3160DaysPrice = Math.Round((Convert.ToDecimal(unmarketablEntity.UnSale3145DaysPrice)),2).ToString();
                        }
                        else if (unmarketablEntity.UnSale4660DaysQty != null)
                        {
                            unmarketablEntity.UnSale3160DaysQty = Convert.ToInt32(unmarketablEntity.UnSale4660DaysQty).ToString();
                            unmarketablEntity.UnSale3160DaysPrice = Math.Round((Convert.ToDecimal(unmarketablEntity.UnSale4660DaysPrice)),2).ToString();
                        }

                        //获取121-180天 数据
                        if (unmarketablEntity.UnSale121150DaysQty != null && unmarketablEntity.UnSale151180DaysQty != null)
                        {
                            unmarketablEntity.UnSale121180DaysQty = (Convert.ToInt32(unmarketablEntity.UnSale121150DaysQty) + Convert.ToInt32(unmarketablEntity.UnSale151180DaysQty)).ToString();
                            unmarketablEntity.UnSale121180DaysPrice = Math.Round(((Convert.ToDecimal(unmarketablEntity.UnSale121150DaysPrice) + Convert.ToDecimal(unmarketablEntity.UnSale151180DaysPrice))),2).ToString();
                        }
                        else if (unmarketablEntity.UnSale121150DaysQty != null)
                        {
                            unmarketablEntity.UnSale121180DaysQty = Convert.ToInt32(unmarketablEntity.UnSale121150DaysQty).ToString();
                            unmarketablEntity.UnSale121180DaysPrice =Math.Round((Convert.ToDecimal(unmarketablEntity.UnSale121150DaysPrice)),2).ToString();
                        }
                        else if (unmarketablEntity.UnSale151180DaysQty != null)
                        {
                            unmarketablEntity.UnSale121180DaysQty = Convert.ToInt32(unmarketablEntity.UnSale151180DaysQty).ToString();
                            unmarketablEntity.UnSale121180DaysPrice = Math.Round((Convert.ToDecimal(unmarketablEntity.UnSale151180DaysPrice)), 2).ToString();
                        }


                        //获取 超过180 天 数据
                        if (unmarketablEntity.UnSale181360DaysQty != null && unmarketablEntity.UnSale361720DaysQty != null && unmarketablEntity.UnSaleUP720DaysQty != null)
                        {
                            unmarketablEntity.UnSaleUP180DaysQty = (Convert.ToInt32(unmarketablEntity.UnSale181360DaysQty) + Convert.ToInt32(unmarketablEntity.UnSale361720DaysQty) + Convert.ToInt32(unmarketablEntity.UnSaleUP720DaysQty)).ToString();
                            unmarketablEntity.UnSaleUP180DaysPrice = Math.Round(((Convert.ToDecimal(unmarketablEntity.UnSale181360DaysPrice) + Convert.ToDecimal(unmarketablEntity.UnSale361720DaysPrice) + Convert.ToDecimal(unmarketablEntity.UnSaleUP720DaysPrice))),2).ToString();
                        }
                        else if (unmarketablEntity.UnSale181360DaysQty != null && unmarketablEntity.UnSale361720DaysQty != null)
                        {
                            unmarketablEntity.UnSaleUP180DaysQty = (Convert.ToInt32(unmarketablEntity.UnSale181360DaysQty) + Convert.ToInt32(unmarketablEntity.UnSale361720DaysQty)).ToString();
                            unmarketablEntity.UnSaleUP180DaysPrice = Math.Round(((Convert.ToDecimal(unmarketablEntity.UnSale181360DaysPrice) + Convert.ToDecimal(unmarketablEntity.UnSale361720DaysPrice))),2).ToString();
                        }
                        else if (unmarketablEntity.UnSale181360DaysQty != null && unmarketablEntity.UnSaleUP720DaysQty != null)
                        {
                            unmarketablEntity.UnSaleUP180DaysQty = (Convert.ToInt32(unmarketablEntity.UnSale181360DaysQty) + Convert.ToInt32(unmarketablEntity.UnSaleUP720DaysQty)).ToString();
                            unmarketablEntity.UnSaleUP180DaysPrice = Math.Round(((Convert.ToDecimal(unmarketablEntity.UnSale181360DaysPrice) + Convert.ToDecimal(unmarketablEntity.UnSaleUP720DaysPrice))),2).ToString();
                        }
                        else if (unmarketablEntity.UnSale361720DaysQty != null && unmarketablEntity.UnSaleUP720DaysQty != null)
                        {
                            unmarketablEntity.UnSaleUP180DaysQty = (Convert.ToInt32(unmarketablEntity.UnSale361720DaysQty) + Convert.ToInt32(unmarketablEntity.UnSaleUP720DaysQty)).ToString();
                            unmarketablEntity.UnSaleUP180DaysPrice = Math.Round(((Convert.ToDecimal(unmarketablEntity.UnSale361720DaysPrice) + Convert.ToDecimal(unmarketablEntity.UnSaleUP720DaysPrice))),2).ToString();
                        }
                        else if (unmarketablEntity.UnSale181360DaysQty != null)
                        {
                            unmarketablEntity.UnSaleUP180DaysQty = Convert.ToInt32(unmarketablEntity.UnSale181360DaysQty).ToString();
                            unmarketablEntity.UnSaleUP180DaysPrice = Math.Round((Convert.ToDecimal(unmarketablEntity.UnSale181360DaysPrice)),2).ToString();
                        }
                        else if (unmarketablEntity.UnSale361720DaysQty != null)
                        {
                            unmarketablEntity.UnSaleUP180DaysQty = Convert.ToInt32(unmarketablEntity.UnSale361720DaysQty).ToString();
                            unmarketablEntity.UnSaleUP180DaysPrice = Math.Round((Convert.ToDecimal(unmarketablEntity.UnSale361720DaysPrice)),2).ToString();
                        }
                        else if (unmarketablEntity.UnSaleUP720DaysQty != null)
                        {
                            unmarketablEntity.UnSaleUP180DaysQty = Convert.ToInt32(unmarketablEntity.UnSaleUP720DaysQty).ToString();
                            unmarketablEntity.UnSaleUP180DaysPrice = Math.Round((Convert.ToDecimal(unmarketablEntity.UnSaleUP720DaysPrice)),2).ToString();
                        }
                         unmarketablEntityList = new List<UnmarketabelInventoryInfo>();
                         unmarketablEntityList.Add(unmarketablEntity);
                         dgUnmarketableInventoryQueryResult.ItemsSource = unmarketablEntityList;
                   }
                });
            }
            else
            {
                dgUnmarketableInventoryQueryResult.ItemsSource = null;
            }
        }

        #endregion
   
    }
}

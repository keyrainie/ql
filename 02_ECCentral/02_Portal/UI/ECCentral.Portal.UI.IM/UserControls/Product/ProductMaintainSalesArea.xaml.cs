using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Views;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.Basic.Components.UserControls.AreaPicker;
using System.Collections.Generic;
using ECCentral.Portal.Basic;


namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainSalesArea : UserControl,ISave
    {
        public ProductMaintainSalesAreaVM VM
        {
            get { return DataContext as ProductMaintainSalesAreaVM; }
        }

        private AreaQueryFacade m_Facade = null;
        private AreaQueryFacade Facade
        {
            get
            {
                if (m_Facade == null)
                {
                    m_Facade = new AreaQueryFacade();
                }
                return m_Facade;
            }
        }

        private ProductFacade m_productFacade = null;
        private ProductFacade ProductFacade
        {
            get
            {
                if (m_productFacade == null)
                {
                    m_productFacade = new ProductFacade();
                }
                return m_productFacade;
            }
        }

        public ProductMaintainSalesArea()
        {
            InitializeComponent();
        }

        private void HyperlinkEditClick(object sender, RoutedEventArgs e)
        {
            var currentVM = dgProductSalesAreaList.SelectedItem as ProductMaintainSalesAreaSelectVM;
            if (VM.ProductMaintainSalesAreaSaveList.Any(salesArea => salesArea.Equals(currentVM)))
            {
                VM.ProductMaintainSalesAreaSaveList.Remove(currentVM);
            }
            dgProductSalesAreaList.ItemsSource = VM.ProductMaintainSalesAreaSaveList;
        }

        private void BtnAddClick(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemSalesArea))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("你无此操作权限");
                return;
            }

            if (VM.ProductMaintainSalesAreaSelect.Province.ProvinceSysNo == 0
                //|| VM.ProductMaintainSalesAreaSelect.City.CitySysNo == 0
                || !VM.ProductMaintainSalesAreaSelect.StockSysNo.HasValue)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("省和仓库都为必选项", MessageType.Error);
                return;
            }
            if (VM.ProductMaintainSalesAreaSelect.City.CitySysNo == 0)
            {
                List<CityVM> cityList= (List<CityVM>)this.cmbCityList.ItemsSource;
                foreach(var item in cityList)
                {
                    if(item.CitySysNo==0)
                    {
                        continue;
                    }
                    var selected = new ProductMaintainSalesAreaSelectVM
                    {
                        StockSysNo = VM.ProductMaintainSalesAreaSelect.StockSysNo,
                        StockName = VM.ProductMaintainSalesAreaSelect.StockName,
                        Province =
                            new ProvinceVM
                            {
                                ProvinceSysNo = VM.ProductMaintainSalesAreaSelect.Province.ProvinceSysNo,
                                ProvinceName = VM.ProductMaintainSalesAreaSelect.Province.ProvinceName
                            },
                        City =
                            new CityVM
                            {
                                CitySysNo = item.CitySysNo,
                                CityName = item.CityName
                            }
                    };
                    if (!VM.ProductMaintainSalesAreaSaveList.Any(salesArea => salesArea.Equals(selected)))
                    {
                        VM.ProductMaintainSalesAreaSaveList.Add(selected);
                        dgProductSalesAreaList.ItemsSource = VM.ProductMaintainSalesAreaSaveList;
                    }
                }
            }
            else
            {
                var selected = new ProductMaintainSalesAreaSelectVM
                {
                    StockSysNo = VM.ProductMaintainSalesAreaSelect.StockSysNo,
                    StockName = VM.ProductMaintainSalesAreaSelect.StockName,
                    Province =
                        new ProvinceVM
                        {
                            ProvinceSysNo = VM.ProductMaintainSalesAreaSelect.Province.ProvinceSysNo,
                            ProvinceName = VM.ProductMaintainSalesAreaSelect.Province.ProvinceName
                        },
                    City =
                        new CityVM
                        {
                            CitySysNo = VM.ProductMaintainSalesAreaSelect.City.CitySysNo,
                            CityName =  VM.ProductMaintainSalesAreaSelect.City.CityName
                        }
                };
                if (!VM.ProductMaintainSalesAreaSaveList.Any(salesArea => salesArea.Equals(selected)))
                {
                    VM.ProductMaintainSalesAreaSaveList.Add(selected);
                    dgProductSalesAreaList.ItemsSource = VM.ProductMaintainSalesAreaSaveList;
                }
            }
        }

        public void Save()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().UpdateProductSalesAreaInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("销售区域信息更新成功", MessageBoxType.Success);
                    });
        }

        private void cmbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<CityVM> cityList = new List<CityVM>();

            if (cmbProvinceList.SelectedIndex > 0)
            {
                int provinceSysNumber = System.Convert.ToInt32(cmbProvinceList.SelectedValue);
                Facade.QueryCityAreaListByProvinceSysNo(provinceSysNumber, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    cityList = ProductFacade.ConvertAreaInfoEntityToCityVM(args.Result);
                    cityList.Insert(0, new CityVM { CitySysNo = 0, CityName = "请选择..." });
                    this.cmbCityList.ItemsSource = cityList;
                    this.cmbCityList.SelectedValue = 0;
                });
            }
            else
            {
                cityList = new List<CityVM>();
                cityList.Insert(0, new CityVM { CitySysNo = 0, CityName = "请选择..." });
                cmbCityList.ItemsSource = cityList;
                cmbCityList.SelectedIndex = 0;
            }
        }
    }
}

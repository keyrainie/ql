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
using ECCentral.Portal.Basic.Components.UserControls.AreaPicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.Portal.UI.Common.Facades;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Common.UserControls
{
    public partial class BatchCreateShipTypeAreaPrice : UserControl
    {
        public ShipTypeAreaUnInfoVM _view;
        public ShipTypeFacade _facade;
        List<AreaQueryVM> _areaViewList;
        public IDialog DiolgHander { get; set; }
        public IPage Page { get; set; }
        public ShipTypeAreaPriceInfoVM _priceview;
        public ShipTypeAreaPriceInfo _entity;

        public BatchCreateShipTypeAreaPrice()
        {
            InitializeComponent();
            _priceview = new ShipTypeAreaPriceInfoVM();
            _view = new ShipTypeAreaUnInfoVM();
            _areaViewList = new List<AreaQueryVM>();
            Loaded += new RoutedEventHandler(BatchCreateShipTypeAreaPrice_Loaded);
        }


        public void BatchCreateShipTypeAreaPrice_Loaded(object sender, RoutedEventArgs e)
        {
            _facade = new ShipTypeFacade(CPApplication.Current.CurrentPage);
            BindComboBoxData();
            LayoutRoot.DataContext = _priceview;
        }

        private void BindComboBoxData()
        {
            //商户:
            this.Merchant.ItemsSource = EnumConverter.GetKeyValuePairs<CompanyCustomer>(EnumConverter.EnumAppendItemType.All);
            this.Merchant.SelectedIndex = 0;
        }


        private void AddNew_Click(object sender, RoutedEventArgs e)
        {

            if (ValidationManager.Validate(LayoutRoot))
            {
                //这里默认给VerdorSysNo为1
                _priceview.VendorSysNo = 1;
                if (!_priceview.SysNo.HasValue)
                {
                    if (_priceview.AreaSysNoList == null || _priceview.AreaSysNoList.Count == 0)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("请选择区域");
                        return;
 
                    }
                    _facade.CreateShipTypeAreaPrice(_priceview, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        CPApplication.Current.CurrentPage.Context.Window.Alert("操作已成功");
                        CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
                    });

                }
                else
                {
                    if (_priceview.AreaSysNoList == null || _priceview.AreaSysNoList.Count > 0)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("请选择区域");
                        return;

                    }
                    _facade.UpdateShipTypeAreaPrice(_priceview, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;

                        }
                        CPApplication.Current.CurrentPage.Context.Window.Alert("操作已成功");
                        CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
                    });
                }
            } 
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _priceview = new ShipTypeAreaPriceInfoVM();
            LayoutRoot.DataContext = _priceview;
        }

        private void Message(string msg)
        {
            Page.Context.Window.Alert(msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
        }

        private void SelectArea_Click(object sender, RoutedEventArgs e)
        {
          
            List<int?> list = new List<int?>();
            UCAreaQuery queryArea = new UCAreaQuery();
            queryArea.DialogHandler = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("地区查询", queryArea, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    _areaViewList = args.Data as List<AreaQueryVM>;
                    if (_areaViewList != null & _areaViewList.Count > 0)
                    {
                      
                        foreach (var _areaView in _areaViewList)
                        {
                            if (!string.IsNullOrEmpty(_areaView.CityName))
                            {
                                list.Add(_areaView.SysNo);
                            }
                        }
                    }
                    _view.AreaSysNoList = list;
                    _priceview.AreaSysNoList = list;
                    QueryResult.ItemsSource = _areaViewList;
                    QueryResult.TotalCount = _areaViewList.Count;
                    QueryResult.Bind();
                }

            }, new Size(700, 420));
        }


        private void Hyperlink_Operate_Click(object sender, RoutedEventArgs e)
        {
            dynamic row = QueryResult.SelectedItem;
            if (row != null)
            {
                var sysno = row.SysNo;
                _areaViewList = _areaViewList.Where(f => f.SysNo != sysno).ToList();
                QueryResult.ItemsSource = _areaViewList;
                QueryResult.Bind();
            }
        }

        private void CloseDialog(ResultEventArgs args)
        {
            if (DiolgHander != null)
            {
                DiolgHander.ResultArgs = args;
                DiolgHander.Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (DiolgHander != null)
            {
                DiolgHander.ResultArgs.Data = null;
                DiolgHander.ResultArgs.DialogResult = DialogResultType.Cancel;
                DiolgHander.Close();
            }
        }
    }
}

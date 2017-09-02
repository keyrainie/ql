using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Common.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Common.Resources;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.Basic.Components.UserControls.CategoryPicker;
using ECCentral.Portal.Basic.Components.UserControls.AreaPicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;



namespace ECCentral.Portal.UI.Common.Views
{
    [View]
    public partial class ShipTypeAreaUnAddNew : PageBase
    {
        public ShipTypeAreaUnInfoVM _view;
        public ShipTypeFacade _facade;
        List<AreaQueryVM> _areaViewList;
        string Msg_HasNoRight = "您没有此功能的操作权限！";

        public ShipTypeAreaUnAddNew()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _view = new ShipTypeAreaUnInfoVM();
            _facade = new ShipTypeFacade(this);
            Main_region.DataContext = _view;
        }

        private void SelectArea_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ShipTypeAreaUnAddNew_ChooseArea))
            {
                Window.Alert(Msg_HasNoRight);
                return;
            }
            List<int?> list = new List<int?>();
            UCAreaQuery queryArea = new UCAreaQuery();
            queryArea.DialogHandler = Window.ShowDialog("地区查询", queryArea, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    _areaViewList = args.Data as List<AreaQueryVM>;
                    if (_areaViewList != null & _areaViewList.Count > 0)
                    {

                        foreach (var _areaView in _areaViewList)
                        {

                            list.Add(_areaView.SysNo);
                        }
                    }
                    _view.AreaSysNoList = list;
                    QueryResult.ItemsSource = _areaViewList;
                    QueryResult.TotalCount = _areaViewList.Count;
                    QueryResult.Bind();
                }

            }, new Size(700, 420));
        }

        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ShipTypeAreaUnAddNew_Add))
            {
                Window.Alert(Msg_HasNoRight);
                return;
            }
            ErroDetail _erro;
            if (ValidationManager.Validate(this.Main_region))
            {
                if (_areaViewList == null || _areaViewList.Count == 0)
                {
                    Window.Alert("请选择地区!");
                    return;
                }

                _view.CompanyCode = CPApplication.Current.CompanyCode;
                _facade.CreateShipTypeAreaUn(_view, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;

                        }
                        _erro = args.Result;
                        if (_erro.ErroList != null && _erro.ErroList.Count > 0)
                        {
                            int countFailed = _erro.ErroList.Count;
                            int countSucceed = _erro.SucceedList.Count;
                            int i = 0;
                            StringBuilder strbuiler = new StringBuilder();
                            strbuiler.AppendFormat("成功保存 {0} 条数据，失败 {1} 条数据,详细信息如下：\r\n\n", countSucceed.ToString(), countFailed.ToString());
                            foreach (var item in _erro.ErroList)
                            {
                                i++;
                                strbuiler.AppendFormat("{0}、地区\"{1}\",已存在配送方式\"{2}\"", i, item.AreaName, item.ShipTypeName);
                            }
                            Window.Alert(strbuiler.ToString(), MessageType.Information);
                            return;
                        }
                        Window.Alert("添加成功！");
                    });
            }
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

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _view = new ShipTypeAreaUnInfoVM();
            Main_region.DataContext = _view;
            _areaViewList = new List<AreaQueryVM>();
            QueryResult.ItemsSource = _areaViewList;
            QueryResult.Bind();
        }
    }
}

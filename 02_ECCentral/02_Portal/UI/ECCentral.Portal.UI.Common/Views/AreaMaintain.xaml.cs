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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Common.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.Common.Views
{
    [View]
    public partial class AreaMaintain : PageBase
    {
        private string _op;
        private int? _sysNo;
        AreaInfoVM  _areaInfoViewModel;
        AreaFacade _areaFace;
        public AreaMaintain()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            List<KeyValuePair<AreaStatus?, string>> statusList = EnumConverter.GetKeyValuePairs<AreaStatus>();
            statusList.Insert(0,new KeyValuePair<AreaStatus?,string>(null,ResCommonEnum.Enum_Select));
            lisStatus.ItemsSource = statusList;
            List<KeyValuePair<AreaIsLocal?, string>> isLocalList = EnumConverter.GetKeyValuePairs<AreaIsLocal>();
            isLocalList.Insert(0, new KeyValuePair<AreaIsLocal?, string>(null, ResCommonEnum.Enum_Select));
            lisIsLocal.ItemsSource = isLocalList;
            _areaInfoViewModel = new AreaInfoVM();
            Area_Feild.DataContext = _areaInfoViewModel;
            _areaFace = new AreaFacade(this);
            if (this.Request.QueryString.ContainsKey("operation"))
            {
                _op =this.Request.QueryString["operation"];
            }
            if (this.Request.QueryString.ContainsKey("sysno"))
            {
                _sysNo = int.Parse(this.Request.QueryString["sysno"]);
                _areaFace.Load(_sysNo, (s, args) =>
                    {
                        _areaInfoViewModel = args.Result;
                        Area_Feild.DataContext = _areaInfoViewModel;
                    });
            }
            InitControl(_sysNo);
        }

        private void Save_New_Click(object sender, RoutedEventArgs e)
        {
             ValidationManager.Validate(Area_Feild);
            if (_areaInfoViewModel.HasValidationErrors) return;
            if (!string.IsNullOrEmpty(_op) && !string.IsNullOrEmpty(_sysNo.ToString()))
            {
                _areaInfoViewModel.SysNo = _sysNo;
                _areaFace.Update(_areaInfoViewModel, (s, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        
                        this.Window.Alert("更新地区成功!");
                        _areaInfoViewModel = args.Result;
                        InitControl(_sysNo);
                        Area_Feild.DataContext = _areaInfoViewModel;
                       
                    });
            }
            else
            {
                
                _areaFace.Create(_areaInfoViewModel, (s, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        _areaInfoViewModel = args.Result;
                        this._op = "edit";
                        this._sysNo = args.Result.SysNo;
                        Area_Feild.DataContext = _areaInfoViewModel;                        
                        this.Window.Alert("添加地区成功");
                        InitControl(_sysNo);
                        
                    });
            }
            
        }

        #region UI方法
        private void InitControl(int? sysNo)
        {
            if (!string.IsNullOrEmpty(sysNo.ToString()))
            {
                Area_FullName.Visibility = Visibility.Visible;
                AreaPicker.Visibility = Visibility.Collapsed;
            }
            else
            {
                Area_FullName.Visibility = Visibility.Collapsed;
                AreaPicker.Visibility = Visibility.Visible;
            }

        }
        #endregion
    }
}

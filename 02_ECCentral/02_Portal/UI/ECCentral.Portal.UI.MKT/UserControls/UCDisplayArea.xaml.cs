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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCDisplayArea : UserControl
    {
        public UCDisplayArea()
        {
            InitializeComponent();

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetAreaList();
        }
        public string SelectedArea
        {

            set { SetValue(SelectedAreaProperty, value); }
            get
            {
                return (string)GetValue(SelectedAreaProperty);
            }
        }
        public static readonly DependencyProperty SelectedAreaProperty =
      DependencyProperty.Register("SelectedArea", typeof(string), typeof(UCDisplayArea), new PropertyMetadata((obj, args) =>
      {
             UCDisplayArea uc = obj as UCDisplayArea;
           //update Bug94506 2012-12-21 
          // uc.SetAreaList();
      }));

        public string ChannelID
        {
            get { return (string)GetValue(ChannelIDProperty); }
            set { SetValue(ChannelIDProperty, value); }
        }
        public static readonly DependencyProperty ChannelIDProperty =
      DependencyProperty.Register("ChannelID", typeof(string), typeof(UCDisplayArea), new PropertyMetadata((obj, args) =>
      {
          UCDisplayArea uc = obj as UCDisplayArea;
          uc.SetAreaList();
      }));

        public event EventHandler<EventArgs> LoadAreaCompleted;


        public void SetAreaList()
        {
            if (!string.IsNullOrEmpty(this.ChannelID))
            {
                string key = string.Format("Area-{0}-{1}-NewsAdv", this.CompanyCode, this.ChannelID);
                CodeNamePairHelper.GetList("MKT", key, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    List<CheckBoxVM> list = new List<CheckBoxVM>();
                    args.Result.ForEach(item =>
                    {
                        CheckBoxVM cbvm = new CheckBoxVM();
                        cbvm.Name = item.Name;
                        cbvm.SysNo = int.Parse(item.Code);

                        if (!string.IsNullOrEmpty(SelectedArea))
                        {
                            cbvm.IsChecked = SelectedArea.Split(',').Where(p => p.ToString() == item.Code).Count() > 0;
                        }

                        cbvm.onSelectedChange = new OnSelectedChange(SetSelectedArea);
                        list.Add(cbvm);
                    });

                    areaList.ItemsSource = list;

                    var handler = LoadAreaCompleted;
                    if (handler != null)
                    {                      
                        handler(this, args);
                    }

                });


            }
        }
        public void SetSelectedArea()
        {
            string str = string.Empty;
            (areaList.ItemsSource as List<CheckBoxVM>).ForEach(item =>
            {
                if (item.IsChecked)
                    str += item.SysNo + ",";
            });
            SelectedArea = str.TrimEnd(',');
        }
        public string CompanyCode
        {
            get { return CPApplication.Current.CompanyCode; }
        }

        /// <summary>
        /// 选中所有的区域
        /// </summary>
        public void SetAllAreaSelected()
        {
            var checkBoxVMList = areaList.ItemsSource as List<CheckBoxVM>;
            if (checkBoxVMList != null)
            {
                checkBoxVMList.ForEach(item =>
                {
                    item.IsChecked = true;
                });
            }
        }
    }

    public delegate void OnSelectedChange();

    public class CheckBoxVM : ModelBase
    {
        public OnSelectedChange onSelectedChange;

        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                base.SetValue("IsChecked", ref isChecked, value);
                if (onSelectedChange != null)
                    onSelectedChange();
            }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { base.SetValue("Name", ref _name, value); }
        }
        private int _SysNo;
        public int SysNo
        {
            get { return _SysNo; }
            set { base.SetValue("SysNo", ref _SysNo, value); }
        }
    }

}


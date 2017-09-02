using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System;


namespace ECCentral.Portal.Basic.Components.UserControls.AreaPicker
{
    public partial class UCAreaPicker : UserControl
    {
        private bool needChange = true;
        private List<KeyValuePair<string, string>> source;

        private AreaPickerVM m_dataContext;
        private AreaPickerVM ViewModel
        {
            get
            {
                if (m_dataContext == null)
                {
                    m_dataContext = new AreaPickerVM();
                }
                return m_dataContext;
            }
            set
            {
                m_dataContext = value;
            }
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

        /// <summary>
        /// 请选择 数据源
        /// </summary>
        private List<KeyValuePair<string, string>> SingleSource
        {
            get
            {
                if (source == null)
                {
                    source = new List<KeyValuePair<string, string>>();
                    source.Insert(0, new KeyValuePair<string, string>(null, ResLinkableDataPicker.ComboBox_PleaseSelect));
                }
                return source;
            }
        }

        /// <summary>
        /// 选择的地区编号
        /// </summary>
        public string SelectedAreaSysNo
        {
            get
            {
                return (string)GetValue(SelectedAreaSysNoProperty);
            }
            set
            {
                SetValue(SelectedAreaSysNoProperty, value);
            }
        }
        public string SelectedProvinceSysNo
        {
            get
            {
                return (string)GetValue(SelectedProvinceSysNoProperty);
            }
            set
            {
               SetValue(SelectedProvinceSysNoProperty, value);
            }
        }
        public string SelectedCitySysNo
        {
            get
            {
                return (string)GetValue(SelectedCitySysNoProperty);
            }
            set
            {
                SetValue(SelectedCitySysNoProperty ,value);
            }
        }
        //public string SelectedDistrictSysNo
        //{
        //    get
        //    {
        //        return (string)GetValue(SelectedDistrictSysNoProperty);
        //    }
        //    set
        //    {
        //        SetValue(SelectedDistrictSysNoProperty, value);
        //    }
        //}
        public string SelectedCityName
        {
            get
            {
                return (string)GetValue(SelectedCityNameProperty);
            }
            set
            {
                SetValue(SelectedCityNameProperty, value);
            }
        }
        public string SelectedProvinceName
        {
            get
            {
                return (string)GetValue(SelectedProvinceNameProperty);
            }
            set
            {
                SetValue(SelectedProvinceNameProperty, value);
            }
        }
        public int? QueryAreaSysNo
        {
            get 
            {
                int? result = default(int?);
                //查询地区
                //if (cmbArea.SelectedValue != null)
                //{
                //    result = Convert.ToInt32(cmbArea.SelectedValue);
                //}
                //else
                //{ 
                    //查询城市
                    if (cmbCity.SelectedValue != null)
                    {
                        result = Convert.ToInt32(cmbCity.SelectedValue);
                    }
                    else
                    {
                        //查询省
                        if (cmbProvince.SelectedValue != null)
                        {
                            result = Convert.ToInt32(cmbProvince.SelectedValue);
                        }
                    }
               // }
                return result;
            }
        }

        public static readonly DependencyProperty SelectedAreaSysNoProperty =
            DependencyProperty.Register("SelectedAreaSysNo", typeof(string), typeof(UCAreaPicker), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedProvinceSysNoProperty =
            DependencyProperty.Register("SelectedProvinceSysNo", typeof(string), typeof(UCAreaPicker), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedCitySysNoProperty =
            DependencyProperty.Register("SelectedCitySysNo", typeof(string), typeof(UCAreaPicker), new PropertyMetadata(null));
        //public static readonly DependencyProperty SelectedDistrictSysNoProperty =
        //    DependencyProperty.Register("SelectedDistrictSysNo", typeof(string), typeof(UCAreaPicker), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedCityNameProperty =
            DependencyProperty.Register("SelectedCityName", typeof(string), typeof(UCAreaPicker), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedProvinceNameProperty =
            DependencyProperty.Register("SelectedProvinceName", typeof(string), typeof(UCAreaPicker), new PropertyMetadata(null));


        //private static void SelectedAreaSysNoPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var control = d as AreaPicker;
        //    control.BindData();
        //}

        public UCAreaPicker()
        {
            InitializeComponent();
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(AreaPicker_DataContextChanged);
            Loaded += new RoutedEventHandler(AreaPicker_Loaded);
        }

        private void AreaPicker_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {           
            if (this.DataContext != null)
            {
                BindData();
            }
        }

        private void AreaPicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(AreaPicker_Loaded);

            var AreaExp = this.GetBindingExpression(UCAreaPicker.SelectedAreaSysNoProperty);
            var ProvinceExp = this.GetBindingExpression(UCAreaPicker.SelectedProvinceSysNoProperty);
            var CityExp = this.GetBindingExpression(UCAreaPicker.SelectedCitySysNoProperty);

            //if (AreaExp != null && AreaExp.ParentBinding != null)
            //{
            //    cmbArea.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, AreaExp.ParentBinding);
            //}
            if(ProvinceExp!= null&&ProvinceExp.ParentBinding!=null)
            {
                cmbProvince.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, ProvinceExp.ParentBinding);
            }
            if(CityExp!=null&&CityExp.ParentBinding!=null)

            {
                cmbCity.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, CityExp.ParentBinding);
            }

            //InitData();
        }

        private void BindData()
        {
            string query = string.IsNullOrEmpty(this.SelectedAreaSysNo) ? "-999" : this.SelectedAreaSysNo;
            Facade.QueryCurrentAreaStructure(System.Convert.ToInt32(query), (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                ViewModel = AreaQueryTransform.Transform(args.Result);

                string areaSysNo = null;
                if (!string.IsNullOrEmpty(this.SelectedAreaSysNo))
                {
                    areaSysNo = this.SelectedAreaSysNo;
                }

                this.needChange = false;

                cmbProvince.ItemsSource = ViewModel.ProvinceAreaList;
                cmbCity.ItemsSource = ViewModel.CityeAreaList;
                //在给ComboBox的数据源赋值后，会触发SelectionChanged时间，导致SelectedAreaSysNo为null，所以需要把SelectedAreaSysNo先存起来
                //cmbArea.ItemsSource = ViewModel.DistrictAreaList;

                this.needChange = true;

                if (!string.IsNullOrEmpty(areaSysNo))
                {
                    this.SelectedAreaSysNo = areaSysNo;
                }

                if (string.IsNullOrEmpty(this.SelectedAreaSysNo))
                {                   
                    cmbProvince.SelectedIndex = 0;
                }
                else
                {                    
                    this.needChange = false;

                    
                    cmbProvince.SelectedValue = ViewModel.CurrentArea.ProvinceSysNumber;
                    cmbCity.SelectedValue = ViewModel.CurrentArea.CitySysNumber;

                    //cmbArea.SelectedValue = ViewModel.CurrentArea.SysNumber;

                    this.needChange = true;
                }
            });
        }

        //private void InitData()
        //{
        //    cmbProvince.ItemsSource = SingleSource;
        //    cmbProvince.SelectedIndex = 0;

        //    cmbCity.ItemsSource = SingleSource;
        //    cmbCity.SelectedIndex = 0;

        //    cmbArea.ItemsSource = SingleSource;
        //    cmbArea.SelectedIndex = 0;
        //}

        private void cmbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.needChange)
            {
                return;
            }
            if (cmbProvince.SelectedIndex > 0)
            {
                int provinceSysNumber = System.Convert.ToInt32(cmbProvince.SelectedValue);
                this.SelectedProvinceSysNo = provinceSysNumber.ToString();
                //this.SelectedProvinceName=  cmbProvince.SelectedItem.Text();
                Facade.QueryCurrentAreaStructure(provinceSysNumber, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    this.needChange = false;
                    ViewModel = AreaQueryTransform.Transform(args.Result);
                    this.SelectedProvinceName = ViewModel.ProvinceAreaList.Where(f => f.Key == provinceSysNumber.ToString()).FirstOrDefault().Value;
                    cmbCity.ItemsSource = ViewModel.CityeAreaList;
                    this.needChange = true;
                    //if (cmbCity.SelectedValue == null)
                    //{
                    //    cmbArea.ItemsSource = this.SingleSource;
                    //    ClearValidationError(cmbArea);
                    //}
                });
            }
            else
            {
                this.needChange = false;
                cmbCity.ItemsSource = SingleSource;
                cmbCity.SelectedIndex = 0;
               // cmbArea.ItemsSource = SingleSource;
               // cmbArea.SelectedIndex = 0;
                this.needChange = true;
            }
        }

        private void cmbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.needChange)
            {
                return;
            }
            if (cmbCity.SelectedIndex > 0)
            {
                int citySysNumber = System.Convert.ToInt32(cmbCity.SelectedValue);
                this.SelectedCitySysNo = citySysNumber.ToString();
               
                Facade.QueryCurrentAreaStructure(citySysNumber, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    this.needChange = false;
                    ViewModel = AreaQueryTransform.Transform(args.Result);
                    //cmbArea.ItemsSource = ViewModel.DistrictAreaList;
                    this.SelectedCityName = ViewModel.CityeAreaList.Where(f => f.Key == citySysNumber.ToString()).FirstOrDefault().Value;
                   // this.SelectedCityName = ViewModel.CityeAreaList;
                    this.needChange = true;
                    //ClearValidationError(cmbArea);
                });
            }
            else
            {
                this.needChange = false;
                //cmbArea.ItemsSource = SingleSource;
                //cmbArea.SelectedIndex = 0;
                this.SelectedAreaSysNo =null;
                this.needChange = true;
            }
        }

        private void ClearValidationError(ComboBox combBox)
        {
            var exp = this.GetBindingExpression(UCAreaPicker.SelectedAreaSysNoProperty);
            if (exp == null)
                return;

            string path = exp.ParentBinding.Path.Path;

            var model = combBox.DataContext as ModelBase;
            if (model != null)
            {
                var error = model.ValidationErrors.FirstOrDefault(p => p.MemberNames.Contains(path));
                if (error != null)
                {
                    model.ValidationErrors.Remove(error);
                }
            }
        }

        //private void cmbArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (cmbArea.SelectedIndex > 0)
        //    {
        //        this.SelectedDistrictSysNo = cmbArea.SelectedValue.ToString();
        //    }
        //    else
        //    {
        //        this.SelectedDistrictSysNo = string.Empty;
        //    }
        //}
    }
}
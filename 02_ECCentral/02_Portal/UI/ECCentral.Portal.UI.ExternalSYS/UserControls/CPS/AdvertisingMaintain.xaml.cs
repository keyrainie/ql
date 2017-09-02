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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls
{
    public partial class AdvertisingMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        public AdvertisingVM Data { private get; set; } //更新时接受数据源
        public bool IsEdit { private get; set; }
        private AdvertisingVM model;
        private AdvertisingFacade facade;
        private ImageSizeFacade facadeImg;
        public AdvertisingMaintain()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                facade = new AdvertisingFacade();
                facadeImg = new ImageSizeFacade();
                if (IsEdit)
                {
                    model = Data;
                }
                else
                {
                    model = new AdvertisingVM();
                    cbProductLineCategory.IsEnabled = true;
                    cbProductLineName.IsEnabled = true;
                    cbAdvertisingType.IsEnabled = true;
                    txtEventCode.IsEnabled = true;

                }

                facade.GetAllProductLineCategory((obj, arg) =>
                {
                    if (!arg.FaultsHandle())
                    {
                        foreach (var item in arg.Result.Rows)
                        {
                            model.ProductLineCategoryList.Add(new CodeNamePair() { Code = item.SysNo.ToString(), Name = item.Name });
                        }
                       
                        facadeImg.GetAllImageSize(100, 0, "", (objI, argI) =>
                        {
                            if (!argI.FaultsHandle())
                            {
                                string sizeTmp = string.Empty;
                                foreach (var item in argI.Result.Rows)
                                {
                                    sizeTmp = item.ImageWidth+"*"+item.ImageHeight;
                                    model.ImageSizeList.Add(new CodeNamePair() { Code = sizeTmp, Name = sizeTmp });
                                }
                                this.DataContext = model;

                                if (IsEdit)
                                {
                                    this.cbProductLineCategory.SelectedItem = model.ProductLineCategoryList
                                        .First(plc => plc.Code == model.ProductLineCategorySysNo.ToString());
                                    var imageSize=(from p in  model.ImageSizeList where p.Code==model.ImageSize select p).ToList(); 
                                    if (imageSize.Count > 0) //处理异常数据 
                                    {
                                        this.cbImageSize.SelectedItem = model.ImageSizeList
                                        .First(i => i.Code == model.ImageSize);
                                    }
                                    else
                                    {

                                        this.cbImageSize.SelectedIndex = 0;
                                    }
                                }
                                else
                                {
                                    this.cbProductLineCategory.SelectedItem = (model.ProductLineCategoryList.Where(s => s.Code == null).FirstOrDefault());
                                    this.cbAdvertisingType.SelectedIndex = 0;
                                    this.cbImageSize.SelectedIndex = 0;
                                }
                                
                                
                               
                            }
                        });
                    }
                });
            };
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            AdvertisingVM vm = this.DataContext as AdvertisingVM;
            vm.ProductLineCategorySysNo =Convert.ToInt32(cbProductLineCategory.SelectedValue);
            vm.ProductLineSysNo =Convert.ToInt32(cbProductLineName.SelectedValue);
            
            if (string.IsNullOrEmpty(vm.Text) && vm.Type==AdvertisingType.TEXT)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("广告语不能为空!", MessageType.Error);
                return;
            }
            if (vm.ProductLineSysNo==null ||vm.ProductLineSysNo == 0 )
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("产品线不能为空", MessageType.Error);
                return;
            }


            if (vm.SysNo > 0)
            {
                facade.Update(vm, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功!");
                    CloseDialog(DialogResultType.OK);
                });
            }
            else
            {
                facade.Create(vm, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("添加成功!");
                    model.Url = "";
                    model.ImageUrl = "";
                    model.AdCode = "";
                });
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbProductLineCate = sender as ComboBox;
            if (cbProductLineCate.SelectedIndex != 0)
            {
                int sysNo = 0;
                if (int.TryParse(cbProductLineCate.SelectedValue.ToString(), out sysNo))
                {
                    List<CodeNamePair> codeNames = new List<CodeNamePair>();
                    facade.GetProductLineByProductLineCategorySysNo(sysNo, (s, args) =>
                    {
                        if (!args.FaultsHandle())
                        {
                            codeNames.Insert(0, new CodeNamePair { Code = null, Name = ResCommonEnum.Enum_All });
                            foreach (var item in args.Result.Rows)
                            {
                                if (IsEdit) //编辑时加载所有producLine 防止ProductLine逻辑删除后 bing报错 
                                {
                                    codeNames.Add(new CodeNamePair { Code = item.ProductLineSysNo.ToString(), Name = item.ProductLineName });
                                }
                                else //新增时加载有效的productLine 
                                {
                                    if (item.flag == 1)
                                    {
                                        codeNames.Add(new CodeNamePair { Code = item.ProductLineSysNo.ToString(), Name = item.ProductLineName });
                                    }
                                }
                            }
                            this.cbProductLineName.ItemsSource = codeNames;
                            if (IsEdit)
                            {
                                this.cbProductLineName.SelectedItem = model.ProductLineList
                                        .First(pl => pl.Code == model.ProductLineSysNo.ToString());
                            }
                            else
                            {
                                this.cbProductLineName.SelectedItem = (from p in model.ProductLineList where p.Code == null select p).ToList().FirstOrDefault();
                            }
                        }
                    });
                }
            }
            else
            {
                this.cbProductLineName.SelectedItem = (from p in model.ProductLineList where p.Code == null select p).ToList().FirstOrDefault(); 
            }
        }

        private void cbAdvertisingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbAdType = sender as ComboBox;
            if (cbAdType.SelectedValue != null)
            {
                switch (cbAdType.SelectedValue as AdvertisingType?)
                {
                    case AdvertisingType.IMG:
                        //广告语隐藏
                        this.model.Text = string.Empty;
                        this.gridAdvertising.RowDefinitions[3].Height = new GridLength(0);
                        //尺寸
                        this.gridAdvertising.RowDefinitions[6].Height = GridLength.Auto;
                        //图片链接
                        this.gridAdvertising.RowDefinitions[8].Height = GridLength.Auto;
                        this.model.ValidationErrors.Clear();
                        break;
                    case AdvertisingType.TEXT:
                        this.gridAdvertising.RowDefinitions[3].Height = GridLength.Auto;
                        //尺寸
                        this.model.ImageSize = string.Empty;
                        this.gridAdvertising.RowDefinitions[6].Height = new GridLength(0);
                        //图片链接
                        this.model.ImageUrl = string.Empty;
                        this.gridAdvertising.RowDefinitions[8].Height = new GridLength(0);
                        this.model.ValidationErrors.Clear();
                        break;
                    default:
                        break;
                }
            }
        }

        private void BtnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (this.model.Type == AdvertisingType.IMG)
            {
                hlAdPreview.Visibility = Visibility.Collapsed;
                imgAdPreview.Visibility = Visibility.Visible;
                imgAdPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(this.model.ImageUrl, UriKind.RelativeOrAbsolute));
                imgAdPreview.MouseLeftButtonDown += new MouseButtonEventHandler(imgAdPreview_MouseLeftButtonDown);
                //hlImgAdPreview.Visibility = Visibility.Visible;
                //hlImgAdPreview.NavigateUri = new Uri(this.model.Url);
                this.model.AdCode = "<a href='" + this.model.Url + "' target='_blank'><img src='" 
                                + this.model.ImageUrl + "' alt='" + this.model.Text + "'/></a>";
            }
            else
            {
                //hlImgAdPreview.Visibility = Visibility.Collapsed;
                imgAdPreview.Visibility = Visibility.Collapsed;
                hlAdPreview.Visibility = Visibility.Visible;
                hlAdPreview.Content = this.model.Text;
                hlAdPreview.NavigateUri = new Uri(this.model.Url);
                this.model.AdCode = "<a href='" + this.model.Url + "' target='_blank'>" + this.model.Text + "</a>";
            }
        }

        void imgAdPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Navigate(this.model.Url);
        }
    }
}

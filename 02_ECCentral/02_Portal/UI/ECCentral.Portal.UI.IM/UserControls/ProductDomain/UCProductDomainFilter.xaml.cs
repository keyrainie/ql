using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UCProductDomainFilter : UserControl
    {
        public UCProductDomainFilter()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCProductDomainFilter_Loaded);
        }

        void UCProductDomainFilter_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCProductDomainFilter_Loaded);

            new PMQueryFacade(CPApplication.Current.CurrentPage).QueryPMLeaderList((obj, args) =>
            {
                //此处因为Oversea的Combox控件的SelectedValuePath不支持多级(类似UserInfo.SysNo)，绑定会报错，所以用PMLeaderInfo对象替代
                List<PMLeaderInfo> list = new List<PMLeaderInfo>();
                args.Result.ForEach(p =>
                {
                    PMLeaderInfo leader = new PMLeaderInfo { SysNo = p.UserInfo.SysNo, UserDisplayName = p.UserInfo.UserDisplayName };                    
                    list.Add(leader);
                });
                list.Insert(0, (new PMLeaderInfo { SysNo = default(int?), UserDisplayName = ResCommonEnum.Enum_All }));

                cmbPMLeaders.ItemsSource = list;               
            });

            new ProductDomainFacade(CPApplication.Current.CurrentPage).LoadDomainForListing((obj, args) =>
            {
                var list = args.Result;
                list.Insert(0, new BizEntity.IM.ProductDomain { ProductDomainName = new BizEntity.LanguageContent { Content = ResCommonEnum.Enum_All } });

                cmbDomains.ItemsSource = list;
            });
        }

        private void chkEmptyCategory_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (chk.IsChecked.Value)
            {
                this.chkAsAggregateStyle.IsChecked = false;
                this.chkAsAggregateStyle.IsEnabled = false;
            }
            else
            {
                chkAsAggregateStyle.IsEnabled = true;
            }
            this.conditionContainer.IsEnabled = !chk.IsChecked.Value;            
        }
    }

    public class PMLeaderInfo
    {
        public int? SysNo { get; set; }
        public string UserDisplayName { get; set; }
    }
}

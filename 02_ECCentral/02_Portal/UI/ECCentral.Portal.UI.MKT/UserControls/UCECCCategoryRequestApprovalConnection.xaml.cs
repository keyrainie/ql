using ECCentral.BizEntity.MKT;
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

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCECCCategoryRequestApprovalConnection : UserControl
    {
        public ECCCategoryManagerType ucECCategoryManagerType
        {
            get;
            set;
        }
        public UCECCCategoryRequestApprovalConnection()
        {
            
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(CategoryRequestApprovalConnection_Loaded);
            this.cbCategoryType.SelectionChanged += new SelectionChangedEventHandler(cboCategoryType_SelectionChanged);
        }

        public event EventHandler<EventArgs> CategoryTypeChanged;

        void cboCategoryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbCategoryType.SelectedValue != null)
            {
                int selectKey = (int)this.cbCategoryType.SelectedValue;
                switch (selectKey)
                {
                    case 1:
                        ucECCategoryManagerType = ECCCategoryManagerType.ECCCategoryType1 ;
                        this.spCategorType.Visibility = Visibility.Collapsed;
                        break;
                    case 2:
                        ucECCategoryManagerType = ECCCategoryManagerType.ECCCategoryType2;
                        this.spCategorType.Visibility = Visibility.Visible;
                        this.myCategory.Category1Visibility = Visibility.Visible;
                        this.myCategory.Category2Visibility = Visibility.Collapsed;
                        break;
                    case 3:
                        ucECCategoryManagerType = ECCCategoryManagerType.ECCCategoryType3;
                        this.spCategorType.Visibility = Visibility.Visible;
                        this.myCategory.Visibility = Visibility.Visible;
                        this.myCategory.Category1Visibility = Visibility.Visible;
                        this.myCategory.Category2Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
            var handler = CategoryTypeChanged;
            if (handler != null)
            {
                EventArgs args = new EventArgs();
                handler(this, args);
            }
        }

        void CategoryRequestApprovalConnection_Loaded(object sender, RoutedEventArgs e)
        {
            this.myCategory.Category3Visibility = Visibility.Collapsed;

        }
    }
}

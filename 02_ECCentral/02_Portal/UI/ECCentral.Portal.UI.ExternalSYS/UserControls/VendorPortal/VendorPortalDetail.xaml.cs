using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls.VendorPortal
{
    public partial class VendorPortalDetail : UserControl
    {
        public IDialog Dialog { get; set; }
        public IPage Page { get; set; }
        public DynamicXml Data { get; set; }

        public VendorPortalDetail()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(VendorPortalDetail_Loaded);
        }

        public void VendorPortalDetail_Loaded(object sender, RoutedEventArgs e)
        {
            BindData();
        }

        private void BindData()
        {
            if (Data != null)
            {
                //需要构造属性列
                LayoutRoot.DataContext = Data;
                
                dataGrid.ItemsSource = GetExtendedProperties(Data["ExtendedProperties"]);
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                Dialog.Close();
            }
        }

        private List<KeyValuePair<string,string>> GetExtendedProperties(object data)
        {
            List<KeyValuePair<string, string>> keyValueList = new List<KeyValuePair<string,string>>();
            if (data != null && data.ToString().Trim().Length > 0)
            {
                try
                {
                    XDocument xmlDoc = XDocument.Parse(data.ToString());
                    var notes = xmlDoc.Descendants("ExtendedPropertyData");
                    foreach (var item in notes)
                    {
                        keyValueList.Add(new KeyValuePair<string, string>(item.Elements().ToArray()[0].Value, item.Elements().ToArray()[1].Value));
                    }
                }
                catch
                {
                    //简单处理屏蔽异常
                }
            }
            return keyValueList;
        }
    }
}

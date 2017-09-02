using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using Newegg.Oversea.Silverlight.ControlPanel.Impl.CommonService;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public partial class About : UserControl
    {
        private XapVersion m_xapVersion;

        public ChildWindow ChildWindow { get; set; }

        public AppVersionV40 AppVersion { get; set; }

        public XapVersion SelectedItem {
            get
            {
                return m_xapVersion;
            }
            set
            {
                m_xapVersion = value;
                this.BorderDescription.DataContext = value;
            }
        }

        public About()
        {
            InitializeComponent();

            this.SelectedItem = new XapVersion();
            Loaded += new System.Windows.RoutedEventHandler(About_Loaded);
            this.KeyUp += new System.Windows.Input.KeyEventHandler(About_KeyUp);
            this.VersionList.SelectionChanged += new SelectionChangedEventHandler(VersionList_SelectionChanged);

            ButtonOK.Click += (sender, e) =>
            {
                if (this.ChildWindow != null)
                {
                    this.ChildWindow.Close();
                }
            };
        }

        void VersionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                this.SelectedItem = e.AddedItems[0] as XapVersion;
            }
        }

        void About_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                if (this.ChildWindow != null)
                {
                    this.ChildWindow.Close();
                }
            }
        }

        void About_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BindAppVersions();
        }

        private void BindAppVersions()
        {
            if (this.AppVersion != null)
            {
                var xapVersions = this.AppVersion.Body;
                var fmkVersion = xapVersions.SingleOrDefault(p => string.Equals(p.XapName, "ControlPanel.SilverlightUI.xap", System.StringComparison.OrdinalIgnoreCase));

                txtFmkVersion.Text = fmkVersion.Version;
                txtComputerName.Text = this.AppVersion.ComputerName;

                xapVersions.Remove(fmkVersion);
                VersionList.ItemsSource = xapVersions;

                if (xapVersions.Count > 0)
                {
                    VersionList.SelectedIndex = 0;
                }
            }
        }
    }
}

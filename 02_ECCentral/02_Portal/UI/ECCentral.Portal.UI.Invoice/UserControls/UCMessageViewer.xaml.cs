using System;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCMessageViewer : PopWindow
    {
        public string Message
        {
            get
            {
                return tbMessageBox.Text;
            }
            set
            {
                tbMessageBox.Text = value;
            }
        }

        public UCMessageViewer()
        {
            InitializeComponent();
        }
    }
}
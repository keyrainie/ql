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

namespace ECCentral.Portal.UI.Invoice.UserControls.DynamicQueryFilter.SaleIncome
{
    public partial class DynamicFilterAO : UserControl
    {
        public DynamicFilterAO()
        {
            InitializeComponent();
        }

        #region IDynamicQueryFilter Members

        public string Identifier
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion IDynamicQueryFilter Members
    }
}
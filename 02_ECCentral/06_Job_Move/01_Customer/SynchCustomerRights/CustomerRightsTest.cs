using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPP.Oversea.CN.CustomerMgmt.SynchCustomerRights
{
    public partial class CustomerRightsTest : Form
    {
        public CustomerRightsTest()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int customerSysNo;
            if (int.TryParse(this.txtCustomerSysNo.Text, out customerSysNo))
            {
                CustomerRightBIZ biz = new CustomerRightBIZ();
                biz.CreateCustomerRights(customerSysNo);
            }
        }

    }
}

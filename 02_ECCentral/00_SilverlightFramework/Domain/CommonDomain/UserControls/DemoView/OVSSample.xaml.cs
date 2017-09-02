using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Rest;
using Newegg.Oversea.Silverlight.CommonDomain.Models;
using System.IO;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Utilities;
using System.Windows.Data;
using System.IO.IsolatedStorage;

namespace Newegg.Oversea.Silverlight.CommonDomain.UserControls
{
    public partial class OVSSample : UserControl
    {
        private ObservableCollection<SO> m_soList = new ObservableCollection<SO>();

        public PageBase Page
        {
            get;
            set;
        }

        public class TestClass : ModelBase
        {
            private int m_id;
            [Integer]
            public int ID
            {
                get
                {
                    return m_id;
                }
                set
                {
                    base.SetValue("ID", ref this.m_id, value);
                }
            }
        }

        public class SalesOrder
        {
            public string SONumber { get; set; }
        }

        public OVSSample()
        {
            InitializeComponent();
            ComboxError.DataContext = new TestClass();
            ComboxError.SelectedIndex = 0;
            DataGridSample.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(DataGridSample_LoadingDataSource);

            InitDataGridDataItems();

            Loaded += new RoutedEventHandler(OVSSample_Loaded);
            DataGridSample.Loaded += new RoutedEventHandler(DataGridSample_Loaded);

            FileUploadControl1.AllFileUploadCompleted += (s, a) =>
                {
                    //    MessageBox.Show("AllFileUploadCompleted");
                };
            FileUploadControl1.ClearFilesCompleted += (s, a) =>
                {
                    MessageBox.Show("ClearFilesCompleted");
                };
            FileUploadControl1.UploadStarted += (s, a) =>
                {

                    System.Diagnostics.Debug.WriteLine("UploadStarted");

                };
            FileUploadControl1.CheckUploadFilesEvent += new FileUploader.CheckUploadFileEvent(FileUploadControl1_CheckUploadFilesEvent);
            FileUploadControl1.UploadUrl = new Uri(CPApplication.Current.PortalBaseAddress + @"HttpHandler/DFISUploadHandler.ashx");
            FileUploadControl1.EachFilePreStartUpload += new FileUploader.EachFilePreStartUploadEvent(FileUploadControl1_EachFilePreStartUpload);
            FileUploadControl2.UploadUrl = new Uri(CPApplication.Current.PortalBaseAddress + @"HttpHandler/DFISUploadHandler.ashx");
            FileUploadControl2.EachFilePreStartUpload += new FileUploader.EachFilePreStartUploadEvent(FileUploadControl2_EachFilePreStartUpload);
            FileUploadControl2.UploadCanceled += new FileUploader.UploadCanceledEvent(FileUploadControl2_UploadCanceled);
        }

        void FileUploadControl2_UploadCanceled(object sender, FileUploader.UploadCanceledEventArgs args)
        {
            MessageBox.Show(args.CanceledFiles[0].Name);
        }

        void FileUploadControl1_CheckUploadFilesEvent(object sender, FileUploader.CheckUploadFileEventArgs args)
        {
            //foreach(FileInfo file in args.Files)
            //{
            //    if (file.Extension.Contains("jpg"))
            //    {
            //        Page.Window.Alert("Error");
            //        args.CheckResult = false;
            //    }
            //}
        }

        void FileUploadControl2_EachFilePreStartUpload(object sender, FileUploader.UploadPreStartEventArgs args)
        {
            FileUploadControl2.UploadParams = new Dictionary<string, string>();
            FileUploadControl2.UploadParams.Add("DFISUploadURL", "http://10.1.24.144:234/upload.ud");
            FileUploadControl2.UploadParams.Add("DFISGroup", "USOZZO");
            FileUploadControl2.UploadParams.Add("DFISType", "ServicePricingAttachment");
            FileUploadControl2.UploadParams.Add("DFISUserName", "wms");
            FileUploadControl2.UploadParams.Add("DFISFileName", args.FileName);
        }


        void FileUploadControl1_EachFilePreStartUpload(object sender, FileUploader.UploadPreStartEventArgs args)
        {
            FileUploadControl1.UploadParams = new Dictionary<string, string>();
            FileUploadControl1.UploadParams.Add("DFISUploadURL", "http://10.1.24.144:234/upload.ud");
            FileUploadControl1.UploadParams.Add("DFISGroup", "USOZZO");
            FileUploadControl1.UploadParams.Add("DFISType", "ServicePricingAttachment");
            FileUploadControl1.UploadParams.Add("DFISUserName", "wms");
            FileUploadControl1.UploadParams.Add("DFISFileName", args.FileName);
        }


        void DataGridSample_Loaded(object sender, RoutedEventArgs e)
        {
        }

        void DataGridSample_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            var data = m_soList.Skip(e.PageSize * e.PageIndex).Take(e.PageSize);

            DataGridSample.ItemsSource = data;
            DataGridSample.TotalCount = m_soList.Count;
        }

        private void InitDataGridDataItems()
        {
            for (var i = 1; i <= 200; i++)
            {
                var random = new Random().Next(1, 5);
                var so = new SO
                {
                    SONumber = 102 + i,
                    CustomerNumber = 7004 + i,
                    SODate = DateTime.Now,
                    Status = (i % random) == 0 ? "Active" : "Inactive"
                };

                m_soList.Add(so);
            }
        }

        void OVSSample_Loaded(object sender, RoutedEventArgs e)
        {
            WizardControl1.GoToStep(0);
            //DataGridSample.ApplyTemplate();
            DataGridSample.Bind();
            //this.Dispatcher.BeginInvoke(() =>
            //                                {

            //                                    DataGridSample.Bind();
            //                                });


            BindAutoComplete();
        }

        private void BindAutoComplete()
        {
            var list = new ObservableCollection<SalesOrder>();

            for (var i = 0; i < 100; i++)
            {
                list.Add(new SalesOrder { SONumber = i.ToString() });
            }

            var collection = new PagedCollectionView(list);
            collection.CurrentChanged += new EventHandler(collection_CurrentChanged);
            ovsAutoComplete.ItemsSource = collection;
            SDKAutoComplete.ItemsSource = collection;
        }

        void collection_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void ShowNotificationBox_Click(object sender, RoutedEventArgs e)
        {
            LinkLabel lbl = new LinkLabel();
            lbl.Text = string.Format("A new version of application update is available. Save your work, click [link=\"{0}\" target=\"_self\"]here[/link] to update.", CPApplication.Current.PortalBaseAddress);
            Page.Window.NotificationBox.Show("Notification", lbl, TimeSpan.FromSeconds(10));
        }

        private void ButtonGoToSetp_Click(object sender, RoutedEventArgs e)
        {
            DataGridSample.IsShowPager = true;
            //System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.baidu.com", UriKind.Absolute),"blank");

            //RestClient restServiceClient = new RestClient("http://localhost:11355/Services/AR.svc");

            //http://localhost:11355/Services/AR.svc/Query/Invoice/100009
            //DataSet GetB2BCustomerMailToInfo(int? customerNumber)
            //restServiceClient.Query<int>("B2BCustomerMailToInfo/100009", new EventHandler<RestClientEventArgs<int>>();

            //restServiceClient.Query<ObservableCollection<MenuItemModel>>("GetInvoice","100009", (target, args) =>
            //{

            //});

            //HttpWebRequest request = WebRequest.Create(new Uri("http://10.1.41.25:777/Portal/Service/Framework/V50/MenuRestService.svc/")) as HttpWebRequest;
            //request.Method = "GET";
            //request.Headers[HttpRequestHeader.Allow] = "True";
            //request.Headers["X-Accept-Language-Override"] = "en-US";
            //request.Accept = "application/json";
            //request.BeginGetRequestStream((obj) => 
            //{
            //    HttpWebRequest request = ((WebRequest)obj.AsyncState).EndGetRequestStream(obj) as HttpWebRequest;
            //    request.
            //}, request);
            //request.BeginGetResponse((obj) =>
            //{
            //HttpWebResponse response = ((WebRequest)obj.AsyncState).EndGetResponse(obj) as HttpWebResponse;
            //Stream stream = response.GetResponseStream();
            //}, request);


            //Page.Window.AuthManager.GetAuthUserByFunctionName(new List<string> { "CP_LogCategoryConfig_CanEditGlobalRegion", "Menu_ControlPanel_CategoryConfig" }, "0dfb2673-3d38-4d7c-b75f-ba582ff080c8", (result) =>
            //{
            //    var result2 = result;
            //});
            //var result4 = Page.Window.AuthManager.HasFunction("CP_OrderMgmt_CanDecryptCreditCardNumber", "0dfb2673-3d38-4d7c-b75f-ba582ff080c8");
            //var result3 = Page.Window.AuthManager.GetAttributesByRoleName("Admin", "0dfb2673-3d38-4d7c-b75f-ba582ff080c8");
            //请注意这里是从0开始的.
            WizardControl1.GoToStep(2);
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(e.Result);
            string str = sr.ReadToEnd();
        }

        public class InvoiceInfo
        {
            public int? ID { get; set; }

            public string Name { get; set; }
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            WizardControl1.Previous();
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            WizardControl1.Next();
        }

        private void btnMessageBox_Click(object sender, RoutedEventArgs e)
        {
            this.Page.Window.MessageBox.Show("Invalid quantity.", Silverlight.Controls.Components.MessageBoxType.Warning);
        }

        private void btnDialog_Click(object sender, RoutedEventArgs e)
        {

            //var control = new SilverlightControl1();

            //control.Dialog = this.Page.Window.ShowDialog("TEST", control, (obj, args) =>
            //{
            //    MessageBox.Show(args..ToString());

            //});


            this.Page.Window.ShowDialog("Test", "/CommonDomain/MenuMaintain", (obj, result) =>
                                                                                   {
                                                                                       var data = result.Data;
                                                                                   });
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            this.Page.Window.Confirm("I am confirm.", (obj, args) =>
            {
                MessageBox.Show(args.DialogResult.ToString());
            });
        }

        private void btnAlert_Click(object sender, RoutedEventArgs e)
        {
            //this.Page.Window.Alert("I am alert.I am alert.I am alert.I am alert.I am alert.I am alert.I am alert.I am alert.I am alert.I am alert.I am alert.", MessageType.Information);

            this.Page.Window.Alert("Test Title", "December 11 2010 (Order# 002-1272296-1339402): &#xa; sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi  sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi  sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi  sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi  sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi  sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi  sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi    &quot;TomTom XXL 540M 5-Inch Widescreen &#xa;Portable GPS Navigator (Lifetime Maps Edition)&quot;&#xa;   Estimated arrival date: December 15 2010 &#xa;If you want to check on the progress of &#xa;your order, take a look at this page in Your", MessageType.Information, (obj, args) =>
            {
                MessageBox.Show(args.DialogResult.ToString());
            });
        }

        private void btnSuccessMessageBox_Click(object sender, RoutedEventArgs e)
        {
            this.Page.Window.MessageBox.Show("Save Success.", Silverlight.Controls.Components.MessageBoxType.Success);
        }

        private void btnErrorMessageBox_Click(object sender, RoutedEventArgs e)
        {
            this.Page.Window.MessageBox.Show("December 11 2010 (Order# 002-1272296-1339402): &#xa; sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi  sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi  sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi  sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi  sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi  sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi  sdfsdfasdfsadfsdfasdfs sdfsdf sdfsdfs sdfsdfs sdfsdfsdfs   sdfsdf sdfsdfsdf s sdfsdf sdfs sdfs  sdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfsdfsdf sdfs sdfsdf sdfsdfsdfsdf sfdsdfsdf sdfsdfsdfsdf sdfsdfsdf sfdsdf sf sdf sdfsdf sdf sdf sdfsdfsd sd fs sdfsdfsfsdfgdf fgh fgh gghjgh jkhjkj lrt w werw trty ui uyi    &quot;TomTom XXL 540M 5-Inch Widescreen &#xa;Portable GPS Navigator (Lifetime Maps Edition)&quot;&#xa;   Estimated arrival date: December 15 2010 &#xa;If you want to check on the progress of &#xa;your order, take a look at this page in Your &#xa;Account: &#xa;https://www.newegg.com/gp/css/&#xa;summary/edit.html?orderID=002-1272296-1339402 &#xa;We hope to see you again soon!", Silverlight.Controls.Components.MessageBoxType.Error);
        }

        private void btnInformationMessageBox_Click(object sender, RoutedEventArgs e)
        {
            this.Page.Window.MessageBox.Show("Invalid quantity.", Silverlight.Controls.Components.MessageBoxType.Information);
        }

        private void WaterMarkTextBox_ICONClick(object sender, EventArgs e)
        {
            this.Page.Window.Alert("WaterMarkTextBox ICON Click!!");
        }


        #region Send Mail

        //Send Business Mail
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mail = new MailMessage();
            mail.From = "Aaron.L.Zhou@newegg.com";
            mail.To = "Aaron.L.Zhou@newegg.com;Jimmy.C.Shi@newegg.com";
            mail.ReplyName = "";
            mail.Subject = "test subject";
            mail.Body = "圣撒地方圣达菲撒旦发";
            mail.Priority = MailPriority.Low;
            mail.BodyType = MailBodyType.Html;

            mail.BusinessNumberList = new List<BusinessNumber>();
            mail.BusinessNumberList.Add(new BusinessNumber { NumberType = NumberType.CustomerNumber, NumberValue = "600037" });

            ComponentFactory.GetComponent<IMail>().SendBusinessMail(mail, (result) =>
            {
                if (result.IsSuccess)
                {
                    this.Page.Window.MessageBox.Show("Send Successfully.", MessageBoxType.Success);
                }
                else
                {
                    this.Page.Window.MessageBox.Show(result.Error, Silverlight.Controls.Components.MessageBoxType.Error);
                }

            });
        }

        //Send Template Biz Mail
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var mail = new MailTemplateMessage();
            mail.To = "Aaron.L.Zhou@newegg.com";
            mail.CompanyCode = "1003";
            mail.CountryCode = "USA";
            mail.LanguageCode = "en-us";

            mail.SystemID = "EGG0502009";
            mail.TemplateID = "000005";
            mail.DomainName = "Oversea";
            mail.MailTemplateVariables = new List<MailTemplateVariable>();
            mail.BusinessNumberList = new List<BusinessNumber>();
            mail.BusinessNumberList.Add(new BusinessNumber() { NumberType = Core.Components.NumberType.ClaimNumber, NumberValue = "1" });
            mail.BusinessNumberList.Add(new BusinessNumber() { NumberType = Core.Components.NumberType.CustomerNumber, NumberValue = "2" });

            ComponentFactory.GetComponent<IMail>().SendBusinessMailByTemplate(mail, (result) =>
            {
                if (result.IsSuccess)
                {
                    this.Page.Window.MessageBox.Show("Send Successfully.", MessageBoxType.Success);
                }
                else
                {
                    this.Page.Window.MessageBox.Show(result.Error, Silverlight.Controls.Components.MessageBoxType.Error);
                }
            });

        }

        //Open Mail Page
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var count = 0;

            var mail = new MailMessage();
            mail.From = "Aaron.L.Zhou@newegg.com";
            mail.To = "Aaron.L.Zhou@newegg.com";
            mail.ReplyName = "";
            mail.Subject = "test subject";
            mail.Body = "test.body";
            mail.Priority = MailPriority.Low;
            mail.BodyType = MailBodyType.Html;

            ComponentFactory.GetComponent<IMail>().OpenMailPage(mail, new MailPageSetting
            {
                IsAllowAttachment = true
            }, args =>
            {
                count++;
                CPApplication.Current.Browser.Alert(args.IsSuccess.ToString());
            });


        }

        //Open Template Mail Page
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var mail = new MailTemplateMessage();
            mail.To = "Aaron.L.Zhou@newegg.com";
            mail.CC = "Jimmy.C.Shi@newegg.com";
            mail.BCC = "Ryan.W.Li@newegg.com";
            mail.CompanyCode = "1003";
            mail.CountryCode = "USA";
            mail.LanguageCode = "en-us";

            mail.SystemID = "EGG0502009";
            mail.TemplateID = "000005";
            mail.DomainName = "Oversea";
            mail.MailTemplateVariables = new List<MailTemplateVariable>();
            mail.MailTemplateVariables.Add(new MailTemplateVariable { Key = "#DV_CustomerName#", Value = "jimmy" });

            mail.BusinessNumberList = new List<BusinessNumber>();
            mail.BusinessNumberList.Add(new BusinessNumber() { NumberType = Core.Components.NumberType.ClaimNumber, NumberValue = "1" });
            mail.BusinessNumberList.Add(new BusinessNumber() { NumberType = Core.Components.NumberType.CustomerNumber, NumberValue = "2" });

            this.Page.Window.Mailer.OpenMailPageByTemplate(mail, new MailPageSetting
            {
                IsAllowAttachment = false,
                IsAllowCC = true,
                IsAllowEdit = false
            });
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var mail = new InternalMailMessage();

            mail.From = "test@newegg.com";
            mail.To = txtTo.Text;
            mail.Subject = "Send Internal Message Test";
            mail.Body = "Send Internal Message Test";

            ComponentFactory.GetComponent<IMail>().SendInternalMail(mail, (result) =>
            {
                if (result.IsSuccess)
                {
                    this.Page.Window.MessageBox.Show("Send Successfully.", MessageBoxType.Success);
                }
                else
                {
                    this.Page.Window.MessageBox.Show(result.Error, Silverlight.Controls.Components.MessageBoxType.Error);
                }
            });

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var mail = new MailTemplateMessage();
            mail.To = "Aaron.L.Zhou@newegg.com";
            mail.CompanyCode = "1003";
            mail.CountryCode = "USA";
            mail.LanguageCode = "en-us";

            mail.SystemID = "EGG0502009";
            mail.TemplateID = "000005";
            mail.DomainName = "Oversea";
            mail.MailTemplateVariables = new List<MailTemplateVariable>();

            mail.MailTemplateVariables.Add(new MailTemplateVariable { Key = "#DV_CustomerName#", Value = "jimmy" });

            this.Page.Window.Mailer.OpenMailPageByTemplate(mail, new MailPageSetting());
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var mail = new MailTemplateMessage();
            mail.To = "Aaron.L.Zhou@newegg.com";
            mail.CompanyCode = "1003";
            mail.CountryCode = "USA";
            mail.LanguageCode = "en-us";

            mail.SystemID = "EGG0502009";
            mail.TemplateID = "000005";
            mail.DomainName = "Oversea";
            mail.MailTemplateVariables = new List<MailTemplateVariable>();

            mail.MailTemplateVariables.Add(new MailTemplateVariable { Key = "#DV_CustomerName#", Value = "jimmy" });

            this.Page.Window.Mailer.OpenMailPageByTemplate(mail, new MailPageSetting(), (result) =>
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(string.Format("Send Mail Completed, Result:{0}", result.IsSuccess));
            });
        }

        //Send Batch Mail
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            var mails = new List<MailMessage>();

            for (int i = 0; i < 3; i++)
            {
                var mail = new MailMessage();
                mail.ID = Guid.NewGuid().ToString();
                mail.From = "Aaron.L.Zhou@newegg.com";
                mail.To = "Aaron.L.Zhou@newegg.com";
                mail.ReplyName = "";
                mail.Subject = "test subject" + i;
                mail.Body = "test.body";
                mail.Priority = MailPriority.Low;
                mail.BodyType = MailBodyType.Html;

                mails.Add(mail);
            }

            CPApplication.Current.Browser.Mailer.OpenMultiMailPage(mails, new MailPageSetting
            {
                IsAllowChangeMailFrom = false,
                IsAllowAttachment = true
            }, (args) =>
            {
                CPApplication.Current.Browser.Alert(string.Format("{0}:{1}", args.ID, args.IsSuccess.ToString()));
            });
        }

        #endregion

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            this.FuncPanel.ScrollTo(txtTo);
        }

        private void AdvanceTooltip_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnWarning_Click(object sender, RoutedEventArgs e)
        {
            this.Page.Window.Alert("Test Title", "test content", MessageType.Warning, (obj, args) =>
            {
                MessageBox.Show(args.DialogResult.ToString());
            });
        }

        private void btnError_Click(object sender, RoutedEventArgs e)
        {
            this.Page.Window.Alert("Test Title", "test content", MessageType.Error, (obj, args) =>
            {
                MessageBox.Show(args.DialogResult.ToString());
            });
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            WriteTestingFile(txtFileName.Text, Convert.ToInt32(txtSize.Text));
        }

        private void WriteTestingFile(string fileName, int size) 
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var writer = new StreamWriter(new IsolatedStorageFileStream(fileName, FileMode.CreateNew, store)))
                {
                    for (var i = 0; i < size * 1024 * 1024; i++)
                    {
                        writer.WriteLine("1");
                    }
                    writer.Close();
                }
            }
        }

        private void btnShowClientComputerName_Click(object sender, RoutedEventArgs e)
        {
            this.Page.Window.MessageBox.Show(String.Format("Client Computer Name:{0}", CPApplication.Current.ClientComputerName), MessageBoxType.Information);
        }
    }
}

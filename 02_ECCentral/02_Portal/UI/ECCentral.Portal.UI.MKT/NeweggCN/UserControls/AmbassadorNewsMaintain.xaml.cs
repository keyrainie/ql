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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.NeweggCN.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.NeweggCN.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.MKT.NeweggCN.UserControls
{
    public partial class AmbassadorNewsMaintain : UserControl
    {
        protected enum FormMode
        {
            Add,
            Maintain
        }

        private int _AmbassadorNewsSysNo;

        /// <summary>
        /// 泰隆优选大使公告SysNo。
        /// </summary>
        public int AmbassadorNewsSysNo
        {
            get { return _AmbassadorNewsSysNo; }
            set
            {
                _AmbassadorNewsSysNo = value;
                if (CurrentAambassadorNews == null)
                {
                    CurrentAambassadorNews = new AmbassadorNewsVM();
                }

                CurrentAambassadorNews.SysNo = value;
            }
        }

        private AmbassadorNewsVM _CurrentAambassadorNews;

        /// <summary>
        /// 泰隆优选大使公告。
        /// </summary>
        public AmbassadorNewsVM CurrentAambassadorNews
        {
            get { return _CurrentAambassadorNews; }
            set { _CurrentAambassadorNews = value; }
        }

        private FormMode _TheFormMode;

        /// <summary>
        /// 编辑模式。新增还是维护。
        /// </summary>
        protected FormMode TheFormMode
        {
            get { return _TheFormMode; }
            set { _TheFormMode = value;


            if (value==FormMode.Add)
            {
                this.ButtonSave.Visibility = Visibility.Visible;
                this.ButtonUndo.Visibility = Visibility.Collapsed;
                this.ButtonPreview.Visibility = Visibility.Visible;
                this.ButtonSumbit.Visibility = Visibility.Visible;
            }
            else
            {
                this.ButtonSave.Visibility = Visibility.Visible;
                this.ButtonUndo.Visibility = Visibility.Visible;
                this.ButtonPreview.Visibility = Visibility.Visible;
                this.ButtonSumbit.Visibility = Visibility.Visible;
            }
            }
        }

        private IDialog _CurrentDialog;

        public IDialog CurrentDialog
        {
            get { return _CurrentDialog; }
            set { _CurrentDialog = value; }
        }

        protected AmbassadorNewsFacade facade;

        public AmbassadorNewsMaintain()
        {
            InitializeComponent();

            facade = new AmbassadorNewsFacade(CPApplication.Current.CurrentPage);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.ucBigArea.BizMode = UCBigArea.BigAreaEdiMode.Maintain;

            if (CurrentAambassadorNews == null)
            {
                TheFormMode = FormMode.Add;
                CurrentAambassadorNews = new AmbassadorNewsVM();
                CurrentAambassadorNews.Status = AmbassadorNewsStatus.UnDisplay;
                this.Grid.DataContext = CurrentAambassadorNews;
            }
            else
            {
                TheFormMode = FormMode.Maintain;

                facade.GetAmbassadorNewsBySysNo((CurrentAambassadorNews.SysNo.HasValue?CurrentAambassadorNews.SysNo.Value:0), (s, args) =>
                {

                    if (args.FaultsHandle())
                        return;

                    var rows = args.Result.Rows.ToList();

                    if (rows.Count <= 0)
                    {
                        return;
                    }

                    CurrentAambassadorNews.Title = rows[0].Title;
                    CurrentAambassadorNews.ReferenceSysNo = rows[0].ReferenceSysNo;
                    CurrentAambassadorNews.Content = rows[0].Content;
                    CurrentAambassadorNews.Status = rows[0].Status;

                    //状态为显示的，撤销按钮可见，状态为不显示的，提交按钮可见。
                    if (CurrentAambassadorNews.Status==AmbassadorNewsStatus.UnDisplay)
                    {
                        this.ButtonSave.Visibility = Visibility.Visible;
                        this.ButtonUndo.Visibility = Visibility.Collapsed;
                        this.ButtonPreview.Visibility = Visibility.Visible;
                        this.ButtonSumbit.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.ButtonSave.Visibility = Visibility.Collapsed;
                        this.ButtonUndo.Visibility = Visibility.Visible;
                        this.ButtonPreview.Visibility = Visibility.Visible;
                        this.ButtonSumbit.Visibility = Visibility.Collapsed;
                    }


                    this.Grid.DataContext = CurrentAambassadorNews;


                });

            }
        }

        

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.Grid);
            if (this.CurrentAambassadorNews.HasValidationErrors)
                return;

     
            if (facade != null)
            {
                facade.SaveAmbassadorNews(this.CurrentAambassadorNews, (s,args) => {
                    if (args.FaultsHandle())
                        return;
                    if (args.Result != null)
                    {
                        this.CurrentAambassadorNews.SysNo = args.Result;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResAmbassadorNews.Info_SaveSuccess);
                    this.ButtonSave.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void ButtonPreview_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CurrentAambassadorNews.Content))
            {
                HtmlViewHelper.ViewHtmlInBrowser("MKT", CurrentAambassadorNews.Content);
            }
        }

        private void ButtonSumbit_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.Grid);
            if (this.CurrentAambassadorNews.HasValidationErrors)
                return;

            if (facade != null)
            {
                facade.SubmitAmbassadorNews(this.CurrentAambassadorNews, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    if (args.Result != null)
                    {
                        this.CurrentAambassadorNews.SysNo = args.Result;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResAmbassadorNews.Info_SaveSuccess);
                    CurrentDialog.ResultArgs.DialogResult = DialogResultType.OK;
                    CurrentDialog.Close();

                });
            }
        }



        private void ButtonUndo_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.Grid);
            if (this.CurrentAambassadorNews.HasValidationErrors)
                return;

            if (facade != null)
            {
                facade.UndoAmbassadorNews(this.CurrentAambassadorNews, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResAmbassadorNews.Info_SaveSuccess);
                    CurrentDialog.ResultArgs.DialogResult = DialogResultType.OK;
                    CurrentDialog.Close();
                });
            }
        }
    }
}

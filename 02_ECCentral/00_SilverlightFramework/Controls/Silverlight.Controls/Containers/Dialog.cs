using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Resources;
using Newegg.Oversea.Silverlight.Core.Components;
using System.Collections.Generic;
using System.Threading;

namespace Newegg.Oversea.Silverlight.Controls.Containers
{
    public class Dialog : ChildWindow, IDialog
    {
        public static int ChildWindowCount = 0;

        public static Stack<IDialog> Dialogs { get; set; }


        public new event EventHandler Closed;
        public new event EventHandler<ClosingEventArgs> Closing;
        public ResultEventArgs ResultArgs
        {
            get;
            set;
        }

        private event ResultHandler m_handler;
        private bool m_isForce;
        private IPageBrowser m_browser;

        static Dialog()
        {
            Dialogs = new Stack<IDialog>();
        }

        public Dialog()
        {
            this.DefaultStyleKey = typeof(Dialog);

            this.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };

            this.KeyDown += new KeyEventHandler(Dialog_KeyDown);

        }

        public Dialog(IPageBrowser browser)
            : this()
        {
            this.m_browser = browser;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.Closing != null && !this.m_isForce)
            {
                var args = new ClosingEventArgs();
                this.Closing(this, args);
                e.Cancel = args.Cancel;
            }

            if (!e.Cancel)
            {
                if (this == Dialogs.Peek())
                {
                    Dialogs.Pop();
                    if (Dialogs.Count <= 0)
                    {
                        Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);
                        CPApplication.Current.CurrentDialog = null;
                    }
                    else
                    {
                        CPApplication.Current.CurrentDialog = Dialogs.Peek();
                    }
                }
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (this.Closed != null)
            {
                this.Closed(this, e);
            }
            if (this.m_handler != null)
            {
                this.m_handler(this, this.ResultArgs);
            }
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            CPApplication.Current.CurrentDialog = this;
            Dialogs.Push(this);
        }

        #region Event

        void container_LoadModule(object sender, LoadedMoudleEventArgs e)
        {
            if (e.Status == Core.Components.LoadModuleStatus.End)
            {
                var container = sender as GridContainer;

                Request request = e.Request;
                IViewInfo viewInfo = request.ModuleInfo.GetViewInfoByName(request.ViewName);
                if (viewInfo != null)
                {
                    object view = viewInfo.GetViewInstance(new PageContext(request, m_browser));
                    IPage page = view as IPage;
                    (sender as IContainer).Children.Add(view as UIElement);
                    var pageBase = view as PageBase;
                    if (pageBase != null)
                    {
                        pageBase.Window.DialogClosed += new EventHandler<ResultEventArgs>(Window_DialogClosed);
                        this.Closed += (obj, args) =>
                        {
                            pageBase.Window.DialogClosed -= new EventHandler<ResultEventArgs>(Window_DialogClosed);
                        };

                        this.Closing += (obj, args) =>
                        {
                            var args2 = new ClosingEventArgs { Cancel = false };

                            pageBase.Window.ClosingDialog(args2);

                            args.Cancel = args2.Cancel;
                        };
                    }

                    if (page != null && pageBase != null)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            page.Context.OnPageLoad(page, new EventArgs());
                        });
                    }
                    CPApplication.Current.CurrentPage.Context.Window.LoadingSpin.Hide();
                    this.Show();
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.LoadingSpin.Hide();
                    CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(MessageResource.PopupBox_CannotFoundPage, MessageBoxType.Error);
                }
            }
        }

        void Window_DialogClosed(object sender, ResultEventArgs e)
        {
            this.ResultArgs = e;
            this.Close(e.isForce);
        }

        void Dialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        #endregion

        #region IDialog Members

        public IDialog ShowDialog(string title, string url, ResultHandler callback, Size size, Panel container, IPageBrowser pageBrowser)
        {
            this.Title = title;

            if (!double.IsNaN(size.Height) && !double.IsInfinity(size.Height))
            {
                this.Height = size.Height;
            }

            if (!double.IsNaN(size.Width) && !double.IsInfinity(size.Width))
            {
                this.Width = size.Width;
            }

            var gridContainer = new GridContainer();
            gridContainer.LoadModule += container_LoadModule;
            gridContainer.Load(url);

            this.Content = gridContainer;

            if (callback != null)
            {
                this.m_handler = callback;
            }

            return this;
        }

        public IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback, Size size, Panel container)
        {
            var gridContainer = new GridContainer();
            gridContainer.Children.Add(content);

            if (!double.IsNaN(size.Height) && !double.IsInfinity(size.Height))
            {
                this.Height = size.Height;
            }

            if (!double.IsNaN(size.Width) && !double.IsInfinity(size.Width))
            {
                this.Width = size.Width;
            }

            this.Title = title;
            this.Content = gridContainer;


            if (callback != null)
            {
                this.m_handler = callback;
            }

            this.Show();

            return this;
        }

        public new void Close()
        {
            base.Close();
        }

        public void Close(bool isForce)
        {
            this.m_isForce = isForce;

            base.Close();
        }

        #endregion

        #region IComponent Members

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
            this.m_browser = browser;
        }

        public object GetInstance(TabItem tab)
        {
            return new Dialog(this.m_browser);
        }

        public void Dispose()
        {
            this.Close(true);
        }

        #endregion
    }
}

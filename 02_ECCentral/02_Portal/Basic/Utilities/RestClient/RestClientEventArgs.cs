using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Text;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.Basic.Utilities
{
    public class RestClientEventArgs<T> : EventArgs
    {
        public object m_result;

        public T Result
        {
            get
            {
                return (T)m_result;
            }
        }

        public RestServiceError Error { get; internal set; }

        private IPage Page { get; set; }

        public RestClientEventArgs(IPage page)
            : base()
        {
            Page = page;
        }

        public RestClientEventArgs(T result, IPage page)
            : this(page)
        {
            m_result = result;
        }

        public bool FaultsHandle()
        {
            return FaultsHandle(Page);
        }

        private bool FaultsHandle(IPage page)
        {
            if (this.Error != null)
            {
                bool isBizException = true;
                string error = GetError(ref isBizException);

                if (page != null && (page as UserControl).Parent != null)
                {
                    if (isBizException)
                    {
                        page.Context.Window.Alert(error, MessageType.Warning);
                    }
                    else
                    {
                        //page.Context.Window.MessageBox.Show(build.ToString(), MessageBoxType.Error);
                        page.Context.Window.Alert(error, MessageType.Error);
                    }
                }

                return true;
            }

            return false;
        }

        public string GetError(ref bool isBizException)
        {
            StringBuilder build = new StringBuilder();
            foreach (Error item in this.Error.Faults)
            {
                if (isBizException && !item.IsBusinessException)
                {
                    isBizException = false;
                }
                build.Append(string.Format("{0}", item.ErrorDescription));
            }
            return build.ToString();
        }
    }
}

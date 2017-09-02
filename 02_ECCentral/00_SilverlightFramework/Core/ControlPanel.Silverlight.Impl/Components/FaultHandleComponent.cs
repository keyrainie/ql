using System;
using System.ServiceModel;

using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Containers;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Text;

using Newegg.Oversea.Silverlight.ControlPanel.Impl.Resources;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class FaultHandleComponent : IFaultHandle
    {
        private IPageBrowser m_browser;
        private IPage m_page;

        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
        [System.Runtime.Serialization.DataContractAttribute(Name = "MessageFault", Namespace = "http://oversea.newegg.com/Framework/Common/Contract")]
        private class MessageFault : object, System.ComponentModel.INotifyPropertyChanged
        {

            private string ErrorCodeField;

            private string ErrorDescriptionField;

            private string ErrorDetailField;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string ErrorCode
            {
                get
                {
                    return this.ErrorCodeField;
                }
                set
                {
                    if ((object.ReferenceEquals(this.ErrorCodeField, value) != true))
                    {
                        this.ErrorCodeField = value;
                        this.RaisePropertyChanged("ErrorCode");
                    }
                }
            }

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string ErrorDescription
            {
                get
                {
                    return this.ErrorDescriptionField;
                }
                set
                {
                    if ((object.ReferenceEquals(this.ErrorDescriptionField, value) != true))
                    {
                        this.ErrorDescriptionField = value;
                        this.RaisePropertyChanged("ErrorDescription");
                    }
                }
            }

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string ErrorDetail
            {
                get
                {
                    return this.ErrorDetailField;
                }
                set
                {
                    if ((object.ReferenceEquals(this.ErrorDetailField, value) != true))
                    {
                        this.ErrorDetailField = value;
                        this.RaisePropertyChanged("ErrorDetail");
                    }
                }
            }

            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

            protected void RaisePropertyChanged(string propertyName)
            {
                System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
                if ((propertyChanged != null))
                {
                    propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public FaultHandleComponent()
        {
        }

        private FaultHandleComponent(IPage page, IPageBrowser browser)
            : this()
        {
            m_page = page;
            m_browser = browser;
        }

        #region IFaultHandle Members

        public bool Handle(AsyncCompletedEventArgs result)
        {
            return Handle(result, false);
        }

        public bool Handle(AsyncCompletedEventArgs result, bool isJustCheck)
        {
            if (result != null)
            {

                if (result.Error != null && result.Error.ToString().Contains("System.TimeoutException"))
                {
                    if (m_page != null && !isJustCheck)
                    {
                        m_page.Context.Window.MessageBox.Show(MessageResource.PageException_Timeout_Message, MessageBoxType.Error);
                    }
                    return true;
                }

                if (result.Error != null && result.Error.ToString().Contains("Newegg.Oversea.Framework.ExceptionBase.BusinessException"))
                {
                    if (m_page != null && !isJustCheck)
                    {
                        m_page.Context.Window.MessageBox.Show(result.Error.Message, MessageBoxType.Warning);
                    }
                    return true;
                }

                if (result.Error != null || m_page == null)
                {
                    if (m_page != null && !isJustCheck)
                    {
                        m_page.Context.Window.MessageBox.Show(result.Error.Message, MessageBoxType.Error);
                    }
                    return true;
                }

                PropertyInfo property = result.GetType().GetProperty("Result");
                object target = property.GetValue(result, null);
                IEnumerable list;
                string message = null;

                if (target != null)
                {
                    property = target.GetType().GetProperty("Faults");
                    target = property.GetValue(target, null);
                    if (target != null && (list = target as IEnumerable) != null)
                    {
                        StringBuilder bulider = new StringBuilder();
                        foreach (object item in list)
                        {
                            bulider.Append(item.ToString());

                            bulider.Append("\r\n");
                        }

                        if (bulider.Length > 0)
                        {
                            message = bulider.ToString();
                            message = message.Substring(0, message.Length - 2);

                        }
                    }

                    if (!string.IsNullOrEmpty(message) || m_page == null)
                    {
                        if (m_page != null && !isJustCheck)
                        {
                            m_page.Context.Window.MessageBox.Show(message, MessageBoxType.Error);
                        }

                        return true;
                    }

                }

            }

            return false;
        }

        #endregion

        #region IComponent Members

        public string Name
        {
            get { return "FaultHandle"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(Newegg.Oversea.Silverlight.Controls.IPageBrowser browser)
        {
            m_browser = browser;
        }

        public object GetInstance(System.Windows.Controls.TabItem tab)
        {
            return new FaultHandleComponent((tab as PageTab).View, m_browser);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_page = null;
        }

        #endregion
    }
}

using System;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Rest;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Text;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class MailComponent : IMail
    {
        private RestClient m_restClient;
        private DispatcherTimer m_timer;
        private DispatcherTimer m_batchTimer;
        private List<MailTask> m_tasks;
        private List<BatchMailTask> m_batchTasks;

        private static object s_syncObj = new object();

        private int m_maxProcessCount = 500;

        #region IComponent Members

        public string Name
        {
            get { return "MailComponent"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
            this.m_restClient = new RestClient("/Service/Framework/V50/MailRestService.svc");
            this.m_tasks = new List<MailTask>();
            this.m_timer = new DispatcherTimer();
            this.m_timer.Interval = TimeSpan.FromSeconds(7);
            this.m_timer.Tick += new EventHandler(Timer_Tick);

            this.m_batchTasks = new List<BatchMailTask>();
            this.m_batchTimer = new DispatcherTimer();
            this.m_batchTimer.Interval = TimeSpan.FromSeconds(7);
            this.m_batchTimer.Tick += new EventHandler(BatchTimer_Tick);
        }

        void BatchTimer_Tick(object sender, EventArgs e)
        {
            this.m_batchTimer.Stop();

            if (this.m_batchTasks != null && this.m_batchTasks.Count > 0)
            {
                for (int i = this.m_batchTasks.Count - 1; i >= 0; i--)
                {
                    var task = this.m_batchTasks[i];

                    if (++task.ProcessCount <= m_maxProcessCount)
                    {
                        m_restClient.Create<Dictionary<string, bool?>>("GetBatchMailStatus", task.MessageIDs, (obj, args) =>
                        {
                            if (args.Error != null && args.Error.Faults.Count > 0)
                            {
                                var message = args.Error.Faults[0].ErrorDescription;

                                CPApplication.Current.Browser.MessageBox.Show(message, MessageBoxType.Error);
                            }
                            else
                            {
                                bool isCallbacked = false;
                                foreach (var item in args.Result)
                                {
                                    if (item.Value.HasValue)
                                    {
                                        var result = new MailResult()
                                        {
                                            IsSuccess = item.Value.Value,
                                            ID = item.Key
                                        };

                                        System.Diagnostics.Debug.WriteLine("Callback:" + item.Key);

                                        task.Callback(result);
                                        isCallbacked = true;
                                    }
                                }

                                if (isCallbacked)
                                {
                                    this.m_batchTasks.Remove(task);
                                }
                            }

                            if (this.m_batchTasks.Count > 0)
                            {
                                this.m_batchTimer.Start();
                            }
                        });
                    }
                }
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            this.m_timer.Stop();

            if (m_tasks != null && m_tasks.Count > 0)
            {
                var count = this.m_tasks.Count;

                for (int i = this.m_tasks.Count - 1; i >= 0; i--)
                {
                    var task = this.m_tasks[i];

                    if (++task.ProcessCount <= m_maxProcessCount)
                    {
                        m_restClient.Query<bool?>(string.Format("{0}/{1}", "GetMailStatus", task.MessageID), (obj, args) =>
                        {
                            if (args.Error != null && args.Error.Faults.Count > 0)
                            {
                                var message = args.Error.Faults[0].ErrorDescription;

                                CPApplication.Current.Browser.MessageBox.Show(message, MessageBoxType.Error);
                            }
                            else
                            {
                                if (args.Result.HasValue)
                                {
                                    var result = new MailResult
                                    {
                                        IsSuccess = args.Result.Value
                                    };

                                    task.Action(result);

                                    lock (s_syncObj)
                                    {
                                        this.m_tasks.Remove(task);
                                    }
                                }

                                Interlocked.Decrement(ref count);

                                if (count == 0 && this.m_tasks.Count > 0)
                                {
                                    this.m_timer.Start();
                                }
                            }
                        });
                    }
                    else
                    {
                        this.m_tasks.Remove(task);
                    }
                }
            }
        }

        public object GetInstance(TabItem tab)
        {
            return this;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.m_timer.Stop();
            this.m_timer.Tick -= new EventHandler(Timer_Tick);
        }

        #endregion

        #region IMail Members

        public void SendInternalMail(InternalMailMessage message, Action<MailResult> callback)
        {
            m_restClient.Create<object>("SendInternalMail", message, (sender, args) =>
            {
                if (callback != null)
                {
                    var result = HandleException(args.Error);

                    callback(result);
                }
            });
        }

        public void SendBusinessMail(MailMessage message, Action<MailResult> callback)
        {
            m_restClient.Create<object>("SendBusinessEmail", message, (sender, args) =>
            {
                if (callback != null)
                {
                    var result = HandleException(args.Error);

                    callback(result);
                }
            });
        }

        public void SendBusinessMailByTemplate(MailTemplateMessage message, Action<MailResult> callback)
        {
            m_restClient.Create<object>("SendBusinessEmail", message, (sender, args) =>
            {
                if (callback != null)
                {
                    var result = HandleException(args.Error);

                    callback(result);
                }
            });

        }

        public void OpenMailPage(MailMessage message, MailPageSetting setting)
        {
            this.OpenMailPage(message, setting, null);
        }

        public void OpenMailPageByTemplate(MailTemplateMessage message, MailPageSetting setting)
        {
            this.OpenMailPageByTemplate(message, setting, null);
        }

        public void OpenMailPage(MailMessage message, MailPageSetting setting, Action<MailResult> callback)
        {
            var msg = new MailPageMessage { MailMessage = message, MailPageSetting = setting };

            CPApplication.Current.Browser.LoadingSpin.Show();
            LogMail(msg, (messageID) =>
            {
                CPApplication.Current.Browser.LoadingSpin.Hide();

                OpenPageInternal(messageID);

                if (callback != null)
                {
                    if (this.m_timer != null)
                    {
                        this.m_timer.Start();
                    }

                    lock (s_syncObj)
                    {
                        this.m_tasks.Add(new MailTask { MessageID = messageID, Action = callback, ProcessCount = 0 });
                    }
                }
            });
        }

        public void OpenMultiMailPage(List<MailMessage> messages, MailPageSetting setting, Action<MailResult> callback)
        {
            if (messages != null && setting != null)
            {
                foreach (var msg in messages)
                {
                    Guid temp = Guid.Empty;

                    if (!Guid.TryParse(msg.ID, out temp))
                    {
                        throw new ArgumentException("MailMessage.ID must be a Guid.");
                    }
                }

                var count = messages.Count;
                var parameter = new StringBuilder();
                var messageIDs = new List<string>();

                CPApplication.Current.Browser.LoadingSpin.Show();
                foreach (var msg in messages)
                {
                    LogMail(new MailPageMessage { MailMessage = msg, MailPageSetting = setting }, (msgID) =>
                    {
                        lock (s_syncObj)
                        {
                            messageIDs.Add(msgID);
                            parameter.Append(string.Format("{0},", msgID));
                        }

                        if (Interlocked.Decrement(ref count) <= 0)
                        {
                            CPApplication.Current.Browser.LoadingSpin.Hide();
                            OpenMultiMailPage(parameter.ToString().Trim().TrimEnd(','));

                            if (callback != null)
                            {
                                if (this.m_batchTimer != null)
                                {
                                    this.m_batchTimer.Start();
                                }

                                lock (s_syncObj)
                                {
                                    this.m_batchTasks.Add(new BatchMailTask
                                    {
                                        MessageIDs = messageIDs,
                                        Callback = callback,
                                        ProcessCount = 0
                                    });
                                }
                            }
                        }
                    });
                }
            }
        }

        public void OpenMailPageByTemplate(MailTemplateMessage message, MailPageSetting setting, Action<MailResult> callback)
        {
            var msg = new MailTemplatePageMessage { MailMessage = message, MailPageSetting = setting };

            CPApplication.Current.Browser.LoadingSpin.Show();
            LogMail(msg, (messageID) =>
            {
                CPApplication.Current.Browser.LoadingSpin.Hide();
                OpenPageInternal(messageID);

                if (callback != null)
                {
                    if (this.m_timer != null)
                    {
                        this.m_timer.Start();
                    }

                    lock (s_syncObj)
                    {
                        this.m_tasks.Add(new MailTask { MessageID = messageID, Action = callback, ProcessCount = 0 });
                    }
                }
            });
        }

        #endregion

        #region Private Methods

        private void OpenMultiMailPage(string messageID)
        {
            var url = string.Format("{0}{1}", CPApplication.Current.PortalBaseAddress, "Pages/BatchMail.aspx?MessageID={0}&LanguageCode={1}");
            var option = new WindowOptions();
            option.Size = new Size
            {
                Width = Application.Current.Host.Content.ActualWidth,
                Height = Application.Current.Host.Content.ActualHeight
            };
            option.Resizable = true;
            UtilityHelper.OpenPage(string.Format(url, messageID, CPApplication.Current.LanguageCode), option);
        }

        private void OpenPageInternal(string id)
        {
            var url = string.Format("{0}{1}", CPApplication.Current.PortalBaseAddress, "Pages/SendMail.aspx?MessageID={0}&LanguageCode={1}");
            var option = new WindowOptions();
            option.Size = new System.Windows.Size { Width = 1024, Height = 768 };
            option.Resizable = true;
            UtilityHelper.OpenPage(string.Format(url, id, CPApplication.Current.LanguageCode), option);
        }

        private void LogMail(object msg, Action<string> callback)
        {
            var message = UtilityHelper.DeepClone(msg, msg.GetType());

            m_restClient.Create<string>("LogMail", message, (sender, args) =>
            {
                if (args.Error != null && args.Error.Faults.Count > 0)
                {
                    CPApplication.Current.Browser.LoadingSpin.Hide();
                    var errorMessage = args.Error.Faults[0].ErrorDescription;

                    CPApplication.Current.Browser.MessageBox.Show(errorMessage, MessageBoxType.Error);
                }
                else if (callback != null)
                {
                    callback(args.Result);
                }
            });
        }

        private MailResult HandleException(RestServiceError error)
        {
            var result = new MailResult { IsSuccess = true };

            if (error != null && error.Faults.Count > 0)
            {
                result.IsSuccess = false;
                result.Error = error.Faults[0].ErrorDescription;
            }
            return result;
        }

        #endregion
    }

    public enum MailType
    {
        Internal,
        Business,
        Template
    }

    public class BatchMailTask
    {
        public List<string> MessageIDs { get; set; }

        public Action<MailResult> Callback { get; set; }

        public int ProcessCount { get; set; }
    }

    public class MailStatus
    {
        public string MessageID { get; set; }

        public bool? Status { get; set; }
    }

    public class MailTask
    {
        public string MessageID { get; set; }

        public Action<MailResult> Action { get; set; }

        public int ProcessCount { get; set; }
    }

    public class MailPageMessage
    {
        public MailPageSetting MailPageSetting { get; set; }

        public MailMessage MailMessage { get; set; }
    }

    public class MailTemplatePageMessage
    {
        public MailPageSetting MailPageSetting { get; set; }

        public MailTemplateMessage MailMessage { get; set; }
    }
}

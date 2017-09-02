using System;
using System.Configuration.Provider;
using System.Text;
using System.Threading;
using IPP.CN.ECommerceMgmt.AutoCommentShow;

namespace IPP.CN.ECommerceMgmt.AutoCommentShow.Providers
{
    public abstract class ServiceJobProvider : ProviderBase
    {
        private Thread m_JobThread;
        public Thread JobThread
        {
            get { return m_JobThread; }
            set { m_JobThread = value; }
        }

        private ServiceJobInfo m_JobInfo;
        public ServiceJobInfo JobInfo
        {
            get { return m_JobInfo; }
            set { m_JobInfo = value; }
        }

        public abstract void PostData();

        public void StartJob()
        {
            m_JobThread = new Thread(new ThreadStart(this.JobStarting));
            m_JobThread.Start();
        }

        //public void StopJob()
        //{
        //    m_JobThread.Abort();
        //}

        public bool CheckRunTime()
        {
            // 可以使用提供者模式
            switch (m_JobInfo.JobType)
            {
                case ServiceJobType.Daily:
                    goto default;
                case ServiceJobType.Weekly:
                    return false;
                case ServiceJobType.Monthly:
                    return false;
                case ServiceJobType.OneTimeOnly:
                    return false;
                case ServiceJobType.Repeter:
                    return CheckRepeterJobRunTime();
                default:
                    return CheckDailyJobRunTime();
            }
        }

        private bool CheckDailyJobRunTime()
        {
            bool canrun = false;

            string[] rumtime = JobInfo.RunTime.Split(';');
            for (int i = 0; i < rumtime.Length; i++)
            {
                if (DateTime.Now.ToString("HH:mm") == rumtime[i].Trim())
                {
                    canrun = true;
                    break;
                }
            }

            return canrun;
        }

        private bool CheckRepeterJobRunTime()
        {
            bool canrun = false;

            string[] rumtime = JobInfo.RunTime.Split(';');

            // 开始的时间
            string startTime = rumtime[0];

            // 间隔的分钟数
            int spanMinute = Int32.Parse(rumtime[1]);

            // 今天的分钟数
            int minute = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

            if (minute % spanMinute == 0)
            {
                canrun = true;
            }

            return canrun;
        }

        private void JobStarting()
        {
            Log.WriteLog(JobInfo.JobName + "job start...", m_JobInfo.InfoLog, true);
            if (JobInfo.SetRunning())
            {
                try
                {
                    PostData();
                }
                catch (Exception ex)
                {
                    Log.WriteLog(ex.Message, m_JobInfo.InfoLog, true);
                    Log.WriteLog(ex.ToString(), m_JobInfo.ErrorLog, true);
                }
                finally
                {
                    JobInfo.ReleaseRunning();
                    Log.WriteLog(JobInfo.JobName + " job ended.", m_JobInfo.InfoLog, true);
                }
            }
            else
            {
                Log.WriteLog(JobInfo.JobName + " job is running ", m_JobInfo.InfoLog, true);
            }
        }
    }
}

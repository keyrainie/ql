﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SendMessage
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //if (args.Length > 0)
            //{
            SmsSender.SendSmsMessage();
            //}
            //else
            //{
            //    Application.EnableVisualStyles();
            //    Application.SetCompatibleTextRenderingDefault(false);
            //    Application.Run(new FrmSMS());
            //}
        }
    }
}

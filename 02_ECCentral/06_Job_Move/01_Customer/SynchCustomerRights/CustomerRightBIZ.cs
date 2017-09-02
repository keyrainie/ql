using System;
using System.Collections.Generic;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.DataAccess;
using System.Configuration;
using System.Windows.Forms;

namespace IPP.Oversea.CN.CustomerMgmt.SynchCustomerRights
{
    public class CustomerRightBIZ : IJobAction
    {
        private JobContext context = null;
        private string Operator = ConfigurationManager.AppSettings["Operator"];
        private string RunMode = ConfigurationManager.AppSettings["RunMode"];
        public void Run(JobContext context)
        {
            this.context = context;
            DoWork();
        }

        private void DoWork()
        {
            CreateCustomerRights();
        }

        private void CreateCustomerRights()
        {
            DataCommand command = null;
           
            command = DataCommandManager.GetDataCommand("InsertCusomerRights");

            PrintMessage(DateTime.Now.ToString() + "：同步顾客权限开始...");

            PrintMessage("设置顾客<每日下未审核订单最大数目>权限开始");
            command.SetParameterValue("@Right", 1);
            command.SetParameterValue("@Operator", Operator);
            command.ExecuteNonQuery();
            PrintMessage("设置顾客<每日下未审核订单最大数目>权限结束");

            //PrintMessage("设置顾客<免打包费>权限开始");
            //command.SetParameterValue("@Right", 2);
            //command.SetParameterValue("@Operator", Operator);
            //command.ExecuteNonQuery();

            //PrintMessage("设置顾客<免打包费>权限结束");

            //PrintMessage("设置顾客<免廉价运费>权限开始");
            //command.SetParameterValue("@Right", 3);
            //command.SetParameterValue("@Operator", Operator);
            //command.ExecuteNonQuery();
            //PrintMessage("设置顾客<免廉价运费>权限结束");


            PrintMessage("设置顾客<允许积分支付和蛋券同时使用>权限开始");
            command.SetParameterValue("@Right", 5);
            command.SetParameterValue("@Operator", Operator);
            command.ExecuteNonQuery();
            PrintMessage("设置顾客<允许积分支付和蛋券同时使用>权限结束");

            PrintMessage("同步顾客权限结束.");
        }

        public void CreateCustomerRights(int customerSysNo)
        {
            DataCommand command = null;

            command = DataCommandManager.GetDataCommand("InsertTestCusomerRights");

            PrintMessage(DateTime.Now.ToString() + "：同步顾客权限开始...");

            PrintMessage("设置顾客<每日下未审核订单最大数目>权限开始");
            command.SetParameterValue("@Right", 1);
            command.SetParameterValue("@Operator", Operator);
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            command.ExecuteNonQuery();
            PrintMessage("设置顾客<每日下未审核订单最大数目>权限结束");          


            PrintMessage("设置顾客<允许积分支付和蛋券同时使用>权限开始");
            command.SetParameterValue("@Right", 5);
            command.SetParameterValue("@Operator", Operator);
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            command.ExecuteNonQuery();
            PrintMessage("设置顾客<允许积分支付和蛋券同时使用>权限结束");

            PrintMessage("同步顾客权限结束.");
            MessageBox.Show("同步顾客权限结束!");
        }

        private void PrintMessage(string msg)
        {
            if (this.context != null)
            {
                context.Message += msg+ Environment.NewLine;
            }
            Console.WriteLine(msg);
        }
    }
}
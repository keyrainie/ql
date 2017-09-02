using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IPP.EcommerceMgmt.SendCommentPoints.DataAccess;
using IPP.EcommerceMgmt.SendCommentPoints.Entities;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using System;
using Newegg.Oversea.Framework.JobConsole.Client;
using ECCentral.BizEntity.Customer;

namespace IPP.EcommerceMgmt.SendCommentPoints.BizProcess
{
    public class SendCommentPointsBP
    {
        private int TestProductSysNo;
        private int TestGroupSysNo; 

        public int PointLimit = Convert.ToInt32(ConfigurationManager.AppSettings["PointLimit"]);
        public string NeweggAccount = ConfigurationManager.AppSettings["NeweggAccount"];
        public string SellerAccount = ConfigurationManager.AppSettings["SellerAccount"];
        public string SysAccount = ConfigurationManager.AppSettings["SysAccountID"];

        public static JobContext jobContext = null;
        /// <summary>
        /// 业务日志文件
        /// </summary>
        public static string BizLogFile;

        public void Process()
        {
            string endMsg = string.Empty;
            WriteLog("******************** Begin ***********************");
            WriteLog("*****************评论发积分job开始运行************");
            try
            {
                CheckMKTTopicPoint();//check 系统积分是否足够   
                string msg = string.Empty;
                TestProductSysNo = 0;
                int.TryParse(ConfigurationManager.AppSettings["TestProductSysNo"], out TestProductSysNo);
                TestGroupSysNo = 0;
                int.TryParse(ConfigurationManager.AppSettings["TestGroupSysNo"], out TestGroupSysNo);

                #region 购物发评论经验积分
                WriteLog("\r\n现在发放经验评论积分");

                //获取日期大于2011-7-1所有评论商品的 评论编号、评论人等级、商品编号，商家编号，商家类型等信息
                //(会根据用户等级来发放积分)
                List<CommentEntity> lastDayCommentList = SendCommentPointsDA.GetCommentListByDate();

                if (lastDayCommentList != null && lastDayCommentList.Count > 0)
                {
                    lastDayCommentList = FilterByTestSysNo(lastDayCommentList);

                    List<CommentEntity> onlyUpdatePoint = new List<CommentEntity>();

                    List<CommentEntity> needPointlist = new List<CommentEntity>();

                    //处理以前的一场数据，point_obtain表有记录，但是review表的obtainpoint为0
                    foreach (CommentEntity item in lastDayCommentList)
                    {
                        if (HadObtainPointCheck(item))
                        {
                            SendCommentPointsDA.UpdateObtainPoint(item);
                        }
                        else
                        {
                            needPointlist.Add(item);
                            msg += "\r\n 客户：" + item.CustomerSysNo + ";评论编号：" + item.SysNo + ";应该获得积分：" + item.CustomerPoint;
                        }
                    }
                    WriteLog("\r\n需要加积分" + needPointlist.Count + "条记录。");
                    List<string> msgList;
                    List<CommentEntity> success1List = SendCommentPoint(needPointlist, out  msgList);
                    if (msgList != null && msgList.Count > 0)
                    {
                        foreach (var str in msgList)
                        {
                            msg += str;
                        }
                    }
                    WriteLog(msg);
                    WriteLog("\r\n评论发积分，成功加积分" + (success1List.Count - msgList.Count) + "条记录。");
                    WriteLog("\r\n评论发积分，加积分失败" + msgList.Count + "条记录。");
                    UpdateObtainPoint(success1List);

                }
                else
                {
                    msg += "\r\n 没有发放评论积分的用户。 ";
                    WriteLog(msg);
                }

                #endregion

                //#region top 5
                //Console.WriteLine("现在发放top5评论积分");
                ////修改为以组为单位top 5 积分
                ////1.获取所有有商品的groupsysno
                ////2.取得每组的没发过积分的top 5 评论
                //if (TestGroupSysNo != 0)
                //{
                //    List<CommentEntity> testTop5Result = SendCommentPointsDA.GetTop5CommentByGroupSysNo(TestGroupSysNo);

                //    if (testTop5Result != null && testTop5Result.Count > 0)
                //    {
                //        WriteLog("组" + TestGroupSysNo + "还需发放" + testTop5Result.Count + "个");
                //        List<CommentEntity> success2List = SendCommentPoint(testTop5Result);
                //        WriteLog("评论top 5 发积分，成功加积分" + success2List.Count + "条记录。");
                //        UpdateObtainPoint(success2List);
                //        InsertPointLog(success2List, "5");
                //    }
                //}
                //else
                //{
                //    List<CommentEntity> GroupSysNoList = SendCommentPointsDA.GetAllGroup();
                //    foreach (CommentEntity item in GroupSysNoList)
                //    {
                //        //获取需要发top 5的评论
                //        List<CommentEntity> top5CommentList = SendCommentPointsDA.GetTop5CommentByGroupSysNo(item.ProductGroupSysNo);

                //        if (top5CommentList != null && top5CommentList.Count > 0)
                //        {
                //            WriteLog("组" + item.ProductGroupSysNo + "还需发放" + top5CommentList.Count + "个");
                //            List<CommentEntity> success2List = SendCommentPoint(top5CommentList);
                //            WriteLog("评论top 5发积分，成功加积分" + success2List.Count + "条记录。");
                //            UpdateObtainPoint(success2List);
                //            InsertPointLog(success2List, "5");
                //        }
                //    }
                //}
                //#endregion

                //#region most useful
                //WriteLog("现在发放最有用评论积分");

                //List<CommentEntity> mostUsefulList = SendCommentPointsDA.GetMostUsefulCommentList();

                //if (mostUsefulList != null && mostUsefulList.Count > 0)
                //{
                //    mostUsefulList = FilterByTestSysNo(mostUsefulList);

                //    List<CommentEntity> success3List = SendCommentPoint(mostUsefulList);
                //    WriteLog("评论最有用发积分，成功加积分" + success3List.Count + "条记录。");
                //    UpdateObtainPoint(success3List);
                //    InsertPointLog(success3List, "U");
                //}
                //#endregion
            }
            catch (Exception er)
            {
                endMsg = DateTime.Now + " job运行异常，异常信息如下：\r\n " + er.ToString();
                WriteLog(endMsg);
            }
            WriteLog("********************** End *************************");
        }

        public void CheckMKTTopicPoint()
        {
            List<CommentEntity> list = SendCommentPointsDA.GetMKTTopicPoint(NeweggAccount, SellerAccount);
            foreach (CommentEntity item in list)
            {
                //当用户有效积分小于等于1000
                if (item.SystemPoint <= PointLimit)
                {
                    //会发送邮件给PM
                    SendCommentPointsDA.SendMailWhenPointNotEnough(item.SystemID);
                }
            }
        }

        private List<CommentEntity> FilterByTestSysNo(List<CommentEntity> commentList)
        {
            List<CommentEntity> result = new List<CommentEntity>();
            if (TestProductSysNo > 0)
            {
                result = commentList.Where(p => p.ProductSysNo == TestProductSysNo).ToList();
            }
            else
            {
                result = commentList;
            }
            return result;
        }

        //插入积分日志表
        private void InsertPointLog(List<CommentEntity> successList, string type)
        {
            foreach (CommentEntity item in successList)
            {
                int num = SendCommentPointsDA.HadObtainPointCheck(item);
                if (num > 1)//双倍积分
                {
                    SendCommentPointsDA.InsertPointLog(item, type);
                }
            }
        }


        private void UpdateObtainPoint(List<CommentEntity> success1List)
        {
            foreach (CommentEntity item in success1List)
            {
                if (HadObtainPointCheck(item))  //point_obtain 有记录                      
                {
                    SendCommentPointsDA.UpdateObtainPoint(item);
                }
            }
        }

        public List<CommentEntity> SendCommentPoint(List<CommentEntity> commentList,out List<string> msg)
        {
            msg = new List<string>() ;
            if (commentList == null || commentList.Count == 0)
            {
                return new List<CommentEntity>();
            }
            List<CustomerPointsAddRequest> body = new List<CustomerPointsAddRequest>();

            foreach (var item in commentList)
            {
                //新蛋网商品（vendortype=0）评论经验积分从MKT-Topic账户发放。
                //商家商品(vendortype=1)评论相关发放积分从Seller –topic中发放。
                CustomerPointsAddRequest aPR = new CustomerPointsAddRequest
                {
                    CustomerSysNo = item.CustomerSysNo,
                    Point = item.CustomerPoint,
                    PointType = item.VendorType,
                    SOSysNo=item.SOSysNo,
                    NewEggAccount = item.VendorType == 0 ? NeweggAccount : SellerAccount,
                    Memo = item.SysNo.ToString(),
                    Source = "ECommerceMgmt",
                    PointExpiringDate=DateTime.Now.AddYears(1)
                };
                body.Add(aPR);
               string msgCode= SendCommentPointsDA.Adjust(aPR).ToString();
               if (msgCode == "10001")
               {
                   msg.Add("\r\n 客户：" + item.CustomerSysNo + ";评论编号：" + item.SysNo + ";获得积分：" + item.CustomerPoint+"-----出现异常，异常原因：发积分账户不存在或者积分不足！");
               }
            }
            
            List<CommentEntity> successList = new List<CommentEntity>();
            if (body == null || body.Count == 0)
            {
                return new List<CommentEntity>();
            }
           
            body.ForEach(item =>
            {
                successList.Add(new CommentEntity()
                {
                    //SysNo = int.Parse(item.Message.Memo),
                    SysNo = Convert.ToInt32(item.SysNo),
                    CustomerPoint =Convert.ToInt32(item.Point),
                    CustomerSysNo = Convert.ToInt32(item.CustomerSysNo)
                });
            });
            return successList;
        }

        public bool HadObtainPointCheck(CommentEntity entity)
        {
            int num = 0;
            num = SendCommentPointsDA.HadObtainPointCheck(entity);
            return num > 0 ? true : false;
        }

        public static void WriteLog(string content)
        {
            Console.WriteLine(content);
            Log.WriteLog(content, BizLogFile);
            if (jobContext != null)
            {
                jobContext.Message += content + "\r\n";
            }
        }
    }
}

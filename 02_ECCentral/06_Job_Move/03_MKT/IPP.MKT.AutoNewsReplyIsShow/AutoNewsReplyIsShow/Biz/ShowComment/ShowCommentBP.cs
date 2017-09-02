using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using IPP.CN.ECommerceMgmt.AutoCommentShow.BusinessEntities;
using IPP.CN.ECommerceMgmt.AutoCommentShow.DA;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.ExceptionBase;

namespace IPP.CN.ECommerceMgmt.AutoCommentShow.Biz
{
    public class ShowCommentBP
    {
        /// <summary>
        /// 业务日志文件
        /// </summary>
        public static string BizLogFile = ConfigurationManager.AppSettings["BizLogFile"];
        public static string CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];

        public static int JuniorMemberScore = Convert.ToInt32(ConfigurationManager.AppSettings["JuniorMember"]);
        public static int BronzeMemberScore = Convert.ToInt32(ConfigurationManager.AppSettings["BronzeMember"]);
        public static int GoldMemberScore = Convert.ToInt32(ConfigurationManager.AppSettings["GoldMember"]);
        public static int DiamondMemberScore = Convert.ToInt32(ConfigurationManager.AppSettings["DiamondMember"]);
        public static int SuperEggMemberScore = Convert.ToInt32(ConfigurationManager.AppSettings["SuperEggMember"]);

        public static JobContext jobContext = null;

        public static void CheckRemarkMode()//string bizLogFile
        {
            string endMsg = string.Empty;
           // BizLogFile = bizLogFile;
            WriteLog("\r\n" + DateTime.Now + " ------------------- Begin-------------------------");
            WriteLog(DateTime.Now + " 前台评论自动显示job开始运行......");
            List<int> sysnolist = new List<int>();
            try
            {
                //load 评论管理模式 （自动展示的，手工展示-节假日自动展示）
                List<RemarkMode> remarkmodelist = ShowCommentDA.GetRemarkModeList();
                if (remarkmodelist != null)
                {
                    foreach (RemarkMode item in remarkmodelist)
                    {
                        if (item.RemarkType == "R"
                            && (item.Status == 0
                                || (item.Status == -1 && item.WeekendRule == 1)))
                        {
                            WriteLog(DateTime.Now + " 正在设置公告及促销评论......");
                            SetNewsAndReply(item);
                        }
                        if (item.RemarkType == "P"
                            && (item.Status == 0 || (item.Status == -1 && item.WeekendRule == 1)))
                        {
                            SetComment(item);
                            SetCommentReply();
                        }
                    }
                }
                endMsg = DateTime.Now + " 本次job成功结束!";
                WriteLog(endMsg);
            }
            catch (Exception er)
            {
                endMsg = DateTime.Now + " job运行异常，异常信息如下：\r\n " + er.Message;
                SendExceptionInfoEmail(endMsg);
                WriteLog(endMsg);
            }
            WriteLog(DateTime.Now + " ------------------- End-----------------------------\r\n");
        }

        private static void SetNewsAndReply(RemarkMode remarkmode)
        {
            //load 系统屏蔽状态公告 newsandreply
            List<NewsAndReply> newsAndReplylist = ShowCommentDA.GetNewsAndReply();
            int num = 0;
            foreach (NewsAndReply item in newsAndReplylist)
            {
                // NewsAndReply  status展示:0; 系统屏蔽:-1; 手工屏蔽：-2]
                //通过脏词库检验
                bool flag = CheckDirtyWords(string.Empty, item.ReplyContent);
                if (!flag)
                {
                    //WeekendRule 是否适用节假日Job   1：适用;0：不适用
                    //remarkmode.Status状态：自动展示  0;手动展示  -1
                    //手动展示 节假日自动；
                    if (remarkmode.Status == -1 && remarkmode.WeekendRule == 1)
                    {
                        //创建时间是不是节假日 ,或周六周日
                        List<NewsAndReply> Hoildaylist = ShowCommentDA.GetHoliday(item.CreateDate.Date);
                        if ((Hoildaylist != null && Hoildaylist.Count > 0)
                            || (item.CreateDate.DayOfWeek == DayOfWeek.Saturday
                               || item.CreateDate.DayOfWeek == DayOfWeek.Sunday))
                        {
                            item.Status = 0;
                        }
                    }
                    if (remarkmode.Status == 0)
                    {
                        item.Status = 0;
                    }
                    if (item.Status == 0)
                    {
                        ShowCommentDA.UpdateNewsAndReply(item);
                        num++;
                        WriteLog(DateTime.Now + " 公告及促销评论SysNo:" + item.SysNo + "展示成功。");
                    }
                }
            }
            if (num >= 0)
            {
                WriteLog(DateTime.Now + " 公告及促销评论设置展示成功" + num + "条记录");
            }
        }
        /// <summary>
        /// 设置展示评论下的回复显示状态
        /// </summary>
        private static void SetCommentReply()
        {
            List<BBSTopicReply> replylist = ShowCommentDA.GetBBSTopicReply();
            if (replylist != null && replylist.Count > 0)
            {
                foreach (BBSTopicReply reply in replylist)
                {
                    int oldstatus = reply.Status;
                    bool rflag = CheckDirtyWords(string.Empty, reply.ReplyContent);
                    if (rflag)
                    {
                        reply.Status = 1;
                    }
                    else if (!rflag && reply.IsFirstShow != 1)
                    {
                        bool repflag = CheckQuestionWords(string.Empty, reply.ReplyContent);
                        reply.Status = repflag == true ? 2 : 0;    //如果有询问的词就是普通询问，否则就是普通。

                        if (reply.IsFirstShow != 1
                            && (oldstatus != 0 || oldstatus != 2)
                            && (reply.Status == 0 || reply.Status == 2))
                        {
                            reply.IsFirstShow = 1;
                        }
                    }
                    ShowCommentDA.UpdateBBSTopicReplyStatus(reply);
                    //发邮件通知
                    ReplyIsFirstShowSendMail(reply);
                }
            }
        }

        private static void SetComment(RemarkMode entity)
        {
            //load 系统屏蔽状态，comment
            int num = 0;
            List<BBSTopicMaster> bbslist = ShowCommentDA.GetBBSMasterList(entity.RemarkID);
            if (bbslist != null && bbslist.Count > 0)
            {
                foreach (BBSTopicMaster item in bbslist)
                {
                    int oldstatus = item.Status;
                    //通过脏词库检验

                    #region 对应值
                    //是商品经验评论  加积分                      
                    // IsAddPoint : 0 未加积分；1 评论第一次展示加积分；2  评论第一次设为精华加积分
                    //weekendrule 是否适用节假日Job   1：适用;0：不适用
                    //RemarkMode.status状态：自动展示  0;手动展示  -1
                    // reply.Status   -2 屏蔽；0普通；1作废；2 普通询问
                    //reply.IsFirstShow  0 未展示未发邮件通知；1 已展示发过邮件通知 
                    #endregion
                    //手动展示 节假日自动；
                    if (item.RemarkModeStatus == -1 && item.WeekendRule == 1)
                    {
                        //创建时间是不是节假日 ,或周六周日
                        List<NewsAndReply> Hoildaylist = ShowCommentDA.GetHoliday(item.CreateDate.Date);
                        if ((Hoildaylist != null && Hoildaylist.Count > 0)
                            || (item.CreateDate.DayOfWeek == DayOfWeek.Saturday
                                || item.CreateDate.DayOfWeek == DayOfWeek.Sunday))
                        {
                            ChangeCommentStatusProcess(item);
                        }
                    }
                    //自动展示
                    else if (item.RemarkModeStatus == 0)
                    {
                        ChangeCommentStatusProcess(item);
                    }

                    #region 商品经验评论 加积分  该评论没加过积分
                    int pointflag = ShowCommentDA.CheckPointLogByCommentSysNo(item); // 该评论是否加过积分
                    if (item.IsAddPoint == 0
                        && item.TopicType == (int)CommentType.Experience
                        && item.Status != (int)CommentStatus.Abandon
                        && item.Status != (int)CommentStatus.SystemHide
                        && oldstatus == (int)CommentStatus.SystemHide
                        && pointflag <= 0)
                    {
                        AddExperienceCommentPoint(item);
                    }
                    if (item.TopicType != (int)CommentType.Experience)
                    {
                        item.IsAddPoint = 1;//不是经验评论只要展示过 IsAddPoint=1
                    }
                    #endregion
                    if (item.Status != (int)CommentStatus.SystemHide)
                    {
                        //后台在评论在第一次显示时，发送email给相应的PM。
                        if (item.Status != (int)CommentStatus.Abandon && item.IsAddPoint != 0)
                        {
                            SendItemCommentFirstShowEmailToPM(item);
                        }
                        ShowCommentDA.UpdateBBSTopicMaster(item);

                        if (item.Status != (int)CommentStatus.Abandon)
                        {
                            WriteLog(DateTime.Now + " 商品评论" + item.CommentSysNo + "设置展示成功.");
                            //评论发表显示后需要更新（评论数和得分）
                            ShowCommentDA.SetProductRemarkCountRemarkScore(item);
                            num++;
                        }
                    }
                }
            }
            if (num > 0)
            {
                WriteLog(DateTime.Now + " 商品评论管理设置展示成功" + num + "条记录。");
            }
        }

        public static BBSTopicMaster ChangeCommentStatusProcess(BBSTopicMaster entity)
        {
            // 经验：带脏词 经验可疑
            //经验：不带脏词，普通
            //经验：不带脏词，带询问词，普通询问
            //讨论： 
            //带脏词 作废
            //不带脏词，普通
            //不带脏词，带询问词，普通询问
            if (!CheckDirtyWords(entity.Title, entity.TopicContent))
            {
                if (CheckQuestionWords(entity.Title, entity.TopicContent))
                {
                    entity.Status = (int)CommentStatus.Question;
                }
                else
                {
                    entity.Status = (int)CommentStatus.Normal;
                }
            }
            else
            {
                if (entity.TopicType == (int)CommentType.Experience)
                {
                    entity.Status = (int)CommentStatus.OnLineNotShowWord;
                }
                else
                {
                    entity.Status = (int)CommentStatus.Abandon;
                }
            }
            return entity;
        }

        public static void AddExperienceCommentPoint(BBSTopicMaster item)
        {
            int addpoint = 0;
            int rank = ShowCommentDA.GetCustomerRank(item.CreateCustomerSysNo);
            CustomerInfo customer = new CustomerInfo();
            customer.CustomerSystemNumber = item.CreateCustomerSysNo;
            List<CustomerInfo> customerlist = new List<CustomerInfo>();
            if (rank != -999)
            {
                switch (rank)
                {
                    //初级会员：1个积分  --------------1
                    //<add key="JuniorMember" value="1" />
                    //<add key="BronzeMember" value="3" />
                    //<add key="GoldMember" value="5" />
                    //<add key="DiamondMember" value="10" />
                    //<add key="SuperEggMember" value="15" />		
                    case 1:
                        customer.CustomerGotPoint = JuniorMemberScore;
                        addpoint = JuniorMemberScore;
                        break;
                    //青铜会员：3个积分  --------------2
                    case 2:
                        customer.CustomerGotPoint = BronzeMemberScore;
                        addpoint = BronzeMemberScore;
                        break;
                    //白银会员、黄金会员：5个积分  ----3,4
                    case 3:
                    case 4:
                        customer.CustomerGotPoint = GoldMemberScore;
                        addpoint = GoldMemberScore;
                        break;
                    //钻石会员、皇冠会员：10个积分 ----5,6
                    case 5:
                    case 6:
                        customer.CustomerGotPoint = DiamondMemberScore;
                        addpoint = DiamondMemberScore;
                        break;
                    //至尊蛋黄：15个积分  -------------7
                    case 7:
                        customer.CustomerGotPoint = SuperEggMemberScore;
                        addpoint = SuperEggMemberScore;
                        break;
                }
                customerlist.Add(customer);
                try
                {
                    AddShowCommentPointForCustomersFromMKT(customerlist, item);
                }
                catch (Exception er)
                {
                    throw new BusinessException(er.Message);
                }
            }
            item.IsAddPoint = 1;
            WriteLog(DateTime.Now + " 该经验评论加积分" + addpoint);
        }

        private static void SendItemCommentFirstShowEmailToPM(BBSTopicMaster entity)
        {
            if (entity == null)
            {
                return;
            }
            //后台在评论在第一次显示时，发送email给相应的PM。 
            #region mail subject&body

            string mailBody = @"<p>您好，@CustomerID</p><p>新蛋网商品（@ProductName）的评论已设置为展示：<br/>“@CommentContent”<br/>" +
                              "<br/>要查看全部的评论和回复，请访问以下的链接：<br/><a href='http://www.newegg.com.cn/Product/@ProductID.htm'>http://www.newegg.com.cn/Product/@ProductID.htm</a> <br/>谢谢" +
                              "!<br/>中国新蛋网<br/> @SendDateString <br/>备注:本邮件是由邮件系统发送，请勿直接回复。 </p>";

            string mailSubject = "新蛋网商品（@ProductID）的评论(@Title)已设置为展示";
            #endregion

            try
            {
                List<BBSTopicMaster> list = ShowCommentDA.GetBBSMasterListBySysNo(entity);
                if (list != null)
                {
                    //没加过积分，没发过邮件
                    if (list[0].IsAddPoint != 1 && entity.IsAddPoint != 2)
                    {
                        mailSubject = mailSubject.Replace("@ProductID", entity.ProductID).
                                                  Replace("@Title", entity.Title);
                        mailBody = mailBody.Replace("@CustomerID", entity.PMUserName).
                                     Replace("@ProductName", entity.ProductName).
                                     Replace("@ProductID", entity.ProductID).
                                     Replace("@SendDateString", System.DateTime.Now.ToString()).
                                     Replace("@CommentContent", entity.TopicContent.Trim()
                                );

                        ShowCommentDA.SendEmail2MailDbWhenCommentShow(entity.PMEmailAddress,
                           mailSubject, mailBody, entity.CompanyCode);
                    }
                }
            }
            catch (Exception er)
            {
                throw new Exception(er.Message);
            }
        }

        private static void ReplyIsFirstShowSendMail(BBSTopicReply entity)
        {
            //并发送email给相应的PM(topiclasteditsysno)，评论发起者(TopicCreateCustomerSysNo),所有评论回复者     

            string MailSubject = @"您关于@ProductName的评论被回复啦";
            List<string> emailaddressList = new List<string>();
            StringBuilder sb = new StringBuilder();
            BBSTopicMaster master = new BBSTopicMaster();
            master.CommentSysNo = entity.TopicSysNo;
            //获取该回复的主题评论信息
            List<BBSTopicMaster> Commentlist = ShowCommentDA.GetBBSMasterListBySysNo(master);
            #region MailBody
            string MailBody = @"<!DOCTYPE HTML PUBLIC \'-//W3C//DTD XHTML 1.0 Transitional//EN\'><html xmlns=\'http:" +
           "//www.w3.org/1999/xhtml\'>\r\n<head><title>\r\n\tUntitled Page\r\n</title><link href=\'ht" +
           "tp://www.newegg.com.cn/WebResources/css/MailCss.css\' rel=\'stylesheet\'\r\n        t" +
           "ype=\'text/css\' />\r\n\r\n    \r\n\r\n</head>\r\n<body>\r\n\r\n<div id=\'outside1\'>\r\n           " +
           " <div id=\'outside2\'>\r\n                <div id=\'head\'>\r\n                    <div " +
           " style=\'background: url(http://img8.newegg.com.cn/neweggPic2/Marketing/hf/head_b" +
           "g.jpg);height: 76px;width: 620px;margin: 0px 0px 20px 0px;\'>\r\n                  " +
           "      <div id=\'logo\'>\r\n                            <a href=\'http://www.newegg.co" +
           "m.cn/\'>\r\n                                <img title=\'新蛋网\' height=\'51\' alt=\'新蛋网\' " +
           "src=\'http://www.newegg.com.cn/WebResources/images/MailImages/logo.jpg\'\r\n        " +
           "                            width=\'258\' /></a></div>\r\n                        <d" +
           "iv id=\'tel\'>\r\n                            全国服务热线：400-820-4400</div>\r\n              " +
           "          \r\n                    </div>\r\n                </div>\r\n                " +
           " <div id=\'content\'>\r\n                    <table border=\'0\' cellpadding=\'0\' cells" +
           "pacing=\'0\' class=\'table01\'>\r\n                        <tr class=\'table_col01\'>\r\n " +
           "                           <td class=\'table_lt\'>&nbsp;\r\n                        " +
           "  </td>\r\n                            <td class=\'table_tb\'>&nbsp;\r\n              " +
           "            </td>\r\n                            <td class=\'table_rt\'>&nbsp;\r\n    " +
           "                      </td>\r\n                        </tr>\r\n                    " +
           "    <tr class=\'table_col01\'>\r\n                            <td>&nbsp;\r\n          " +
           "                </td>\r\n                            <td>\r\n                       " +
           "          <div id=\'status_forReply\'>\r\n                                </div>\r\n  " +
           "                              <div id=\'status_iconforReply\'>\r\n                  " +
           "              </div>\r\n                            </td>\r\n                       " +
           "     <td>&nbsp;\r\n                          </td>\r\n                        </tr>\r" +
           "\n                        <tr>\r\n                            <td class=\'table_lb\'>" +
           "&nbsp;\r\n                          </td>\r\n                            <td class=\'" +
           "table_tb\'>&nbsp;\r\n                          </td>\r\n                            <" +
           "td class=\'table_rb\'>&nbsp;\r\n                          </td>\r\n                   " +
           "     </tr>\r\n                    </table>\r\n                    <div class=\'subhea" +
           "d\'>\r\n                        <span></span>\r\n                        <div>\r\n     " +
           "                       评论详情</div>\r\n                    </div>\r\n                 " +
           "   <table border=\'0\' cellpadding=\'0\' cellspacing=\'0\' class=\'table01\'>\r\n         " +
           "               <tr class=\'table_col01\'>\r\n                            <td class=\'" +
           "table_lt\'>&nbsp;\r\n                          </td>\r\n                            <" +
           "td class=\'td_width02 table_tb\'>&nbsp;\r\n                          </td>\r\n        " +
           "                    <td class=\'table_tb\'>&nbsp;\r\n                          </td>" +
           "\r\n                            <td class=\'td_width03 table_tb\'>&nbsp;\r\n          " +
           "                </td>\r\n                            <td class=\'td_width03 table_t" +
           "b\'>&nbsp;\r\n                          </td>\r\n                            <td clas" +
           "s=\'td_width03 table_tb\'>&nbsp;\r\n                          </td>\r\n               " +
           "             <td class=\'table_rt\'>&nbsp;\r\n                          </td>\r\n     " +
           "                   </tr>\r\n                        <tr>\r\n                        " +
           "    <td>&nbsp;\r\n                          </td>\r\n                            <td" +
           " colspan=\'5\'>\r\n                                <ul>\r\n                           " +
           "         <li>相关评论内容及该评论下的所有回复</li>\r\n                              </ul>\r\n\r\n@AllT" +
           "opicContent\r\n                            </td>\r\n                            <td>" +
           "&nbsp;\r\n                          </td>\r\n                        </tr>\r\n        " +
           "                <tr>\r\n                            <td class=\'table_lb\'>&nbsp;\r\n " +
           "                         </td>\r\n\r\n                            <td class=\'table_t" +
           "b\'>&nbsp;\r\n                          </td>\r\n                            <td clas" +
           "s=\'table_tb\'>&nbsp;\r\n                          </td>\r\n                          " +
           "  <td class=\'table_tb\'>&nbsp;\r\n                          </td>\r\n                " +
           "            <td class=\'table_tb\'>&nbsp;\r\n                          </td>\r\n      " +
           "                      <td class=\'table_tb\'>&nbsp;\r\n                          </t" +
           "d>\r\n                            <td class=\'table_rb\'>&nbsp;\r\n                   " +
           "       </td>\r\n                        </tr>\r\n                    </table>\r\n     " +
           "               <br />\r\n                    <table width=\'100%\' border=\'0\' cellpa" +
           "dding=\'0\'>\r\n                      <tr>\r\n                        <td height=\'27\'>" +
           "                          您可以点击@TopicReplyLink，继续与其他的蛋友们分" +
           "享您的心得和体会哦~</td>\r\n                      </tr>\r\n<tr>\r\n                        <td " +
           "height=\'27\'>不想再收到此类邮件？点击<a href=\'http://www.newegg.com.cn/Favorite/ModifyPersona" +
           "lData.aspx?SubItem=02&type=1\' target=\'_blank\'><span style=\'font-size: 12pt\'><i><" +
           "u>这里</u></i></span></a>前往帐户中心进行设置</td>\r\n                      </tr>\r\n\r\n         " +
           "           </table>\r\n                \r\n                    <table border=\'0\' cel" +
           "lpadding=\'0\' cellspacing=\'0\' class=\'table01\'>\r\n                        <tr>\r\n   " +
           "                         <td style=\'width: 5px\'>\r\n                            </" +
           "td>\r\n                            <td id=\'Memo\' colspan=\'3\'></td>\r\n\r\n            " +
           "            </tr>\r\n                    </table>\r\n                    \r\n         " +
           "           <br>\r\n                </div>\r\n                <div id=\'foot\'>\r\n      " +
           "              <div id=\'bar02\'>\r\n                        <div id=\'bar02_left\'>\r\n " +
           "                       </div>\r\n                        <div id=\'bar02_right\'>\r\n " +
           "                       </div>\r\n                    </div>\r\n                    <" +
           "div id=\'foot_text\'>\r\n                        <div align=\'center\'>上海新蛋电子商务有限公司(<a" +
           " href=\'http://www.newegg.com.cn\'>www.newegg.com.cn</a>) | 美国新蛋(<a href=\'http://w" +
           "ww.newegg.com\'>www.newegg.com</a>)<br>\r\n                        公司地址：上海延安西路号华敏翰尊" +
           "国际楼<br>\r\n                        公司邮编：<br>\r\n                        订购电话：400-820-44" +
           "00<br>\r\n                        电子邮件：<a href=\'mailto:support@newegg.com.cn\'>supp" +
           "ort@newegg.com.cn </a> </div>\r\n                  </div>\r\n                </div>\r" +
           "\n            </div>\r\n        </div>\r\n</body>\r\n</html>";

            #endregion
            if (Commentlist != null && Commentlist.Count > 0)
            {
                BBSTopicMaster topicMaster = Commentlist[0];
                #region 产品标题部分
                string productLink = "http://www.newegg.com.cn/Products/ProductDetail.aspx?sysno=" + topicMaster.ReferenceSysNo;
                sb = new StringBuilder(" <table   cellpadding='0' cellspacing='0' width='80%' bordercolor='#cccccc'  style='border-top: Silver 1px solid;word-break:break-all'>");
                sb.Append("<tr><td colspan='2'><a href='" + productLink + "' target='_blank'><span style='font-size: 12pt'><b>商品：</b>" + topicMaster.ProductName + "</span></a><br/><hr border='1'/></td></tr>");
                sb.Append("<tr><td style='padding-left: 5px; padding-top: 3px; height: 20px; width: 219px;'>");
                sb.Append("<strong><span style='color: black'>" + topicMaster.Title + "</td><td align='right'>&nbsp;</td></tr>");
                sb.Append("<tr><td colspan='2' style='padding-left: 10px;'>" + topicMaster.TopicContent.ToString().Replace("\r\n", "<br />") + "</td></tr>");
                sb.Append("<tr style='height: 20px;'><td colspan='2' align='right' style='height: 20px'>");
                sb.Append("<strong>" + topicMaster.CustomerName + "</strong>&nbsp;评论于" + topicMaster.CreateDate.ToString("yyyy-MM-dd HH:mm") + "&nbsp;</td></tr>");

                #endregion

                #region 回复部分
                sb.Append("<tr><td colspan='2' style='padding-left: 10px;'>");
                sb.Append("<table width='100%' cellpadding='0' cellspacing='0' style='border-top: gainsboro 1px solid'><tr> <td style='width: 32px' valign='top'><span style='color: royalblue'><strong>回复</strong></span></td><td>");

                //发邮件该商品的PM
                if (!string.IsNullOrEmpty(Commentlist[0].PMEmailAddress))
                {
                    emailaddressList.Add(Commentlist[0].PMEmailAddress);
                }
                if (Commentlist[0].IsSubscribe == 1)
                {
                    emailaddressList.Add(Commentlist[0].CustomerEmail);
                }
                List<BBSTopicReply> replylist = ShowCommentDA.GetAllShowBBSTopicReplyByTopicSysNo(entity.TopicSysNo);
                if (replylist != null && replylist.Count > 0)
                {
                    foreach (BBSTopicReply repInfo in replylist)
                    {
                        sb.Append("<table width='100%' cellpadding='0' cellspacing='0'>");
                        sb.Append("<tr style='" + (repInfo == null ? "background-color:#FFFFCC;" : " ") + "'><td>");
                        sb.Append("<img src='http://www.newegg.com.cn/WebResources/Images/BBS/arrow_blue_right.gif' />&nbsp;" + repInfo.ReplyContent.ToString().Replace("\r\n", "<br />"));
                        sb.Append("&nbsp;" + (Convert.ToInt32(repInfo.WithAdditionalText) == 1 ? (repInfo == null ? "感谢您对新蛋网的关注与支持，祝您购物愉快！" : "") : "") + "</td></tr>");
                        sb.Append(" <tr><td align='right' valign='top' style='word-break: normal'><strong>");
                        sb.Append(repInfo.CreateUserType != 0 ? "新蛋网" : repInfo.ReplyCustomerName + "</strong>");
                        sb.Append("&nbsp;回复于&nbsp;" + Convert.ToDateTime(repInfo.CreateDate).ToString("yyyy-MM-dd HH:mm") + "&nbsp;");
                        sb.Append("</td></tr></table>");

                        if (repInfo.IsFirstShow == 1)
                        {
                            if (!string.IsNullOrEmpty(repInfo.ReplyCustomerEmail)
                                && repInfo.CreateUserSysNo != entity.CreateUserSysNo
                                && repInfo.IsSubscribe == 1 && repInfo.CreateUserType == 0
                                && !emailaddressList.Contains(repInfo.ReplyCustomerEmail))
                            {
                                emailaddressList.Add(repInfo.ReplyCustomerEmail); //所有评论回复已展示的回复者
                            }
                        }
                    }
                    sb.Append("  </td></tr></table>");
                    sb.Append("</td></tr>");
                    sb.Append("</table>");
                    string replyLink = "http://www.newegg.com.cn/Products/AddTopicReply.aspx?ReplyTopicSysNo="
                                           + entity.TopicSysNo + "&ProductSysNo=" + topicMaster.ReferenceSysNo;
                    MailBody = MailBody.Replace("@TopicReplyLink", "<a href='" + replyLink + "' target='_blank'><span style='font-size: 14pt'><i><u>回复</u></i></span></a>");
                }
                MailSubject = MailSubject.Replace("@ProductName", topicMaster.ProductName);
                MailBody = MailBody.Replace("@AllTopicContent", sb.ToString())
                            .Replace("@ProductSysNo", topicMaster.ReferenceSysNo.ToString());
                #endregion
            }
            emailaddressList.Distinct();
            foreach (string emailAddress in emailaddressList)
            {
                bool mailstatus = ShowCommentDA.InsertReplyMail(emailAddress, string.Empty, MailSubject, MailBody);
                if (!mailstatus)
                {
                    throw new Exception(DateTime.Now + "对评论" + entity.TopicSysNo + "的回复"
                                            + entity.TopicReplySysNo + "第一次展示发邮件通知出错！");
                }
            }
            WriteLog("评论" + Commentlist[0].CommentSysNo + "下的回复" + entity.TopicReplySysNo
                          + "展示成功。并发送email给相应的PM，评论发起者,所有评论回复者。");
        }

        public static void AddShowCommentPointForCustomersFromMKT(List<CustomerInfo> customersPoint, BBSTopicMaster comment)
        {
            if (customersPoint == null || customersPoint.Count == 0)
            {
                return;
            }
            List<ECCentral.BizEntity.Customer.CustomerPointsAddRequest> request = new List<ECCentral.BizEntity.Customer.CustomerPointsAddRequest>();
            customersPoint.ForEach(c =>
            {
                request.Add(new ECCentral.BizEntity.Customer.CustomerPointsAddRequest()
                {
                    CustomerSysNo = c.CustomerSystemNumber,
                    Point = c.CustomerGotPoint,
                    PointType = 16,
                    NewEggAccount = "MKT-topic",
                    Memo = comment.CommentSysNo.ToString(),
                    Note = "后台设置屏蔽的评论为展示"
                });
            });

            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["CustomerRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Create("/Point/BatchAdjustPoint", request, out error);
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                string errorMsg = "";
                foreach (var errorItem in error.Faults)
                {
                    errorMsg += errorItem.ErrorDescription;
                }
                ECCentral.Job.Utility.Logger.WriteLog(errorMsg, "IPP.MKT.AutoNewsReplyIsShow");
            }


            //IBatchAdjustPointV31 service = ServiceBroker.FindService<IBatchAdjustPointV31>();

            //BatchMessageV31<AdjustPointRequestMsg> actionPara = new BatchMessageV31<AdjustPointRequestMsg>()
            //{
            //    Messages = (from item in customersPoint
            //                select new AdjustPointRequestMsg()
            //                {
            //                    CustomerSysNo = item.CustomerSystemNumber,
            //                    Point = item.CustomerGotPoint,
            //                    PointLogType = 16,
            //                    NewEggAccount = "MKT-topic",
            //                    Memo = comment.CommentSysNo.ToString(),
            //                    Note = "后台设置屏蔽的评论为展示"
            //                }).ToList()
            //};
            //actionPara.Header = new MessageHeader();
            //actionPara.Header.CompanyCode = CompanyCode;
            //actionPara.Header.Language = "zh-CN";
            //actionPara.Header.OperationUser = new OperationUser();
            //actionPara.Header.OperationUser.CompanyCode = CompanyCode;
            //actionPara.Header.OperationUser.LogUserName = "IPPSystemAdmin\\bitkoo\\IPPSystemAdmin[8601]";
            //actionPara.Header.OperationUser.UniqueUserName = "IPPSystemAdmin\\bitkoo\\IPPSystemAdmin[8601]";
            //actionPara.Header.OperationUser.SourceUserName = "IPPSystemAdmin";
            //actionPara.Header.OperationUser.FullName = "IPPSystemAdmin";
            //actionPara.Header.OperationUser.SourceDirectoryKey = "bitkoo";
            //DefaultDataContract result = service.BatchAdjustPoint(actionPara);
            //ServiceAdapterHelper.DealServiceFault(result);
        }

        private static bool CheckDirtyWords(string title, string content)
        {
            string dirtywords = ShowCommentDA.GetCannotOnlineWords();
            string[] words = dirtywords.Split(',');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    if ((!string.IsNullOrEmpty(title) && title.Contains(words[i]))
                        || (!string.IsNullOrEmpty(content) && content.Contains(words[i])))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool CheckQuestionWords(string title, string replycontent)
        {
            string queswords = ShowCommentDA.GetQuestionWords();
            string[] words = queswords.Split(',');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    if (!string.IsNullOrEmpty(replycontent) && replycontent.Contains(words[i])
                        || !string.IsNullOrEmpty(title) && title.Contains(words[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void SendExceptionInfoEmail(string ErrorMsg)
        {
            bool sendmailflag = Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailFlag"]);
            if (sendmailflag == true)
            {
                ShowCommentDA.SendMailAboutExceptionInfo(ErrorMsg);
            }
        }

        public static void WriteConsoleInfo(string content)
        {
            Console.WriteLine(content);
        }

        public static void WriteLog(string content)
        {
            Console.WriteLine(content);
            Log.WriteLog(content, BizLogFile);
        }
    }
}

using System.Text.RegularExpressions;
using ECommerce.Entity.Store;
using ECommerce.Service.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Utility;
using ECommerce.WebFramework;
using ECommerce.Entity.Store.Vendor;
using System.IO;
using ECommerce.Service.Product;

namespace ECommerce.Web.Controllers
{
    public class StoreController : SSLControllerBase
    {
        //
        // GET: /Store/
        /// <summary>
        /// 店铺维护页面
        /// </summary>
        /// <returns></returns>

        public ActionResult StoreQuery()
        {
            return View();
        }

        /// <summary>
        /// 查询店铺页面列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxQueryStorePageList()
        {
            var user = UserAuthHelper.GetCurrentUser();
            StorePageListQueryFilter qFilter = BuildQueryFilterEntity<StorePageListQueryFilter>();
            var result = StoreService.QueryStorePageList(qFilter, user.SellerSysNo);
            return AjaxGridJson(result);
        }

        [HttpPost]
        public JsonResult AjaxQueryStoreNavigationList()
        {
            var user = UserAuthHelper.GetCurrentUser();
            StorePageListQueryFilter qFilter = BuildQueryFilterEntity<StorePageListQueryFilter>();
            var result = StoreService.QueryStoreNavigationList(qFilter, user.SellerSysNo);
            return AjaxGridJson(result);
        }

        [HttpPost]
        public JsonResult AjaxSaveNavigationForm()
        {
            var user = UserAuthHelper.GetCurrentUser();
            StoreNavigation navigation = new StoreNavigation();

            navigation.InUserName = user.UserDisplayName;
            navigation.InUserSysNo = user.SellerSysNo;
            navigation.SellerSysNo = user.SellerSysNo;

            navigation.LinkUrl = Request["LinkUrl"].ToString();
            navigation.Content = Request["Content"].ToString();
            navigation.Priority = int.Parse(Request["Priority"].ToString());
            if (Request["Status"] != null)
            {
                navigation.Status = 1;
            }
            else
            {
                navigation.Status = 0;
            }

            StoreService.SaveNavigationForm(navigation);

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult AjaxDeleteNavigation()
        {
            var user = UserAuthHelper.GetCurrentUser();
            int sysno = 0;
            int.TryParse(Request["sysno"].ToString(), out sysno);
            StoreService.DeleteNavigation(sysno, user.SellerSysNo);
            return Json(new { Success = true, Message = LanguageHelper.GetText("删除成功") });
        }

        /// <summary>
        /// 禁用页面
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxDisableStorePage()
        {
            var user = UserAuthHelper.GetCurrentUser();
            int sysno = 0;
            int.TryParse(Request["sysno"].ToString(), out sysno);
            int result = StoreService.DisableStorePage(sysno, user.SellerSysNo);
            return Json(new { success = true });
        }

        /// <summary>
        /// 启用页面
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxEnableStorePage()
        {
            var user = UserAuthHelper.GetCurrentUser();
            int sysno = 0;
            int.TryParse(Request["sysno"].ToString(), out sysno);
            int result = StoreService.EnableStorePage(sysno, user.SellerSysNo);
            return Json(new { success = true });
        }

        /// <summary>
        /// 删除页面
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxDeleteStorePage()
        {
            var user = UserAuthHelper.GetCurrentUser();
            int sysno = 0;
            int.TryParse(Request["sysno"].ToString(), out sysno);
            int result = StoreService.DeleteStorePage(sysno, user.SellerSysNo);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult AjaxDeleteStorePageFromPublish()
        {
            var user = UserAuthHelper.GetCurrentUser();
            int sysno = 0;
            int.TryParse(Request["sysno"].ToString(), out sysno);
            int result = StoreService.DeleteStorePageFromPublish(sysno, user.SellerSysNo);
            return Json(new { success = true });
        }

        /// <summary>
        /// 回复默认
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxRestoreDefaultStorePage()
        {
            var user = UserAuthHelper.GetCurrentUser();
            int sysno = 0;
            int.TryParse(Request["sysno"].ToString(), out sysno);
            StoreService.RestoreDefaultStorePage(sysno, user.SellerSysNo);
            return Json(new { Success = true, Message = LanguageHelper.GetText("恢复默认成功") });
        }

        public ActionResult BasicSetting()
        {
            return View();
        }


        public ActionResult PageManager()
        {
            return View();
        }

        public ActionResult PageElementSelect(string pageTypeKey)
        {
            Dictionary<string, List<StorePageElement>> model = StoreService.GetPageElementByPageTypeKey(pageTypeKey);
            return View(model);
        }

        #region Page Manager

        public ActionResult PageHeaderManager()
        {
            var model = StoreService.QueryPageHeaderBySellerSysNo(CurrUser.SellerSysNo);
            return View(model);
        }

        public ActionResult AjaxAddTemplate()
        {
            var storePage = SerializationUtility.JsonDeserialize2<StorePage>(Request.Form["data"]);
            var pageLayouts = StoreService.QueryAllPageLayout();
            var pageTemplateKey = storePage.StorePageTemplate.Key;
            var Temp = StoreService.QueryStorePageTemplateByTemplateKey(pageTemplateKey);

            storePage.StorePageTemplate.PageTemplateType = Temp.PageTemplateType;
            storePage.StorePageTemplate.DataValue = Temp.DataValue;
            storePage.StorePageTemplate.PageTypeKey = Temp.PageTypeKey.Trim();
            storePage.StorePageTemplate.Name = Temp.Name;
            storePage.StorePageTemplate.Desc = Temp.Desc;
            storePage.StorePageTemplate.TemplateViewPath = Temp.TemplateViewPath;
            storePage.StorePageTemplate.MockupUrl = Temp.MockupUrl;
            storePage.StorePageTemplate.Status = Temp.Status;
            storePage.StorePageTemplate.Memo = Temp.Memo;
            storePage.StorePageTemplate.Priority = Temp.Priority;

            foreach (var storePageLayout in pageLayouts)
            {
                if (pageTemplateKey.Contains(storePageLayout.PageLayoutKey))
                {
                    storePage.StorePageTemplate.StorePageLayouts.Add(storePageLayout);
                }
            }
            return Json(storePage);
        }
        #endregion

        public ActionResult AjaxSaveBasicInfo()
        {
            var info = SerializationUtility.JsonDeserialize2<StoreBasicInfo>(Request.Form["data"]);
            info.Status = 1;
            info.SellerSysNo = CurrUser.SellerSysNo;
            info.InUserSysNo = CurrUser.UserSysNo;
            info.InUserName = CurrUser.UserDisplayName;
            info.EditDate = DateTime.Now;
            info.EditUserName = CurrUser.UserDisplayName;
            info.EditUserSysNo = CurrUser.UserSysNo;
            StoreService.SaveStoreBasicInfo(info);
            var result = new { Success = true, Message = LanguageHelper.GetText("保存成功") };
            return Json(result);
        }

        //public ActionResult AjaxDownloadAttachment()
        //{
        //    string url = Request.Form["url"];
        //    url = System.Web.HttpContext.Current.Server.MapPath(string.Format("~/UploadFiles/{0}", url));
        //    if (System.IO.File.Exists(url))
        //    {
        //        FileStream fs = new FileStream(url, FileMode.Open);
        //        byte[] bytes = new byte[(int)fs.Length];
        //        fs.Read(bytes, 0, bytes.Length);
        //        fs.Close();
        //        Response.Charset = "UTF-8";
        //        Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
        //        Response.ContentType = "application/octet-stream";

        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode("附件下载"));
        //        Response.BinaryWrite(bytes);
        //        Response.Flush();
        //        Response.End();
        //        return new EmptyResult();
        //    }
        //    else
        //    {
        //        throw new BusinessException("文件不存在！");
        //    }
        //    return null;
        //}

        public FileStreamResult AjaxDownloadAttachment(string url)
        {
            url = System.Web.HttpContext.Current.Server.MapPath(string.Format("~/DownloadFiles/{0}", url));
            if (System.IO.File.Exists(url))
            {
                return File(new FileStream(url, FileMode.Open), "application/octet-stream", System.IO.Path.GetFileName(url));
            }
            else
            {
                throw new BusinessException("文件不存在！");
            }
            return null;
        }

        [ValidateInput(false)]
        public ActionResult AjaxSaveStorePage()
        {
            var data = SerializationUtility.JsonDeserialize2<StorePage>(Request.Form["data"]);
            data.SellerSysNo = CurrUser.SellerSysNo;
            data.EditDate = DateTime.Now;
            data.EditUserName = CurrUser.UserDisplayName;
            data.EditUserSysNo = CurrUser.UserSysNo;
            
            data.InUserSysNo = CurrUser.UserSysNo;
            data.InDate = DateTime.Now;
            data.InUserName = CurrUser.UserDisplayName;

            var flag = Request.Form["flag"];
            
            if (flag.Equals("save"))
            {
                if (!data.Status.HasValue)
                {
                    data.Status = 1;
                }
                data.DataValue = SerializationUtility.JsonSerialize2(data);
                StoreService.SaveStorePage(data);
            }
            else if(flag.Equals("audit"))
            {
                data.Status = 2;//待审核   审核通过Status=3 
                data.DataValue = SerializationUtility.JsonSerialize2(data);
                StoreService.SaveStorePage(data);
            }
            else
            {
                data.Status = 1;
                data.DataValue = SerializationUtility.JsonSerialize2(data);
                StoreService.PublishStorePage(data);
            }
            return Json(new { Success = true, Message = LanguageHelper.GetText("操作成功") });
        }

        [ValidateInput(false)]
        public ActionResult AjaxSavePageHeader()
        {
            var data = SerializationUtility.JsonDeserialize2<StorePageHeader>(Request.Form["data"]);
            data.SellerSysNo = CurrUser.SellerSysNo;
            data.EditDate = DateTime.Now;
            data.EditUserName = CurrUser.UserDisplayName;
            data.EditUserSysNo = CurrUser.UserSysNo;
            if (!data.SysNo.HasValue)
            {
                data.InDate = data.EditDate;
                data.InUserName = data.EditUserName;
                data.InUserSysNo = data.EditUserSysNo;
            }
            StoreService.SavePageHeader(data);

            return Json(new { Success = true, Message = LanguageHelper.GetText("操作成功") });
        }

        public PartialViewResult AjaxGetDialog(string pageTypeKey, string key)
        {
            string url = System.Web.HttpContext.Current.Server.MapPath("~/Configuration/ElementMappingDialog.xml");
            List<ElementMappingDialog> dlist = StoreService.GetElementMappingDialog(url);
            for (int i = 0; i < dlist.Count; i++)
            {
                if (dlist[i].ElementKeys.Contains(key))
                {
                    return PartialView(dlist[i].DialogView);
                }
            }
            return null;
        }

        public PartialViewResult AjaxAddStoreNavigation()
        {
            return PartialView("~/Views/Store/AddStoreNavigationDialog.cshtml");
        }

        public ActionResult AjaxSaveAttachment()
        {
            var data = SerializationUtility.JsonDeserialize2<List<StoreAttachment>>(Request.Form["data"]);
            data.ForEach(p =>
            {
                p.SellerSysNo = CurrUser.SellerSysNo;
                p.EditDate = DateTime.Now;
                p.EditUserName = CurrUser.UserDisplayName;
                p.EditUserSysNo = CurrUser.UserSysNo;
                p.Status = 1;
                if (!p.SysNo.HasValue)
                {
                    p.InUserName = CurrUser.UserDisplayName;
                    p.InUserSysNo = CurrUser.UserSysNo;
                }
            });

            StoreService.SaveStoreAttachment(data);

            return Json(new { Success = true, Message = LanguageHelper.GetText("操作成功"), Data = StoreService.QueryStoreAttachment(CurrUser.SellerSysNo) });
        }


        public ActionResult AgentProduct()
        {
            return View();
        }

        //[ValidateInput(false)]
        public ActionResult AjaxSaveStoreAgentProduct()
        {
            List<VendorAgentInfo> agent = new List<VendorAgentInfo>();
            var data = SerializationUtility.JsonDeserialize2<List<VendorAgentInfo>>(Request.Form["data"]);
            var isRequest = bool.Parse(Request.Form["isRequest"]);
            data.ForEach(p =>
            {
                p.CompanyCode = CurrUser.CompanyCode;
                //p.Status = VendorAgentStatus.Draft;
            });
            var sellerSysNo = CurrUser.SellerSysNo;
            var sellerName = CurrUser.SellerName;

            if (data[0].C3SysNo.HasValue && data[0].C3SysNo.Value != 0)
            {
                agent = data;
            }
            else if (data[0].C2SysNo.HasValue && data[0].C2SysNo.Value != 0)
            {
                var c3 = ProductMaintainService.GetAllCategory3List(data[0].C2SysNo.Value);
                c3.ForEach(p =>
                {
                    VendorAgentInfo temp = new VendorAgentInfo();
                    temp.AgentLevel = data[0].AgentLevel;
                    temp.BrandInfo = new BrandInfo
                    {
                        SysNo = data[0].BrandInfo.SysNo,
                        BrandNameLocal = data[0].BrandInfo.BrandNameLocal
                    };
                    temp.Status = data[0].Status;
                    temp.C2SysNo = data[0].C2SysNo;
                    temp.C2Name = data[0].C2Name;
                    temp.C3SysNo = p.SysNo;
                    temp.C3Name = p.CategoryName;
                    temp.CompanyCode = data[0].CompanyCode;
                    agent.Add(temp);
                });
                var originAgentInfo = StoreService.QueryStoreAgentInfos(sellerSysNo);
                agent.RemoveAll(p => originAgentInfo.Any(q => q.C3Name == p.C3Name
                        && q.BrandInfo.SysNo == p.BrandInfo.SysNo));
            }
            else if (data[0].C1SysNo.HasValue && data[0].C1SysNo.Value != 0)
            {
                List<ECommerce.Entity.Product.CategoryInfo> c2list = ProductMaintainService.GetAllCategory2List(data[0].C1SysNo.Value);
                for (int n = 0; n < c2list.Count; n++)
                {
                    List<ECommerce.Entity.Product.CategoryInfo> c3list = ProductMaintainService.GetAllCategory3List(c2list[n].SysNo);
                    for (int i = 0; i < c3list.Count; i++)
                    {
                        VendorAgentInfo temp = new VendorAgentInfo();
                        temp.AgentLevel = data[0].AgentLevel;
                        temp.BrandInfo = new BrandInfo { SysNo = data[0].BrandInfo.SysNo, BrandNameLocal = data[0].BrandInfo.BrandNameLocal };
                        temp.Status = data[0].Status;
                        temp.C2SysNo = data[0].C2SysNo;
                        temp.C2Name = data[0].C2Name;
                        temp.C3SysNo = c3list[i].SysNo;
                        temp.C3Name = c3list[i].CategoryName;
                        temp.CompanyCode = data[0].CompanyCode;
                        agent.Add(temp);
                    }
                }

                var originAgentInfo = StoreService.QueryStoreAgentInfos(sellerSysNo);
                agent.RemoveAll(p => originAgentInfo.Any(q => q.C3Name == p.C3Name
                        && q.BrandInfo.SysNo == p.BrandInfo.SysNo));
            }

            StoreService.SaveStoreAgentProduct(agent, sellerSysNo, sellerName, isRequest);
            return Json(new { Success = true, Message = LanguageHelper.GetText("操作成功"), Data = StoreService.QueryStoreAgentInfos(CurrUser.SellerSysNo) });
        }

        public ActionResult AjaxDelStoreAgentProduct()
        {
            var agentSysNo = int.Parse(Request.Form["data"]);
            StoreService.DelStoreAgentProduct(agentSysNo);
            return Json(new { Success = true, Message = LanguageHelper.GetText("操作成功") });
        }

        public ActionResult AjaxDelStoreAttachment()
        {
            var attachmentSysNo = int.Parse(Request.Form["data"]);
            StoreService.DelStoreAttachment(attachmentSysNo);
            return Json(new { Success = true, Message = LanguageHelper.GetText("操作成功") });
        }

        public ActionResult AjaxQueryStoreAttachment()
        {
            return Json(new { Success = true, Data = StoreService.QueryStoreAttachment(CurrUser.SellerSysNo) });
        }

        public ActionResult QueryStoreAgentInfosByPage()
        {
            StorePageListQueryFilter filter = BuildQueryFilterEntity<StorePageListQueryFilter>();
            
            return AjaxGridJson(StoreService.QueryStoreAgentInfosByPage(filter,CurrUser.SellerSysNo));
        }
    }
}

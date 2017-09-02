using System;
using System.Collections.Generic;
using System.Web;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Entity.Member;
using Nesoft.ECWeb.Entity.Order;
using Nesoft.ECWeb.Entity.Product;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Facade.Member;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.Utility;
using Nesoft.ECWeb.UI;
using Nesoft.ECWeb.Entity.Shipping;
using Nesoft.ECWeb.Facade.Shipping;
using System.IO;
using System.Configuration;
using Nesoft.ECWeb.Entity.Common;
using Nesoft.ECWeb.WebFramework;
using System.Xml.Linq;
using System.Linq;
using System.Text.RegularExpressions;
using Nesoft.ECWeb.MobileService.Models.Order;
using Nesoft.ECWeb.MobileService.Models.MemberService;
using Nesoft.ECWeb.Entity.RMA;
using Nesoft.ECWeb.MobileService.Models.RMA;
using Nesoft.ECWeb.MobileService.Models.Common;
using Nesoft.ECWeb.Facade.RMA;
using Nesoft.ECWeb.MobileService.Core;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public class CustomerManager
    {
        public static QueryResult<OrderListItemViewModel> GetCustomerOrderList(SOPaymentStatus? paymentStatus, SOSearchType soSearchType, int pageIndex, int pageSize, string SearchKey)
        {
            SOQueryInfo queryInfo = null;
            if (paymentStatus.HasValue)
            {
                if (paymentStatus == SOPaymentStatus.All)
                {
                    queryInfo = new SOQueryInfo() { PagingInfo = new PageInfo() { PageIndex = pageIndex, PageSize = pageSize }, Status = SOStatus.OutStock, SearchType = soSearchType, CustomerID = UserMgr.ReadUserInfo().UserSysNo, SearchKey = SearchKey };
                }
                else
                {
                    queryInfo = new SOQueryInfo() { PagingInfo = new PageInfo() { PageIndex = pageIndex, PageSize = pageSize }, SOPaymentStatus = paymentStatus.Value, SearchType = soSearchType, CustomerID = UserMgr.ReadUserInfo().UserSysNo, SearchKey = SearchKey };
                }
            }
            else
            {
                queryInfo = new SOQueryInfo() { PagingInfo = new PageInfo() { PageIndex = pageIndex, PageSize = pageSize }, Status = null, SearchType = soSearchType, CustomerID = UserMgr.ReadUserInfo().UserSysNo, SearchKey = SearchKey };
            }
            //用户信息
            LoginUser suer = UserMgr.ReadUserInfo();

            #region 能进行售后申请的订单列表
            RMAQueryInfo query = new RMAQueryInfo();
            query.PagingInfo = new PageInfo();
            query.PagingInfo.PageSize = 100000;
            query.PagingInfo.PageIndex = 1;
            query.CustomerSysNo = suer.UserSysNo;
            QueryResult<OrderInfo> orders = RMARequestFacade.QueryCanRequestOrders(query);

            #endregion

            var originResult = CustomerFacade.GetOrderList(queryInfo, true);

            //originResult.ResultList中能进行售后申请的订单ID
            List<int> SOIDList = new List<int>();
            if (orders.ResultList != null && originResult.ResultList != null)
                SOIDList = orders.ResultList.Where(s => originResult.ResultList.Exists(t => t.SoSysNo == s.SoSysNo)).Select(s => s.SoSysNo).ToList();

            return EntityConverter<QueryResult<OrderInfo>, QueryResult<OrderListItemViewModel>>.Convert(originResult, (s, t) =>
            {
                if (null != s.ResultList && s.ResultList.Count > 0)
                {
                    for (int i = 0; i < s.ResultList.Count; i++)
                    {
                        if (SOIDList.Exists(x => x.Equals(s.ResultList[i].SoSysNo)))
                        {
                            t.ResultList[i].CanRequest = true;
                        }
                        else
                        {
                            t.ResultList[i].CanRequest = false;
                        }
                        t.ResultList[i].CanCancelOrder = (t.ResultList[i].Status == SOStatus.Original && t.ResultList[i].IsNetPay);
                        t.ResultList[i].CanPayOrder = CustomerFacade.IsShowPay(s.ResultList[i]);

                        t.ResultList[i].OrderDateString = s.ResultList[i].OrderDate.ToString("yyyy-MM-dd");
                        t.ResultList[i].DeliveryDateString = s.ResultList[i].DeliveryDate.ToString("yyyy-MM-dd HH:mm:ss");

                        if (t.ResultList[i].SOItemList != null && t.ResultList[i].SOItemList.Count > 0)
                        {
                            for (int j = 0; j < t.ResultList[i].SOItemList.Count; j++)
                            {
                                ImageSize imageSize = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(Nesoft.ECWeb.MobileService.Models.Product.ImageType.Small);
                                t.ResultList[i].SOItemList[j].DefaultImage = ProductFacade.BuildProductImage(imageSize, t.ResultList[i].SOItemList[j].DefaultImage);
                            }
                        }

                    }
                }

            });
        }

        public static OrderListItemViewModel GetCustomerOrderDetail(int type, int soSysNo)
        {
            OrderListItemViewModel detailInfo = null;
            int getCustomerSysNo = UserMgr.ReadUserInfo().UserSysNo;

            OrderInfo originOrderInfo = null;
            switch (type)
            {
                case 0:
                    //查询QueryDB:
                    originOrderInfo = CustomerFacade.GetQuerySODetailInfo(getCustomerSysNo, soSysNo);
                    detailInfo = EntityConverter<OrderInfo, OrderListItemViewModel>.Convert(originOrderInfo);
                    break;
                case 1:
                    //查询CentreDB:
                    originOrderInfo = CustomerFacade.GetCenterSODetailInfo(getCustomerSysNo, soSysNo);

                    detailInfo = EntityConverter<OrderInfo, OrderListItemViewModel>.Convert(originOrderInfo);
                    break;
                default:
                    break;
            }
            if (detailInfo == null)
            {

                //查询CentreDB:
                originOrderInfo = CustomerFacade.GetCenterSODetailInfo(getCustomerSysNo, soSysNo);

                detailInfo = EntityConverter<OrderInfo, OrderListItemViewModel>.Convert(originOrderInfo);
            }

            detailInfo.CanCancelOrder = (detailInfo.Status == SOStatus.Original && detailInfo.IsNetPay);
            detailInfo.CanPayOrder = CustomerFacade.IsShowPay(originOrderInfo);
            detailInfo.OrderDateString = originOrderInfo.OrderDate.ToString("yyyy-MM-dd");
            detailInfo.DeliveryDateString = originOrderInfo.DeliveryDate.ToString("yyyy-MM-dd");
            if (detailInfo != null && detailInfo.SOItemList != null && detailInfo.SOItemList.Count > 0)
            {
                foreach (var item in detailInfo.SOItemList)
                {
                    ImageSize imageSize = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(Nesoft.ECWeb.MobileService.Models.Product.ImageType.Small);
                    item.DefaultImage = ProductFacade.BuildProductImage(imageSize, item.DefaultImage);
                }
            }


            var step = 0;
            var nStatus = (int)detailInfo.Status;
            if (nStatus == 0 && !detailInfo.IsNetPay)
            {
                step = 1;
            }
            //if (nStatus == 1 || nStatus == 2 || nStatus == 3 || nStatus == 4)
            //{
            //    step = 2;
            //}
            //if (nStatus == 41)
            //{
            //    step = 3;
            //}
            if (nStatus == 1)
            {
                step = 2;
            }
            if (nStatus == 4)
            {
                step = 3;
            }
            if (nStatus == 5)
            {
                step = 4;
            }
            detailInfo.OrderStatusProgressStep = step;
            if (detailInfo.TariffAmt <= ConstValue.TariffFreeLimit)
            {
                detailInfo.TariffAmt = 0;
            }

            return detailInfo;
        }

        public static QueryResult<ProductNotifyInfoViewModel> GetProductNotifyList(int pageIndex, int pageSize)
        {
            ProduceNotifiyQueryFilter queryFilter = new ProduceNotifiyQueryFilter() { PageInfo = new PageInfo() { PageSize = pageSize, PageIndex = pageIndex }, CustomerSysNo = Nesoft.ECWeb.UI.UserMgr.ReadUserInfo().UserSysNo };

            var originList = ProductNotifyFacade.QueryProductNotify(queryFilter);
            var resultList = EntityConverter<QueryResult<ProductNotifyInfo>, QueryResult<ProductNotifyInfoViewModel>>.Convert(originList);
            if (resultList.ResultList != null && resultList.ResultList.Count > 0)
            {
                for (int i = 0; i < resultList.ResultList.Count; i++)
                {
                    ImageSize imageSize = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(Nesoft.ECWeb.MobileService.Models.Product.ImageType.Small);
                    resultList.ResultList[i].DefaultImage = ProductFacade.BuildProductImage(imageSize, originList.ResultList[i].DefaultImage);
                }
            }
            return resultList;
        }

        public static void DeleteProductNofityInfo(int nofitySysNo)
        {
            ProductNotifyFacade.DeleteProductNotify(nofitySysNo, Nesoft.ECWeb.UI.UserMgr.ReadUserInfo().UserSysNo);
        }

        public static void AddProductNofityInfo(int productSysNo, string email)
        {
            int getCustomerSysNo = Nesoft.ECWeb.UI.UserMgr.ReadUserInfo().UserSysNo;

            var getTempInfo = ProductNotifyFacade.GetProductNotify(email, productSysNo);
            if (null != getTempInfo)
            {
                throw new BusinessException("此邮箱已经被验证过，请使用其它邮箱!");
            }

            ProductNotifyInfo entity = new ProductNotifyInfo()
            {
                CustomerSysNo = getCustomerSysNo,
                ProductSysNo = productSysNo,
                Email = email,
                Status = 0
            };
            ProductNotifyFacade.CreateProductNotify(entity);
        }

        public static ProductNotifyInfo GetProductNotifyInfo(string mail, int productSysNo)
        {
            return ProductNotifyFacade.GetProductNotify(mail, productSysNo);
        }

        public static QueryResult<ProductPriceNotifyInfoViewModel> GetProductPriceNotifyList(int pageIndex, int pageSize)
        {
            ProducePriceNotifiyQueryFilter queryFilter = new ProducePriceNotifiyQueryFilter() { PageInfo = new PageInfo() { PageSize = pageSize, PageIndex = pageIndex }, CustomerSysNo = Nesoft.ECWeb.UI.UserMgr.ReadUserInfo().UserSysNo };


            var originList = ProductPriceNotifyFacade.QueryProductPriceNotify(queryFilter);
            var resultList = EntityConverter<QueryResult<ProductPriceNotifyInfo>, QueryResult<ProductPriceNotifyInfoViewModel>>.Convert(originList);
            if (resultList.ResultList != null && resultList.ResultList.Count > 0)
            {
                for (int i = 0; i < resultList.ResultList.Count; i++)
                {
                    ImageSize imageSize = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(Nesoft.ECWeb.MobileService.Models.Product.ImageType.Small);
                    resultList.ResultList[i].DefaultImage = ProductFacade.BuildProductImage(imageSize, originList.ResultList[i].DefaultImage);
                }
            }
            return resultList;
        }

        public static void DeleteProductPriceNofityInfo(int priceNofitySysNo)
        {
            ProductPriceNotifyFacade.DeleteProductPriceNotify(priceNofitySysNo, Nesoft.ECWeb.UI.UserMgr.ReadUserInfo().UserSysNo);
        }

        public static void AddProductPriceNofityInfo(int productSysNo, decimal expectedPrice, decimal instantPrice)
        {
            ProductPriceNotifyInfo entity = new ProductPriceNotifyInfo()
         {
             CustomerSysNo = Nesoft.ECWeb.UI.UserMgr.ReadUserInfo().UserSysNo,
             ProductSysNo = productSysNo,
             ExpectedPrice = expectedPrice,
             InstantPrice = instantPrice
         };
            ProductPriceNotifyFacade.CreateProductPriceNotify(entity);
        }

        /// <summary>
        /// 收藏商品List
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static QueryResult<ProductFavoriteInfoViewModel> GetProductFavoriteList(int pageIndex, int pageSize)
        {
            PageInfo pageInfo = new PageInfo() { PageIndex = pageIndex, PageSize = pageSize };
            int getCustomerSysNo = Nesoft.ECWeb.UI.UserMgr.ReadUserInfo().UserSysNo;

            var originList = CustomerFacade.GetMyFavoriteProductList(getCustomerSysNo, pageInfo);
            var resultList = EntityConverter<QueryResult<ProductFavorite>, QueryResult<ProductFavoriteInfoViewModel>>.Convert(originList);

            if (resultList.ResultList != null && resultList.ResultList.Count > 0)
            {
                for (int i = 0; i < resultList.ResultList.Count; i++)
                {
                    ImageSize imageSize = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(Nesoft.ECWeb.MobileService.Models.Product.ImageType.Small);
                    resultList.ResultList[i].DefaultImage = ProductFacade.BuildProductImage(imageSize, originList.ResultList[i].DefaultImage);
                }
            }
            return resultList;

        }

        /// <summary>
        /// 收藏店铺List
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static QueryResult<SellerFavoriteInfoViewModel> GetSellerFavoriteList(int pageIndex, int pageSize)
        {
            PageInfo pageInfo = new PageInfo() { PageIndex = pageIndex, PageSize = pageSize };
            int getCustomerSysNo = Nesoft.ECWeb.UI.UserMgr.ReadUserInfo().UserSysNo;

            var originList = CustomerFacade.GetMyFavoriteSeller(getCustomerSysNo, pageInfo);
            var resultList = EntityConverter<QueryResult<MyFavoriteSeller>, QueryResult<SellerFavoriteInfoViewModel>>.Convert(originList);

            if (resultList.ResultList != null && resultList.ResultList.Count > 0)
            {
                for (int i = 0; i < resultList.ResultList.Count; i++)
                {
                    ImageSize imageSize = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(Nesoft.ECWeb.MobileService.Models.Product.ImageType.Small);
                    resultList.ResultList[i].LogoURL = ProductFacade.BuildProductImage(imageSize, originList.ResultList[i].LogoURL);
                }
            }
            return resultList;

        }


        /// <summary>
        /// 取消商品收藏
        /// </summary>
        /// <param name="sysNo"></param>
        public static void DeleteProductFavoriteInfo(int sysNo)
        {
            CustomerFacade.DeleteMyFavorite(sysNo);
        }

        /// <summary>
        /// 取消店铺收藏
        /// </summary>
        /// <param name="sysNo"></param>
        public static void DeleteSelectedSellers(int sysNo)
        {
            CustomerFacade.DeleteMyFavoriteSeller(sysNo);
        }

        /// <summary>
        /// 收藏商品
        /// </summary>
        /// <param name="productSysNo"></param>
        public static void AddProductFavorite(int productSysNo)
        {
            CustomerFacade.AddProductToWishList(UserMgr.ReadUserInfo().UserSysNo, productSysNo);
        }

        /// <summary>
        /// 收藏店铺
        /// </summary>
        /// <param name="productSysNo"></param>
        public static void AddFavoriteSeller(int sellerSysNo)
        {
            CustomerFacade.AddMyFavoriteSeller(UserMgr.ReadUserInfo().UserSysNo, sellerSysNo);
        }

        public static QueryResult<MyReviewInfoViewModel> GetReviewList(int productSysNo, int reviewStatus, List<ReviewScoreType> scoreType, int pageIndex, int pageSize)
        {
            Product_ReviewQueryInfo queryInfo = new Product_ReviewQueryInfo() { ProductSysNo = productSysNo, PagingInfo = new PageInfo() { PageIndex = pageIndex, PageSize = pageSize }, SearchType = scoreType, ReviewStatus = reviewStatus, CustomerID = UserMgr.ReadUserInfo().UserSysNo };
            var result = CustomerFacade.GetMyReview(queryInfo);
            QueryResult<MyReviewInfoViewModel> result2 = new QueryResult<MyReviewInfoViewModel>();
            result2.PageInfo = new PageInfo() { PageIndex = result.PageNumber, PageSize = result.PageSize, TotalCount = result.TotalRecords };
            result2.ResultList = EntityConverter<List<MyReview>, List<MyReviewInfoViewModel>>.Convert(result.CurrentPageData);

            if (result2.ResultList != null && result2.ResultList.Count > 0)
            {
                for (int i = 0; i < result2.ResultList.Count; i++)
                {

                    result2.ResultList[i].ReviewNumble = CustomerFacade.CheckReviewedBySoSysNo(result2.ResultList[i].SOSysno, result2.ResultList[i].ProductSysNo);
                    result2.ResultList[i].ProductName = StringUtility.RemoveHtmlTag(result2.ResultList[i].ProductName);
                    ImageSize imageSize = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(Nesoft.ECWeb.MobileService.Models.Product.ImageType.Small);
                    result2.ResultList[i].ProductImage = ProductFacade.BuildProductImage(imageSize, result.CurrentPageData[i].ProductImage);
                }
            }
            return result2;
        }

        public static bool AddReview(AddProductReviewRequestViewModel request)
        {

            if (request.ProductSysNo <= 0)
            {
                throw new BusinessException("商品编号不能为空!");
            }
            else if (string.IsNullOrEmpty(request.Prons))
            {
                throw new BusinessException("请输入评论优点!");
            }

            Product_ReviewDetail info = new Product_ReviewDetail();
            info.ProductSysNo = request.ProductSysNo;
            info.SOSysno = request.OrderSysNo;
            info.Title = request.Title;
            info.Prons = request.Prons;
            info.Cons = request.Cons;
            info.Service = request.Service;
            decimal s1 = request.Score1;
            decimal s2 = request.Score2;
            decimal s3 = request.Score3;
            decimal s4 = request.Score4;
            info.Score1 = Convert.ToInt32(s1);
            info.Score2 = Convert.ToInt32(s2);
            info.Score3 = Convert.ToInt32(s3);
            info.Score4 = Convert.ToInt32(s4);
            info.CustomerInfo.SysNo = Nesoft.ECWeb.UI.UserMgr.ReadUserInfo().UserSysNo;
            info.ReviewType = ReviewType.Common;
            return ReviewFacade.CreateProductReview(info);
        }

        public static QueryResult<ConsultationInfoViewModel> GetConsultList(int pageIndex, int pageSize)
        {
            ConsultQueryInfo queryInfo = new ConsultQueryInfo() { CustomerSysNo = Nesoft.ECWeb.UI.UserMgr.ReadUserInfo().UserSysNo, PagingInfo = new PageInfo() { PageIndex = pageIndex, PageSize = pageIndex } };
            var result = CustomerFacade.GetConsultListByCustomerSysNo(queryInfo);
            QueryResult<ConsultationInfoViewModel> result2 = new QueryResult<ConsultationInfoViewModel>();

            result2.PageInfo = new PageInfo() { PageIndex = result.PageNumber, PageSize = result.PageSize, TotalCount = result.TotalRecords };
            result2.ResultList = EntityConverter<List<ConsultationInfo>, List<ConsultationInfoViewModel>>.Convert(result.CurrentPageData);
            return result2;
        }

        public static CustomerInfoViewModel GetCustomerInfo(LoginUser currUser)
        {
            return EntityConverter<CustomerInfo, CustomerInfoViewModel>.Convert(CustomerFacade.GetCustomerInfo(currUser.UserSysNo), (s, t) =>
            {
                if (s != null && s.ExtendInfo != null)
                {
                    t.AvtarImage = s.ExtendInfo.AvtarImage;
                    t.AvtarImageDBStatus = s.ExtendInfo.AvtarImageDBStatus;
                }
            });
        }

        public static bool UpdateCustomerPassword(LoginUser CurrUser, ChangePasswordViewModel changePasswordInfo, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (changePasswordInfo == null)
            {
                errorMsg = "请求参数错误";
                return false;
            }
            if (string.IsNullOrEmpty(changePasswordInfo.OldPassword))
            {
                errorMsg = "请输入旧密码";
                return false;
            }
            if (!CheckPassword(changePasswordInfo, out errorMsg))
            {
                return false;
            }
            //验证新密码不能与旧密码一样
            if (changePasswordInfo.OldPassword.Equals(changePasswordInfo.Password))
            {
                errorMsg = "旧密码和新密码相同";
                return false;
            }
            string OldPassword = changePasswordInfo.OldPassword;
            string Password = changePasswordInfo.Password;
            string RePassword = changePasswordInfo.RePassword;

            //string salt = LoginFacade.GetCustomerPasswordSalt(CurrUser.UserID);
            //OldPassword = PasswordHelper.GetEncryptedPassword(HttpUtility.UrlDecode(OldPassword.Replace("+", "%2b")) + salt);

            // [2014/12/22 by Swika]增加支持第三方系统导入的账号的密码验证
            var encryptMeta = LoginFacade.GetCustomerEncryptMeta(CurrUser.UserID);
            OldPassword = PasswordHelper.GetEncryptedPassword(HttpUtility.UrlDecode(OldPassword.Replace("+", "%2b")), encryptMeta);

            if (LoginFacade.CustomerLogin(CurrUser.UserID, OldPassword) == null)
            {
                errorMsg = "旧密码不正确";
                return false;
            }
            else
            {
                string encryptPassword = string.Empty;
                string passwordSalt = string.Empty;
                PasswordHelper.GetNewPasswordAndSalt(ref Password, ref encryptPassword, ref passwordSalt);
                //重置密码
                if (!CustomerFacade.UpdateCustomerPassword(CurrUser.UserID, encryptPassword, passwordSalt))
                {
                    errorMsg = "服务器忙,稍后重试";
                    return false;
                }
            }
            return true;
        }

        public static List<ShippingContactInfo> GetCustomerShippingAddressList(LoginUser currUser)
        {
            return CustomerShippingAddresssFacade.GetCustomerShippingAddressList(currUser.UserSysNo);
        }

        public static ShippingContactInfo GetCustomerShippingAddressDetail(int shippingAddressSysNo)
        {
            return CustomerShippingAddresssFacade.GetCustomerShippingAddress(shippingAddressSysNo, UserMgr.ReadUserInfo().UserSysNo);
        }

        public static void EditCustomerShippingAddress(LoginUser currUser, ShippingContactInfo shippingAddress, out int sysNo)
        {
            //确保ReceiveContact也存有值
            if (string.IsNullOrWhiteSpace(shippingAddress.ReceiveContact))
            {
                shippingAddress.ReceiveContact = shippingAddress.ReceiveName;
            }
            CustomerShippingAddresssFacade.EditCustomerContactInfo(shippingAddress, currUser.UserSysNo);
            sysNo = shippingAddress.SysNo;
        }

        public static void DeleteCustomerShippingAddress(LoginUser currUser, int shippingAddressSysNo)
        {
            CustomerShippingAddresssFacade.DeleteCustomerContactInfo(shippingAddressSysNo, currUser.UserSysNo);
        }

        public static QueryResult<CustomerCouponInfoViewModel> GetCustomerCouponCode(LoginUser currUser, int pageIndex, int pageSize)
        {
            CustomerCouponCodeQueryInfo query = new CustomerCouponCodeQueryInfo();
            query.PageInfo.PageIndex = pageIndex;
            query.PageInfo.PageSize = pageSize > 0 ? pageSize : 10;
            query.CustomerSysNo = currUser.UserSysNo;
            query.Status = "A";
            return EntityConverter<QueryResult<CustomerCouponInfo>, QueryResult<CustomerCouponInfoViewModel>>.Convert(CustomerFacade.QueryCouponCode(query));
        }

        public static bool UpdateCustomerPersonInfo(LoginUser currUser, CustomerInfoViewModel customerInfo, out string errorMsg)
        {
            CustomerInfo personInfo = EntityConverter<CustomerInfoViewModel, CustomerInfo>.Convert(customerInfo);
            personInfo.Birthday = DateTime.Parse(customerInfo.BirthdayStr);
            errorMsg = string.Empty;
            personInfo.CustomerID = currUser.UserID;
            if (CustomerFacade.UpdateCustomerPersonInfo(personInfo))
            {
                errorMsg = "操作成功";
                return true;
            }
            else
            {
                errorMsg = "服务器繁忙，稍后重试";
                return false;
            }
        }

        public static string UploadAvatar(HttpContext context)
        {
            string filePath = string.Empty;

            string dateTimeFolderPath = string.Format("{0}/{1}", DateTime.Now.ToString("yyyy-MM"), DateTime.Now.ToString("yyyy-MM-dd"));
            string fileKey = context.Request.Form["filekey"];
            HttpPostedFile imageUploadFile = context.Request.Files[fileKey];
            if (null != imageUploadFile)
            {
                int fileLength = imageUploadFile.ContentLength;
                byte[] imageFileBytes = new byte[fileLength];
                imageUploadFile.InputStream.Read(imageFileBytes, 0, fileLength);

                string imageGuid = Guid.NewGuid().ToString();
                string uploadFileDirectory = context.Server.MapPath(string.Format("~/UploadFiles/{0}", dateTimeFolderPath));
                if (!Directory.Exists(uploadFileDirectory))
                {
                    Directory.CreateDirectory(uploadFileDirectory);
                }
                string fileName = imageGuid + Path.GetExtension(imageUploadFile.FileName);
                string uploadProductImagePath = Path.Combine(uploadFileDirectory, fileName);

                ImageUtility.ZoomAuto(imageFileBytes, uploadProductImagePath, 90, 90);

                filePath = Path.Combine(dateTimeFolderPath, fileName).Replace('\\', '/');
            }
            return filePath;
        }

        public static bool UpdateCustomerAvatar(LoginUser currUser, HttpContext context, out string fileUrl, out string errorMsg)
        {
            fileUrl = string.Empty;
            errorMsg = string.Empty;
            string filePath = UploadAvatar(context);
            if (string.IsNullOrEmpty(filePath))
            {
                errorMsg = "参数错误";
                return false;
            }
            if (CustomerFacade.ChangeCustomerAvatarImg(filePath, currUser.UserSysNo, AvtarImageStatus.D))
            {
                errorMsg = "保存头像成功，后台审核通过后可以显示！";
                fileUrl = ImageUrlHelper.BuildAvatarImageSrc(filePath);
                return true;
            }
            else
            {
                errorMsg = "服务器忙,稍后重试";
                return false;
            }
        }

        /// <summary>
        /// 检查该用户是否可以用此手机号进行手机验证
        /// </summary>
        /// <param name="currUser"></param>
        /// <param name="cellphone"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private static bool CheckValidateCellphone(LoginUser currUser, string cellphone, out string errorMsg)
        {
            errorMsg = string.Empty;
            CustomerInfo customerInfo = CustomerFacade.GetCustomerInfo(currUser.UserSysNo);
            if (customerInfo.IsPhoneValided == 1)
            {
                string cellphones = customerInfo.CellPhone;
                string temp = cellphones.Substring(3, 5);
                cellphones = cellphones.Replace(temp, "******");
                errorMsg = "你的账户已经验证手机" + cellphones + ",可能还未生效，请不要重复验证。";
                return false;
            }
            //判断手机号码是否被验证过
            if (CustomerFacade.PhoneIsValidate(cellphone))
            {
                errorMsg = "此手机号码已经被验证过,不能进行重复验证";
                return false;
            }
            return true;
        }
        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="currUser"></param>
        /// <param name="validateCellphone"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static bool SendCellphoneCode(LoginUser currUser, ValidateCellphoneViewModel validateCellphone, out string errorMsg)
        {
            errorMsg = string.Empty;
            string cellphone = validateCellphone.Cellphone;
            if (!string.IsNullOrEmpty(cellphone))
            {
                if (!cellphone.IsPhoneNo())
                {
                    errorMsg = "请输入正确的手机号码";
                    return false;
                }
                if (!(CheckValidateCellphone(currUser, cellphone, out errorMsg)))
                {
                    return false;
                }
                CellPhoneConfirm item = new CellPhoneConfirm();
                item.CustomerSysNo = currUser.UserSysNo;
                item.CellPhone = cellphone;
                VerifyImage v = new VerifyImage();
                item.ConfirmKey = v.CreateVerifyCode().ToLower();

                int CellPhoneSysNo = CustomerFacade.CreateCellPhoneConfirm(item).SysNo;
                if (CellPhoneSysNo > 0)
                {
                    errorMsg = "短信检验码已发出，请注意查收。";
                    return true;
                }
                if (CellPhoneSysNo == -99999)
                {
                    errorMsg = "同一个IP地址24小时内只能请求验证码10次，同一个手机号码请求验证码5次。";
                    return false;
                }
                errorMsg = "服务器忙,稍后重试";
                return false;
            }
            errorMsg = "参数错误";
            return false;
        }
        /// <summary>
        /// 验证用户手机号码
        /// </summary>
        /// <param name="currUser"></param>
        /// <param name="validateCellphone"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static bool ValidateCellphoneCode(LoginUser currUser, ValidateCellphoneViewModel validateCellphone, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (validateCellphone != null && !string.IsNullOrEmpty(validateCellphone.Cellphone))
            {
                var CurrUser = UserMgr.ReadUserInfo();
                Nesoft.ECWeb.Entity.Common.Point point = new Nesoft.ECWeb.Entity.Common.Point();
                point.CustomerSysNo = CurrUser.UserSysNo;
                point.AvailablePoint = ConstValue.GetPointByValidatePhone;
                point.ExpireDate = DateTime.Now.AddYears(1);
                point.InDate = DateTime.Now;
                point.InUser = CurrUser.UserID;
                point.Memo = EnumHelper.GetDescription(PointType.MobileVerification);
                point.ObtainType = (int)PointType.MobileVerification;
                point.Points = ConstValue.GetPointByValidatePhone;
                point.IsFromSysAccount = 1;
                point.SysAccount = int.Parse(ConstValue.PointAccountSysNo);


                if (!validateCellphone.Cellphone.IsPhoneNo())
                {
                    errorMsg = "请输入正确的手机号码";
                    return false;
                }
                if (string.IsNullOrEmpty(validateCellphone.SMSCode))
                {
                    errorMsg = "请输入短信校验码";
                    return false;
                }
                if (!(CheckValidateCellphone(currUser, validateCellphone.Cellphone, out errorMsg)))
                {
                    return false;
                }
                if (CustomerFacade.ValidateCustomerPhone(validateCellphone.Cellphone, validateCellphone.SMSCode.ToLower(), point))
                {
                    errorMsg = "手机已验证，稍候生效";
                    return true;
                }
                else
                {
                    errorMsg = "短信校验码不正确或不存在";
                    return false;
                }
            }
            errorMsg = "参数错误";
            return false;
        }
        /// <summary>
        /// 发送邮箱验证邮件
        /// </summary>
        /// <param name="currUser"></param>
        /// <param name="validateEmail"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static bool SendValidateEmail(LoginUser currUser, ValidateEmailViewModel validateEmail, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (validateEmail != null && !string.IsNullOrEmpty(validateEmail.Email))
            {
                if (!validateEmail.Email.IsEmail())
                {
                    errorMsg = "请输入正确的邮箱地址";
                    return false;
                }
                string imgBaseUrl = ConfigurationManager.AppSettings["CDNWebDomain"].ToString();//图片根目录
                string domain = ConfigurationManager.AppSettings["WebDomain"].ToString();

                if (CustomerFacade.SendEmailValidatorMail(currUser.UserID, validateEmail.Email, imgBaseUrl, domain))
                {
                    CustomerInfo info = CustomerFacade.GetCustomerByID(currUser.UserID);
                    if (validateEmail.Email != info.Email)
                        CustomerFacade.UpdateCustomerEmailAddress(currUser.UserID, validateEmail.Email);
                    errorMsg = "验证邮件已经发送成功，请登录您的邮箱，查收并继续完成验证。如果没有收到，您可以查看您常用邮箱的\"垃圾邮件\"文件夹。";
                    return true;
                }
                else
                {
                    errorMsg = "服务器忙,稍后重试";
                    return false;
                }
            }
            errorMsg = "参数错误";
            return false;
        }

        public static List<CodeNamePair> GetVoidOrderReasons()
        {
            return CustomerFacade.GetToVoidedOrderReasons();
        }

        public static string VoidOrder(VoidOrderRequestViewModel request)
        {
            int getCustomerSysNo = UserMgr.ReadUserInfo().UserSysNo;
            return CustomerFacade.VoidedOrder(request.SOSysNo, request.VoidMemo, getCustomerSysNo);
        }

        public static List<SOLogViewModel> GetOrderDetailLogs(int soSysNo)
        {
            var list = EntityConverter<List<SOLog>, List<SOLogViewModel>>.Convert(CustomerFacade.GetOrderDetailLogBySOSysNo(soSysNo));
            if (null != list && list.Count > 0)
            {
                foreach (var item in list)
                {
                    item.OptTimeString = item.OptTime.ToString("yyyy-MM-dd HH:mm:ss");
                    item.Note = item.Note.Replace("\n                      ", "");
                    if (item.Note.IndexOf("<ActionName>") > 0)
                    {
                        try
                        {
                            XDocument xmlDoc = XDocument.Parse(item.Note);
                            var actionNameNotes = xmlDoc.Descendants("ActionName");
                            item.Note = actionNameNotes.First().Value;
                        }
                        catch
                        {
                            //简单处理屏蔽异常
                            item.Note = string.Empty;
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 获取找回密码的方式
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static FindPasswordTypeViewModel FindPasswordType(string customerID, out string errorMsg)
        {
            errorMsg = string.Empty;
            FindPasswordTypeViewModel result = new FindPasswordTypeViewModel();
            if (!string.IsNullOrEmpty(customerID))
            {
                if (!LoginFacade.IsExistCustomer(customerID))//检查是否存在该用户名
                {
                    errorMsg = "不存在该用户";
                    return result;
                }
                int type = 0;
                CustomerInfo customerInfo = CustomerFacade.GetCustomerByID(customerID);
                if (customerInfo.IsEmailConfirmed == 1)
                {
                    type += 2;
                    string email = customerInfo.Email;
                    int x = email.IndexOf("@");
                    string account = email.Substring(0, x);
                    if (account.Length > 1)
                        account = account.Substring(1, account.Length - 1);
                    email = email.Replace(account, "******");
                    result.Email = email;
                }
                if (customerInfo.IsPhoneValided == 1)
                {
                    type += 1;
                    string cellphone = customerInfo.CellPhone;
                    string temp = cellphone.Substring(3, 5);
                    cellphone = cellphone.Replace(temp, "******");
                    result.Cellphone = cellphone;
                }
                if (type == 0)
                {
                    errorMsg = "对不起，您的账户没有验证手机或者邮箱，无法使用此功能找回密码";
                }
                result.Type = (Models.Member.FindPasswordType)type;
                return result;
            }
            errorMsg = "参数错误";
            return result;
        }
        /// <summary>
        /// 发送找回密码验证码到验证手机
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static bool SendFindPasswordCellphoneCode(string customerID, out string data, out string errorMsg)
        {
            errorMsg = string.Empty;
            data = string.Empty;
            if (!LoginFacade.IsExistCustomer(customerID))//检查是否存在该用户名
            {
                errorMsg = "不存在该用户";
                return false;
            }
            CustomerInfo customerInfo = CustomerFacade.GetCustomerByID(customerID);
            if (customerInfo.IsPhoneValided == 1)
            {
                string cell = customerInfo.CellPhone;
                if (!string.IsNullOrEmpty(cell))
                {
                    SMSInfo item = new SMSInfo();
                    item.CreateUserSysNo = customerInfo.SysNo;
                    item.CellNumber = cell;
                    item.Status = SMSStatus.NoSend;
                    item.Type = SMSType.FindPassword;
                    item.Priority = 100;
                    item.RetryCount = 0;
                    VerifyImage v = new Nesoft.ECWeb.UI.VerifyImage();
                    string code = v.CreateVerifyCode();
                    item.SMSContent = string.Format(AppSettingManager.GetSetting("SMSTemplate", "AlertConfirmPhoneCode"),
                        DateTime.Now.ToString("MM月dd日 HH:mm"), code);
                    if (Nesoft.ECWeb.Facade.CommonFacade.InsertNewSMS(item))
                    {
                        string cellphone = customerInfo.CellPhone;
                        string temp = cellphone.Substring(3, 5);
                        cellphone = cellphone.Replace(temp, "******");
                        CookieHelper.SaveCookie<string>("FindPasswordSMSCode", code);
                        data = "验证码已发送至：" + cellphone;
                        return true;
                    }
                }
                errorMsg = "服务器忙,稍后重试";
                return false;
            }
            else
            {
                errorMsg = "对不起，您的账户没有验证手机，无法使用此功能找回密码";
                return false;
            }
        }
        /// <summary>
        /// 验证找回密码验证码是否正确
        /// </summary>
        /// <param name="validateCellphone"></param>
        /// <param name="data"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static bool ValidateFindPasswordCellphoneCode(ValidateCellphoneViewModel validateCellphone, out string data, out int customerSysNo, out string errorMsg)
        {
            data = string.Empty;
            errorMsg = string.Empty;
            customerSysNo = 0;
            if (!LoginFacade.IsExistCustomer(validateCellphone.CustomerID))//检查是否存在该用户名
            {
                errorMsg = "不存在该用户";
                return false;
            }
            CustomerInfo customerInfo = CustomerFacade.GetCustomerByID(validateCellphone.CustomerID);
            if (customerInfo.IsPhoneValided == 1)
            {
                string cell = customerInfo.CellPhone;
                if (!string.IsNullOrEmpty(cell))
                {
                    if (CookieHelper.GetCookie<String>("FindPasswordSMSCode") == null)
                    {
                        errorMsg = "请重新获取手机验证码";
                        return false;
                    }
                    if (validateCellphone.SMSCode == null)
                    {
                        errorMsg = "请输入手机验证码";
                        return false;
                    }
                    //检查验证码
                    if (CookieHelper.GetCookie<String>("FindPasswordSMSCode").ToLower() == validateCellphone.SMSCode.ToLower())
                    {
                        //验证成功给用户一个token
                        string token = string.Empty;
                        token = Guid.NewGuid().ToString("N");
                        LoginFacade.CreatePasswordToken(customerInfo.SysNo, token, "E");
                        data = token;
                        customerSysNo = customerInfo.SysNo;
                        return true;
                    }
                    else
                    {
                        errorMsg = "验证码不正确";
                        return false;
                    }
                }
                errorMsg = "服务器忙,稍后重试";
                return false;
            }
            else
            {
                errorMsg = "对不起，您的账户没有验证手机，无法使用此功能找回密码";
                return false;
            }
        }
        public static bool CheckPasswordFormat(string password, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (password.Length < 6 || password.Length > 20)
            {
                errorMsg = "请输入6-20个大小写英文字母与数字的混合,可包含符号!";
                return false;
            }
            int matchCount = 0;
            //匹配数字
            var match = Regex.Match(password, "[0-9]", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                matchCount++;
            }
            //匹配字母
            match = Regex.Match(password, "[a-zA-Z]", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                matchCount++;
            }
            //匹配特殊符号
            match = Regex.Match(password, "[\\x21-\\x2f\\x3a-\\x40\\x5b-\\x60\\x7b-\\x7e]", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                matchCount++;
            }
            if (matchCount < 2)
            {
                errorMsg = "请输入6-20个大小写英文字母、符号或数字中至少两种类型以上的组合!";
                return false;
            }
            return true;
        }
        public static bool CheckPassword(ChangePasswordViewModel passwordInfo, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (passwordInfo == null)
            {
                errorMsg = "参数错误!";
                return false;
            }
            if (string.IsNullOrEmpty(passwordInfo.Password))
            {
                errorMsg = "请输入新密码!";
                return false;
            }
            if (string.IsNullOrEmpty(passwordInfo.RePassword))
            {
                errorMsg = "请输入确认密码!";
                return false;
            }
            if (passwordInfo.Password != passwordInfo.RePassword)
            {
                errorMsg = "两次输入密码不一致!";
                return false;
            }
            if (!CheckPasswordFormat(passwordInfo.Password, out errorMsg))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="findResetPassword"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static bool FindResetPassword(FindResetPasswordViewModel findResetPassword, out string errorMsg)
        {
            errorMsg = string.Empty;
            string token = findResetPassword.Token;

            CustomerPasswordTokenInfo passwordToken = LoginFacade.GetPasswordTokenInfo(token, "E");
            if (passwordToken != null || findResetPassword.CustomerSysNo > 0)
            {
                if (!CheckPassword(findResetPassword, out errorMsg))
                {
                    return false;
                }
                string Password = findResetPassword.Password;
                string rePassword = findResetPassword.RePassword;
                CustomerInfo customerInfo = CustomerFacade.GetCustomerInfo(passwordToken == null ? findResetPassword.CustomerSysNo : passwordToken.CustomerSysNo);
                if (customerInfo != null)
                {
                    string customerID = customerInfo.CustomerID;
                    string encryptPassword = string.Empty;
                    string passwordSalt = string.Empty;
                    PasswordHelper.GetNewPasswordAndSalt(ref Password, ref encryptPassword, ref passwordSalt);
                    //重置密码
                    CustomerFacade.UpdateCustomerPassword(customerID, encryptPassword, passwordSalt);
                    //更新Token状态
                    LoginFacade.UpdatePasswordToken(token);

                    errorMsg = "重置成功";
                    return true;
                }
                errorMsg = "没找到可重置密码的用户";
                return false;
            }
            errorMsg = "Token已经过期或不存在！请重新获取手机验证码！";
            return false;
        }
        /// <summary>
        /// 发送找回密码邮件
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static bool SendFindPasswordEmail(string customerID, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (!LoginFacade.IsExistCustomer(customerID))//检查是否存在该用户名
            {
                errorMsg = "不存在该用户";
                return false;
            }
            CustomerInfo customerInfo = CustomerFacade.GetCustomerByID(customerID);
            if (customerInfo.IsEmailConfirmed == 1)
            {
                if (string.IsNullOrEmpty(customerInfo.Email))
                {
                    errorMsg = "'该用户没有绑定邮箱地址";
                    return false;
                }
                string imgBaseUrl = ConfigurationManager.AppSettings["CDNWebDomain"].ToString();//图片根目录
                string domain = ConfigurationManager.AppSettings["WebDomain"].ToString();
                if (LoginFacade.SendFindPasswordMail(customerID, imgBaseUrl, domain))
                {
                    errorMsg = "找回密码验证邮件已发出，请注意查收。";
                    return true;
                }
                errorMsg = "服务器忙,稍后重试";
                return false;
            }
            else
            {
                errorMsg = "对不起，您的账户没有验证邮箱，无法使用此功能找回密码";
                return false;
            }
        }

        /// <summary>
        /// 积分获得记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PointObtainQueryResult GetPointObtainList(int pageIndex, int pageSize)
        {
            LoginUser suer = UserMgr.ReadUserInfo();
            PointQueryInfoFilter filter1 = new PointQueryInfoFilter();
            filter1.CustomerID = suer.UserSysNo;
            filter1.PagingInfo = new Nesoft.ECWeb.Entity.PageInfo();
            filter1.PagingInfo.PageSize = pageSize;
            filter1.PagingInfo.PageIndex = pageIndex;
            //积分获取记录
            var queryResult = CustomerFacade.GetPointObtainList(filter1);
            PointObtainQueryResult result = new PointObtainQueryResult();
            result.PageInfo = queryResult.PageInfo;
            result.ResultList = MapPointObtainInfo(queryResult.ResultList);
            //仅在获取积分获取记录第一页时计算总积分数
            if (pageIndex == 1)
            {
                //顾客信息
                CustomerInfo customerInfo = CustomerFacade.GetCustomerInfo(suer.UserSysNo);
                if (customerInfo != null)
                {
                    result.TotalScores = customerInfo.ValidScore;
                }
            }
            return result;
        }

        /// <summary>
        /// 积分使用记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static QueryResult<PointConsumeViewModel> GetPointConsumeList(int pageIndex, int pageSize)
        {
            LoginUser suer = UserMgr.ReadUserInfo();
            PointQueryInfoFilter filter1 = new PointQueryInfoFilter();
            filter1.CustomerID = suer.UserSysNo;
            filter1.PagingInfo = new Nesoft.ECWeb.Entity.PageInfo();
            filter1.PagingInfo.PageSize = pageSize;
            filter1.PagingInfo.PageIndex = pageIndex;

            var queryResult = CustomerFacade.GetPointConsumeList(filter1);
            QueryResult<PointConsumeViewModel> result = new QueryResult<PointConsumeViewModel>();
            result.PageInfo = queryResult.PageInfo;
            result.ResultList = MapPointConsumeInfo(queryResult.ResultList);
            return result;
        }

        private static List<PointObtainViewModel> MapPointObtainInfo(List<PointObtainInfo> pointInfoList)
        {
            List<PointObtainViewModel> result = new List<PointObtainViewModel>();
            if (pointInfoList != null)
            {
                foreach (var item in pointInfoList)
                {
                    PointObtainViewModel viewModel = new PointObtainViewModel();
                    viewModel.Point = item.Point;
                    viewModel.ObtainType = Nesoft.Utility.EnumHelper.GetDescription(item.ObtainType);
                    viewModel.CreateDate = item.CreateDate.ToString("yyyy-MM-dd");
                    viewModel.ExpireDate = item.ExpireDate.ToString("yyyy-MM-dd");

                    if (item.Status == "A")
                    {
                        viewModel.Status = "有效";
                    }
                    else
                    {
                        viewModel.Status = "无效";
                    }
                    if (item.SOSysNo != 0)
                    {
                        viewModel.Memo = "订单编号：" + item.SOSysNo.ToString();
                    }
                    result.Add(viewModel);
                }
            }

            return result;
        }

        private static List<PointConsumeViewModel> MapPointConsumeInfo(List<PointConsumeInfo> pointInfoList)
        {
            List<PointConsumeViewModel> result = new List<PointConsumeViewModel>();
            if (pointInfoList != null)
            {
                foreach (var item in pointInfoList)
                {
                    PointConsumeViewModel viewModel = new PointConsumeViewModel();
                    viewModel.Point = item.Point;
                    viewModel.ConsumeType = Nesoft.Utility.EnumHelper.GetDescription(item.ConsumeType);
                    viewModel.CreateDate = item.CreateDate.ToString("yyyy-MM-dd");
                    string memo = "";
                    if (item.ConsumeType == Nesoft.ECWeb.Enums.PointType.DisusePoint)
                    {
                        memo = "已过期，系统自动作废";
                    }
                    else if (item.ConsumeType == Nesoft.ECWeb.Enums.PointType.CreateOrder || item.ConsumeType == Nesoft.ECWeb.Enums.PointType.UpdateSO)
                    {
                        memo = "订单编号:" + item.SOSysNo.ToString();
                    }
                    else if (item.ConsumeType == Nesoft.ECWeb.Enums.PointType.GroupBuyingLotteryPoint)
                    {
                        memo = "抽奖号:" + item.Memo;
                    }
                    else
                    {
                        memo = item.Memo;
                    }
                    viewModel.Memo = memo;
                    result.Add(viewModel);
                }
            }

            return result;
        }

        #region 实名认证

        /// <summary>
        /// 实名认证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static CustomerAuthenticationInfoModel SaveAuthenticationInfo(CustomerAuthenticationInfoModel model)
        {
            if (model == null)
            {
                throw new BusinessException("无效的请求");
            }
            var customer = CustomerFacade.GetCustomerInfo(UserMgr.ReadUserInfo().UserSysNo);
            if (customer != null)
            {
                model.CustomerSysNo = customer.SysNo;
                var info = CustomerFacade.SaveCustomerAuthenticationInfo(EntityConverter<CustomerAuthenticationInfoModel, CustomerAuthenticationInfo>.Convert(model));
                return EntityConverter<CustomerAuthenticationInfo, CustomerAuthenticationInfoModel>.Convert(info);
            }
            else
            {
                throw new BusinessException("此用户不存在");
            }
        }

        #endregion

        #region 售后服务

        /// <summary>
        /// 查询售后List
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="search_ordertype">按哪种查询：值为requestsysno按售后单号，值为sosysno按订单编号</param>
        /// <param name="txtSearch">售后单号或者订单编号</param>
        /// <returns></returns>
        public QueryResult<RMARequestInfoModel> RMAQuery(int pageIndex, int pageSize)
        {
            //用户信息
            LoginUser suer = UserMgr.ReadUserInfo();
            CustomerInfo customerInfo = CustomerFacade.GetCustomerInfo(suer.UserSysNo);

            List<CodeNamePair> ShipTypeList = CustomerFacade.GetShipTypes();
            List<CodeNamePair> RMAReasonList = CustomerFacade.GetRMAReasons();
            List<CodeNamePair> RequestList = CustomerFacade.GetRequests();

            RMAQueryInfo query = new RMAQueryInfo();
            query.PagingInfo = new PageInfo();
            query.PagingInfo.PageSize = pageSize;
            query.PagingInfo.PageIndex = pageIndex;
            query.CustomerSysNo = suer.UserSysNo;

            string keyType = HttpContext.Current.Request.QueryString["search_ordertype"];
            string txtSearch = HttpContext.Current.Request.QueryString["txtSearch"];

            if (!string.IsNullOrWhiteSpace(keyType))
            {
                if (!string.IsNullOrWhiteSpace(txtSearch))
                {
                    if (keyType.Equals("requestsysno", StringComparison.InvariantCultureIgnoreCase))
                    {
                        query.RequestID = txtSearch;
                    }
                    else
                    {
                        int SOSysNo = 0;
                        if (!int.TryParse(txtSearch, out SOSysNo))
                        {
                            query.SOSysNo = -1;
                        }
                        else
                        {
                            query.SOSysNo = SOSysNo;
                        }
                    }
                }
            }

            QueryResult<RMARequestInfo> originList = Nesoft.ECWeb.Facade.RMA.RMARequestFacade.QueryRequestInfos(query);

            var resultList = EntityConverter<QueryResult<RMARequestInfo>, QueryResult<RMARequestInfoModel>>.Convert(originList);
            if (resultList.ResultList != null && resultList.ResultList.Count > 0)
            {
                foreach (var RMAitem in resultList.ResultList)
                {
                    foreach (var item in RMAitem.Registers)
                    {
                        ImageSize imageSize = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(Nesoft.ECWeb.MobileService.Models.Product.ImageType.Small);
                        item.DefaultImage = ProductFacade.BuildProductImage(imageSize, item.DefaultImage);
                    }
                }
            }

            return resultList;

        }

        /// <summary>
        /// 售后申请单详情
        /// </summary>
        /// <param name="sysno">售后单系统编号</param>
        /// <returns></returns>
        public RMARequestDetailModel RMARequestDetail(int sysNo)
        {
            RMARequestDetailModel result = new RMARequestDetailModel();

            //用户信息
            LoginUser suer = UserMgr.ReadUserInfo();
            CustomerInfo customerInfo = CustomerFacade.GetCustomerInfo(suer.UserSysNo);
            //List<CodeNamePair> ShipTypeList = CustomerFacade.GetShipTypes();
            List<CodeNamePair> RMAReasonList = CustomerFacade.GetRMAReasons();
            List<CodeNamePair> RequestList = CustomerFacade.GetRequests();
            List<CodeNamePair> RequestStatusList = CustomerFacade.GetRMARequestStatus();
            List<CodeNamePair> RMARevertStatus = CustomerFacade.GetRMARevertStatus();
            //***************************************

            RMAQueryInfo query = new RMAQueryInfo();
            query.PagingInfo = new PageInfo();
            query.PagingInfo.PageSize = 1;
            query.PagingInfo.PageIndex = 1;
            query.CustomerSysNo = suer.UserSysNo;
            query.SysNo = sysNo;

            QueryResult<RMARequestInfo> originList = Nesoft.ECWeb.Facade.RMA.RMARequestFacade.QueryRequestInfos(query);

            RMARequestInfo requestInfo = null;
            Nesoft.ECWeb.Entity.Store.StoreBasicInfo store = new Entity.Store.StoreBasicInfo();
            Area area = new Area();
            if (originList.ResultList != null && originList.ResultList.Count > 0)
            {
                //售后基本信息
                requestInfo = originList.ResultList[0];
                //商家信息
                store = Nesoft.ECWeb.Facade.Store.StoreFacade.QueryStoreBasicInfo(requestInfo.VendorSysNo);
                //地区省市区
                area = Nesoft.ECWeb.Facade.CommonFacade.GetArea(requestInfo.AreaSysNo.Value);
                if (area != null)
                    requestInfo.Address = area.ProvinceName + area.CityName + area.DistrictName + requestInfo.Address;

                foreach (var item in requestInfo.Registers)
                {
                    ImageSize imageSize = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(Nesoft.ECWeb.MobileService.Models.Product.ImageType.Small);
                    item.DefaultImage = ProductFacade.BuildProductImage(imageSize, item.DefaultImage);

                    item.RequestTypeString = RequestList.Find(s => s.Code.Equals(item.RequestType.ToString())).Name;
                    item.RMAReasonString = RMAReasonList.Find(s => s.Code.Equals(item.RMAReason.ToString())).Name;
                    item.StatusString = RequestStatusList.Find(s => s.Code.Equals(((int)item.Status).ToString())).Name;
                }
            }
            else
            {
                requestInfo = new RMARequestInfo();
            }
            //售后基本信息
            RMARequestInfoModel rmarequestInfo = EntityConverter<RMARequestInfo, RMARequestInfoModel>.Convert(requestInfo);
            result.RMARequestInfo = rmarequestInfo;
            //商家信息
            StoreBasicInfoModel storebasicInfo = EntityConverter<Nesoft.ECWeb.Entity.Store.StoreBasicInfo, StoreBasicInfoModel>.Convert(store);
            result.StoreBasicInfo = storebasicInfo;
            //地区省市区
            AreaViewModel areaView = EntityConverter<Area, AreaViewModel>.Convert(area);
            result.AreaView = areaView;


            return result;
        }

        /// <summary>
        /// 售后明细
        /// </summary>
        /// <param name="registSysNo">明细系统编号</param>
        /// <param name="requestSysNo">售后单系统编号</param>
        /// <returns></returns>
        public RMARegisterDetailModel RMARegisterDetail(int registSysNo, int requestSysNo)
        {
            RMARegisterDetailModel result = new RMARegisterDetailModel();
            //用户信息
            LoginUser suer = UserMgr.ReadUserInfo();
            CustomerInfo customerInfo = CustomerFacade.GetCustomerInfo(suer.UserSysNo);

            List<CodeNamePair> RequestList = CustomerFacade.GetRequests();
            //***************************************

            RMARegisterInfo register = Nesoft.ECWeb.Facade.RMA.RMARequestFacade.LoadRegisterByRegisterSysNo(registSysNo);

            RMAQueryInfo query = new RMAQueryInfo();
            query.PagingInfo = new PageInfo();
            query.PagingInfo.PageSize = 1;
            query.PagingInfo.PageIndex = 1;
            query.CustomerSysNo = suer.UserSysNo;
            query.SysNo = requestSysNo;
            QueryResult<RMARequestInfo> originList = Nesoft.ECWeb.Facade.RMA.RMARequestFacade.QueryRequestInfos(query);
            RMARequestInfo requestInfo = null;
            Area area = new Area();
            if (originList.ResultList != null && originList.ResultList.Count > 0)
            {
                //售后基本信息
                requestInfo = originList.ResultList[0];
                //地区省市区
                area = Nesoft.ECWeb.Facade.CommonFacade.GetArea(requestInfo.AreaSysNo.Value);
                if (area != null)
                    requestInfo.Address = area.ProvinceName + area.CityName + area.DistrictName + requestInfo.Address;
                foreach (var item in requestInfo.Registers)
                {
                    ImageSize imageSize = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(Nesoft.ECWeb.MobileService.Models.Product.ImageType.Small);
                    item.DefaultImage = ProductFacade.BuildProductImage(imageSize, item.DefaultImage);

                    item.RequestTypeString = RequestList.Find(s => s.Code.Equals(item.RequestType.ToString())).Name;
                }
            }
            else
            {
                requestInfo = new RMARequestInfo();
            }
            #region 售后服务状态
            register.StatusString = Nesoft.Utility.EnumHelper.GetDescription(register.Status);
            if (register.RequestType == (int)RMARequestType.Return)
            {
                if (register.RefundStatus.HasValue)
                {
                    register.StatusString = register.RefundStatus.Value.GetDescription();
                }
            }
            else if (register.RequestType == (int)RMARequestType.Exchange)
            {
                if (register.RevertStatus.HasValue)
                {
                    register.StatusString = register.RevertStatus.Value.GetDescription();
                }
            }
            #endregion
            //售后基本信息
            RMARequestInfoModel rmarequestInfo = EntityConverter<RMARequestInfo, RMARequestInfoModel>.Convert(requestInfo);
            result.RMARequestInfo = rmarequestInfo;

            //售后明细
            RMARegisterInfoModel rmaregisterInfo = EntityConverter<RMARegisterInfo, RMARegisterInfoModel>.Convert(register);
            result.RMARegisterInfo = rmaregisterInfo;

            return result;
        }

        /// <summary>
        /// 获取需要申请售后订单的商品列表
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        public List<SOItemInfoModel> RMANewRequestProductList(int soSysNo)
        {
            List<SOItemInfoModel> SOItemInfoList = new List<SOItemInfoModel>();
            //用户信息
            LoginUser suer = UserMgr.ReadUserInfo();
            CustomerInfo customerInfo = CustomerFacade.GetCustomerInfo(suer.UserSysNo);

            ////收货地址
            //CheckOutResult result = new CheckOutResult();
            //var custShippingAddressListResult = ShoppingFacade.GetCustomerShippingAddressList(null, suer.UserSysNo);
            //result.ShippingAddressList = custShippingAddressListResult.ShippingAddressList;
            //result.SelShippingAddress = custShippingAddressListResult.SelShippingAddress;

            OrderInfo currentOrderInfo = RMARequestFacade.GetCanRequestOrder(customerInfo.SysNo, soSysNo);
            //***************************************
            var rmaSOItemList = new List<SOItemInfo>();
            if (currentOrderInfo != null && currentOrderInfo.SOItemList != null)
            {
                for (int i = 0; i < currentOrderInfo.SOItemList.Count; i++)
                {
                    var soItem = currentOrderInfo.SOItemList[i];
                    for (int j = 0; j < currentOrderInfo.SOItemList[i].Quantity; j++)
                    {
                        var clonedSOItem = new SOItemInfo();
                        clonedSOItem.ProductSysNo = soItem.ProductSysNo;
                        clonedSOItem.ProductType = soItem.ProductType;
                        clonedSOItem.ProductName = soItem.ProductName;
                        clonedSOItem.ProductTitle = soItem.ProductTitle;
                        ImageSize imageSize = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(Nesoft.ECWeb.MobileService.Models.Product.ImageType.Middle);
                        clonedSOItem.DefaultImage = ProductFacade.BuildProductImage(imageSize, soItem.DefaultImage);
                        clonedSOItem.Quantity = 1;
                        clonedSOItem.MerchantSysNo = currentOrderInfo.MerchantSysNo;
                        rmaSOItemList.Add(clonedSOItem);
                    }
                }
            }
            SOItemInfoList = EntityConverter<List<SOItemInfo>, List<SOItemInfoModel>>.Convert(rmaSOItemList);

            return SOItemInfoList;
        }

        /// <summary>
        /// 创建售后申请
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static AjaxResult CreateRMARequestInfo(RMARequestInfoModel request)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.Success = false;
            ajaxResult.Code = -1;
            if (request == null)
            {
                ajaxResult.Code = -2;
                ajaxResult.Message = "无效的请求";
                return ajaxResult;
            }

            //用户信息
            LoginUser suer = UserMgr.ReadUserInfo();
            request.CustomerSysNo = suer.UserSysNo;
            request.IsSubmit = true;
            request.Registers.RemoveAll(s => s == null);
            RMARequestInfo requestInfo = EntityConverter<RMARequestInfoModel, RMARequestInfo>.Convert(request);

            string requestrSysno = RMARequestFacade.CreateRMARequest(requestInfo);
            ajaxResult.Code = 0;
            ajaxResult.Success = true;
            ajaxResult.Data = requestrSysno;
            return ajaxResult;
        }

        public static QueryResult<OrderListItemViewModel> QueryCanRequestOrders(int pageIndex, int pageSize)
        {
            //用户信息
            LoginUser suer = UserMgr.ReadUserInfo();

            #region 订单 （All、已出库）
            RMAQueryInfo query = new RMAQueryInfo();
            query.PagingInfo = new PageInfo();
            query.PagingInfo.PageSize = pageSize;
            query.PagingInfo.PageIndex = pageIndex;
            query.CustomerSysNo = suer.UserSysNo;
            QueryResult<OrderInfo> orders = RMARequestFacade.QueryCanRequestOrders(query);
            return EntityConverter<QueryResult<OrderInfo>, QueryResult<OrderListItemViewModel>>.Convert(orders, (s, t) =>
            {
                if (null != s.ResultList && s.ResultList.Count > 0)
                {
                    for (int i = 0; i < s.ResultList.Count; i++)
                    {
                        t.ResultList[i].CanCancelOrder = (t.ResultList[i].Status == SOStatus.Original && t.ResultList[i].IsNetPay);
                        t.ResultList[i].CanPayOrder = CustomerFacade.IsShowPay(s.ResultList[i]);

                        t.ResultList[i].OrderDateString = s.ResultList[i].OrderDate.ToString("yyyy-MM-dd");
                        t.ResultList[i].DeliveryDateString = s.ResultList[i].DeliveryDate.ToString("yyyy-MM-dd HH:mm:ss");

                        if (t.ResultList[i].SOItemList != null && t.ResultList[i].SOItemList.Count > 0)
                        {
                            for (int j = 0; j < t.ResultList[i].SOItemList.Count; j++)
                            {
                                ImageSize imageSize = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(Nesoft.ECWeb.MobileService.Models.Product.ImageType.Small);
                                t.ResultList[i].SOItemList[j].DefaultImage = ProductFacade.BuildProductImage(imageSize, t.ResultList[i].SOItemList[j].DefaultImage);
                            }
                        }

                    }
                }

            });

            #endregion
        }
        #endregion
    }
}
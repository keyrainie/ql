using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.SO.BizProcessor;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using System.Text;
using ECCentral.Service.SO.BizProcessor.SO;
using ECCentral.BizEntity.IM;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;

namespace ECCentral.Service.SO.AppService
{
    public class SOPrintData : IPrintDataBuild
    {
        public void BuildData(System.Collections.Specialized.NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();
            string soSysNos = requestPostData["SOSysNoList"];
            if (soSysNos != null && soSysNos.Trim() != String.Empty)
            {
                string[] noList = System.Web.HttpUtility.UrlDecode(soSysNos).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                List<int> soSysNoList = new List<int>();
                noList.ForEach(no =>
                {
                    int tno = 0;
                    if (int.TryParse(no, out tno))
                    {
                        soSysNoList.Add(tno);
                    }
                });

                if (soSysNoList.Count > 0)
                {
                    List<SOInfo> soInfoList = ObjectFactory<SOProcessor>.Instance.GetSOBySOSysNoList(soSysNoList);
                    System.Data.DataTable dtSO = new System.Data.DataTable();
                    dtSO.Columns.AddRange(new System.Data.DataColumn[] 
                    { 
                        new DataColumn("BarCodeImage"), 
                        new DataColumn("SelfCompanyName"), 
                        new DataColumn("SelfCompanyAddress"), 
                        new DataColumn("SelfCompanyPhone"), 
                        new DataColumn("SelfCompanyWebAddress"), 
                        new DataColumn("OriginalReceivableAmount"), 
                        new DataColumn("SOSysNo"), 
                        new DataColumn("Pager"), 
                        new DataColumn("SheetNumberDisplay",typeof(bool)), 
                        new DataColumn("SheetNumber"), 
                        new DataColumn("InvoiceHeader"), 
                        new DataColumn("ReceiveAddress"), 
                        new DataColumn("ReceiveName"),
                        new DataColumn("ReceivePhone"), 
                        new DataColumn("VATDisplay"), 
                        new DataColumn("VATNo"), 
                        new DataColumn("CompanyName"), 
                        new DataColumn("CompanyAddress"), 
                        new DataColumn("CompanyPhone"), 
                        new DataColumn("OpenAccountBank"), 
                        new DataColumn("SOStatus"), 
                        new DataColumn("PayType"), 
                        new DataColumn("ShipType"), 
                        new DataColumn("CreateUser"), 
                        new DataColumn("CreateDate"), 
                        new DataColumn("OutTime"),
                        new DataColumn("AuditUser"), 
                        new DataColumn("AuditDate"), 
                        new DataColumn("Memo"), 
                        new DataColumn("Note"), 
                        new DataColumn("InvoiceNote"), 
                        new DataColumn("ReceivableAmount"), 
                        new DataColumn("ProductList",typeof(DataTable)), 
                        new DataColumn("ComboList",typeof(DataTable)),
                        new DataColumn("ShipPriceDisplay",typeof(bool)),
                        new DataColumn("ShipPrice"),
                        new DataColumn("PremiumAmountDisplay",typeof(bool)),
                        new DataColumn("PremiumAmount"),
                        new DataColumn("PayPriceDisplay",typeof(bool)),
                        new DataColumn("PayPrice"),
                        new DataColumn("PointPayDisplay",typeof(bool)),
                        new DataColumn("PointPay"),
                        new DataColumn("ChangeAmountDisplay",typeof(bool)),
                        new DataColumn("ChangeAmount"),
                    });

                    tableVariables.Add("OrderList", dtSO);

                    Dictionary<int, ECCentral.BizEntity.Inventory.WarehouseInfo> dicStock = new Dictionary<int, BizEntity.Inventory.WarehouseInfo>();
                    soInfoList.ForEach(soInfo =>
                    {
                        #region 订单基本属性
                        DataRow dr = dtSO.NewRow();
                        ECCentral.BizEntity.Common.PayType payType = ExternalDomainBroker.GetPayTypeBySysNo(soInfo.BaseInfo.PayTypeSysNo.Value);
                        ECCentral.BizEntity.Common.ShippingType shippingType = ExternalDomainBroker.GetShippingTypeBySysNo(soInfo.ShippingInfo.ShipTypeSysNo.Value);
                        dr["BarCodeImage"] = GetBarCodeUrl(soInfo.SysNo.ToString());
                        dr["SelfCompanyName"] = ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Name");
                        dr["SelfCompanyAddress"] = ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Address");
                        dr["SelfCompanyPhone"] = ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Phone");
                        dr["SelfCompanyWebAddress"] = ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Url");
                        dr["OriginalReceivableAmount"] = soInfo.BaseInfo.PayWhenReceived.Value ? ECCentral.Service.SO.BizProcessor.UtilityHelper.ToMoney(soInfo.BaseInfo.OriginalReceivableAmount) : 0M;
                        dr["SOSysNo"] = soInfo.SysNo;
                        dr["Pager"] = "";
                        dr["SheetNumberDisplay"] = soInfo.InvoiceInfo.IsVAT.Value;
                        dr["InvoiceHeader"] = soInfo.InvoiceInfo.Header;
                        AreaInfo receiveAreaInfo = ExternalDomainBroker.GetAreaInfoByDistrictSysNo(soInfo.ReceiverInfo.AreaSysNo.Value);
                        dr["ReceiveAddress"] = receiveAreaInfo == null ? soInfo.ReceiverInfo.Address : String.Format("{0},{1},{2},{3}{4}", receiveAreaInfo.ProvinceName, receiveAreaInfo.CityName, receiveAreaInfo.DistrictName, soInfo.ReceiverInfo.Address, soInfo.ReceiverInfo.Zip);
                        dr["ReceiveName"] = soInfo.ReceiverInfo.Name;
                        soInfo.ReceiverInfo.MobilePhone = soInfo.ReceiverInfo.MobilePhone == null ? string.Empty : soInfo.ReceiverInfo.MobilePhone.Trim();
                        soInfo.ReceiverInfo.Phone = soInfo.ReceiverInfo.Phone == null ? string.Empty : soInfo.ReceiverInfo.Phone.Trim();
                        string phone = soInfo.ReceiverInfo.MobilePhone == soInfo.ReceiverInfo.Phone ? soInfo.ReceiverInfo.MobilePhone : String.Format("{0},{1}", soInfo.ReceiverInfo.MobilePhone, soInfo.ReceiverInfo.Phone);
                        dr["ReceivePhone"] = phone.Trim(',');
                        dr["VATDisplay"] = soInfo.InvoiceInfo.IsVAT.Value;
                        dr["SOStatus"] = soInfo.BaseInfo.Status.ToDisplayText();
                        dr["PayType"] = payType.PayTypeName;
                        dr["ShipType"] = shippingType.ShippingTypeName.StartsWith("(*) ") ? shippingType.ShippingTypeName.Substring(4) : shippingType.ShippingTypeName;
                        dr["CreateDate"] = soInfo.BaseInfo.CreateTime.Value.ToString(SOConst.DateFormat);
                        //dr["OutTime"] = soInfo.ShippingInfo.OutTime.Value.ToString(SOConst.DateFormat);
                        dr["OutTime"] = soInfo.ShippingInfo.OutTime.HasValue ? soInfo.ShippingInfo.OutTime.Value.ToString(SOConst.DateFormat) : "";
                        dr["Memo"] = soInfo.BaseInfo.Memo;
                        dr["Note"] = soInfo.BaseInfo.Note;
                        dr["InvoiceNote"] = soInfo.InvoiceInfo.InvoiceNote;
                        dr["ReceivableAmount"] = ECCentral.Service.SO.BizProcessor.UtilityHelper.ToMoney(soInfo.BaseInfo.ReceivableAmount);
                        UserInfo userInfo = soInfo.BaseInfo.SalesManSysNo.HasValue ? ExternalDomainBroker.GetUserInfoBySysNo(soInfo.BaseInfo.SalesManSysNo.Value) : null;
                        string defaultUserName = ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Creater_Name");
                        dr["CreateUser"] = userInfo == null ? defaultUserName : userInfo.UserDisplayName;
                        if (soInfo.StatusChangeInfoList != null && soInfo.StatusChangeInfoList.Count > 0)
                        {
                            SOStatusChangeInfo statusChangeInfo = soInfo.StatusChangeInfoList.FirstOrDefault<SOStatusChangeInfo>(info => info.OldStatus == SOStatus.Origin);
                            if (statusChangeInfo != null)
                            {
                                dr["AuditDate"] = statusChangeInfo.ChangeTime.Value.ToString(SOConst.DateFormat);
                                userInfo = soInfo.BaseInfo.SalesManSysNo.HasValue ? ExternalDomainBroker.GetUserInfoBySysNo(statusChangeInfo.OperatorSysNo.Value) : null;
                                dr["AuditUser"] = userInfo == null ? defaultUserName : userInfo.UserDisplayName;
                            }
                        }

                        #endregion

                        #region 填充增值税发票相关信息
                        if (soInfo.InvoiceInfo.IsVAT.Value)
                        {
                            dr["VATNo"] = soInfo.InvoiceInfo.VATInvoiceInfo.TaxNumber;
                            dr["CompanyName"] = soInfo.InvoiceInfo.VATInvoiceInfo.CompanyName;
                            dr["CompanyAddress"] = soInfo.InvoiceInfo.VATInvoiceInfo.CompanyAddress;
                            dr["CompanyPhone"] = soInfo.InvoiceInfo.VATInvoiceInfo.CompanyPhone;
                            dr["OpenAccountBank"] = soInfo.InvoiceInfo.VATInvoiceInfo.BankAccount;
                            string number = ExternalDomainBroker.GetSystemConfigurationValue("SOVatNumber", soInfo.CompanyCode);
                            string flownumber = "";
                            string updateNumber = string.Empty;

                            if (number == string.Empty)
                            {
                                number = "0";
                                flownumber = "1";
                            }
                            else
                            {
                                flownumber = Convert.ToString(int.Parse(number) + 1);

                            }
                            //老系统中的第三个参数User.SysNo由后台添加
                            //commonBizInteract.UpdateSystemConfigurationValue("SOVatNumber", flownumber, soInfo.CompanyCode);

                            //加入SO单以前的流水号
                            string oldInvoiceNumber = soInfo.InvoiceInfo.InvoiceNo;
                            if (oldInvoiceNumber != null)
                            {
                                if (oldInvoiceNumber.Trim().Length > 0)
                                {
                                    updateNumber = oldInvoiceNumber.ToString().Trim() + "," + number;
                                }
                                else
                                {
                                    updateNumber = number;
                                }
                            }
                            //CommonService.UpdateSoMaster(soMastInfo.SystemNumber, updateNumber);

                            dr["SheetNumber"] = number;
                        }
                        #endregion

                        #region  填充商品
                        DataTable dtProduct = new DataTable();
                        dtProduct.Columns.AddRange(new System.Data.DataColumn[] 
                        { 
                            new DataColumn("ProductID"), 
                            new DataColumn("ProductName"), 
                            new DataColumn("Price"), 
                            new DataColumn("Quantity"), 
                            new DataColumn("Amount"), 
                            new DataColumn("Tax"), 
                            new DataColumn("Stock")
                        });
                        soInfo.Items.ForEach(p =>
                        {
                            if (!dicStock.ContainsKey(p.StockSysNo.Value))
                            {
                                dicStock.Add(p.StockSysNo.Value, ExternalDomainBroker.GetWarehouseInfo(p.StockSysNo.Value));
                            }
                            DataRow drProduct = dtProduct.NewRow();
                            drProduct["ProductID"] = p.ProductType == SOProductType.Coupon || p.ProductType == SOProductType.ExtendWarranty ? p.ProductSysNo.ToString() : p.ProductID;
                            drProduct["ProductName"] = p.ProductName;
                            drProduct["Price"] = ECCentral.Service.SO.BizProcessor.UtilityHelper.ToMoney(p.OriginalPrice.Value);
                            drProduct["Quantity"] = p.Quantity;
                            drProduct["Amount"] = ECCentral.Service.SO.BizProcessor.UtilityHelper.ToMoney(p.Quantity.Value * p.OriginalPrice.Value);
                            drProduct["Tax"] = ECCentral.Service.SO.BizProcessor.UtilityHelper.ToMoney(p.TariffAmount.Value);
                            drProduct["Stock"] = dicStock[p.StockSysNo.Value] == null ? String.Empty : dicStock[p.StockSysNo.Value].WarehouseName;
                            dtProduct.Rows.Add(drProduct);
                        });
                        dr["ProductList"] = dtProduct;
                        #endregion

                        #region 促销优惠：组合销售
                        List<SOPromotionInfo> comboList = (from promotion in soInfo.SOPromotions
                                                           where promotion.PromotionType == SOPromotionType.Combo
                                                           select promotion).ToList();
                        if (comboList.Count > 0)
                        {
                            List<ECCentral.BizEntity.MKT.ComboInfo> comboInfoList = ExternalDomainBroker.GetComboList(comboList.Select<SOPromotionInfo, int>(p => p.PromotionSysNo.Value).ToList<int>());
                            DataTable dtCombo = new DataTable();
                            dtCombo.Columns.AddRange(new System.Data.DataColumn[] 
                            {  
                                new DataColumn("ProductName"),  
                                new DataColumn("Quantity"), 
                                new DataColumn("Amount"),  
                            });
                            comboList.ForEach(combo =>
                            {
                                ECCentral.BizEntity.MKT.ComboInfo comboInfo = comboInfoList.FirstOrDefault(cb => cb.SysNo == combo.PromotionSysNo);
                                DataRow drCombo = dtCombo.NewRow();
                                drCombo["ProductName"] = comboInfo.Name.Content;
                                drCombo["Quantity"] = combo.Time;
                                drCombo["Amount"] = combo.DiscountAmount.Value.ToString(SOConst.DecimalFormat);
                                dtCombo.Rows.Add(drCombo);
                            });
                            dr["ComboList"] = dtCombo;
                        }
                        #endregion

                        #region 运费
                        dr["ShipPriceDisplay"] = soInfo.BaseInfo.ShipPrice.HasValue && soInfo.BaseInfo.ShipPrice > 0M;
                        if (soInfo.BaseInfo.ShipPrice > 0)
                        {
                            dr["ShipPrice"] = ECCentral.Service.SO.BizProcessor.UtilityHelper.ToMoney(soInfo.BaseInfo.ShipPrice.Value);
                        }
                        #endregion

                        #region 保价费
                        dr["PremiumAmountDisplay"] = soInfo.BaseInfo.PremiumAmount.HasValue && soInfo.BaseInfo.PremiumAmount > 0M;
                        if (soInfo.BaseInfo.PremiumAmount > 0)
                        {
                            dr["PremiumAmount"] = ECCentral.Service.SO.BizProcessor.UtilityHelper.ToMoney(soInfo.BaseInfo.PremiumAmount.Value);
                        }
                        #endregion

                        #region 手续费
                        dr["PayPriceDisplay"] = soInfo.BaseInfo.PayPrice.HasValue && soInfo.BaseInfo.PayPrice > 0M;
                        if (soInfo.BaseInfo.PayPrice > 0)
                        {
                            dr["PayPrice"] = ECCentral.Service.SO.BizProcessor.UtilityHelper.ToMoney(soInfo.BaseInfo.PayPrice.Value);
                        }
                        #endregion

                        #region 积分抵扣
                        dr["PointPayDisplay"] = soInfo.BaseInfo.PointPay.HasValue && soInfo.BaseInfo.PointPay > 0;
                        if (soInfo.BaseInfo.PointPay > 0)
                        {
                            dr["PointPay"] = ECCentral.Service.SO.BizProcessor.UtilityHelper.ToMoney(soInfo.BaseInfo.PointPayAmount.Value);
                        }
                        #endregion

                        #region 去零头
                        decimal fen = ECCentral.Service.SO.BizProcessor.UtilityHelper.ToMoney(soInfo.BaseInfo.OriginalReceivableAmount) - soInfo.BaseInfo.ReceivableAmount;
                        dr["ChangeAmountDisplay"] = fen > 0M;
                        if (fen > 0)
                        {
                            dr["ChangeAmount"] = ECCentral.Service.SO.BizProcessor.UtilityHelper.ToMoney(fen);
                        }
                        #endregion

                        dtSO.Rows.Add(dr);
                    });
                }
            }

        }
        protected string GetBarCodeUrl(string sysno)
        {
            string strSysNo = sysno;
            if (strSysNo == null)
            {
                return "";
            }

            string barCodePath = System.Web.HttpContext.Current.Server.MapPath("~/Style/Images/BarCode");
            if (!Directory.Exists(barCodePath))
            {
                Directory.CreateDirectory(barCodePath);
            }
            string barCodeFontPath = barCodePath + @"\Font\free3of9.ttf";
            if (!Directory.Exists(barCodePath + @"\Font"))
            {
                Directory.CreateDirectory(barCodeFontPath);
            }
            string barCodePicPath = barCodePath + @"\Pics\";
            if (!Directory.Exists(barCodePath + @"\Pics"))
            {
                Directory.CreateDirectory(barCodePicPath);
            }
            string retStr;
            string phyFileName = "";
            try
            {
                Code39 c39 = new Code39();
                c39.FontFamilyName = "Free 3 of 9";
                c39.FontFileName = barCodeFontPath;
                c39.FontSize = 50;
                c39.FontHeight = 30;
                c39.ShowCodeString = false;
                c39.Title = "";
                Bitmap objBitmap = c39.GenerateBarcode("*" + strSysNo + "*");
                phyFileName = barCodePicPath + strSysNo + ".jpg";
                objBitmap.Save(phyFileName, ImageFormat.Jpeg);
                retStr = "Style/Images/barcode/pics/" + strSysNo + ".jpg";
            }
            catch (Exception ex)
            {
                retStr = ex.Message + "and" + phyFileName;
            }
            return retStr;
        }
    }

    public class Code39
    {
        private const int _itemSepHeight = 3;

        SizeF _titleSize = SizeF.Empty;
        SizeF _barCodeSize = SizeF.Empty;
        SizeF _codeStringSize = SizeF.Empty;

        #region Barcode Title

        private string _titleString = null;
        private Font _titleFont = null;

        public string Title
        {
            get { return _titleString; }
            set { _titleString = value; }
        }

        public Font TitleFont
        {
            get { return _titleFont; }
            set { _titleFont = value; }
        }
        #endregion

        #region Barcode code string

        private bool _showCodeString = false;
        private Font _codeStringFont = null;

        public bool ShowCodeString
        {
            get { return _showCodeString; }
            set { _showCodeString = value; }
        }

        public Font CodeStringFont
        {
            get { return _codeStringFont; }
            set { _codeStringFont = value; }
        }
        #endregion

        #region Barcode Font

        private Font _c39Font = null;
        private float _c39FontSize = 12;
        private float _c39FontHeight = 20;
        private string _c39FontFileName;//= AppSettingManager.GetSetting(SOConst.DomainName, "BarCodeFontFile");
        private string _c39FontFamilyName;// = AppSettingManager.GetSetting(SOConst.DomainName, "BarCodeFontFamily");

        public string FontFileName
        {
            get { return _c39FontFileName; }
            set { _c39FontFileName = value; }
        }

        public string FontFamilyName
        {
            get { return _c39FontFamilyName; }
            set { _c39FontFamilyName = value; }
        }

        public float FontSize
        {
            get { return _c39FontSize; }
            set { _c39FontSize = value; }
        }

        public float FontHeight
        {
            get { return _c39FontHeight; }
            set { _c39FontHeight = value; }
        }

        private Font Code39Font
        {
            get
            {
                if (_c39Font == null)
                {
                    // Load the barcode font			
                    PrivateFontCollection pfc = new PrivateFontCollection();
                    pfc.AddFontFile(_c39FontFileName);
                    FontFamily family = new FontFamily(_c39FontFamilyName, pfc);
                    _c39Font = new Font(family, _c39FontSize);
                }
                return _c39Font;
            }
        }

        #endregion

        public Code39()
        {
            _titleFont = new Font("Arial", 10);
            _codeStringFont = new Font("Arial", 10);
        }

        #region Barcode Generation

        public Bitmap GenerateBarcode(string barCode)
        {

            int bcodeWidth = 0;
            int bcodeHeight = 0;

            // Get the image container...
            Bitmap bcodeBitmap = CreateImageContainer(barCode, ref bcodeWidth, ref bcodeHeight);
            Graphics objGraphics = Graphics.FromImage(bcodeBitmap);

            // Fill the background			
            objGraphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, bcodeWidth, bcodeHeight));

            int vpos = 0;

            // Draw the title string
            if (_titleString != null)
            {
                objGraphics.DrawString(_titleString, _titleFont, new SolidBrush(Color.Black), XCentered((int)_titleSize.Width, bcodeWidth), vpos);
                vpos += (((int)_titleSize.Height) + _itemSepHeight);
            }
            // Draw the barcode
            objGraphics.DrawString(barCode, Code39Font, new SolidBrush(Color.Black), XCentered((int)_barCodeSize.Width, bcodeWidth), vpos);

            // Draw the barcode string
            if (_showCodeString)
            {
                vpos += (((int)_barCodeSize.Height));
                objGraphics.DrawString(barCode, _codeStringFont, new SolidBrush(Color.Black), XCentered((int)_codeStringSize.Width, bcodeWidth), vpos);
            }

            // return the image...									
            return bcodeBitmap;
        }

        private Bitmap CreateImageContainer(string barCode, ref int bcodeWidth, ref int bcodeHeight)
        {

            Graphics objGraphics;

            // Create a temporary bitmap...
            Bitmap tmpBitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            objGraphics = Graphics.FromImage(tmpBitmap);

            // calculate size of the barcode items...
            if (_titleString != null)
            {
                _titleSize = objGraphics.MeasureString(_titleString, _titleFont);
                bcodeWidth = (int)_titleSize.Width;
                bcodeHeight = (int)_titleSize.Height + _itemSepHeight;
            }

            _barCodeSize = objGraphics.MeasureString(barCode, Code39Font);

            _barCodeSize.Height = this.FontHeight;

            bcodeWidth = Max(bcodeWidth, (int)_barCodeSize.Width);
            bcodeHeight += (int)_barCodeSize.Height;

            if (_showCodeString)
            {
                _codeStringSize = objGraphics.MeasureString(barCode, _codeStringFont);
                bcodeWidth = Max(bcodeWidth, (int)_codeStringSize.Width);
                bcodeHeight += (_itemSepHeight + (int)_codeStringSize.Height);
            }

            // dispose temporary objects...
            objGraphics.Dispose();
            tmpBitmap.Dispose();

            return (new Bitmap(bcodeWidth, bcodeHeight, PixelFormat.Format32bppArgb));
        }

        #endregion


        #region Auxiliary Methods

        private int Max(int v1, int v2)
        {
            return (v1 > v2 ? v1 : v2);
        }

        private int XCentered(int localWidth, int globalWidth)
        {
            return ((globalWidth - localWidth) / 2);
        }

        #endregion
    }
}

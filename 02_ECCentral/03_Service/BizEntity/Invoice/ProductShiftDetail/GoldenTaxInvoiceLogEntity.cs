using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ECCentral.BizEntity.Invoice
{
    public class GoldenTaxInvoiceLogEntity 
    {
      public string OrderID{get;set;}
      public string OrderType{get;set;}
      public int? LogStatus {get;set;}
      public string LogDescription{get;set;}
      public DateTime LogTime {get;set;}
      public int? WarehouseNumber { get; set; }
      public string CompanyCode { get; set; }
      public string LanguageCode { get; set; }
      public string StoreCompanyCode { get; set; }
    }
}



USE [OverseaECommerceManagement]

GO

CREATE PROCEDURE [dbo].[UP_EC_GetProductDetailMatchedTradingList] (       
 @ProductGroupSysNo int,   
 @TotalCount int output,          
 @PageSize   int, 
 @LanguageCode char(5),     
 @CompanyCode char(50),
 @StoreCompanyCode varchar(50)
)  

AS          
BEGIN          
SET NOCOUNT ON;          
 DECLARE @Sql nvarchar(MAX)          
          
 DECLARE @groupProducts TABLE           
 (           
   productSysno int NOT NULL  PRIMARY KEY           
  ,productCode varchar(20) NULL          
        ,ProStatus int null          
 )          
          
 INSERT INTO @groupProducts           
 (           
   productSysno           
   ,productCode          
         ,ProStatus          
 )          
 SELECT           
  DISTINCT p.SysNo           
  ,p.productID          
        ,p.Status          
 FROM ipp3.dbo.Product p  WITH(NOLOCK)           
 INNER JOIN OverseaContentManagement.dbo.ProductCommonInfo c  WITH(NOLOCK)           
  ON p.ProductCommonInfoSysno=c.SysNo           
 WHERE           
  c.ProductGroupSysno=@ProductGroupSysNo          
          
   SELECT           
    @TotalCount=COUNT(1)           
    FROM [dbo].[ProductMatchedTrading_Detail] A WITH(NOLOCK)           
    INNER JOIN @groupProducts G           
    ON A.ProductSysNo = G.productSysno           
    WHERE           
    A.[Status]='A'           
    AND A.[CustomerSysNo]>0          
    AND A.LanguageCode=@LanguageCode           
    AND A.CompanyCode=@CompanyCode           
    AND A.StoreCompanyCode=@StoreCompanyCode          
          
          
 DECLARE @IDs TABLE           
    (           
         ID int PRIMARY KEY           
         ,productCode varchar(20)           
         ,ProStatus  int          
    )          
 INSERT INTO @IDs           
    (           
         ID           
         ,productCode          
         ,ProStatus          
    )          
    SELECT TOP (@PageSize)          
   A.[SysNo],          
   G.productCode,          
            G.ProStatus          
         FROM [dbo].[ProductMatchedTrading_Detail] A WITH(NOLOCK)           
         INNER JOIN @groupProducts G           
             ON A.ProductSysNo = G.productSysno           
         WHERE           
             A.[CustomerSysNo]>0          
             AND A.STATUS='A'          
  AND A.LanguageCode=@LanguageCode           
  AND A.CompanyCode=@CompanyCode           
  AND A.StoreCompanyCode=@StoreCompanyCode          
  ORDER BY A.InDate DESC          
            
  DECLARE @TempProductMatchedTrading TABLE          
  (          
     [SysNo] INT PRIMARY KEY          
    ,[ProductSysNo] INT          
          ,[ProductCode] VARCHAR(20)          
    ,[CustomerSysNo] INT          
    ,[Content] NVARCHAR(600)          
    ,[Type] CHAR(1)          
    ,[ReplyCount] INT           
    ,[InDate] DATETIME          
    ,[CompanyCode] CHAR(50)          
    ,[StoreCompanyCode] VARCHAR(50)          
    ,[LanguageCode] CHAR(5)          
    ,[CustomerID] NVARCHAR(50)          
    ,[CustomerRank] INT           
    ,ProStatus int        
    ,[CustomerName] NVARCHAR(100)        
  )          
            
  INSERT INTO @TempProductMatchedTrading           
        ([SysNo]          
          ,[ProductSysNo]          
          ,[ProductCode]          
    ,[CustomerSysNo]          
    ,[Content]           
    ,[Type]           
    ,[ReplyCount]           
    ,[InDate]           
    ,[CompanyCode]           
    ,[StoreCompanyCode]           
    ,[LanguageCode]           
    ,[CustomerID]          
    ,[CustomerRank]          
    ,[ProStatus]        
    ,[CustomerName]        
)           
  SELECT A.[SysNo]          
    ,A.[ProductSysNo]          
          ,B.ProductCode          
    ,A.[CustomerSysNo]          
    ,A.[Content]          
    ,A.[Type]          
    ,A.[ReplyCount]          
    ,A.[InDate]          
    ,A.[CompanyCode]          
    ,A.[StoreCompanyCode]          
    ,A.[LanguageCode]          
    ,C.[CustomerID]          
    ,C.[Rank] AS CustomerRank            
    ,B.ProStatus              ,C.CustomerName        
   FROM [dbo].[ProductMatchedTrading_Detail] A WITH(NOLOCK)          
   INNER JOIN @IDs AS B           
  ON A.SysNo = B.ID          
  CROSS APPLY(          
    SELECT TOP 1          
       C1.CustomerID,C1.[Rank],c1.CustomerName        
    FROM Ipp3.dbo.Customer C1 WITH(NOLOCK)          
    WHERE A.[CustomerSysno]=C1.SysNo          
  )C          
             
   SELECT A.[SysNo]          
    ,A.[ProductSysNo]          
    ,A.ProductCode AS ProductID          
    ,A.[CustomerSysNo] as [CustomerInfo.CustomerSysNo]      
    ,A.[Content]          
    ,A.[Type]          
    ,A.[ReplyCount] 
    - (SELECT COUNT(1) FROM [dbo].[ProductMatchedTrading_Reply] DA WITH(NOLOCK)           
		WHERE DA.Type IN('N','M') AND DA.[Status]='A' AND DA.MatchedTradingSysno = A.[SysNo])    
	AS [ReplyCount]       
    ,A.[InDate]          
    ,A.[CompanyCode]          
    ,A.[StoreCompanyCode]          
    ,A.[LanguageCode]          
    ,A.[CustomerID] as   [CustomerInfo.CustomerID]      
    ,A.CustomerRank as   [CustomerInfo.CustomerRank]      
    ,CE.AvtarImageStatus  as [CustomerExtendInfo.AvtarImageStatus]      
    ,CE.AvtarImage    as [CustomerExtendInfo.AvtarImage]      
    ,A.ProStatus      
    ,A.CustomerName  as [CustomerInfo.NickName]      
    FROM @TempProductMatchedTrading AS A          
  CROSS APPLY(          
   SELECT TOP 1          
      AvtarImageStatus,AvtarImage          
   FROM Ipp3.dbo.Customer_Extend CE1 WITH(NOLOCK)          
   WHERE A.[CustomerSysno]=CE1.[CustomerSysno]                  
  )CE          
    ORDER BY A.InDate DESC          
            
            
   ;WITH DATA          
   AS(          
   SELECT           
     DA.[SysNo]          
     ,DA.[MatchedTradingSysNo]          
     ,DA.[CustomerSysNo]          
     ,DA.[Content]          
     ,DA.[Type]          
     ,DA.[IsTop]          
     ,DA.[InDate]          
     ,DA.[EditDate]          
     ,DA.[CompanyCode]          
     ,DA.[StoreCompanyCode]          
     ,DA.[LanguageCode]          
     ,DA.[NeedAdditionalText]          
   FROM  [dbo].[ProductMatchedTrading_Reply] DA WITH(NOLOCK)           
   INNER JOIN @IDs AS B ON DA.MatchedTradingSysNo = B.ID          
   WHERE (DA.Type IN('N','M') OR DA.IsTop='Y') AND DA.[Status]='A')              
            
   SELECT           
   C.[SysNo]          
   ,C.[MatchedTradingSysNo]          
   ,C.[CustomerSysNo]          
   ,C.[Content]          
   ,C.[Type]          
   ,C.[IsTop]          
   ,C.[InDate]          
   ,C.[EditDate]          
   ,C.[CompanyCode]          
   ,C.[StoreCompanyCode]          
   ,C.[LanguageCode]          
   ,C.[NeedAdditionalText]             
   FROM  DATA C              
    WHERE NOT EXISTS( SELECT TOP 1 1 FROM DATA AS C1           
    WHERE C1.MatchedTradingSysNo=C.MatchedTradingSysNo                         
     AND (C1.[Type] = C.[Type] OR (C.[Type]='M' AND C1.IsTop='Y'))           
     AND (C1.indate > C.indate OR (C1.indate = C.indate AND C1.SysNo > C.SysNo))                                             
     )
    
           
       
 SELECT *FROM(  
 SELECT         
     DA.[SysNo]          
     ,DA.[MatchedTradingSysNo]          
     ,DA.[CustomerSysNo]   
     ,DA.[CustomerSysNo] as [CustomerInfo.CustomerSysNo]  
     ,C.CustomerID AS [CustomerInfo.CustomerID]  
     ,C.CustomerName AS  [CustomerInfo.NickName]  
     ,DA.[Content]          
     ,DA.[Type]          
     ,DA.[IsTop]          
   ,DA.[InDate]          
     ,DA.[EditDate]          
     ,DA.[CompanyCode]          
     ,DA.[StoreCompanyCode]          
     ,DA.[LanguageCode]          
     ,DA.[NeedAdditionalText]  
     ,ROW_NUMBER() OVER(PARTITION BY DA.MatchedTradingSysNo ORDER BY DA.[InDate] DESC) as  NUM         
   FROM  [dbo].[ProductMatchedTrading_Reply] DA WITH(NOLOCK)   
   INNER JOIN @IDs AS B ON DA.MatchedTradingSysNo = B.ID      
    CROSS APPLY(          
    SELECT TOP 1          
       C1.CustomerID,C1.[Rank],c1.CustomerName        
    FROM Ipp3.dbo.Customer C1 WITH(NOLOCK)          
    WHERE DA.[CustomerSysno]=C1.SysNo          
  )C       
   WHERE (DA.Type IN('W') OR DA.IsTop='Y') AND DA.[Status]='A'   
   )  AS A  
   WHERE  A.NUM<=5           
     
          
END          


GO



USE [OverseaECommerceManagement]

CREATE TABLE [dbo].[ProductMatchedTrading_Detail](
	[SysNo] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ProductSysNo] [int] NOT NULL,
	[CustomerSysNo] [int] NOT NULL,
	[Content] [nvarchar](600) NOT NULL,
	[Status] [char](1) NOT NULL,
	[Type] [char](1) NOT NULL,
	[ReplyCount] [int] NOT NULL CONSTRAINT [DF_ProductMatchedTrading_Detail_ReplyCount]  DEFAULT ((0)),
	[InDate] [datetime] NULL CONSTRAINT [DF_ProductMatchedTrading_Detail_InDate]  DEFAULT (getdate()),
	[InUser] [nvarchar](100) NULL CONSTRAINT [DF_ProductMatchedTrading_Detail_InUser]  DEFAULT (N''),
	[EditDate] [datetime] NULL CONSTRAINT [DF_ProductMatchedTrading_Detail_EditDate]  DEFAULT (getdate()),
	[EditUser] [nvarchar](100) NULL CONSTRAINT [DF_ProductMatchedTrading_Detail_EditUser]  DEFAULT (N''),
	[CompanyCode] [char](50) NULL CONSTRAINT [DF_ProductMatchedTrading_Detail_CompanyCode]  DEFAULT (N'8601'),
	[StoreCompanyCode] [varchar](50) NULL CONSTRAINT [DF_ProductMatchedTrading_Detail_StoreCompanyCode]  DEFAULT (N'8601'),
	[LanguageCode] [char](5) NULL CONSTRAINT [DF_ProductMatchedTrading_Detail_LanguageCode]  DEFAULT (N'zh-CN'),
	[Email] [nvarchar](50) NULL,
 CONSTRAINT [PK_ProductMatchedTrading_Detail] PRIMARY KEY CLUSTERED 
(
	[SysNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



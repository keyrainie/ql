USE [OverseaECommerceManagement]
CREATE TABLE [dbo].[ProductMatchedTrading_Reply](
	[SysNo] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[MatchedTradingSysNo] [int] NOT NULL,
	[CustomerSysNo] [int] NOT NULL CONSTRAINT [DF_ProductMatchedTrading_Reply_CustomerSysNo]  DEFAULT ((0)),
	[Content] [nvarchar](2000) NULL,
	[Status] [char](1) NOT NULL,
	[Type] [char](1) NOT NULL,
	[IsTop] [char](1) NOT NULL CONSTRAINT [DF_ProductMatchedTrading_Reply_IsTop]  DEFAULT ('N'),
	[InDate] [datetime] NULL CONSTRAINT [DF_ProductMatchedTrading_Reply_InDate]  DEFAULT (getdate()),
	[InUser] [nvarchar](100) NULL CONSTRAINT [DF_ProductMatchedTrading_Reply_InUser]  DEFAULT (N''),
	[EditDate] [datetime] NULL CONSTRAINT [DF_ProductMatchedTrading_Reply_EditDate]  DEFAULT (getdate()),
	[EditUser] [nvarchar](100) NULL CONSTRAINT [DF_ProductMatchedTrading_Reply_EditUser]  DEFAULT (N''),
	[CompanyCode] [char](50) NULL CONSTRAINT [DF_ProductMatchedTrading_Reply_CompanyCode]  DEFAULT (N'8601'),
	[StoreCompanyCode] [varchar](50) NULL CONSTRAINT [DF_ProductMatchedTrading_Reply_StoreCompanyCode]  DEFAULT (N'8601'),
	[LanguageCode] [char](5) NULL CONSTRAINT [DF_ProductMatchedTrading_Reply_LanguageCode]  DEFAULT (N'zh-CN'),
	[NeedAdditionalText] [char](1) NOT NULL CONSTRAINT [DF_ProductMatchedTrading_Reply_NeedAdditionalText]  DEFAULT ('Y'),
	[RefSysno] [int] NULL,
	[Email] [nvarchar](50) NULL,
 CONSTRAINT [PK_ProductMatchedTrading_Reply] PRIMARY KEY CLUSTERED 
(
	[SysNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



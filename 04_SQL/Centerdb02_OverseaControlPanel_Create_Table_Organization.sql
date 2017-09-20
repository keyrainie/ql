USE [OverseaControlPanel]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Organization](
	[OrganizationID] [int] IDENTITY(1,1) NOT NULL,
	[OrganizationName] [nvarchar](200) NOT NULL,
	[Province] [nvarchar](100) NOT NULL,
	[InDate] [datetime] NOT NULL,
	[InUser] [nvarchar](100) NOT NULL,
	[EditDate] [datetime] NULL,
	[EditUser] [nvarchar](100) NULL,
 CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED 
(
	[OrganizationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Organization] ADD  CONSTRAINT [DF_Organization_InDate]  DEFAULT (getdate()) FOR [InDate]
GO



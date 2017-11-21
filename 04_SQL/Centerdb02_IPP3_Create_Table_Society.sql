USE IPP3
CREATE TABLE [dbo].[Society](
  [TransactionNumber] [int] IDENTITY(1,1) NOT NULL,
  [Description] [nvarchar](200) NOT NULL,
  [OrganizationID] INT NOT NULL,--ÇÈÁªID
  [Status] [char](1) NOT NULL,
  [InDate] [datetime] NOT NULL,
  [InUser] [nvarchar](100) NOT NULL,
  [EditDate] [datetime] NULL,
  [EditUser] [nvarchar](100) NULL,
 CONSTRAINT [PK_Society] PRIMARY KEY CLUSTERED 
(
  [TransactionNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[Society] ADD CONSTRAINT DF_InDate_Society  DEFAULT GETDATE() FOR [InDate]





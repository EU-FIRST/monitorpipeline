/****** Object:  Table [dbo].[Documents]    ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Documents]') AND type in (N'U'))
DROP TABLE [dbo].[Documents]
GO

/****** Object:  Table [dbo].[Documents]    ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Documents](		
	[corpusId] [uniqueidentifier] NOT NULL,
	[docId] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](400) NULL,
	[description] [nvarchar](400) NULL,
	[url] [varchar](400) NULL,
	[time] [char](26) NULL,
	[pubDate] [char](100) NULL,
	[domain] [varchar](100) NULL,
	[rev] [int] NULL,
	[category] [ntext] NULL,
	constraint UQ_Documents unique (corpusId, docId) with (ignore_dup_key = on)
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[term]    Script Date: 05/25/2013 14:14:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[term](
	[occurrence_id] [int] NOT NULL,
	[term] [varchar](400) NOT NULL,
 CONSTRAINT [PK_term] PRIMARY KEY CLUSTERED 
(
	[occurrence_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[sentiment_word_occurrence]    Script Date: 05/25/2013 14:14:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sentiment_word_occurrence](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[date] [datetime] NULL,
	[location_start] [int] NULL,
	[location_end] [int] NULL,
	[sentence_num] [smallint] NULL,
	[block_num] [smallint] NULL,
	[document_id] [int] NOT NULL,
	[entity_id] [int] /*NOT*/ NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[occurrence]    Script Date: 05/25/2013 14:14:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[occurrence](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[date] [datetime] NULL,
	[location_start] [int] NULL,
	[location_end] [int] NULL,
	[sentence_num] [smallint] NULL,
	[block_num] [smallint] NULL,
	[document_id] [int] NOT NULL,
	[entity_id] [int] /*NOT*/ NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[entity]    Script Date: 05/25/2013 14:14:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[entity](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[entity_uri] [varchar](200) NULL,
	[entity_label] [varchar](200) NULL,
	[flags] [varchar](100) NULL,
	[class_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[document_sentiment]    Script Date: 05/25/2013 14:14:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[document_sentiment](
	[document_id] [int] NULL,
	[positives] [smallint] NULL,
	[negatives] [smallint] NULL,
	[polarity] [float] NULL,
	[tokens] [int] NULL,
	[sentiment] [float] NULL,
UNIQUE NONCLUSTERED 
(
	[document_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[document]    Script Date: 05/25/2013 14:14:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[document](
	[title] [varchar](1000) NULL,
	[date] [date] NULL,
	[pub_date] [varchar](100) NULL,
	[time_get] [datetime] NULL,
	[response_url] [varchar](1000) NULL,
	[url_key] [varchar](1000) NULL,
	[domain_name] [varchar](255) NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[is_financial] [bit] NULL,
	[pump_dump_index] [float] NULL,
	[document_guid] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[class]    Script Date: 05/25/2013 14:14:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[class](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[class_uri] [varchar](100) NULL,
	[class_label] [varchar](100) NULL,
	[parent_class_label] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[block_sentiment]    Script Date: 05/25/2013 14:14:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[block_sentiment](
	[document_id] [int] NULL,
	[block_num] [smallint] NULL,
	[positives] [smallint] NULL,
	[negatives] [smallint] NULL,
	[polarity] [float] NULL,
	[tokens] [smallint] NULL,
	[date] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Default [DF__block_sen__docum__145C0A3F]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT (NULL) FOR [document_id]
GO
/****** Object:  Default [DF__block_sen__block__15502E78]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT (NULL) FOR [block_num]
GO
/****** Object:  Default [DF__block_sen__posit__164452B1]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT ('0') FOR [positives]
GO
/****** Object:  Default [DF__block_sen__negat__173876EA]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT ('0') FOR [negatives]
GO
/****** Object:  Default [DF__block_sen__polar__182C9B23]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT ('0') FOR [polarity]
GO
/****** Object:  Default [DF__block_sen__token__1920BF5C]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT ('0') FOR [tokens]
GO
/****** Object:  Default [DF__block_sent__date__1A14E395]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT (NULL) FOR [date]
GO
/****** Object:  Default [DF__class__class_lab__1B0907CE]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[class] ADD  DEFAULT (NULL) FOR [class_label]
GO
/****** Object:  Default [DF__class__parent_cl__1BFD2C07]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[class] ADD  DEFAULT (NULL) FOR [parent_class_label]
GO
/****** Object:  Default [DF__document__title__1ED998B2]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [title]
GO
/****** Object:  Default [DF__document__date__1FCDBCEB]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [date]
GO
/****** Object:  Default [DF__document__pub_da__20C1E124]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [pub_date]
GO
/****** Object:  Default [DF__document__time_g__21B6055D]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [time_get]
GO
/****** Object:  Default [DF__document__respon__22AA2996]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [response_url]
GO
/****** Object:  Default [DF__document__url_ke__239E4DCF]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [url_key]
GO
/****** Object:  Default [DF__document__domain__24927208]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [domain_name]
GO
/****** Object:  Default [DF__document___docum__25869641]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[document_sentiment] ADD  DEFAULT (NULL) FOR [document_id]
GO
/****** Object:  Default [DF__document___posit__267ABA7A]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[document_sentiment] ADD  DEFAULT ('0') FOR [positives]
GO
/****** Object:  Default [DF__document___negat__276EDEB3]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[document_sentiment] ADD  DEFAULT ('0') FOR [negatives]
GO
/****** Object:  Default [DF__document___polar__286302EC]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[document_sentiment] ADD  DEFAULT ('0') FOR [polarity]
GO
/****** Object:  Default [DF__document___senti__29572725]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[document_sentiment] ADD  DEFAULT (NULL) FOR [sentiment]
GO
/****** Object:  Default [DF__entity__entity_u__2A4B4B5E]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[entity] ADD  DEFAULT (NULL) FOR [entity_uri]
GO
/****** Object:  Default [DF__entity__entity_l__2B3F6F97]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[entity] ADD  DEFAULT (NULL) FOR [entity_label]
GO
/****** Object:  Default [DF__entity__flags__2C3393D0]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[entity] ADD  DEFAULT (NULL) FOR [flags]
GO
/****** Object:  Default [DF__occurrence__date__2D27B809]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[occurrence] ADD  DEFAULT (NULL) FOR [date]
GO
/****** Object:  Default [DF__occurrenc__locat__2E1BDC42]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[occurrence] ADD  DEFAULT (NULL) FOR [location_start]
GO
/****** Object:  Default [DF__occurrenc__locat__2F10007B]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[occurrence] ADD  DEFAULT (NULL) FOR [location_end]
GO
/****** Object:  Default [DF__occurrenc__sente__300424B4]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[occurrence] ADD  DEFAULT (NULL) FOR [sentence_num]
GO
/****** Object:  Default [DF__occurrenc__block__30F848ED]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[occurrence] ADD  DEFAULT (NULL) FOR [block_num]
GO
/****** Object:  Default [DF__sentiment___date__31EC6D26]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[sentiment_word_occurrence] ADD  DEFAULT (NULL) FOR [date]
GO
/****** Object:  Default [DF__sentiment__locat__32E0915F]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[sentiment_word_occurrence] ADD  DEFAULT (NULL) FOR [location_start]
GO
/****** Object:  Default [DF__sentiment__locat__33D4B598]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[sentiment_word_occurrence] ADD  DEFAULT (NULL) FOR [location_end]
GO
/****** Object:  Default [DF__sentiment__sente__34C8D9D1]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[sentiment_word_occurrence] ADD  DEFAULT (NULL) FOR [sentence_num]
GO
/****** Object:  Default [DF__sentiment__block__35BCFE0A]    Script Date: 05/25/2013 14:14:47 ******/
ALTER TABLE [dbo].[sentiment_word_occurrence] ADD  DEFAULT (NULL) FOR [block_num]
GO

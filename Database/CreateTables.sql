/****** Object:  Table [dbo].[term]    Script Date: 05/25/2013 12:52:42 ******/
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
/****** Object:  Table [dbo].[sentiment_word_occurrence]    Script Date: 05/25/2013 12:52:42 ******/
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
	[entity_id] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[occurrence]    Script Date: 05/25/2013 12:52:42 ******/
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
	[entity_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[entity]    Script Date: 05/25/2013 12:52:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[entity](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[entity_uri] [varchar](100) NULL,
	[entity_label] [varchar](100) NULL,
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
/****** Object:  Table [dbo].[document_sentiment]    Script Date: 05/25/2013 12:52:42 ******/
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
/****** Object:  Table [dbo].[document]    Script Date: 05/25/2013 12:52:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[document](
	[corpus_id] [char](41) NULL,
	[document_id] [char](41) NULL,
	[title] [varchar](1000) NULL,
	[date] [date] NULL,
	[pub_date] [varchar](100) NULL,
	[time_get] [datetime] NULL,
	[response_url] [varchar](1000) NULL,
	[url_key] [varchar](1000) NULL,
	[domain_name] [varchar](255) NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[class]    Script Date: 05/25/2013 12:52:42 ******/
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
/****** Object:  Table [dbo].[block_sentiment]    Script Date: 05/25/2013 12:52:42 ******/
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
CREATE NONCLUSTERED INDEX [__document_id_block_num] ON [dbo].[block_sentiment] 
(
	[document_id] ASC,
	[block_num] ASC
)
INCLUDE ( [polarity]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Default [DF__sentiment___date__06CD04F7]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[sentiment_word_occurrence] ADD  DEFAULT (NULL) FOR [date]
GO
/****** Object:  Default [DF__sentiment__locat__07C12930]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[sentiment_word_occurrence] ADD  DEFAULT (NULL) FOR [location_start]
GO
/****** Object:  Default [DF__sentiment__locat__08B54D69]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[sentiment_word_occurrence] ADD  DEFAULT (NULL) FOR [location_end]
GO
/****** Object:  Default [DF__sentiment__sente__09A971A2]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[sentiment_word_occurrence] ADD  DEFAULT (NULL) FOR [sentence_num]
GO
/****** Object:  Default [DF__sentiment__block__0A9D95DB]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[sentiment_word_occurrence] ADD  DEFAULT (NULL) FOR [block_num]
GO
/****** Object:  Default [DF__occurrence__date__787EE5A0]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[occurrence] ADD  DEFAULT (NULL) FOR [date]
GO
/****** Object:  Default [DF__occurrenc__locat__797309D9]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[occurrence] ADD  DEFAULT (NULL) FOR [location_start]
GO
/****** Object:  Default [DF__occurrenc__locat__7A672E12]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[occurrence] ADD  DEFAULT (NULL) FOR [location_end]
GO
/****** Object:  Default [DF__occurrenc__sente__7B5B524B]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[occurrence] ADD  DEFAULT (NULL) FOR [sentence_num]
GO
/****** Object:  Default [DF__occurrenc__block__7C4F7684]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[occurrence] ADD  DEFAULT (NULL) FOR [block_num]
GO
/****** Object:  Default [DF__entity1__entity___656C112C]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[entity] ADD  DEFAULT (NULL) FOR [entity_uri]
GO
/****** Object:  Default [DF__entity1__entity___66603565]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[entity] ADD  DEFAULT (NULL) FOR [entity_label]
GO
/****** Object:  Default [DF__entity1__flags__6754599E]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[entity] ADD  DEFAULT (NULL) FOR [flags]
GO
/****** Object:  Default [DF__document___docum__24927208]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document_sentiment] ADD  DEFAULT (NULL) FOR [document_id]
GO
/****** Object:  Default [DF__document___posit__25869641]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document_sentiment] ADD  DEFAULT ('0') FOR [positives]
GO
/****** Object:  Default [DF__document___negat__267ABA7A]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document_sentiment] ADD  DEFAULT ('0') FOR [negatives]
GO
/****** Object:  Default [DF__document___polar__276EDEB3]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document_sentiment] ADD  DEFAULT ('0') FOR [polarity]
GO
/****** Object:  Default [DF__document___senti__29572725]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document_sentiment] ADD  DEFAULT (NULL) FOR [sentiment]
GO
/****** Object:  Default [DF__document1__corpu__6C190EBB]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [corpus_id]
GO
/****** Object:  Default [DF__document1__docum__6D0D32F4]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [document_id]
GO
/****** Object:  Default [DF__document1__title__6E01572D]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [title]
GO
/****** Object:  Default [DF__document1__date__6EF57B66]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [date]
GO
/****** Object:  Default [DF__document1__pub_d__6FE99F9F]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [pub_date]
GO
/****** Object:  Default [DF__document1__time___70DDC3D8]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [time_get]
GO
/****** Object:  Default [DF__document1__respo__71D1E811]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [response_url]
GO
/****** Object:  Default [DF__document1__url_k__72C60C4A]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [url_key]
GO
/****** Object:  Default [DF__document1__domai__73BA3083]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[document] ADD  DEFAULT (NULL) FOR [domain_name]
GO
/****** Object:  Default [DF__class1__class_la__5629CD9C]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[class] ADD  DEFAULT (NULL) FOR [class_label]
GO
/****** Object:  Default [DF__class1__parent_c__571DF1D5]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[class] ADD  DEFAULT (NULL) FOR [parent_class_label]
GO
/****** Object:  Default [DF__block_sen__docum__1A14E395]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT (NULL) FOR [document_id]
GO
/****** Object:  Default [DF__block_sen__block__1B0907CE]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT (NULL) FOR [block_num]
GO
/****** Object:  Default [DF__block_sen__posit__1BFD2C07]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT ('0') FOR [positives]
GO
/****** Object:  Default [DF__block_sen__negat__1CF15040]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT ('0') FOR [negatives]
GO
/****** Object:  Default [DF__block_sen__polar__1DE57479]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT ('0') FOR [polarity]
GO
/****** Object:  Default [DF__block_sen__token__1ED998B2]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT ('0') FOR [tokens]
GO
/****** Object:  Default [DF__block_sent__date__1FCDBCEB]    Script Date: 05/25/2013 12:52:42 ******/
ALTER TABLE [dbo].[block_sentiment] ADD  DEFAULT (NULL) FOR [date]
GO
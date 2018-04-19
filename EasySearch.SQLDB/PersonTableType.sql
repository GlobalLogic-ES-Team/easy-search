CREATE TYPE [dbo].[PersonTableType] AS TABLE(
	[ssn] [nchar](20) NOT NULL,
	[gender] [nchar](10) NOT NULL,
	[firstname] [nvarchar](100) NOT NULL,
	[lastname] [nvarchar](100) NOT NULL,
	[email] [nvarchar](200) NULL,
	[dob] [nvarchar](50) NOT NULL,
	[cell] [nchar](15) NULL,
	[salary] [float] NULL,
	[interests] [nvarchar](max) NULL,
	[json_data] [nvarchar](max) NOT NULL,
	[street] [nvarchar](200) NULL,
	[state] [nvarchar](200) NULL,
	[city] [nvarchar](200) NULL,
	[zip] [nvarchar](100) NULL
)
GO
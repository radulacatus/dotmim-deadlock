USE [master]
GO 

-- Server database
if (exists (select * from sys.databases where name = 'ServerDb'))
Begin
	ALTER DATABASE [ServerDb] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE [ServerDb]
End
Create database [ServerDb]
Go
-- Client database. No need to create the schema, Dotmim.Sync will do
if (exists (select * from sys.databases where name = 'Client'))
Begin
	ALTER DATABASE [ClientDb] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE [ClientDb]
End
Create database [ClientDb]
GO

ALTER DATABASE [ServerDb] SET CHANGE_TRACKING = ON (CHANGE_RETENTION = 14 DAYS, AUTO_CLEANUP = ON)
ALTER DATABASE [ClientDb] SET CHANGE_TRACKING = ON (CHANGE_RETENTION = 14 DAYS, AUTO_CLEANUP = ON)

USE [ServerDb]
GO

/****** Object:  Table [dbo].[Customer]    Script Date: 11/07/2023 21:19:21 ******/
CREATE TABLE [dbo].[Customer](
	[CustomerID] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[rowguid] [uniqueidentifier] NULL
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[CustomerAddress]    Script Date: 11/07/2023 21:19:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerAddress](
	[AddressId] [uniqueidentifier] NOT NULL,
	[CustomerID] [uniqueidentifier] NOT NULL,
	[AddressLine1] [nvarchar](max) NOT NULL,
	[ValidUntil] [datetime] NULL,
	[rowguid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_CustomerAddress] PRIMARY KEY CLUSTERED 
(
	[AddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CustomerAddress]  WITH CHECK ADD  CONSTRAINT [FK_CustomerAddress_Customer_CustomerID] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customer] ([CustomerID])
GO
ALTER TABLE [dbo].[CustomerAddress] CHECK CONSTRAINT [FK_CustomerAddress_Customer_CustomerID]
GO

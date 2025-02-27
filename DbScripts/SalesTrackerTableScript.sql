USE [SaleTracker]
GO
/****** Object:  Table [dbo].[tSalTrkr]    Script Date: 1/25/2025 4:29:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tSalTrkr](
	[LeadId] [int] IDENTITY(1,1) NOT NULL,
	[LeadNumber] [nchar](50) NULL,
	[DocNo] [nvarchar](50) NULL,
	[DocDate] [datetime] NULL,
	[EnteredBy] [nvarchar](100) NULL,
	[LeadSource] [nvarchar](100) NULL,
	[LeadOwner] [nvarchar](100) NULL,
	[LeadOwnerContactNo] [nvarchar](10) NULL,
	[LeadPriority] [nvarchar](20) NULL,
	[LeadTitle] [nvarchar](200) NULL,
	[LeadCategory] [nvarchar](50) NULL,
	[LeadDesc] [nvarchar](max) NULL,
	[SpecialRequirement] [nvarchar](max) NULL,
	[PotentialDealValue] [nvarchar](20) NULL,
	[ProbabilityOfConversion] [nvarchar](10) NULL,
	[ClosureForecast] [nvarchar](10) NULL,
	[CompanyName] [nvarchar](100) NULL,
	[BusinessSector] [nvarchar](50) NULL,
	[ContactPerson] [nvarchar](100) NULL,
	[ContactNo] [nchar](10) NULL,
	[Designation] [nvarchar](20) NULL,
	[Email] [nvarchar](50) NULL,
	[Address] [nvarchar](max) NULL,
	[Website] [nvarchar](100) NULL,
	[PEmail] [nvarchar](50) NULL,
	[Remark] [nvarchar](250) NULL,
	[CurrentStatus] [nvarchar](50) NULL,
	[NextAppointmentDate] [datetime] NULL,
	[LeadDate] [datetime] NULL,
	[Address1] [nvarchar](100) NULL,
	[Address2] [nvarchar](100) NULL,
	[Address3] [nvarchar](100) NULL,
	[City] [nvarchar](100) NULL,
	[District] [nvarchar](50) NULL,
	[StateName] [nvarchar](50) NULL,
	[Zip] [nvarchar](50) NULL,
	[isActive] [bit] NULL,
	[AddedBy] [nvarchar](50) NULL,
	[UpdatedBy] [nvarchar](50) NULL,
	[UpdatedDate] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tSalTrkr1]    Script Date: 1/25/2025 4:29:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tSalTrkr1](
	[LeadActivityId] [int] IDENTITY(1,1) NOT NULL,
	[LeadId] [int] NULL,
	[LeadStatus] [nvarchar](50) NOT NULL,
	[LeadDate] [datetime] NULL,
	[MeetingMode] [nvarchar](20) NULL,
	[MeetingDate] [datetime] NULL,
	[FollowupDate] [datetime] NULL,
	[ResponseDesc] [nvarchar](max) NULL,
	[NextAppointmentDate] [datetime] NULL,
	[IsReminderSet] [bit] NULL,
	[ReminderDate] [datetime] NULL,
	[RemarkBP] [nvarchar](max) NULL,
	[RemarkSlf] [nvarchar](max) NULL,
	[RemarkSpl] [nvarchar](500) NULL,
	[UploadedFileName] [nvarchar](200) NULL,
	[AddedDate] [datetime] NULL,
	[AddedBy] [nvarchar](50) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[tSalTrkr] ADD  CONSTRAINT [DF_tSalTrkr_LeadNumber]  DEFAULT ('LN'+CONVERT([nvarchar](50),NEXT VALUE FOR [LeadNumberSequence])) FOR [LeadNumber]
GO
ALTER TABLE [dbo].[tSalTrkr] ADD  CONSTRAINT [DF__tSalTrkr1T__isAct__3B40CD36]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[tSalTrkr1] ADD  DEFAULT ((1)) FOR [IsReminderSet]
GO

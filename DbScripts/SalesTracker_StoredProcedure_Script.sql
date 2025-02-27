USE [SaleTracker]
GO
/****** Object:  StoredProcedure [dbo].[GetLeadCountsNew]    Script Date: 1/25/2025 4:30:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetLeadCountsNew]
AS
BEGIN
    -- Total Leads
    SELECT COUNT(*) AS TotalLeads FROM tSalTrkr;

    -- Active Leads
    SELECT COUNT(*) AS ActiveLeads FROM tSalTrkr WHERE isActive =1 and CurrentStatus<>'Qualified';

    -- Converted Leads
    SELECT COUNT(*) AS ConvertedLeads FROM tSalTrkr WHERE CurrentStatus = 'Qualified';

    -- Today's Actionable Leads
   
	WITH RankedActivities AS (
    SELECT 
        A.LeadActivityId,
        A.LeadId,
        A.LeadStatus,
        A.MeetingDate,
        A.ReminderDate,
        A.UpdatedDate,
        L.CurrentStatus,
        L.NextAppointmentDate,
        ROW_NUMBER() OVER (PARTITION BY A.LeadId ORDER BY A.UpdatedDate DESC) AS RowNum
    FROM 
        tSalTrkr L
    INNER JOIN 
        tSalTrkr1 A 
        ON L.LeadId = A.LeadId
		Where isActive=1
   
)
	
	 SELECT COUNT(*) AS TodaysActionableLeads FROM 
	
    RankedActivities
	WHERE 
    RowNum = 1 -- Only take the latest activity for each LeadId
	and (ReminderDate < GETDATE() AND UpdatedDate < ReminderDate) 
	 

	
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AddLeadNew]    Script Date: 1/25/2025 4:30:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_AddLeadNew]
(
    @LeadId INT = NULL,
	@LeadNumber NVARCHAR(50)=NULL,
    @DocNo NVARCHAR(50) = NULL,
    @DocDate DATETIME = NULL,
    @EnteredBy NVARCHAR(100) = NULL,
    @LeadSource NVARCHAR(100) = NULL,
    @LeadOwner NVARCHAR(100) = NULL,
    @LeadOwnerContactNo NVARCHAR(10) = NULL,
    @LeadPriority NVARCHAR(20) = NULL,
    @LeadTitle NVARCHAR(200) = NULL,
    @LeadCategory NVARCHAR(50) = NULL,
    @LeadDesc NVARCHAR(MAX) = NULL,
    @SpecialRequirement NVARCHAR(MAX) = NULL,
    @PotentialDealValue NVARCHAR(20) = NULL,
    @ProbabilityOfConversion NVARCHAR(10) = NULL,
    @ClosureForecast NVARCHAR(10) = NULL,
    @CompanyName NVARCHAR(100) = NULL,
    @BusinessSector NVARCHAR(50) = NULL,
    @ContactPerson NVARCHAR(100) = NULL,
    @ContactNo NCHAR(10) = NULL,
    @Designation NVARCHAR(20) = NULL,
    @Email NVARCHAR(50) = NULL,
    @Address NVARCHAR(MAX) = NULL,
    @Website NVARCHAR(100) = NULL,
	@PEmail NVARCHAR(100)=NULL,
	@Remark NVARCHAR(MAX)=NULL,   
	@CurrentStatus NVARCHAR(50)=NULL,
	@NextReminderDate DATETIME=NULL,
	@Address1 NVARCHAR(100) = NULL,
    @Address2 NVARCHAR(100) = NULL,
    @Address3 NVARCHAR(100) = NULL,
    @City NVARCHAR(100) = NULL,
    @District NVARCHAR(50) = NULL,
    @StateName NVARCHAR(50) = NULL,
    @Zip NVARCHAR(50) = NULL,
	@isActive BIT = 1,
    @AddedBy NVARCHAR(50)='Ankit Kumar',
	@LeadActivityId INT=NULL,
	@LeadStatus NVARCHAR(50)=NULL,
    @LeadDate DATETIME = NULL,
	@NextAppointmentDate DATETIME = NULL,
    @IsReminderSet BIT = 0,
    @ReminderDate DATETIME = NULL,
	@MeetingMode nvarchar(20) =Null,
	@MeetingDate datetime =Null,
	@FollowupDate datetime =Null,
	@ResponseDesc nvarchar(max)= Null,
    @RemarkBP NVARCHAR(MAX) = NULL,
    @RemarkSlf NVARCHAR(MAX) = NULL,
    @RemarkSpl NVARCHAR(500) = NULL,
    @UploadedFileName NVARCHAR(200) = NULL,
    @AddedDate Datetime=NULL,
	@UpdatedBy Nvarchar(50)=NULL,
	@UpdatedDate Datetime=NULL
	)
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @InsertedValuesTable TABLE (LeadId INT);
    MERGE INTO [tSalTrkr] AS target
    USING (SELECT
                @LeadId AS LeadId,
                @DocNo AS DocNo,
                Getdate() AS DocDate,
                @EnteredBy AS EnteredBy,
                @LeadSource AS LeadSource,
                @LeadOwner AS LeadOwner,
                @LeadOwnerContactNo AS LeadOwnerContactNo,
                @LeadPriority AS LeadPriority,
                @LeadTitle AS LeadTitle,
                @LeadCategory AS LeadCategory,
                @LeadDesc AS LeadDesc,
                @SpecialRequirement AS SpecialRequirement,
                @PotentialDealValue AS PotentialDealValue,
                @ProbabilityOfConversion AS ProbabilityOfConversion,
                @ClosureForecast AS ClosureForecast,
                @CompanyName AS CompanyName,
                @BusinessSector AS BusinessSector,
                @ContactPerson AS ContactPerson,
                @ContactNo AS ContactNo,
                @Designation AS Designation,
                @Email AS Email,
                @Address AS Address,
                @Website AS Website,
                @Remark AS Remark,
                @LeadStatus AS CurrentStatus,
                @NextAppointmentDate AS NextAppointmentDate,
                getdate() AS LeadDate,
                @Address1 AS Address1,
                @Address2 AS Address2,
                @Address3 AS Address3,
                @City AS City,
                @District AS District,
                @StateName AS StateName,
                @Zip AS Zip,
                @isActive AS isActive,
                'Ankit Kumar' AS AddedBy,
                'Ankit Kumar' AS UpdatedBy,
                GETDATE() AS UpdatedDate
           ) AS source
    ON (target.LeadId = source.LeadId)

    WHEN MATCHED THEN
        UPDATE SET
            target.DocNo = source.DocNo,
            target.DocDate = source.DocDate,
            target.EnteredBy = source.EnteredBy,
            target.LeadSource = source.LeadSource,
            target.LeadOwner = source.LeadOwner,
            target.LeadOwnerContactNo = source.LeadOwnerContactNo,
            target.LeadPriority = source.LeadPriority,
            target.LeadTitle = source.LeadTitle,
            target.LeadCategory = source.LeadCategory,
            target.LeadDesc = source.LeadDesc,
            target.SpecialRequirement = source.SpecialRequirement,
            target.PotentialDealValue = source.PotentialDealValue,
            target.ProbabilityOfConversion = source.ProbabilityOfConversion,
            target.ClosureForecast = source.ClosureForecast,
            target.CompanyName = source.CompanyName,
            target.BusinessSector = source.BusinessSector,
            target.ContactPerson = source.ContactPerson,
            target.ContactNo = source.ContactNo,
            target.Designation = source.Designation,
            target.Email = source.Email,
            target.Address = source.Address,
            target.Website = source.Website,
            target.Remark = source.Remark,
            target.CurrentStatus = source.CurrentStatus,
            target.LeadDate = source.LeadDate,
            target.Address1 = source.Address1,
            target.Address2 = source.Address2,
            target.Address3 = source.Address3,
            target.City = source.City,
            target.District = source.District,
            target.StateName = source.StateName,
            target.Zip = source.Zip,
            target.isActive = source.isActive,
            --target.AddedBy = source.AddedBy,
            target.UpdatedBy = source.UpdatedBy,
            target.UpdatedDate = source.UpdatedDate

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            DocNo,
            DocDate,
            EnteredBy,
            LeadSource,
            LeadOwner,
            LeadOwnerContactNo,
            LeadPriority,
            LeadTitle,
            LeadCategory,
            LeadDesc,
            SpecialRequirement,
            PotentialDealValue,
            ProbabilityOfConversion,
            ClosureForecast,
            CompanyName,
            BusinessSector,
            ContactPerson,
            ContactNo,
            Designation,
            Email,
            [Address],
            Website,
            Remark,
            CurrentStatus,
            NextAppointmentDate,
            LeadDate,
            Address1,
            Address2,
            Address3,
            City,
            District,
            StateName,
            Zip,
            isActive,
            AddedBy,
            UpdatedBy,
			UpdatedDate
        )
        VALUES (
            source.DocNo,
            source.DocDate,
            source.EnteredBy,
            source.LeadSource,
            source.LeadOwner,
            source.LeadOwnerContactNo,
            source.LeadPriority,
            source.LeadTitle,
            source.LeadCategory,
            source.LeadDesc,
            source.SpecialRequirement,
            source.PotentialDealValue,
            source.ProbabilityOfConversion,
            source.ClosureForecast,
            source.CompanyName,
            source.BusinessSector,
            source.ContactPerson,
            source.ContactNo,
            source.Designation,
            source.Email,
            source.Address,
            source.Website,
            source.Remark,
            source.CurrentStatus,
            source.NextAppointmentDate,
            source.LeadDate,
            source.Address1,
            source.Address2,
            source.Address3,
            source.City,
            source.District,
            source.StateName,
            source.Zip,
            source.isActive,
            source.AddedBy,
            source.UpdatedBy,
			source.UpdatedDate
        )
OUTPUT inserted.LeadId INTO @InsertedValuesTable;

if(@LeadId is Null or @LeadId=0)
begin
SELECT @LeadId= LeadId from  @InsertedValuesTable;
end

 INSERT INTO tSalTrkr1 (
        LeadId,
        LeadStatus,
        LeadDate,
        NextAppointmentDate,
        IsReminderSet,
        ReminderDate,
		MeetingMode,
		MeetingDate,
		FollowupDate,
		ResponseDesc,
        RemarkBP,
        RemarkSlf,
        RemarkSpl,
        UploadedFileName,
        AddedDate,
        AddedBy,
        UpdatedDate,
        UpdatedBy
    )
    VALUES (
        @LeadId,
        @LeadStatus,
        GetDate(),
        @NextAppointmentDate,
        @IsReminderSet,
        @ReminderDate,
		@MeetingMode,
		@MeetingDate,
		@FollowupDate,
		@ResponseDesc,
        @RemarkBP,
        @RemarkSlf,
        @RemarkSpl,
        @UploadedFileName,
        GETDATE(),  -- Automatically set the current date and time for AddedDate
        'Ankit Kumar',
        GETDATE(),  -- Automatically set the current date and time for UpdatedDate
        'Ankit Kumar'
    );

	SELECT CONVERT(NVARCHAR(10),@LeadId) + '#'+ LeadNumber from tSalTrkr where LeadId=@LeadId;

END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLeadByIDNew]    Script Date: 1/25/2025 4:30:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetLeadByIDNew] 
(
@LeadId int=Null
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT top 1 tblLead.LeadId,LeadNumber,tblLead.DocNo,tblLead.DocDate, tblActivity.LeadActivityId,
	tblLead.AddedBy,LeadStatus,tblLead.LeadDate,City,tblLead.LeadSource,tblLead.LeadOwner,tblLead.LeadPriority,
	tblLead.LeadOwnerContactNo,tblLead.LeadTitle,LeadCategory,LeadDesc,SpecialRequirement,
	tbllead.PotentialDealValue,tblLead.ProbabilityOfConversion,tblLead.ClosureForecast,
	tblLead.CompanyName,tblLead.BusinessSector,tblLead.ContactPerson,tblLead.ContactNo,
	tblLead.Designation,tblLead.Email,tblLead.[Address],tblLead.Website,
	tblActivity.MeetingMode,tblActivity.MeetingDate,tblActivity.ResponseDesc,tblActivity.NextAppointmentDate,
	tblActivity.IsReminderSet,tblActivity.ReminderDate,tblActivity.RemarkSlf as Remark
	

	From tSalTrkr tblLead inner join tSalTrkr1 tblActivity on tblLead.LeadId=tblActivity.LeadId
	where tblLead.LeadId=@LeadId order by tblActivity.UpdatedDate desc


	SELECT tblLead.LeadId,tblActivity.LeadActivityId,LeadStatus,Remark,ResponseDesc,tblActivity.UpdatedBy,tblActivity.UpdatedDate

	From tSalTrkr tblLead inner join tSalTrkr1 tblActivity on tblLead.LeadId=tblActivity.LeadId
	where tblLead.LeadId=@LeadId order by tblActivity.UpdatedDate asc
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLeadsBySelectedKPI]    Script Date: 1/25/2025 4:30:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetLeadsBySelectedKPI] 
(
@SelectedKPI NVARCHAR(50)=NULL
)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--Declare table variable 
	DECLARE @tblLead Table
	(
		LeadId int
	)
	if(@SelectedKPI='TotalLeads')
	begin
	INSERT INTO @tblLead(LeadId)
	SELECT LeadId from tSalTrkr
	end
	else if (@SelectedKPI='ActiveLeads')
	begin
	INSERT INTO @tblLead(LeadId)
	SELECT LeadId from tSalTrkr where isActive=1
	end
	else if(@SelectedKPI='ConvertedLeads')
	Begin
	INSERT INTO @tblLead(LeadId)
	SELECT LeadId from tSalTrkr where CurrentStatus='Qualified'
	end
	else if(@SelectedKPI='ActionableLeads')
	Begin
	WITH RankedActivities AS (
    SELECT 
        A.LeadActivityId,
        A.LeadId,
        A.LeadStatus,
        A.MeetingDate,
        A.ReminderDate,
        A.UpdatedDate,
        L.CurrentStatus,
        L.NextAppointmentDate,
        ROW_NUMBER() OVER (PARTITION BY A.LeadId ORDER BY A.UpdatedDate DESC) AS RowNum
    FROM 
        tSalTrkr L
    INNER JOIN 
        tSalTrkr1 A 
        ON L.LeadId = A.LeadId
		Where isActive=1
   
)
	INSERT INTO @tblLead(LeadId)
	SELECT 
    LeadId
   FROM 
    RankedActivities
WHERE 
    RowNum = 1 -- Only take the latest activity for each LeadId
	and (ReminderDate < GETDATE() AND UpdatedDate < ReminderDate) 
	and CurrentStatus<>'Qualified'
Union 
	SELECT 
    LeadId
   FROM 
    RankedActivities
WHERE 
    RowNum = 1 -- Only take the latest activity for each LeadId
	and (MeetingDate <= GETDATE() AND UpdatedDate < MeetingDate) and CurrentStatus<>'Qualified' 
	end
Begin
;With cteData as (
SELECT  tblLead.LeadId,tblLead.LeadNumber ,tblLeadActivity.LeadActivityId,tblLead.ContactPerson,ContactNo,tblLead.LeadDate,LeadStatus,tblLeadActivity.MeetingDate,tblLead.LeadCategory
,ReminderDate,ROW_NUMBER() Over(Partition by tblLeadActivity.LeadId order by tblLeadActivity.UpdatedDate) as RowNo

From [dbo].[tSalTrkr] tblLead inner join @tblLead as tblVarLead on tblLead.LeadId=tblVarLead.LeadId
inner join [dbo].[tSalTrkr1] tblLeadActivity on tblLead.LeadId=tblLeadActivity.LeadId
)
, MaxRowCTE As (
Select LeadId , Max(RowNo) As MaxRowNo from cteData group by LeadId
)
select cteData.LeadId,LeadNumber,LeadActivityId,ContactPerson,ContactNo,LeadDate,LeadStatus,MeetingDate,LeadCategory,ReminderDate,
case when @SelectedKPI='ActionableLeads' and MeetingDate<CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END AS IsOverdue
							from cteData inner join MaxRowCTE on 
							cteData.LeadId=MaxRowCTE.LeadId and cteData.RowNo=MaxRowCTE.MaxRowNo ORDER BY LeadDate desc
End
END
GO



ALTER TABLE [dbo].[tSalTrkr] ADD  CONSTRAINT [DF_tSalTrkr_LeadNumber]  DEFAULT ('LN'+CONVERT([nvarchar](50),NEXT VALUE FOR [LeadNumberSequence])) FOR [LeadNumber]




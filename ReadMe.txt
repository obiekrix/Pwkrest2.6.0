OBJECTIVE
----------
Password reset
Push notifications/Inbox messages
Phone number required to make payment
New parameters (CIS vend details) on printout


PROCEDURES TO CREATE
--------------------
UP_VerifyDealerEmail
UPM_GetMsg
UPM_GetMsgs
UP_LogANS


PROCEDURES TO CREATE
--------------------
UPR_GetPaymentLog


TABLE TO CREATE
---------------
CREATE TABLE [dbo].[ControlTable](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[phoneCheck] [bit] NOT NULL,
	[ringFenceCheck] [bit] NOT NULL
) ON [PRIMARY]
GO
insert into ControlTable(phoneCheck, ringFenceCheck) values (1, 1)
GO

FILES TO UPLOAD
---------------
All except web.config 


NOTE
----
We are no longer hardcoding the endpoint of which Conlog API to point to.
Open the live web.config file and modify <appSettings> section as below:
  <appSettings>
    <add key="clientId" value="Pawakad"/>
    <add key="clientKey" value="1234"/>
    <add key="apiEndPoint" value="http://conlogie.live.pawakad.com/svc/VendService.svc"/>
  </appSettings>


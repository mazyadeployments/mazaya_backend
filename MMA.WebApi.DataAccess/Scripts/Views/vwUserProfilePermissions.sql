CREATE VIEW [dbo].[vwUserProfilePermissions]
AS
SELECT DISTINCT
rp.PermissionId, user_data.AccountDataId, rp.RoleId,  user_profile.Id AS ProfileId, user_profile.CompanyId as ProfileCompanyId, upu.UnitId
FROM dbo.[User] AS user_data 
left JOIN dbo.UserProfile AS user_profile ON user_profile.UserId = user_data.Id AND user_profile.Active = 1 
LEFT JOIN UserProfileUnit upu on user_profile.Id = upu.UserProfileId
INNER JOIN dbo.RolePermission AS rp ON rp.RoleId = user_profile.RoleId


union

SELECT DISTINCT
rp.PermissionId, user_data.AccountDataId, rp.RoleId,  user_profile.Id AS ProfileId, user_profile.CompanyId as ProfileCompanyId, upu.UnitId  
from 
dbo.[User] user_data 
inner join UserDelegation ud on user_data.Id = ud.DelegateId and ud.Active = 1
left join UserProfile user_profile on user_profile.UserId = ud.UserId and user_profile.Active = 1
LEFT JOIN UserProfileUnit upu on user_profile.Id = upu.UserProfileId
INNER JOIN dbo.RolePermission AS rp ON rp.RoleId = user_profile.RoleId
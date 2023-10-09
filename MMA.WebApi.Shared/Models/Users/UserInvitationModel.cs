using System;

namespace MMA.WebApi.Shared.Models.Users
{
    public class UserInvitationModel
    {
        public string InvitedUserEmail { get; set; }
        public string UserId { get; set; }
        public string UserType { get; set; }
        public DateTime InvitedOn { get; set; }
        public bool IsRegistered { get; set; }
        public DateTime RegisteredOn { get; set; }
        public DateTime LastLoggedInOn { get; set; }
    }


    public class InviteUsersModel
    {
        public string InvitedUserEmail { get; set; }
        public string UserId { get; set; }
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }
    }

    public class InvitedFamilyMembers
    {
        public string UserId { get; set; }
        public string InvitedUserEmail { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UserType { get; set; }
    }
    public class InvitedByUser
    {
        public string Name { get; set; }
        public string ECard { get; set; }
        public string Email { get; set; }
    }
}

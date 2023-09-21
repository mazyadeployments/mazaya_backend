namespace MMA.WebApi.Shared.Models.ApplicationUsers
{
    public class InvitedUserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhotoUrl { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsActivated { get; set; }
        public bool IsInvited { get; set; }
    }
}

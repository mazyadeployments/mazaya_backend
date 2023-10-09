using Microsoft.AspNetCore.Identity;
using MMA.WebApi.Shared.Models.ApplicationUserDocument;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.Users;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.ApplicationUser
{
    public class ApplicationUserModel : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }
        public string Title { get; set; }
        public ICollection<ApplicationUserDocumentModel> ApplicationUserDocuments { get; set; }
        public ImageModel Image { get; set; }
        public List<ImageModel> ImageSets { get; set; }
        public ImageUrlsModel ImageUrls { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDescription { get; set; }
        public string ApproveStatus { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedOn { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string ConfirmPassword { get; set; }
        public string ECard { get; set; }
        public bool IsInvited { get; set; }
        public UserDomainModel UserType { get; set; }
        public DateTime LastDataSynchronizationOn { get; set; }
        public string PlatformType { get; set; }
        public List<InvitedFamilyMembers> InvitedFamilyMembers { get; set; }
        public InvitedByUser InvitedBy { get; set; }
        public string PhoneNumber { get; set; }
        public int CompanyId { get; set; }
        public IEnumerable<string> FcmDevice { get; set; }
        public IEnumerable<ECardModel> ECards { get; set; }
        public bool ReceiveAnnouncement { get; set; }
        public string ECardSequence { get; set; }
    }
}

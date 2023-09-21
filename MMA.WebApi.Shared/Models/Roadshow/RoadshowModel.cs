using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Models.Attachment;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.Image;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowModel
    {
        public int Id { get; set; }
        public int RoadshowInviteId { get; set; }
        public ICollection<DefaultLocationModel> Locations { get; set; } = new List<DefaultLocationModel>();
        public ICollection<RoadshowDocumentModel> Documents { get; set; } = new List<RoadshowDocumentModel>();
        public ICollection<RoadshowInviteModel> RoadshowInvites { get; set; } = new List<RoadshowInviteModel>();
        public ICollection<RoadshowCommentModel> Comments { get; set; } = new List<RoadshowCommentModel>();
        public ImageModel Image { get; set; }
        public List<ImageModel> ImageSets { get; set; } = new List<ImageModel>();
        public ImageUrlsModel ImageUrls { get; set; }
        public Declares.RoadshowStatus Status { get; set; }
        public string Title { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Description { get; set; }
        public string Activities { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public int OffersCount { get; set; }
        public int InvitedCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int ReviewCount { get; set; }
        public int RenegotiationCount { get; set; }
        public ICollection<RoadshowVoucherModel> RoadshowVouchers { get; set; } = new List<RoadshowVoucherModel>();
        public string InstructionBox { get; set; }
        public PhoneNumberModel PhoneNumber { get; set; }
        public string FocalPointName { get; set; }
        public string FocalPointSurname { get; set; }
        public string FocalPointEmail { get; set; }
        public AttachmentModel EmiratesId { get; set; }
        public string SupplierName { get; set; }



    }
}

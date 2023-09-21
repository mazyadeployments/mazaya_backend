using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.DataAccess.Models
{
    public class Announcement : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }
        public AnnouncementStatus Status { get; set; }
        public bool AllBuyers { get; set; }
        public bool AllSuppliers { get; set; }
        public bool SpecificBuyers { get; set; }
        public bool SpecificSuppliers { get; set; }
        public int CountAllToSend { get; set; }
        public int CountFailed { get; set; }
        public int CountSent { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public virtual ICollection<AnnouncementSpecificBuyer> SpecificBuyersCollection { get; set; }
        public virtual ICollection<AnnouncementSpecificSupplier> SpecificSuppliersCollection { get; set; }
        public virtual ICollection<AnnouncementAttachments> Attachments { get; set; }

        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }
    }
}

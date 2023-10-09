using Microsoft.AspNetCore.Identity;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class ApplicationUser : IdentityUser, IChangeable, IVisitable<IChangeable>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }
        public string Title { get; set; }
        public virtual ICollection<ApplicationUserDocument> ApplicationUserDocuments { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime LastDataSynchronizationOn { get; set; }
        public int UserType { get; set; }
        [ForeignKey("UserType")]
        public virtual UserDomain UserDomain { get; set; }
        public string ECardSequence { get; set; }
        public string PlatformType { get; set; }
        public bool ReceiveAnnouncement { get; set; }

        /// <summary>
        /// Accepts visit by visitor
        /// </summary>
        /// <param name="visitor">Visitor object can be null</param>
        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }
    }
}

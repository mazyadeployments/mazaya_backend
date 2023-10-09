using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class UserInvitations : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }
        public string InvitedUserEmail { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public int UserType { get; set; }

        [ForeignKey("UserType")]
        public virtual UserDomain UserDomain { get; set; }

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

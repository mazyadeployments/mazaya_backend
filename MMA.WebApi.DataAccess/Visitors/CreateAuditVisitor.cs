using MMA.WebApi.Shared.Interfaces.GenericData;
using System;

namespace MMA.WebApi.Shared.Visitor
{
    public class CreateAuditVisitor : IVisitor<IChangeable>
    {
        public string UserId { get; }
        public DateTime? UtcNow { get; }

        public CreateAuditVisitor(string userId, DateTime? utcNow = null)
        {
            UserId = userId;
            UtcNow = utcNow;
        }

        public void Visit(IChangeable model)
        {
            model.UpdatedBy = model.CreatedBy = UserId;
            model.UpdatedOn = model.CreatedOn = UtcNow ?? DateTime.UtcNow;
        }
    }
}

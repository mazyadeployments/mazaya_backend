using MMA.WebApi.Shared.Interfaces.GenericData;
using System;

namespace MMA.WebApi.Shared.Visitor
{
    public class UpdateAuditVisitor : IVisitor<IChangeable>
    {
        public string UserId { get; }
        public DateTime? UtcNow { get; }

        public UpdateAuditVisitor(string userId, DateTime? utcNow = null)
        {
            UserId = userId;
            UtcNow = utcNow;
        }

        public void Visit(IChangeable model)
        {
            model.UpdatedBy = UserId;
            model.UpdatedOn = UtcNow ?? DateTime.UtcNow;
        }
    }
}

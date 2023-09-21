using MMA.WebApi.Shared.Interfaces.GenericData;
using System;

namespace MMA.WebApi.Shared.Helpers
{
    public static class ChangeableHelper
    {
        public static void Set(IChangeable model, string userId, bool bIsNew)
        {
            if (bIsNew == true)
            {
                model.CreatedBy = userId;
                model.CreatedOn = DateTime.UtcNow;
            }


            model.UpdatedBy = userId;
            model.UpdatedOn = DateTime.UtcNow;
        }
    }
}

using System;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
	/// Represents changeable entity for which we can track details of a change.
	/// </summary>
	public interface IChangeable
    {
        DateTime CreatedOn { get; set; }
        string CreatedBy { get; set; }
        DateTime UpdatedOn { get; set; }
        string UpdatedBy { get; set; }
    }
}

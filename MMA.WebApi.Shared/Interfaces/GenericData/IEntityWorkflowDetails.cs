using System;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    public interface IChangeableWorkflowDetails
    {
        int WorkflowInstId { get; set; }
        DateTime WorkflowInstCreated { get; set; }
    }
}

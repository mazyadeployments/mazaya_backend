using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Jobs
{
    public interface ICheckExpiredDelegationsJob
    {
        Task CheckExpiredDelegationsTask();
    }
}

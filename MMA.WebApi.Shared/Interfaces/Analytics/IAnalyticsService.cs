using System;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Analytics
{
    public interface IAnalyticsService
    {
        Task<byte[]> ExportAnalyticsReporttData(string analyticsReportType);
        Task<byte[]> ExportSupplierReport(DateTime startDate, DateTime endDate);
    }
}

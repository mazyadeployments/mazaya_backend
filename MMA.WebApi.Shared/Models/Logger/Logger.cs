using MMA.WebApi.Shared.Interfaces.Logger;
using NLog;

namespace MMA.WebApi.Shared.Models.Logger
{
    public class Logger : IAppLogger
    {
        private static readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();

        public Logger()
        {
        }

        public void Debug(string message)
        {
            try
            { _log.Debug(message); }
            catch (System.Exception) { }

        }

        public void Error(string message)
        {
            try
            { _log.Error(message); }
            catch (System.Exception) { }

        }

        public void Fatal(string message)
        {
            try
            { _log.Fatal(message); }
            catch (System.Exception) { }

        }

        public void Info(string message)
        {
            try
            { _log.Info(message); }
            catch (System.Exception) { }

        }

        public void Warn(string message)
        {
            try
            { _log.Warn(message); }
            catch (System.Exception) { }

        }
    }
}

namespace MMA.WebApi.Shared.Interfaces.Logger
{
    public interface IAppLogger
    {
        void Debug(string message);
        void Error(string message);
        void Fatal(string message);
        void Info(string message);
        void Warn(string message);
    }
}

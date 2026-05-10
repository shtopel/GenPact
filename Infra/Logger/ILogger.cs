using System;
using System.Threading.Tasks;

namespace Infra.Logging
{
    public interface ILoggerAdapter
    {
        void Info(string message);
        void Error(string message, Exception ex = null);
        void Step(string name, Action action);
        Task StepAsync(string name, Func<Task> action);
        void AttachText(string name, string content);
        void AttachFile(string name, string path);
        Task<T> StepAsync<T>(string name, Func<Task<T>> action);
    }
}
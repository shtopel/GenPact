using Allure.Net.Commons;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Logging
{
    public class AllureLogger : ILoggerAdapter
    {
        public void Info(string message)
        {
            AllureApi.Step($"[INFO] {message}");
            System.Console.WriteLine($"INFO: {message}");
        }

        public void Error(string message, System.Exception ex = null)
        {
            AllureApi.Step($"❌ ERROR: {message}");
            if (ex != null)
            {
                AttachText("Exception Details", ex.ToString());
            }
        }

        public void Step(string name, System.Action action) => AllureApi.Step(name, action);

        public async Task StepAsync(string name, System.Func<Task> action) => await AllureApi.Step(name, action);

        public async Task<T> StepAsync<T>(string name, System.Func<Task<T>> action) => await AllureApi.Step(name, action);

        public void AttachText(string name, string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            // AllureApi handles raw byte attachments with extension
            AllureApi.AddAttachment(name, "text/plain", bytes, ".txt");
        }

        public void AttachFile(string name, string path)
        {
            if (File.Exists(path))
            {
                // AllureApi handles file paths directly
                AllureApi.AddAttachment(name, "application/octet-stream", path);
            }
            else
            {
                Error($"Failed to attach file: {path} does not exist.");
            }
        }
    }
}

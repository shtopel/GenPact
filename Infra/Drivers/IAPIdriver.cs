using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Drivers
{
    public interface IAPIdriver
    {
   
        Task<string> PostAsync(string url, object? body = null);
        Task<string> GetAsync(string url, Dictionary<string, object>? query = null);
    }
}

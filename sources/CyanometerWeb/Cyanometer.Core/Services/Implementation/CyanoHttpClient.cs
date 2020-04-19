using Cyanometer.Core.Services.Abstract;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cyanometer.Core.Services.Implementation
{
    public class CyanoHttpClient : ICyanoHttpClient
    {
        readonly HttpClient client = new HttpClient();

        public Task<string> GetAsync(string requestUri) => client.GetStringAsync(requestUri);
    }
}

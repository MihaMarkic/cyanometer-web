using System.Threading.Tasks;

namespace Cyanometer.Core.Services.Abstract
{
    public interface ICyanoHttpClient
    {
        Task<string> GetAsync(string requestUri);
    }
}

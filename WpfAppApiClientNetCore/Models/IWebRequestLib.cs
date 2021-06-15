using System.Threading.Tasks;
using WpfAppApiClientNetCore.Helpers;

namespace WpfAppApiClientNetCore.Models
{
    public interface IWebRequestLib
    {
        void CheckForNullValues(WebRequest webRequest);
        Task<WebReturn> MyWebRequestMethodAsync(WebRequest webRequest);
    }
}
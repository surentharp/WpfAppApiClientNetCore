using System.Collections.Generic;
using System.Threading.Tasks;
using WpfAppApiClientNetCore.Helpers;

namespace WpfAppApiClientNetCore.Models
{
    public interface IApiConsumerModel
    {
        string ConvertToJson(Person person);
        Task<RequestReturn> DeleteDataAsync(Person person);
        Task<List<Person>> ExportUserAsync(Person user, int totalPages);
        Task<PagePerson> GetPageAsync(int page);
        Task<Person> GetUserAsync(int id);
        Task<RequestReturn> PatchDataAsync(Person person);
        Task<RequestReturn> PostDataAsync(Person person);
        Task<PagePerson> SearchedUserGetPageAsync(Person user, int page);
        Task<PagePerson> SearchUserAsync(Person user);
    }
}
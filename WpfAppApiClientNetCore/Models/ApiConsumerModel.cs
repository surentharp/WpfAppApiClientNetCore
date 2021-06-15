using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WpfAppApiClientNetCore.Helpers;

namespace WpfAppApiClientNetCore.Models
{
    public class ApiConsumerModel : IApiConsumerModel
    {
        //BaseURI
        private string BaseURI { get; set; }
        //Bearer Token
        private string BearerToken { get; set; }
        //Model for requesting api calls
        private static IWebRequestLib _webRequestLib;
        //Configuration object
        private static IConfiguration _iConfig;

        public ApiConsumerModel(IWebRequestLib webRequestLib, IConfiguration iConfig)
        {
            //Instantiating the configuration object
            _iConfig = iConfig;
            //Instantiating the webqrequest class
            _webRequestLib = webRequestLib;
            //set base uri
            BaseURI = _iConfig.GetValue<string>("MySettings:BaseURI");
            //set bearer token
            BearerToken = _iConfig.GetValue<string>("MySettings:BearerToken");
        }

        //Method for retreving the page of users from api
        public async Task<PagePerson> GetPageAsync(int page)
        {
            //set page correct
            page = page < 0 ? 0 : page;

            //prepare the object for the call
            WebRequest ww = new WebRequest
            {
                BaseURI = BaseURI,
                RequestURI = $"users?page={page.ToString()}",
                BearerToken = BearerToken,
                Body = "",
                HttpVerb = MyHttpVerb.GET
            };

            //call api
            WebReturn webReturn = await _webRequestLib.MyWebRequestMethodAsync(ww);

            //parse the received object
            PagePerson _person = JsonConvert.DeserializeObject<PagePerson>(webReturn.ResponseContent);

            //return
            return _person;
        }

        //Method for searching the user
        public async Task<PagePerson> SearchUserAsync(Person user)
        {
            //convert and check, if there are any null or empty value, if any, throw exception
            string json = ConvertToJson(user);

            //returning object
            PagePerson _person = new PagePerson();

            // set input correct
            string name = user.name ?? "";
            string email = user.email ?? "";

            //call the api
            WebReturn webReturn = await _webRequestLib.MyWebRequestMethodAsync(new WebRequest
            {
                BaseURI = BaseURI,
                RequestURI = $"users?name={name}&email={email}",
                BearerToken = BearerToken,
                Body = "",
                HttpVerb = MyHttpVerb.GET
            });

            //parse and store the response
            _person = JsonConvert.DeserializeObject<PagePerson>(webReturn.ResponseContent);

            //return
            return _person;
        }

        //Method for getting users of a specified page in the searched group
        public async Task<PagePerson> SearchedUserGetPageAsync(Person user, int page)
        {
            //set page correct
            page = page < 0 ? 0 : page;

            //convert and check, if there are any null or empty value, if any, throw exception
            string json = ConvertToJson(user);

            //returing object
            PagePerson _person = new PagePerson();

            //set the input correct
            string name = user.name ?? "";
            string email = user.email ?? "";

            //prepare and call the api
            WebReturn webReturn = await _webRequestLib.MyWebRequestMethodAsync(new WebRequest
            {
                BaseURI = BaseURI,
                RequestURI = $"users?name={name}&email={email}&page={page}",
                BearerToken = BearerToken,
                Body = "",
                HttpVerb = MyHttpVerb.GET
            });

            //parse and store the response
            _person = JsonConvert.DeserializeObject<PagePerson>(webReturn.ResponseContent);

            //return
            return _person;
        }

        //Method for query and export users
        public async Task<List<Person>> ExportUserAsync(Person user, int totalPages)
        {
            //convert and check, if there are any null or empty value, if any, throw exception
            string json = ConvertToJson(user);
            
            //returing object
            List<Person> _mySearchedPersons = new List<Person>();

            //set the input correct
            string name = user.name ?? "";
            string email = user.email ?? "";

            //if total pages is greater than zero, continue
            if (totalPages != 0)
            {
                //call all the pages requested and store it in the collection
                for (int i = 0; i < totalPages; i++)
                {
                    //call the api
                    WebReturn webReturn = await _webRequestLib.MyWebRequestMethodAsync(new WebRequest
                    {
                        BaseURI = BaseURI,
                        RequestURI = $"users?name={name}&email={email}",
                        BearerToken = BearerToken,
                        Body = "",
                        HttpVerb = MyHttpVerb.GET
                    });

                    //parse the response and store
                    var _person = JsonConvert.DeserializeObject<PagePerson>(webReturn.ResponseContent);

                    //store it in the collection
                    _mySearchedPersons.AddRange(_person.data);
                }
            }

            //return
            return _mySearchedPersons;
        }

        public async Task<Person> GetUserAsync(int id)
        {
            //returning object
            Person _person = new Person();

            //prepare and call the api
            WebReturn webReturn = await _webRequestLib.MyWebRequestMethodAsync(new WebRequest
            {
                BaseURI = BaseURI,
                RequestURI = $"users/{id.ToString()}",
                BearerToken = BearerToken,
                Body = "",
                HttpVerb = MyHttpVerb.GET
            });

            //if the response code is 200, then store the retrevied object, or else, store only the eTag
            if (webReturn.code == 200)
            {
                var result = (JObject)JsonConvert.DeserializeObject(webReturn.ResponseContent.ToString());
                _person = JsonConvert.DeserializeObject<Person>(result["data"].ToString());
            }

            //store the retreived eTag
            _person.Etag = webReturn.Etag;

            //return
            return _person;
        }

        //Method for posting the data ot eh api
        public async Task<RequestReturn> PostDataAsync(Person person)
        {
            //parse the input to json
            var json = JsonConvert.SerializeObject(new { name = person.name, email = person.email, status = person.status, gender = person.gender });

            //prepare and call the api
            WebReturn webReturn = await _webRequestLib.MyWebRequestMethodAsync(new WebRequest
            {
                BaseURI = BaseURI,
                RequestURI = $"users",
                BearerToken = BearerToken,
                Body = json,
                HttpVerb = MyHttpVerb.POST
            });

            //store the response object
            RequestReturn requestReturn = new RequestReturn { code = webReturn.code, content = webReturn.ResponseContent };

            //return
            return requestReturn;
        }

        //Method for patching the data in the api
        public async Task<RequestReturn> PatchDataAsync(Person person)
        {
            //convert and check, if there are any null or empty value, if any, throw exception
            string json = ConvertToJson(person);

            //prepare and call the api
            WebReturn webReturn = await _webRequestLib.MyWebRequestMethodAsync(new WebRequest
            {
                BaseURI = BaseURI,
                RequestURI = $"users/{person.id.ToString()}",
                BearerToken = BearerToken,
                Body = json,
                HttpVerb = MyHttpVerb.PATCH
            });

            //store the response
            RequestReturn requestReturn = new RequestReturn { code = webReturn.code, content = webReturn.ResponseContent };

            //return
            return requestReturn;

        }

        //convert and check the input, if there are any null or empty value, if any, throw exception
        public string ConvertToJson(Person person)
        {
            return JsonConvert.SerializeObject(new { id = person.id, name = person.name, email = person.email, status = person.status, gender = person.gender });
        }

        //Method for deleting a data in the api
        public async Task<RequestReturn> DeleteDataAsync(Person person)
        {
            //convert and check, if there are any null or empty value, if any, throw exception
            var MyJsonString = ConvertToJson(person);

            //prepare and call the api
            WebReturn webReturn = await _webRequestLib.MyWebRequestMethodAsync(new WebRequest
            {
                BaseURI = BaseURI,
                RequestURI = $"users/{person.id.ToString()}",
                BearerToken = BearerToken,
                Body = "",
                HttpVerb = MyHttpVerb.DELETE
            });

            //store the response
            RequestReturn requestReturn = new RequestReturn { code = webReturn.code, content = webReturn.ResponseContent };

            //return
            return requestReturn;
        }



    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WpfAppApiClientNetCore.Helpers;

namespace WpfAppApiClientNetCore.Models
{
    public class WebRequestLib : IWebRequestLib
    {
        //Restsharp
        private static IRestClient _restClient;

        public WebRequestLib(IRestClient restClient)
        {
            //Instantiate restsharp client
            _restClient = restClient;
        }

        //public WebReturn()
        public async Task<WebReturn> MyWebRequestMethodAsync(WebRequest webRequest)
        {
            //Check for null input values, if there, throw exception
            CheckForNullValues(webRequest);

            //return object
            WebReturn webReturn = new WebReturn();

            //value for checking the receiving status code
            int SuccessCode = 0;

            //set base url
            _restClient.BaseUrl = new Uri(webRequest.BaseURI);

            //restsharp restrequest
            var restRequest = new RestRequest(webRequest.RequestURI);

            //json body for the request
            var json = webRequest.Body ?? "";

            switch (webRequest.HttpVerb)
            {
                case MyHttpVerb.GET:
                    {
                        //get success code is 200 (gorest)
                        restRequest.Method = Method.GET;
                        SuccessCode = 200;
                        break;
                    }
                case MyHttpVerb.PATCH:
                    { 
                        //get success code is 200 (gorest) and it have body
                        restRequest.Method = Method.PATCH;
                        restRequest.AddJsonBody(json);
                        SuccessCode = 200;
                        break;
                    }
                case MyHttpVerb.DELETE:
                    {
                        //get success code is 204 (gorest)
                        restRequest.Method = Method.DELETE;
                        SuccessCode = 204;
                        break;
                    }
                case MyHttpVerb.POST:
                    {
                        //get success code is 201 (gorest) and it have body
                        restRequest.Method = Method.POST;
                        restRequest.AddJsonBody(json);
                        SuccessCode = 201;
                        break;
                    }
                default:
                    {
                        //default http verb is get
                        restRequest.Method = Method.GET;
                        break;
                    }
            }

            //set content type and bearer token
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("Authorization", "Bearer " + $"{webRequest.BearerToken}");

            //execute api call
            var response = await _restClient.ExecuteAsync(restRequest);

            //parse and store the result in json format
            var result = (JObject)JsonConvert.DeserializeObject(response.Content.ToString());

            //parse the return code
            int code = (int)result["code"];

            //get eTag from the received response, only if the request is successful
            string ETag = "";
            if (code == SuccessCode)
            {
                foreach (var item_header in response.Headers)
                {
                    if (item_header.Name.ToLower() == "etag")
                    {
                        ETag = (item_header.Value ?? "").ToString();
                    }
                }
            }

            // return value
            webReturn = new WebReturn { code = code, Etag = ETag, ResponseContent = response.Content.ToString() };
            return webReturn;

        }

        //if null or empty value, throw exception
        public void CheckForNullValues(WebRequest webRequest)
        {
            if (webRequest == null)
            {
                throw new ArgumentNullException("WebRequest is NULL");
            }

            if (webRequest.BaseURI == "" && webRequest.BearerToken == "")
            {
                throw new ArgumentException("Some arguments are empty");
            }
        }
    }
}

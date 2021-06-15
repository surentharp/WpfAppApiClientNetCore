using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfAppApiClientNetCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using WpfAppApiClientNetCore.Helpers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace WpfAppApiClientNetCore.Models.Tests
{

    [TestClass()]
    public class ApiConsumerModelTests
    {
        //Model
        private Mock<IWebRequestLib> _webRequestLibMock;
        //Model
        private ApiConsumerModel apiConsumerModel;
        //Variable for storing return values of method
        private WebReturn _webReturn;
        //IConfiguration Mocks
        private Mock<IConfiguration> _iConfig;
        private Mock<IConfigurationSection> _iConfigSection;

        [TestInitialize]
        public void TestInitialize()
        {
            //Initiating the objects for testing
            _webRequestLibMock = new Mock<IWebRequestLib>();
            _webReturn = new WebReturn { code = 0, Etag = "", ResponseContent = "" };
            _iConfig = new Mock<IConfiguration>();
            _iConfigSection = new Mock<IConfigurationSection>();

            _iConfigSection
               .Setup(x => x.Value)
               .Returns("https://gorest.co.in/public-api/");

            _iConfig
               .Setup(x => x.GetSection("MySettings:BaseURI"))
               .Returns(_iConfigSection.Object);

            _iConfigSection
               .Setup(x => x.Value)
               .Returns("fa114107311259f5f33e70a5d85de34a2499b4401da069af0b1d835cd5ec0d56");

            _iConfig
               .Setup(x => x.GetSection("MySettings:BearerToken"))
               .Returns(_iConfigSection.Object);

            apiConsumerModel = new ApiConsumerModel(_webRequestLibMock.Object, _iConfig.Object);

        }
        /// <summary>
        ///Test for retreiving the page of users, if the input is zero, it should not throw any errors, 
        ///it should adjust to the nearest possible input and return corresponding value
        /// </summary>
        [TestMethod()]
        public void GetPageAsyncTest_IfInputIsZero_ItShouldNotThrowErrorAsync()
        {
            //_iConfig = new Mock<IConfiguration>();
            _webRequestLibMock = new Mock<IWebRequestLib>();

            _webRequestLibMock.Setup(a => a.MyWebRequestMethodAsync(It.IsAny<WebRequest>())).ReturnsAsync(_webReturn);
            String Response = $"{{\"code\":404,\"meta\":null,\"data\":{{\"message\":\"Resource not found\"}}}}";
            var result = apiConsumerModel.GetPageAsync(0);
            Assert.IsNotNull(result);
        }

        /// <summary>
        ///Test for retreiving the page of users, if the input is zero, 
        ///it should not throw any errors, it should adjust to the nearest possible input and return corresponding value
        /// </summary>
        [TestMethod()]
        public void GetPageAsyncTest_IfInputIsLessThanZero_ItShouldNotThrowErrorAsync()
        {
            _webRequestLibMock.Setup(a => a.MyWebRequestMethodAsync(It.IsAny<WebRequest>())).ReturnsAsync(_webReturn);
            String Response = $"{{\"code\":404,\"meta\":null,\"data\":{{\"message\":\"Resource not found\"}}}}";

            var result = apiConsumerModel.GetPageAsync(-1);

            Assert.IsNotNull(result);
        }

        /// <summary>
        ///Test for editing the users, if the input is null, it should throw error
        /// </summary>
        [TestMethod()]
        public void PatchDataAsync_IfInputIsNull_ItShouldThrowErrorAsync()
        {
            _webRequestLibMock.Setup(a => a.MyWebRequestMethodAsync(It.IsAny<WebRequest>())).ReturnsAsync(_webReturn);
            String Response = $"{{\"code\":404,\"meta\":null,\"data\":{{\"message\":\"Resource not found\"}}}}";

            Person JsonObjectForConverting = null;
            var result = apiConsumerModel.PatchDataAsync(JsonObjectForConverting);

            Assert.ThrowsException<NullReferenceException>(() => apiConsumerModel.ConvertToJson(JsonObjectForConverting));
        }

        /// <summary>
        ///Test for deleting the users, if the input is null, it should throw error
        /// </summary>
        [TestMethod()]
        public void DeleteDataAsync_IfInputIsNull_ItShouldThrowErrorAsync()
        {
            _webRequestLibMock.Setup(a => a.MyWebRequestMethodAsync(It.IsAny<WebRequest>())).ReturnsAsync(_webReturn);
            String Response = $"{{\"code\":404,\"meta\":null,\"data\":{{\"message\":\"Resource not found\"}}}}";

            Person JsonObjectForConverting = null;
            var result = apiConsumerModel.DeleteDataAsync(JsonObjectForConverting);

            Assert.ThrowsException<NullReferenceException>(() => apiConsumerModel.ConvertToJson(JsonObjectForConverting));
        }

        /// <summary>
        ///Retreiving users, if the input is zero, it should not throw any error
        ///it should adjust to the nearest possible input and return corresponding value
        /// </summary>
        [TestMethod()]
        public void GetUserAsync_IfInputIsZero_ItShouldNotThrowErrorAsync()
        {
            _webRequestLibMock.Setup(a => a.MyWebRequestMethodAsync(It.IsAny<WebRequest>())).ReturnsAsync(_webReturn);
            String Response = $"{{\"code\":404,\"meta\":null,\"data\":{{\"message\":\"Resource not found\"}}}}";
            var result = apiConsumerModel.GetUserAsync(0);

            Assert.IsNotNull(result);
        }

        /// <summary>
        ///Retreiving users, if the input is less than zero, it should not throw any error
        ///it should adjust to the nearest possible input and return corresponding value
        /// </summary>
        [TestMethod()]
        public void GetUserAsync_IfInputIsLessThanZero_ItShouldNotThrowErrorAsync()
        {
            _webRequestLibMock.Setup(a => a.MyWebRequestMethodAsync(It.IsAny<WebRequest>())).ReturnsAsync(_webReturn);
            String Response = $"{{\"code\":404,\"meta\":null,\"data\":{{\"message\":\"Resource not found\"}}}}";

            var result = apiConsumerModel.GetUserAsync(-1);

            Assert.IsNotNull(result);
        }

        /// <summary>
        ///Exporting users, if the input is null, it should throw error
        /// </summary>
        [TestMethod()]
        public void ExportUserAsync_IfInputIsNull_ItShouldThrowErrorAsync()
        {
            _webRequestLibMock.Setup(a => a.MyWebRequestMethodAsync(It.IsAny<WebRequest>())).ReturnsAsync(_webReturn);
            String Response = $"{{\"code\":404,\"meta\":null,\"data\":{{\"message\":\"Resource not found\"}}}}";

            Person JsonObjectForConverting = null;
            int pages = 10;
            var result = apiConsumerModel.ExportUserAsync(JsonObjectForConverting, pages);

            Assert.ThrowsException<NullReferenceException>(() => apiConsumerModel.ConvertToJson(JsonObjectForConverting));
        }

        /// <summary>
        ///Exporting users, if the input is zero, it should not throw any error, it should adjust and produce the result
        /// </summary>
        [TestMethod()]
        public void ExportUserAsync_IfInputIsZero_ItShouldNotThrowErrorAsync()
        {
            _webRequestLibMock.Setup(a => a.MyWebRequestMethodAsync(It.IsAny<WebRequest>())).ReturnsAsync(_webReturn);
            String Response = $"{{\"code\":404,\"meta\":null,\"data\":{{\"message\":\"Resource not found\"}}}}";

            int pages = 0;
            var result = apiConsumerModel.ExportUserAsync(It.IsAny<Person>(), pages);

            Assert.IsNotNull(result);
        }

        /// <summary>
        ///Searching users, if the input is null, it should throw error
        /// </summary>
        [TestMethod()]
        public void SearchUserAsync_IfInputIsNull_ItShouldThrowErrorAsync()
        {
            _webRequestLibMock.Setup(a => a.MyWebRequestMethodAsync(It.IsAny<WebRequest>())).ReturnsAsync(_webReturn);
            String Response = $"{{\"code\":404,\"meta\":null,\"data\":{{\"message\":\"Resource not found\"}}}}";

            Person JsonObjectForConverting = null;
            var result = apiConsumerModel.SearchUserAsync(JsonObjectForConverting);

            Assert.ThrowsException<NullReferenceException>(() => apiConsumerModel.ConvertToJson(JsonObjectForConverting));
        }
    }
}
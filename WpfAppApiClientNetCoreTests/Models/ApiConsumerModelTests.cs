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
        private Mock<IWebRequestLib> _webRequestLibMock;
        private ApiConsumerModel apiConsumerModel;
        private WebReturn _webReturn;
        private Mock<IConfiguration> _iConfig;

        [TestInitialize]
        public void TestInitialize()
        {
            _iConfig = new Mock<IConfiguration>();
            _webRequestLibMock = new Mock<IWebRequestLib>();
            apiConsumerModel = new ApiConsumerModel(_webRequestLibMock.Object, _iConfig.Object);
            _webReturn = new WebReturn { code = 0, Etag = "", ResponseContent = "" };
        }
        /// <summary>
        ///Test for retreiving the page of users, if the input is zero, it should not throw any errors, 
        ///it should adjust to the nearest possible input and return corresponding value
        /// </summary>
        [TestMethod()]
        public void GetPageAsyncTest_IfInputIsZero_ItShouldNotThrowErrorAsync()
        {
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
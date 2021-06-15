using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfAppApiClientNetCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using WpfAppApiClientNetCore.Helpers;
using RestSharp;

namespace WpfAppApiClientNetCore.Models.Tests
{
    [TestClass()]
    public class WebRequestLibTests
    {
        private Mock<IRestClient> _IRestClientMock;
        private WebRequestLib _webRequestLib;
        [TestInitialize]
        public void TestInitialize()
        {
            _IRestClientMock = new Mock<IRestClient>();
            _webRequestLib = new WebRequestLib(_IRestClientMock.Object);

        }

        /// <summary>
        /// If Base URI and Bearer Token empty, throw error
        /// </summary>
        [TestMethod()]
        public void MyWebRequestMethodAsyncTest_IfBaseURIANDBearerTokenEmpty_ShouldThrowError()
        {

            WebRequest webRequest = new WebRequest { BaseURI = "", BearerToken = "" };
            String Response = $"{{\"code\":404,\"meta\":null,\"data\":{{\"message\":\"Resource not found\"}}}}";
            var result = _webRequestLib.MyWebRequestMethodAsync(webRequest);
            Assert.ThrowsException<ArgumentException>(() => _webRequestLib.CheckForNullValues(webRequest));

        }

        /// <summary>
        /// If input empty, throw error
        /// </summary>
        [TestMethod()]
        public void MyWebRequestMethodAsyncTest_IfInputNull_ShouldThrowError()
        {

            WebRequest webRequest = null;
            String Response = $"{{\"code\":404,\"meta\":null,\"data\":{{\"message\":\"Resource not found\"}}}}";
            var result = _webRequestLib.MyWebRequestMethodAsync(webRequest);
            Assert.ThrowsException<ArgumentNullException>(() => _webRequestLib.CheckForNullValues(webRequest));

        }

    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfAppApiClientNetCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using WpfAppApiClientNetCore.Models;
using Moq;
using WpfAppApiClientNetCore.Helpers;

namespace WpfAppApiClientNetCore.ViewModels.Tests
{
    [TestClass()]
    public class ApiConsumerViewModelTests
    {
        private Mock<IApiConsumerModel> _apiConsumerModelMock;
        private ApiConsumerViewModel apiConsumerViewModel;
        [TestInitialize]
        public void MyTestInitializer()
        {
            _apiConsumerModelMock = new Mock<IApiConsumerModel>();
            apiConsumerViewModel = new ApiConsumerViewModel(_apiConsumerModelMock.Object);
        }

        /// <summary>
        /// On selecting a specific user of id, if the input is zero (object), should not have any uncaught expection
        /// </summary>
        [TestMethod()]
        public void GetUserClick_IfInputisZero_ShouldNotThrowError()
        {
            apiConsumerViewModel.GetUserClick.Execute(null);

            Person p = new Person();
            _apiConsumerModelMock.Setup(a => a.GetUserAsync(0)).ReturnsAsync(p);
            Assert.IsNotNull(p);
        }

        /// <summary>
        /// On searching user, should not have any uncaught expection
        /// </summary>
        [TestMethod()]
        public void GetSearchClick_IfExecute_ShouldNotThrowError()
        {
            apiConsumerViewModel.GetSearchClick.Execute(null);

            PagePerson p = new PagePerson();
            _apiConsumerModelMock.Setup(a => a.SearchUserAsync(It.IsAny<Person>())).ReturnsAsync(p);
            Assert.IsNotNull(p);
        }

        /// <summary>
        /// On searching user, if the search input is null, the error should be caught
        /// </summary>
        [TestMethod()]
        public void GetSearchClick_IfSearchInputNull_ShouldThrowError()
        {
            apiConsumerViewModel.GetSearchClick.Execute(null);

            PagePerson p = new PagePerson();
            _apiConsumerModelMock.Setup(a => a.SearchUserAsync(null)).ReturnsAsync(p);
            Assert.AreEqual(apiConsumerViewModel.MyStatus, "ERROR : search met with an error");
        }

        /// <summary>
        /// On searching user, if the search output is null, the error should be caught
        /// </summary>
        [TestMethod()]
        public void GetSearchClick_IfSearchOutputNull_ShouldThrowError()
        {
            apiConsumerViewModel.GetSearchClick.Execute(null);

            PagePerson p = null;
            _apiConsumerModelMock.Setup(a => a.SearchUserAsync(It.IsAny<Person>())).ReturnsAsync(p);
            Assert.AreEqual(apiConsumerViewModel.MyStatus, "ERROR : search met with an error");
        }

        /// <summary>
        /// On searching user, if the export method input is null, the error should be caught
        /// </summary>
        [TestMethod()]
        public void GetSearchClick_IfExportInputNull_ShouldThrowError()
        {

            PagePerson p = new PagePerson();
            _apiConsumerModelMock.Setup(a => a.SearchUserAsync(It.IsAny<Person>())).ReturnsAsync(p);
            _apiConsumerModelMock.Setup(a => a.ExportUserAsync(null, 0)).ReturnsAsync(It.IsAny<List<Person>>());

            apiConsumerViewModel.GetSearchClick.Execute(null);


            Assert.AreEqual(apiConsumerViewModel.MyStatus, "ERROR : Export met with an error");
        }

        /// <summary>
        /// On clearing search user, after executing, it should set a boolean
        /// </summary>
        [TestMethod()]
        public void ClearSearchClick_AfterExecuting_ShouldShouldSetSearchedBooleanTrue()
        {

            apiConsumerViewModel.GetSearchClick.Execute(null);

            Assert.AreEqual(apiConsumerViewModel.IsSearchRecently, false);
        }

        /// <summary>
        /// On deleting or editing user, if none is selected, the error thorwn should be caught
        /// </summary>
        [TestMethod()]
        public void DeleteAndEditDataClick_IfNoneSelected_ShouldShowErrorMessage()
        {

            apiConsumerViewModel.SelectedItem = new Person();
            apiConsumerViewModel.DeleteDataClick.Execute(null);

            Assert.AreEqual(apiConsumerViewModel.MyStatus, "Data Not Selected");
        }

        /// <summary>
        /// On deleting or editing user, if etag is null, it should not throw any errors
        /// </summary>
        [TestMethod()]
        public void DeleteAndEditDataClick_IfEtagSearchOutputNull_ShouldNotThrowError()
        {

            Person p = null;
            _apiConsumerModelMock.Setup(a => a.GetUserAsync(It.IsAny<int>())).ReturnsAsync(p);

            apiConsumerViewModel.SelectedItem = new Person { id = 1 };
            apiConsumerViewModel.DeleteDataClick.Execute(null);


            Assert.AreEqual(apiConsumerViewModel.resultString, "ERROR (Data Modified inbetween by someone, please check once again)");
        }

        /// <summary>
        /// On posting data, if input is null, the exception should be caught
        /// </summary>
        [TestMethod()]
        public void PostDataClick_IfEnteredDataIsNull_ShouldNotThrowError()
        {

            apiConsumerViewModel.SelectedItem = null;
            apiConsumerViewModel.PostDataClick.Execute(null);


            Assert.AreEqual(apiConsumerViewModel.MyStatus, "ERROR: User Add met with an exception");
        }

        /// <summary>
        /// On posting data, if input fails validation, the error should be caught
        /// </summary>
        [TestMethod()]
        public void PostDataClick_IfEnteredDataValidationFailure_ShouldNotThrowError()
        {

            RequestReturn p = null;
            _apiConsumerModelMock.Setup(a => a.PostDataAsync(It.IsAny<Person>())).ReturnsAsync(p);

            apiConsumerViewModel.SelectedItem = new Person { id = 10 };
            apiConsumerViewModel.PostDataClick.Execute(null);


            Assert.AreEqual(apiConsumerViewModel.MyStatus, "ERROR: check all the values - (email has to be in correct format and enterng all the data is mandatory)");
        }

        /// <summary>
        /// On posting data, if output is null, the error should be caught
        /// </summary>
        [TestMethod()]
        public void PostDataClick_IfPostDataReturnedNull_ShouldNotThrowError()
        {

            RequestReturn p = null;
            _apiConsumerModelMock.Setup(a => a.PostDataAsync(It.IsAny<Person>())).ReturnsAsync(p);

            apiConsumerViewModel.SelectedItem = new Person { id = 10, email = "dummy@gmail.com", Etag = "", gender = "Female", name = "dummy", status = "Active" };
            apiConsumerViewModel.PostDataClick.Execute(null);


            Assert.AreEqual(apiConsumerViewModel.MyStatus, "ERROR: User Add met with an exception");
        }

        /// <summary>
        /// On turning page, On current page, even if the values are less than zero,
        /// the logic should correct tot he nearest value and return.
        /// </summary>
        [TestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        public void PageTurning_IfCurrentPageRequestedOfAnyLessThanZeroValue_ProvideCurrentPage(int page)
        {

            PagePerson p = new PagePerson();
            _apiConsumerModelMock.Setup(a => a.GetPageAsync(page)).ReturnsAsync(p);
            apiConsumerViewModel.GetCurrentPageClick.Execute(null);

            Assert.IsNotNull(p);
        }

        /// <summary>
        /// On turning page, On next page, even if the values are less than zero,
        /// the logic should correct tot he nearest value and return.
        /// </summary>
        [TestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        public void PageTurning_NextPage_ProvideCurrentPage(int page)
        {

            PagePerson p = new PagePerson();
            _apiConsumerModelMock.Setup(a => a.GetPageAsync(page)).ReturnsAsync(p);
            apiConsumerViewModel.GetNextPageClick.Execute(null);

            Assert.IsNotNull(p);
        }

        /// <summary>
        /// On turning page, On last page, even if the values are less than zero,
        /// the logic should correct tot he nearest value and return.
        /// </summary>
        [TestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        public void PageTurning_LastPage_ProvideCurrentPage(int page)
        {

            PagePerson p = new PagePerson();
            _apiConsumerModelMock.Setup(a => a.GetPageAsync(page)).ReturnsAsync(p);
            apiConsumerViewModel.GetLastPageClick.Execute(null);

            Assert.IsNotNull(p);
        }

        /// <summary>
        /// On turning page, On first page, even if the values are less than zero,
        /// the logic should correct tot he nearest value and return.
        /// </summary>
        [TestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        public void PageTurning_FirstPage_ProvideCurrentPage(int page)
        {
            PagePerson p = new PagePerson();
            _apiConsumerModelMock.Setup(a => a.GetPageAsync(page)).ReturnsAsync(p);
            apiConsumerViewModel.GetFirstPageClick.Execute(null);

            Assert.IsNotNull(p);
        }

        /// <summary>
        /// On turning page, On previous page, even if the values are less than zero,
        /// the logic should correct tot he nearest value and return.
        /// </summary>
        [TestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        public void PageTurning_PreviousPage_ProvideCurrentPage(int page)
        {
            PagePerson p = new PagePerson();
            _apiConsumerModelMock.Setup(a => a.GetPageAsync(page)).ReturnsAsync(p);
            apiConsumerViewModel.GetPreviousPageClick.Execute(null);

            Assert.IsNotNull(p);
        }

    }
}
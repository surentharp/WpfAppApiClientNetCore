using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfAppApiClientNetCore.Helpers;
using WpfAppApiClientNetCore.Models;

namespace WpfAppApiClientNetCore.ViewModels
{
    public class ApiConsumerViewModel : INotifyPropertyChanged, IApiConsumerViewModel
    {
        //Serilog
        private readonly ILogger _logger;
        //Message to display in the message box of view
        private string _resultString { get; set; }
        //Boolean for tracking search button click
        private bool _isSearchRecently { get; set; }
        //Boolean for tracking background work
        private bool _navigationVisibility { get; set; }
        //String for setting the status bar message
        private string _myStatus { get; set; }
        //The object for storing the pagination values received from api
        private Pagination _pagination { get; set; }
        //The collection for storing the user data received from the api
        private List<Person> _persons { get; set; }
        //Object for retreiving the received response from the api
        private List<RequestReturn> _myRequestReturns { get; set; }
        //Object for selected users from the list box
        private Person _SelectedItem { get; set; }
        //Object for search user
        private Person _SearchUser { get; set; }
        //Command for going next page of the users retreived list
        public ICommand GetNextPageClick { get; set; }
        //Command for going previous page of the users retreived list
        public ICommand GetPreviousPageClick { get; set; }
        //Command for going First page of the users retreived list
        public ICommand GetFirstPageClick { get; set; }
        //Command for going Last page of the users retreived list
        public ICommand GetLastPageClick { get; set; }
        //Command for adding user
        public ICommand PostDataClick { get; set; }
        //Command for editing user
        public ICommand EditDataClick { get; set; }
        //Command for deleting user
        public ICommand DeleteDataClick { get; set; }
        //Command for refreshing the viewed page again
        public ICommand GetCurrentPageClick { get; set; }
        //Command for retreiving single user
        public ICommand GetUserClick { get; set; }
        //Command for retreiving the searched user
        public ICommand GetSearchClick { get; set; }
        //Command for clearing the searched user
        public ICommand ClearSearchClick { get; set; }
        //bool for tracking the successful task completion
        private bool isSuccessful { get; set; }
        //bool for tracking the failure task completion
        private bool isError { get; set; }
        //Genders collection for editing and deleting purpose
        public ObservableCollection<string> Genders { get; }
        //Status collection for editing and deleting purpose
        public ObservableCollection<string> Statuses { get; }

        //Our Model
        IApiConsumerModel ApiConsumerModelInstance;

        //Property changed event
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public ApiConsumerViewModel(IApiConsumerModel _ApiConsumerModelInstance)
        {
            //Instantiating srilog
            _logger = Log.ForContext<ApiConsumerViewModel>();

            //Instantiating necessary values
            ApiConsumerModelInstance = _ApiConsumerModelInstance;
            GetNextPageClick = new RelayCommand(async () => await GetNextPageAsync());
            GetPreviousPageClick = new RelayCommand(async () => await GetPreviousPageAsync());
            GetFirstPageClick = new RelayCommand(async () => await GetFirstPageAsync());
            GetLastPageClick = new RelayCommand(async () => await GetLastPageAsync());
            GetCurrentPageClick = new RelayCommand(async () => await GetCurrentPageAsync());
            PostDataClick = new RelayCommand(async () => await PostDataAsync());
            EditDataClick = new RelayCommand(async () => await EditDataAsync());
            DeleteDataClick = new RelayCommand(async () => await DeleteDataAsync());
            GetSearchClick = new RelayCommand(async () => await SearchDataAsync());
            GetUserClick = new RelayCommand(async () => await GetUserPageAsync());
            ClearSearchClick = new RelayCommand(async () => await ClearSearchDataAsync());
            this.Genders = new ObservableCollection<string>() { "Male", "Female" };
            this.Statuses = new ObservableCollection<string>() { "Active", "Inactive" };
            this.navigationVisibility = true;
            this.SelectedItem = new Person();
            this.SearchUser = new Person();
            this.IsSearchRecently = false;

        }

        #region Property Setting and Getting

        public bool IsSuccessful
        {
            get
            {
                return this.isSuccessful;
            }
            set
            {
                if (this.isSuccessful != value)
                {
                    this.isSuccessful = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSuccessful"));
                }
            }
        }

        public bool IsError
        {
            get
            {
                return this.isError;
            }
            set
            {
                if (this.isError != value)
                {
                    this.isError = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsError"));
                }
            }
        }
        public bool IsSearchRecently
        {
            get
            {
                return this._isSearchRecently;
            }
            set
            {
                if (this._isSearchRecently != value)
                {
                    this._isSearchRecently = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSearchRecently"));
                }
            }
        }

        public List<RequestReturn> MyRequestReturns
        {
            get
            {
                return this._myRequestReturns;
            }
            set
            {
                this._myRequestReturns = value;
                PropertyChanged(this, new PropertyChangedEventArgs("MyRequestReturns"));

            }
        }

        public bool navigationVisibility
        {
            get
            {
                return _navigationVisibility;
            }
            set
            {
                _navigationVisibility = value;
                PropertyChanged(this, new PropertyChangedEventArgs("navigationVisibility"));
            }
        }

        public string MyStatus
        {
            get
            {
                return _myStatus;
            }
            set
            {
                _myStatus = value;
                PropertyChanged(this, new PropertyChangedEventArgs("MyStatus"));
            }
        }

        public string resultString
        {
            get
            {
                return _resultString;
            }
            set
            {
                _resultString = value;
                PropertyChanged(this, new PropertyChangedEventArgs("resultString"));
            }
        }

        public Pagination pagination
        {
            get
            {
                return _pagination;
            }
            set
            {
                _pagination = value;
                PropertyChanged(this, new PropertyChangedEventArgs("pagination"));
            }
        }

        public Person SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedItem"));
            }
        }

        public Person SearchUser
        {
            get
            {
                return _SearchUser;
            }
            set
            {
                _SearchUser = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SearchUser"));
            }
        }

        public List<Person> persons
        {
            get
            {

                return _persons;

            }
            set
            {
                _persons = value;
                PropertyChanged(this, new PropertyChangedEventArgs("persons"));
            }
        }

        private async Task GetNextPageAsync()
        {
            this.MyStatus = "Retreiving Next Page...";

            this.navigationVisibility = false;
            int nextPage = this.pagination.page + 1;
            nextPage = nextPage > this.pagination.pages ? this.pagination.pages : nextPage;

            await GetPageAsRequestedAsync(nextPage);
        }

        #endregion

        //method for retreiving the previous page
        private async Task GetPreviousPageAsync()
        {
            //setting the status message
            this.MyStatus = "Retreiving Previous Page...";

            //setting the background work is happening
            this.navigationVisibility = false;

            //validating the page, so the page numbers inside the total page
            int previousPage = this.pagination.page - 1;
            previousPage = previousPage < 0 ? 1 : previousPage;

            //retreiving the page
            await GetPageAsRequestedAsync(previousPage);
        }

        private async Task GetFirstPageAsync()
        {
            //setting the status message
            this.MyStatus = "Retreiving First Page...";

            //setting the background work is happening
            this.navigationVisibility = false;

            //first page
            int firstPage = 1;

            //retreiving the page
            await GetPageAsRequestedAsync(firstPage);
        }

        private async Task GetLastPageAsync()
        {
            //setting the status message
            this.MyStatus = "Retreiving Last Page...";

            //setting the background work is happening
            this.navigationVisibility = false;

            //last page
            int lastPage = this.pagination.pages;

            //retreiving the page
            await GetPageAsRequestedAsync(lastPage);
        }

        private async Task GetCurrentPageAsync()
        {
            //setting the status message
            this.MyStatus = "Refreshing Current Page...";

            //setting the background work is happening
            this.navigationVisibility = false;

            //current page
            int currentPage = this.pagination.page;

            //retreiving the page
            await GetPageAsRequestedAsync(currentPage);
        }

        private async Task GetUserPageAsync()
        {
            try
            {
                //setting the status message
                this.MyStatus = "Retreiving Selected User...";

                //setting result string, can be used for message box
                this.resultString = "Getting User";

                //Setting success and error booleans
                this.IsSuccessful = false;
                this.IsError = false;

                //setting the background work is happening
                this.navigationVisibility = false;

                //Calling model method for retreivning the selected user
                this.SelectedItem = await ApiConsumerModelInstance.GetUserAsync(this.SelectedItem.id);

                //setting the background work not happening
                this.navigationVisibility = true;

                //Setting success and error booleans
                this.IsSuccessful = true;
                this.IsError = false;

                //setting the status bar message
                this.MyStatus = "Completed retreiving selected User";
            }
            catch (Exception ex)
            {
                //setting the background work not happening
                this.navigationVisibility = true;

                //setting the status bar message
                this.MyStatus = "ERROR: error in retreiving selected User";
            }
        }

        private async Task GetPageAsRequestedAsync(int page)
        {
            try
            {
                //setting the status bar message
                this.MyStatus = "Retreiving Page...";

                //setting the background work is happening
                this.navigationVisibility = false;

                //setting result string, can be used for message box
                this.resultString = "";

                //Setting success and error booleans
                this.IsSuccessful = false;
                this.IsError = false;

                PagePerson result = new PagePerson();

                //If search button is clicked, the belo logic will receive the searched data or else all data of given page
                if (IsSearchRecently)
                {
                    result = await ApiConsumerModelInstance.SearchedUserGetPageAsync(SearchUser, page);
                }
                else
                {
                    result = await ApiConsumerModelInstance.GetPageAsync(page);
                }

                //Storing the retreived data
                var _pagePerson = result;
                this.pagination = _pagePerson.meta.pagination;
                this.persons = _pagePerson.data;

                //clearing the selected user
                this.SelectedItem = new Person();

                //setting the status bar message
                this.MyStatus = "Retreived the page";

                //Setting success and error booleans
                this.IsSuccessful = true;
                this.IsError = false;

                //setting the background work not happening
                this.navigationVisibility = true;

            }
            catch (Exception ex)
            {
                //clearing the selected user
                this.SelectedItem = new Person();

                //setting the status bar message
                this.MyStatus = "Retreived the page";

                //setting the background work not happening
                this.navigationVisibility = true;

                //Setting success and error booleans
                this.IsSuccessful = false;
                this.IsError = true;
            }

        }

        private async Task PostDataAsync()
        {
            try
            {
                //setting result string, can be used for message box
                this.resultString = "";

                //Setting success and error booleans
                this.IsSuccessful = false;
                this.IsError = false;

                //setting the status bar message
                this.MyStatus = "Adding User...";

                //setting the background work is happening
                this.navigationVisibility = false;

                //Validate the email, ender, status and name
                if (ValidateEmail(SelectedItem.email) && SelectedItem.gender != null && SelectedItem.gender != "" && SelectedItem.status != null && SelectedItem.status != "" && SelectedItem.name != null && SelectedItem.name != "")
                {
                    //add data
                    var result = await ApiConsumerModelInstance.PostDataAsync(SelectedItem);

                    RequestReturn requestReturn = result;

                    if (requestReturn.code == 201)
                    {
                        //setting result string, can be used for message box
                        this.resultString = $"Success, the data added: {requestReturn.content}";

                        //setting the status bar message
                        this.MyStatus = "User Added";

                        //Setting success and error booleans
                        this.IsSuccessful = true;
                        this.IsError = false;
                    }
                    else
                    {
                        //setting result string, can be used for message box
                        this.resultString = $"ERROR: {requestReturn.content}";

                        //Setting success and error booleans
                        this.IsSuccessful = false;
                        this.IsError = true;
                    }

                    //setting the background work not happening
                    this.navigationVisibility = true;


                }
                else
                {
                    //setting result string, can be used for message box
                    this.resultString = "check all the values - (email has to be in correct format and enterng all the data is mandatory)";

                    //setting the status bar message
                    this.MyStatus = "ERROR: check all the values - (email has to be in correct format and enterng all the data is mandatory)";

                    //setting the background work not happening
                    this.navigationVisibility = true;

                    //Setting success and error booleans
                    this.IsSuccessful = false;
                    this.IsError = true;

                }
            }
            catch (Exception ex)
            {
                //setting the status bar message
                this.MyStatus = "ERROR: User Add met with an exception";

                //setting the background work not happening
                this.navigationVisibility = true;

                //Setting success and error booleans
                this.IsSuccessful = false;
                this.IsError = true;
            }
        }

        private async Task EditDataAsync()
        {
            //setting result string, can be used for message box
            this.resultString = "";

            //Setting success and error booleans
            this.IsSuccessful = false;
            this.IsError = false;

            //setting the status bar message
            this.MyStatus = "Editing of user started...";

            //setting the background work is happening
            this.navigationVisibility = false;

            //validating email, status, gender, anme and id
            if (SelectedItem.email != null && SelectedItem.email != "" && SelectedItem.gender != null && SelectedItem.gender != "" && SelectedItem.status != null && SelectedItem.status != "" && SelectedItem.name != null && SelectedItem.name != "" && SelectedItem.id != 0)
            {
                //edit the data
                await PatchThePersonDataAsync(false);
            }
            else
            {
                //setting the background work not happening
                this.navigationVisibility = true;

                //setting the status bar message
                this.MyStatus = "";

                //setting result string, can be used for message box
                this.resultString = "check all the values including ID - (email has to be in correct format and enterng all the data is mandatory)";

                //Setting success and error booleans
                this.IsSuccessful = false;
                this.IsError = true;
            }

        }

        private async Task SearchDataAsync()
        {
            bool isCaughtException = false;
            try
            {
                //Setting success and error booleans
                this.IsSuccessful = false;
                this.IsError = false;

                //setting the background work is happening
                this.navigationVisibility = false;

                //setting the status bar message
                this.MyStatus = "User search started...";

                //Search the data by the given user data
                PagePerson result = await ApiConsumerModelInstance.SearchUserAsync(this.SearchUser);

                //store the retreived data
                var _pagePerson = (PagePerson)result;
                this.pagination = _pagePerson.meta.pagination;
                this.persons = _pagePerson.data;
                this.MyStatus = "User searches, now started exporting data...";

                //set the search button is clicked
                this.IsSearchRecently = true;

                try
                {
                    //If the total retreived page is more than 100 throw error
                    if (this.pagination.total > 100)
                    {
                        //clear selected item
                        this.SelectedItem = new Person();

                        //setting the status bar message
                        this.MyStatus = "ERROR : File count Exceeded 100 size limit";

                        //Setting success and error booleans
                        this.IsSuccessful = false;
                        this.IsError = true;

                        //Throw exception
                        throw new AccessViolationException("File count Exceeded 100 size limit");
                    }

                    //Export the searched data retreived from the api
                    var result2 = await ApiConsumerModelInstance.ExportUserAsync(this.SearchUser, this.pagination.pages);

                    //store the retreived data
                    List<Person> _persons = (List<Person>)result2;

                    //If retreived data is greater than zero export the data or else raise exception
                    if (_persons.Count > 0)
                    {
                        //write it to a file
                        ExportCSV(_persons);

                        //setting the status bar message
                        this.MyStatus = "Searched data exported";

                        //Setting success and error booleans
                        this.IsSuccessful = true;
                        this.IsError = false;

                        //setting the background work not happening
                        this.navigationVisibility = true;
                    }
                    else
                    {
                        //setting the status bar message
                        this.MyStatus = "The collected data is zero";

                        //Setting success and error booleans
                        this.IsSuccessful = false;
                        this.IsError = true;

                        //setting the background work not happening
                        this.navigationVisibility = true;

                        //throw exception
                        throw new InvalidDataException("The collected data is zero");

                    }
                }
                catch (InvalidDataException ex)
                {
                    isCaughtException = true;

                    //setting the status bar message
                    this.MyStatus = ex.Message;
                }
                catch (AccessViolationException ex)
                {
                    isCaughtException = true;

                    //setting the status bar message
                    this.MyStatus = ex.Message;
                }
                catch (Exception ex)
                {
                    isCaughtException = true;

                    //setting the status bar message
                    this.MyStatus = "ERROR : Export met with an error";

                }
                finally
                {
                    if (isCaughtException == true)
                    {
                        //clear selected item
                        this.SelectedItem = new Person();

                        //setting the background work not happening
                        this.navigationVisibility = true;

                        //Setting success and error booleans
                        this.IsSuccessful = false;
                        this.IsError = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //clear selected item
                this.SelectedItem = new Person();

                //setting the status bar message
                this.MyStatus = "ERROR : search met with an error";

                //setting the background work not happening
                this.navigationVisibility = true;

                //Setting success and error booleans
                this.IsSuccessful = false;
                this.IsError = true;
            }

        }

        private async Task ClearSearchDataAsync()
        {
            //clear selected user
            this.SearchUser = new Person();

            //retreive all data
            await GetFirstPageAsync();

            //set the clear search button is clicked
            this.IsSearchRecently = false;
        }

        private async Task DeleteDataAsync()
        {
            //setting result string, can be used for message box
            this.resultString = "";

            //Setting success and error booleans
            this.IsSuccessful = false;
            this.IsError = false;

            //setting the background work is happening
            this.navigationVisibility = false;

            //setting the status bar message
            this.MyStatus = "User deletition started...";

            //if selected item is not empty, process further
            if (this.SelectedItem.id != 0)
            {
                await PatchThePersonDataAsync(true);
            }
            else
            {
                //setting the background work not happening
                this.navigationVisibility = true;

                //setting the status bar message
                this.MyStatus = "Data Not Selected";

                //setting result string, can be used for message box
                this.resultString = "check all the values including ID - (email has to be in correct format and enterng all the data is mandatory)";

                //Setting success and error booleans
                this.IsSuccessful = false;
                this.IsError = true;
            }

        }

        private async Task PatchThePersonDataAsync(bool isDelete)
        {
            try
            {
                //retreive the current user again to check the eTag
                var result = await ApiConsumerModelInstance.GetUserAsync(SelectedItem.id);

                string ETag = "";
                if (result != null)
                {
                    ETag = result.Etag;
                }

                //set the eTag
                ETag = ETag ?? "";

                _logger.Debug($"ETag: {ETag}");

                //If eTag is same, process further or else cancel the process
                if (ETag == SelectedItem.Etag)
                {
                    //If this is a edit process, proceed further or else go for delete process
                    if (!isDelete)
                    {
                        //edit the data
                        var result2 = await ApiConsumerModelInstance.PatchDataAsync(SelectedItem);

                        //store the data
                        RequestReturn requestReturn = result2;

                        //if return code is 200, then t=it is successful or else it may have failed
                        if (requestReturn.code == 200)
                        {
                            //setting result string, can be used for message box
                            this.resultString = $"Success, the data edited: {requestReturn.content}";

                            //Setting success and error booleans
                            this.IsSuccessful = true;
                            this.IsError = false;
                        }
                        else
                        {
                            //setting result string, can be used for message box
                            this.resultString = $"ERROR: {requestReturn.content}";

                            //Setting success and error booleans
                            this.IsSuccessful = false;
                            this.IsError = true;
                        }

                        //setting the status bar message
                        this.MyStatus = "User editing completed";

                        //setting the background work not happening
                        this.navigationVisibility = true;

                    }
                    else
                    {
                        //delte the data
                        var result2 = await ApiConsumerModelInstance.DeleteDataAsync(SelectedItem);

                        //store the retreived data
                        RequestReturn requestReturn = result2;

                        if (requestReturn.code == 204)
                        {
                            //setting result string, can be used for message box
                            this.resultString = $"Success, the data deleted: {requestReturn.content}";

                            //Setting success and error booleans
                            this.IsSuccessful = true;
                            this.IsError = false;

                        }
                        else
                        {
                            //setting result string, can be used for message box
                            this.resultString = $"ERROR: {requestReturn.content}";

                            //Setting success and error booleans
                            this.IsSuccessful = false;
                            this.IsError = true;

                        }

                        //setting the status bar message
                        this.MyStatus = "User deletition completed";

                        //setting the background work not happening
                        this.navigationVisibility = true;

                    }

                }
                else
                {
                    //setting the status bar message
                    this.MyStatus = "";

                    //setting the background work not happening
                    this.navigationVisibility = true;

                    //setting result string, can be used for message box
                    this.resultString = $"ERROR (Data Modified inbetween by someone, please check once again)";

                    //Setting success and error booleans
                    this.IsSuccessful = false;
                    this.IsError = true;

                }

            }
            catch (Exception ex)
            {
                //setting the status bar message
                this.MyStatus = "ERROR: task delete/edit exception";

                //Setting success and error booleans
                this.IsSuccessful = false;
                this.IsError = true;

                //setting the background work not happening
                this.navigationVisibility = true;
            }
        }


        private void ExportCSV(List<Person> _persons)
        {
            //Prepare the save dialog for saving the generated file
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "ExportedFile"; // Default file name
            dialog.DefaultExt = ".csv"; // Default file extension
            dialog.Filter = "CSV file (*.csv) | *.csv"; // Filter files by extension

            // Display the save file dialog box
            Nullable<bool> result = dialog.ShowDialog();

            // If the user have choosen the file apth, continue
            if (result == true)
            {
                //retreive the file path
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                //Write the file
                StreamWriter streamWriter = new StreamWriter(dialog.FileName);
                streamWriter.WriteLine($"ID,Name,Email,Status,Gender,Updated Date, Created Date");

                foreach (var item_person in _persons)
                {
                    streamWriter.WriteLine($"{item_person.id},{item_person.name},{item_person.email},{item_person.gender},{item_person.status},{item_person.updated_at},{item_person.created_at}");
                }

                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        //Completion method for the export process
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            //setting the status bar message
            this.MyStatus = "Export completed";
        }

        //validate the emil for adding a new data to the api
        private bool ValidateEmail(string _email)
        {
            if (_email != null)
            {
                string email = _email;
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(email);
                if (match.Success)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

    }
}

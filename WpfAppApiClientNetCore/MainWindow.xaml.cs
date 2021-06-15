using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAppApiClientNetCore.Helpers;
using WpfAppApiClientNetCore.ViewModels;

namespace WpfAppApiClientNetCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Our view model
        private readonly IApiConsumerViewModel _apiConsumerViewModel;
        //Seri log
        private readonly ILogger _logger;
        public MainWindow(ILogger logger)
        {

            InitializeComponent();

            //Instantiating logger
            _logger = logger;

            //Gathering the datacontext from view model locator
            _apiConsumerViewModel = (IApiConsumerViewModel)this.DataContext;

            _logger.Information("Welcome");

            //On starting go the retreived users first page
            if (_apiConsumerViewModel.GetFirstPageClick.CanExecute(null))
                _apiConsumerViewModel.GetFirstPageClick.Execute(null);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Set Is add window open to false, this is for only raising message box that are relavant for add data window
            AddData.IsAddDataWindowOpen = false;
            //Get the logger instance for the constructor injection for the AddData class
            var _log = Log.ForContext<AddData>();
            //Instantiating AddData window
            AddData addDataWindow = new AddData(_log);
            //Opening the AddData window
            var result = addDataWindow.ShowDialog();
            //After add data window closed, query the all users in the add data window. This will help retreiving the recent data.
            if (AddData.IsAddDataWindowOpen == true)
            {
                if (_apiConsumerViewModel.GetLastPageClick.CanExecute(null))
                    _apiConsumerViewModel.GetLastPageClick.Execute(null);
                AddData.IsAddDataWindowOpen = false;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //Check for the list view selected item is not null
            if (((Person)lvMyList.SelectedItem) != null)
            {
                //Check for list view selected item is not zero (only a initialized value)
                if (((Person)lvMyList.SelectedItem).id != 0)
                {
                    //Set Is edit window open to false, this is for only raising message box that are relavant for edit data window
                    EditData.IsEditDataWindowOpen = false;

                    //Get the logger instance for the constructor injection for the EditData class
                    var _log = Log.ForContext<EditData>();
                    //Instantiating EditData window
                    EditData editDataWindow = new EditData(_log);

                    //Opening the EditData window
                    var result = editDataWindow.ShowDialog();

                    //After edit data window closed, query the all users in the edit data window. This will help retreiving the recent data.
                    if (EditData.IsEditDataWindowOpen == true)
                    {
                        if (_apiConsumerViewModel.GetCurrentPageClick.CanExecute(null))
                            _apiConsumerViewModel.GetCurrentPageClick.Execute(null);
                        EditData.IsEditDataWindowOpen = false;
                    }
                }
                else
                {
                    MessageBox.Show("Please select data");
                }
            }
            else
            {
                MessageBox.Show("Please select data");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //Check for the list view selected item is not null
            if (((Person)lvMyList.SelectedItem) != null)
            {
                //Check for list view selected item is not zero (only a initialized value)
                if (((Person)lvMyList.SelectedItem).id != 0)
                {
                    //Set Is delete window open to false, this is for only raising message box that are relavant for delete data window
                    DeleteData.IsDeleteDataWindowOpen = false;

                    //Get the logger instance for the constructor injection for the DeleteData class
                    var _log = Log.ForContext<DeleteData>();
                    //Instantiating DeleteData window
                    DeleteData deleteDataWindow = new DeleteData(_log);

                    //Opening the DeleteData window
                    var result = deleteDataWindow.ShowDialog();

                    //After delete data window closed, query the all users in the delete data window. This will help retreiving the recent data.
                    if (DeleteData.IsDeleteDataWindowOpen == true)
                    {
                        if (_apiConsumerViewModel.GetCurrentPageClick.CanExecute(null))
                            _apiConsumerViewModel.GetCurrentPageClick.Execute(null);
                        DeleteData.IsDeleteDataWindowOpen = false;
                    }
                }
                else
                {
                    MessageBox.Show("Please select data");
                }
            }
            else
            {
                MessageBox.Show("Please select data");
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //Confirm users that the system only export 100 data to the csv file or else it will not generate file but display the search data in the list view
            MessageBoxResult result = MessageBox.Show("The system only export 100 users, if you met with errors, kindly check the search result has more than 100 users","Search confirmation",MessageBoxButton.YesNo);
            if(result == MessageBoxResult.Yes)
            {
                //Get the queried data
                if (_apiConsumerViewModel.GetSearchClick.CanExecute(null))
                    _apiConsumerViewModel.GetSearchClick.Execute(null);
            }
        }

        private void lblIsSearch_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            //Set the text block above list view diplay what data the list view displays
            if (lblIsSearched.Text.ToLower() == "true")
            {
                _logger.Debug($"Searched data displaying in the list view");
                lblDataStatus.Text = "Searched data is diplaying in the list view, to view all data please click \"clear search\" button";
            }
            else if(lblIsSearched.Text.ToLower() == "false")
            {
                _logger.Debug($"All data displaying in the list view");
                lblDataStatus.Text = "All data are displaying in the list view";

            }
        }
    }
}

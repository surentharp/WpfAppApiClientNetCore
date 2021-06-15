using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAppApiClientNetCore.ViewModels;

namespace WpfAppApiClientNetCore
{
    /// <summary>
    /// Interaction logic for DeleteData.xaml
    /// </summary>
    public partial class DeleteData : Window
    {
        //Our view model
        private readonly IApiConsumerViewModel _apiConsumerViewModel;

        //bools for display messages in the message box
        public static bool IsDeleteDataWindowOpen;
        private static bool isDeleteButtonClicked;

        //Serilog
        private readonly ILogger _logger;
        public DeleteData(ILogger logger)
        {
            InitializeComponent();

            //Instantiating logger
            _logger = logger;

            //Gathering the datacontext from view model locator
            _apiConsumerViewModel = (IApiConsumerViewModel)this.DataContext;

            //set that the delete windows is open for displaying the add data related error messages in the message box
            IsDeleteDataWindowOpen = true;

            //refresh the selected user once again to get recent data
            if (_apiConsumerViewModel.GetUserClick.CanExecute(null))
                _apiConsumerViewModel.GetUserClick.Execute(null);

            //Set the edit window is opened
            isDeleteButtonClicked = false;

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //Check for any double clicking of delete button
            if (lblId.Text != "" && lblId.Text != "0")
            {
                //Set a bool for displaying status message in the messagebox
                isDeleteButtonClicked = true;

                //Delete the data in the api by calling  a command in the view model
                if (_apiConsumerViewModel.DeleteDataClick.CanExecute(null))
                    _apiConsumerViewModel.DeleteDataClick.Execute(null);
            }
            else
            {
                MessageBox.Show("No user to delete");
            }
        }

        private void lblId_TargetUpdated(object sender, DataTransferEventArgs e)
        {
        }

        private void lblIsSuccessful_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            //check that the IsSuccessful from the viewmodel is true and delete button clicked to diplay success message box
            if (lblIsSuccessful.Text.ToLower() == "true" && isDeleteButtonClicked)
            {
                MessageBox.Show(_apiConsumerViewModel.resultString);
                isDeleteButtonClicked = false;
            }
        }

        private void lblIsError_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            //check that the IsError from the viewmodel is true and delete button clicked to diplay error message box
            if (lblIsError.Text.ToLower() == "true" && isDeleteButtonClicked)
            {
                MessageBox.Show(_apiConsumerViewModel.resultString);
                isDeleteButtonClicked = false;
            }
        }
    }
}

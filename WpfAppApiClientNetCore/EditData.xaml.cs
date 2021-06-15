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
    /// Interaction logic for EditData.xaml
    /// </summary>
    public partial class EditData : Window
    {
        //Our view model
        private readonly IApiConsumerViewModel _apiConsumerViewModel;

        //bools for display messages in the message box
        public static bool IsEditDataWindowOpen;
        public static bool isEditButtonClicked;
        
        //Serilog
        private readonly ILogger _logger;
        public EditData(ILogger logger)
        {

            InitializeComponent();

            //Instantiating logger
            _logger = logger;

            //Gathering the datacontext from view model locator
            _apiConsumerViewModel = (IApiConsumerViewModel)this.DataContext;

            //set that the edit windows is open for displaying the edit data related error messages in the message box
            IsEditDataWindowOpen = true;

            //refresh the selected user once again to get recent data
            if (_apiConsumerViewModel.GetUserClick.CanExecute(null))
                _apiConsumerViewModel.GetUserClick.Execute(null);

            //Set the edit window is opened
            isEditButtonClicked = false;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //Set a bool for displaying status message in the messagebox
            isEditButtonClicked = true;

            //Edit the data in the api by calling  a command in the view model
            if (_apiConsumerViewModel.EditDataClick.CanExecute(null))
                _apiConsumerViewModel.EditDataClick.Execute(null);

        }

        private void lblIsSuccessful_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            //check that the IsSuccessful from the viewmodel is true and edit button clicked to diplay success message box
            if (lblIsSuccessful.Text.ToLower() == "true" && isEditButtonClicked)
            {
                MessageBox.Show(_apiConsumerViewModel.resultString);
                isEditButtonClicked = false;
            }
        }

        private void lblIsError_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            //check that the IsError from the viewmodel is true and edit button clicked to diplay error message box
            if (lblIsError.Text.ToLower() == "true" && isEditButtonClicked)
            {
                MessageBox.Show(_apiConsumerViewModel.resultString);
                isEditButtonClicked = false;
            }
        }
    }
}

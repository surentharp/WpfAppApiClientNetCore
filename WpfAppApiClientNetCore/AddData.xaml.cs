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
    /// Interaction logic for AddData.xaml
    /// </summary>
    public partial class AddData : Window
    {
        //Our view model
        private readonly IApiConsumerViewModel _apiConsumerViewModel;

        //bools for display messages in the message box
        public static bool IsAddDataWindowOpen;
        private static bool isAddButtonClicked;

        //Serilog
        private readonly ILogger _logger;
        public AddData(ILogger logger)
        {
            InitializeComponent();

            //Instantiating logger
            _logger = logger;

            //Gathering the datacontext from view model locator
            _apiConsumerViewModel = (IApiConsumerViewModel)this.DataContext;

            //set that the add windows is open for displaying the add data related error messages in the message box
            IsAddDataWindowOpen = true;
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Set a bool for displaying status message in the messagebox
            isAddButtonClicked = true;

            //Add the data in the api by calling  a command in the view model
            if (_apiConsumerViewModel.PostDataClick.CanExecute(null))
                _apiConsumerViewModel.PostDataClick.Execute(null);
        }

        private void lblIsSuccessful_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            //check that the IsSuccessful from the viewmodel is true and add button clicked to diplay success message box
            if (lblIsSuccessful.Text.ToLower() == "true" && isAddButtonClicked)
            {
                MessageBox.Show(_apiConsumerViewModel.resultString);
                isAddButtonClicked = false;
            }
        }

        private void lblIsError_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            //check that the IsError from the viewmodel is true and add button clicked to diplay error message box
            if (lblIsError.Text.ToLower() == "true" && isAddButtonClicked)
            {
                MessageBox.Show(_apiConsumerViewModel.resultString);
                isAddButtonClicked = false;
            }
        }
    }
}

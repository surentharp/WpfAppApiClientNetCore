using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using WpfAppApiClientNetCore.Helpers;

namespace WpfAppApiClientNetCore.ViewModels
{
    public interface IApiConsumerViewModel
    {
        ICommand ClearSearchClick { get; set; }
        ICommand DeleteDataClick { get; set; }
        ICommand EditDataClick { get; set; }
        ObservableCollection<string> Genders { get; }
        ICommand GetCurrentPageClick { get; set; }
        ICommand GetFirstPageClick { get; set; }
        ICommand GetLastPageClick { get; set; }
        ICommand GetNextPageClick { get; set; }
        ICommand GetPreviousPageClick { get; set; }
        ICommand GetSearchClick { get; set; }
        ICommand GetUserClick { get; set; }
        bool IsError { get; set; }
        bool IsSearchRecently { get; set; }
        bool IsSuccessful { get; set; }
        List<RequestReturn> MyRequestReturns { get; set; }
        string MyStatus { get; set; }
        bool navigationVisibility { get; set; }
        Pagination pagination { get; set; }
        List<Person> persons { get; set; }
        ICommand PostDataClick { get; set; }
        string resultString { get; set; }
        Person SearchUser { get; set; }
        Person SelectedItem { get; set; }
        ObservableCollection<string> Statuses { get; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}
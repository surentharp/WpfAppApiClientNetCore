using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using WpfAppApiClientNetCore.ViewModels;

namespace WpfAppApiClientNetCore
{
    //Vuew Model Locator
    public class ViewModelLocator
    {
        //View model locator for our view models
        public IApiConsumerViewModel ApiConsumerViewModel => App.ServiceProvider.GetRequiredService<IApiConsumerViewModel>();
    }
}

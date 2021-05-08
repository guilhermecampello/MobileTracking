using MobileTracking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileTracking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocaleSummaryPage : ContentPage
    {
        private readonly LocaleProvider localeProvider;

        public LocaleSummaryPage(LocaleProvider localeProvider)
        {
            this.localeProvider = localeProvider;
            this.BindingContext = this.localeProvider.Locale;
            InitializeComponent();
        }
    }
}
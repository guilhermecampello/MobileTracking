using MobileTracking.Core.Models;
using System.ComponentModel;

namespace MobileTracking.Pages.Locales
{
    public class LocaleView : INotifyPropertyChanged
    {
        public LocaleView(Locale? locale, bool selected)
        {
            this.IsSelected = selected;
            this.Locale = locale;
        }

        private Locale? locale;
        public Locale? Locale
        {
            get => locale;
            set
            {
                locale = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Locale)));
            }
        }

        private bool isSelected; 
        public bool IsSelected
        { 
            get => isSelected;
            set
            {
                isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

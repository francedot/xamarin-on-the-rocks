using System.ComponentModel;
using FormsEmbedding.Controls;
using Xamarin.Forms;

namespace FormsEmbedding.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            ResetSettingsCommand = new Command(() =>
            {
                foreach (var view in RemindersStackLayout.Children)
                {
                    var reminder = view as DayOfWeekReminderView;
                    if (reminder == null)
                    {
                        continue;
                    }
                    reminder.IsReminderOn = false;
                }
            });

            InitializeComponent();
        }

        public Command ResetSettingsCommand { get; private set; }
    }
}

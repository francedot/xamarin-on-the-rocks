using System.Windows.Input;
using Movies.Models;
using Movies.Services;
using Xamarin.Forms;

namespace Movies.ViewModels
{
    public class MoviePageViewModel : PageViewModelBase
    {
        private Movie _movie;

        public MoviePageViewModel()
        {
            TextToSpeechCommand = new Command<string>(text =>
            {
                var speech = DependencyService.Get<ITextToSpeech>();
                speech.Speak(text);
            });
            MessagingCenter.Subscribe<DetailPageViewModel, Movie>(this, "SelectedMovie", (model, movie) =>
            {
                // TODO Uncomment
                // Movie = movie;
            });
        }

        public ICommand TextToSpeechCommand { get; set; }

        public Movie Movie
        {
            get => _movie;
            // TODO Uncomment
            // set => Set(ref _movie, value);
        }
    }
}

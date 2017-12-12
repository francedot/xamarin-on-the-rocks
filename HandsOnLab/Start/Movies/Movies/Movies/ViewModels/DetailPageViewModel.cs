using System.Collections.Generic;
using Movies.Models;
using Xamarin.Forms;

namespace Movies.ViewModels
{
    public class DetailPageViewModel : PageViewModelBase
    {
        private IList<Movie> _movies;

        public DetailPageViewModel()
        {
            

            DeleteCommand = new Command<Movie>(async movie =>
            {
                // TODO Uncomment
                // await MoviesSource.RemoveMovieAsync(movie.Id);
                MessagingCenter.Send<DetailPageViewModel>(this, "RefreshMovies");
            });
            NavigateCommand = new Command<Movie>(movie =>
            {
                MessagingCenter.Send<DetailPageViewModel, Movie>(this, "SelectedMovie", movie);
            });
        }

        public IList<Movie> Movies
        {
            get => _movies;
            // TODO Uncomment
            //set => Set(ref _movies, value);
        }

        public Command<Movie> DeleteCommand { get; }
        public Command<Movie> NavigateCommand { get; }
    }
}

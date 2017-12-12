using System.Collections.ObjectModel;
using Movies.Models;
using Movies.Services;

namespace Movies.ViewModels
{
    public class PageViewModelBase : ViewModelBase
    {
        public static readonly IMoviesSource MoviesSource = new MobileAppMoviesSource();

        private static bool _isLoading;
        private static ObservableCollection<Movie> _allMovies;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }
        public ObservableCollection<Movie> AllMovies
        {
            get => _allMovies;
            set => Set(ref _allMovies, value);
        }

        protected PageViewModelBase()
        {
        }
    }
}
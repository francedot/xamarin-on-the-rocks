using Movies.Services;

namespace Movies.ViewModels
{
    public class PageViewModelBase
    {
        public static readonly IMoviesSource MoviesSource = new MobileAppMoviesSource();

        protected PageViewModelBase()
        {
        }
    }
}
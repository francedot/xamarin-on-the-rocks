using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Movies.ViewModels
{
    public class MenuPageViewModel : PageViewModelBase
    {
        private IList<string> _genres;
        private string _selectedGenre;

        public IList<string> Genres
        {
            get => _genres;
            // TODO Uncomment
            // set => Set(ref _genres, value);
        }
        public string SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                // TODO Uncomment
                // Set(ref _selectedGenre, value);
                MessagingCenter.Send<MenuPageViewModel, string>(this, "SelectedGenre", value);
            }
        }

        public void UpdateGenres()
        {
            // TODO Uncomment

            //var splittedGenres = new List<string>();
            //var genres = AllMovies.Select(m => m.Genre);
            //foreach (var genre in genres)
            //{
            //    splittedGenres.AddRange(genre.Split(',').Select(s => s.Trim()));
            //}
            //Genres = splittedGenres.Distinct().ToList();
            //SelectedGenre = Genres.FirstOrDefault();
        }
    }
}

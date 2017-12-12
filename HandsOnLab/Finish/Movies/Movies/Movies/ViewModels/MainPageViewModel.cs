﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Movies.Models;
using Xamarin.Forms;

namespace Movies.ViewModels
{
    public class MainPageViewModel : PageViewModelBase
    {
        public MainPageViewModel()
        {
            MessagingCenter.Subscribe<MenuPageViewModel, string>(this,
                "SelectedGenre", (sender, selectedGenre) =>
            {
                DetailPageViewModel.Movies = AllMovies.Where(m => m.Genre.Contains(MenuPageViewModel.SelectedGenre)).ToList();
            });
            MessagingCenter.Subscribe<AddMoviePageViewModel>(this,
                "RefreshMovies", async sender => await RefreshAsync());
            MessagingCenter.Subscribe<DetailPageViewModel>(this,
                "RefreshMovies", async sender => await RefreshAsync());

            MenuPageViewModel = new MenuPageViewModel();
            DetailPageViewModel = new DetailPageViewModel();
        }

        private async Task RefreshAsync()
        {
            IsLoading = true;
            DetailPageViewModel.RefreshCommand.ChangeCanExecute();

            AllMovies = new ObservableCollection<Movie>(await MoviesSource.GetMoviesAsync());
            MenuPageViewModel.UpdateGenres();

            IsLoading = false;
            DetailPageViewModel.RefreshCommand.ChangeCanExecute();
        }

        public MenuPageViewModel MenuPageViewModel { get; set; }
        public DetailPageViewModel DetailPageViewModel { get; set; }

        public async Task OnAppearingAsync()
        {
            AllMovies = new ObservableCollection<Movie>(await MoviesSource.GetMoviesAsync());
            MenuPageViewModel.UpdateGenres();
        }
    }
}

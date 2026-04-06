using MatteoAPP1.Services;
using MatteoAPP1.ViewModels;

namespace MatteoAPP1
{
    public partial class CharactersPage : ContentPage
    {
        private readonly CharactersViewModel _viewModel;

        public CharactersPage()
        {
            InitializeComponent();
            _viewModel = ServiceHelper.GetService<CharactersViewModel>();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.EnsureLoadedAsync();
        }
    }
}

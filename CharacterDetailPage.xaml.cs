using MatteoAPP1.Services;
using MatteoAPP1.ViewModels;

namespace MatteoAPP1
{
    public partial class CharacterDetailPage : ContentPage
    {
        public CharacterDetailPage()
        {
            InitializeComponent();
            BindingContext = ServiceHelper.GetService<CharacterDetailViewModel>();
        }
    }
}

using MatteoAPP1.Services;
using MatteoAPP1.ViewModels;

namespace MatteoAPP1
{
    public partial class GalleryPage : ContentPage
    {
        public GalleryPage()
        {
            InitializeComponent();
            BindingContext = ServiceHelper.GetService<AddCharacterViewModel>();
        }
    }
}

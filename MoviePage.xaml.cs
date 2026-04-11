using MatteoAPP1.Models;
using MatteoAPP1.Services;
using Microsoft.Maui.Controls.Shapes;

namespace MatteoAPP1
{
    public partial class MoviePage : ContentPage
    {
        private const string TrainMovieKey = "train-infini";
        private const string CastleMovieKey = "chateau-infini";

        private readonly IAppDatabaseService _databaseService;
        private readonly Button[] _trainStars;
        private readonly Button[] _trainingStars;
        private int _trainRating;
        private int _trainingRating;
        public MoviePage()
        {
            InitializeComponent();

            _databaseService = ServiceHelper.GetService<IAppDatabaseService>();
            _trainStars = [TrainStar1, TrainStar2, TrainStar3, TrainStar4, TrainStar5];
            _trainingStars = [TrainingStar1, TrainingStar2, TrainingStar3, TrainingStar4, TrainingStar5];
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCommentsAsync();
        }

        private void OnTrainStarClicked(object? sender, EventArgs e)
        {
            _trainRating = ReadRating(sender);
            TrainRatingLabel.Text = $"Ta note : {_trainRating}/5";
            UpdateStars(_trainStars, _trainRating);
        }

        private void OnTrainingStarClicked(object? sender, EventArgs e)
        {
            _trainingRating = ReadRating(sender);
            TrainingRatingLabel.Text = $"Ta note : {_trainingRating}/5";
            UpdateStars(_trainingStars, _trainingRating);
        }

        private async void OnAddTrainCommentClicked(object? sender, EventArgs e)
        {
            await AddCommentAsync(TrainMovieKey, _trainRating, TrainReviewEditor, TrainCommentsLayout, _trainStars, TrainRatingLabel);
        }

        private async void OnAddTrainingCommentClicked(object? sender, EventArgs e)
        {
            await AddCommentAsync(CastleMovieKey, _trainingRating, TrainingReviewEditor, TrainingCommentsLayout, _trainingStars, TrainingRatingLabel);
        }

        private async Task LoadCommentsAsync()
        {
            await PopulateCommentsAsync(TrainMovieKey, TrainCommentsLayout);
            await PopulateCommentsAsync(CastleMovieKey, TrainingCommentsLayout);
        }

        private async Task PopulateCommentsAsync(string movieKey, VerticalStackLayout layout)
        {
            layout.Children.Clear();

            var comments = await _databaseService.GetMovieCommentsAsync(movieKey);
            if (comments.Count == 0)
            {
                layout.Children.Add(new Label
                {
                    Text = "Aucun avis pour le moment.",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#6B7280")
                });
                return;
            }

            foreach (var comment in comments)
            {
                layout.Children.Add(BuildCommentView(comment));
            }
        }

        private View BuildCommentView(MovieComment comment)
        {
            var stars = new string('\u2605', comment.Rating) + new string('\u2606', Math.Max(0, 5 - comment.Rating));

            var deleteButton = new Button
            {
                Text = "Supprimer",
                BackgroundColor = Color.FromArgb("#FEE2E2"),
                TextColor = Color.FromArgb("#991B1B"),
                CornerRadius = 12,
                Padding = new Thickness(14, 10),
                HorizontalOptions = LayoutOptions.Start
            };
            deleteButton.Clicked += async (_, _) => await DeleteCommentAsync(comment);

            return new Border
            {
                Stroke = Color.FromArgb("#E5E7EB"),
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = 16 },
                BackgroundColor = Colors.White,
                Padding = 14,
                Content = new VerticalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                        new Label
                        {
                            Text = stars,
                            FontSize = 20,
                            TextColor = Color.FromArgb("#D97706")
                        },
                        new Label
                        {
                            Text = comment.Comment,
                            FontSize = 15,
                            TextColor = Color.FromArgb("#111827")
                        },
                        deleteButton
                    }
                }
            };
        }

        private async Task AddCommentAsync(
            string movieKey,
            int rating,
            Editor editor,
            VerticalStackLayout layout,
            IEnumerable<Button> stars,
            Label ratingLabel)
        {
            var commentText = editor.Text?.Trim() ?? string.Empty;
            if (rating <= 0 || string.IsNullOrWhiteSpace(commentText))
            {
                await DisplayAlert("Avis incomplet", "Choisis une note et ecris un commentaire.", "OK");
                return;
            }

            await _databaseService.SaveMovieCommentAsync(new MovieComment
            {
                Id = Guid.NewGuid().ToString("N"),
                MovieKey = movieKey,
                Rating = rating,
                Comment = commentText
            });

            editor.Text = string.Empty;

            if (movieKey == TrainMovieKey)
            {
                _trainRating = 0;
            }
            else
            {
                _trainingRating = 0;
            }

            ratingLabel.Text = "Ta note : 0/5";
            UpdateStars(stars, 0);
            await PopulateCommentsAsync(movieKey, layout);
        }

        private async Task DeleteCommentAsync(MovieComment comment)
        {
            var confirmed = await DisplayAlert("Supprimer", "Supprimer cet avis ?", "Oui", "Non");
            if (!confirmed)
            {
                return;
            }

            await _databaseService.DeleteMovieCommentAsync(comment.Id);

            if (comment.MovieKey == TrainMovieKey)
            {
                await PopulateCommentsAsync(TrainMovieKey, TrainCommentsLayout);
                return;
            }

            await PopulateCommentsAsync(CastleMovieKey, TrainingCommentsLayout);
        }

        private static int ReadRating(object? sender)
        {
            if (sender is Button { CommandParameter: string value } && int.TryParse(value, out var rating))
            {
                return rating;
            }

            return 0;
        }

        private static void UpdateStars(IEnumerable<Button> stars, int rating)
        {
            var index = 1;
            foreach (var star in stars)
            {
                star.Text = index <= rating ? "\u2605" : "\u2606";
                star.BackgroundColor = index <= rating
                    ? Color.FromArgb("#FEF3C7")
                    : Color.FromArgb("#F9FAFB");
                index++;
            }
        }
    }
}

using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Mscc.GenerativeAI;
using System.Threading.Tasks;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private const string apiKey = "your_api_key";

        FileResult currentPhoto;
        string currentFilePath;

        string userInput;
        string geminiOutput = "";


        public MainPage()
        {
            InitializeComponent();

            //userInput = InputTxt.Text;
            //OutputTxt.Text = geminiOutput;
        }

        private async void TakePhoto(object sender, EventArgs e)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                currentPhoto = await MediaPicker.Default.CapturePhotoAsync();

                if (currentPhoto != null)
                {
                    // Save the file into local storage
                    currentFilePath = Path.Combine(FileSystem.CacheDirectory, currentPhoto.FileName);

                    using Stream sourceStream = await currentPhoto.OpenReadAsync();
                    using FileStream localFileStream = File.OpenWrite(currentFilePath);

                    await sourceStream.CopyToAsync(localFileStream);

                    // Update the current photo path
                    currentPhoto = new FileResult(currentFilePath);

                    // Display the photo
                    DisplayPhoto(currentFilePath);
                }
            }
        }

        private void DisplayPhoto(string filePath)
        {
            PhotoDisplay.Source = ImageSource.FromFile(filePath);
        }

        private void OnCoverBoxTapped(object sender, EventArgs e)
        {
            CoverBox.IsVisible = false;
        }

        private void OnCoverBoxTapped2(object sender, EventArgs e)
        {
            CoverBox2.IsVisible = false;
        }

        private async void AskGemini(object sender, EventArgs e)
        {
            if (InputTxt.Text == null)
            {
                InputTxt.Text = " ";
            }

            var googleAI = new GoogleAI(apiKey);
            var model = googleAI.GenerativeModel(model: Model.Gemini20Flash);

            GeneratingText.Text = "Generating...";

            CoverBox.IsVisible = true;
            CoverBox2.IsVisible = true;

            var request1 = new GenerateContentRequest("Only give the answer to this qustion: " + InputTxt.Text);
            var request2 = new GenerateContentRequest("Only give steps to this question: " + InputTxt.Text);

            await request1.AddMedia(currentPhoto.FullPath);
            await request2.AddMedia(currentPhoto.FullPath);

            var response1 = await model.GenerateContent(request1);
            var response2 = await model.GenerateContent(request2);

            OutputTxt.Text = response1.Text.Replace('*', ' ');
            OutputTxt2.Text = response2.Text.Replace('*', ' ');

            GeneratingText.Text = "Generated";
        }
    }
}

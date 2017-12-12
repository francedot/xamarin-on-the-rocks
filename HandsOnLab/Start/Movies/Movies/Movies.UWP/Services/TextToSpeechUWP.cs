using Movies.Services;
using Movies.UWP.Services;

[assembly: Xamarin.Forms.Dependency(typeof(TextToSpeechUWP))]
namespace Movies.UWP.Services
{
    public class TextToSpeechUWP : ITextToSpeech
    {
        public void Speak(string text)
        {
            throw new System.NotImplementedException();
        }
    }
}
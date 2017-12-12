using Movies.iOS.Services;
using Movies.Services;

[assembly: Xamarin.Forms.Dependency(typeof(TextToSpeechiOS))]
namespace Movies.iOS.Services
{
    public class TextToSpeechiOS : ITextToSpeech
    {
        public void Speak(string text)
        {
            throw new System.NotImplementedException();
        }
    }d
}
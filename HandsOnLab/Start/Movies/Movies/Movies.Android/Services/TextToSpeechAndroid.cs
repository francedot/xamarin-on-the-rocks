using Movies.Droid.Services;
using Xamarin.Forms;
using ITextToSpeech = Movies.Services.ITextToSpeech;

[assembly: Dependency(typeof(TextToSpeechAndroid))]
namespace Movies.Droid.Services
{
    public class TextToSpeechAndroid : ITextToSpeech
    {
        public void Speak(string text)
        {
            throw new System.NotImplementedException();
        }
    }
}
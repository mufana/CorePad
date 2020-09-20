using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using System.Reflection;

namespace CoreNotes.ViewModels
{
    public class AboutViewModel : Screen
    {
        private SnackbarMessageQueue _messageQueue;
        private string _versionInfo;

        public AboutViewModel(SnackbarMessageQueue MessageQueue)
        {
            _messageQueue = MessageQueue;
        }

        public string VersionInfo
        {
            get
            {
                _versionInfo = GetVersionInfo();
                return _versionInfo;
            }
        }

        private int _rating;

        public int Rating
        {
            get
            {
                return _rating;            }
            set
            {
                _rating = value;
                NotifyOfPropertyChange(() => Rating);
                if (_rating > 3)
                {
                    _messageQueue.Enqueue($"Thx for giving {_rating} stars!");
                }
            }
        }

        public string GetVersionInfo()
        {
            string major = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString();
            string minor = Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();
            string build = Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();

            return $"CoreNotes version {major}.{minor}.{build}";
        }

        public void BrowseToGithub()
        {
            System.Diagnostics.Process.Start("https://github.com/mufana/CoreNotes");
        }
        public void Close()
        {
            TryClose();
        }
    }
}

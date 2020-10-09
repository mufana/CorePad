using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using System.Reflection;

namespace CoreNotes.ViewModels
{
    /// <summary>
    /// AboutViewModel that inherits from screen
    /// </summary>
    public class AboutViewModel : Screen
    {

        /// <summary>
        /// The SnackbarMessageQueue that will receive snackbar messages
        /// </summary>
        private SnackbarMessageQueue _messageQueue;

        /// <summary>
        /// Private backing field for the public 'VersionInfo' property
        /// </summary>
        private string _versionInfo;

        /// <summary>
        /// Private backing field for the public 'Rating' property
        /// </summary>
        private int _rating;

        /// <summary>
        /// Public field for the 'VersionInfo' property. Whenever the UI updates, this property will update as well
        /// </summary>
        public string VersionInfo
        {
            get
            {
                _versionInfo = GetVersionInfo();
                return _versionInfo;
            }
        }

        /// <summary>
        /// Public field for the 'Rating' property. Whenever the UI updates, this property will update as well
        /// </summary>
        public int Rating
        {
            get { return _rating; }
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

        /// <summary>
        /// AboutViewModel ctor
        /// </summary>
        /// <param name="MessageQueue"></param>
        public AboutViewModel(SnackbarMessageQueue MessageQueue)
        {
            _messageQueue = MessageQueue;
        }

        /// <summary>
        /// Retrieves the version info from the AssemblyInfo through reflection
        /// </summary>
        /// <returns></returns>
        public string GetVersionInfo()
        {
            string major = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString();
            string minor = Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();
            string build = Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();

            return $"CoreNotes version {major}.{minor}.{build}";
        }

        /// <summary>
        /// Closes the AboutView
        /// </summary>
        public void Close()
        {
            TryClose();
        }
    }
}

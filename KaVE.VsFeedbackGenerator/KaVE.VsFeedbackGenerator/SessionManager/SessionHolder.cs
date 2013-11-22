using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using JetBrains.Application;
using KaVE.EventGenerator.ReSharper8.SessionManager;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [ShellComponent]
    public sealed class SessionHolder : INotifyPropertyChanged
    {
        public SessionHolder()
        {
            RefreshSessions();
        }

        public void RefreshSessions()
        {
            var logFiles = Directory.GetFiles(Initializer.EventLogsDirectory, "*." + Initializer.LogFileExtension);
            Sessions = logFiles.Select(logFileName => new Session(logFileName)).ToList();
            OnPropertyChanged("Sessions");
        }

        public List<Session> Sessions { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

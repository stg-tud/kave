using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using JetBrains;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.UI.Extensions.Commands;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NuGet;
using HttpClient = System.Net.Http.HttpClient;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [ShellComponent]
    public sealed class FeedbackView : INotifyPropertyChanged
    {
        private readonly JsonLogFileManager _logFileManager;
        private readonly IList<SessionView> _sessions;
        private readonly IList<SessionView> _selectedSessions;
        private DelegateCommand _deleteSessionsCommand;
        private DelegateCommand _exportSessionsCommand;
        private DelegateCommand _sendSessionsCommand;
        private bool _refreshing;
        private DateTime _lastRefresh;

        public FeedbackView(JsonLogFileManager logFileManager)
        {
            _logFileManager = logFileManager;
            _sessions = new ObservableCollection<SessionView>();
            _selectedSessions = new List<SessionView>();
            Released = true;
        }

        public void Refresh()
        {
            Refreshing = true;
            Invoke.Async(
                () =>
                {
                    _lastRefresh = DateTime.Now;
                    Sessions =
                        _logFileManager.GetLogFileNames()
                            .Select(logFileName => new SessionView(_logFileManager, logFileName));
                    Refreshing = false;
                    Released = false;
                });
        }

        public void Release()
        {
            Sessions = null;
            Released = true;
        }

        public bool Released { get; private set; }

        public bool Refreshing
        {
            get { return _refreshing; }
            private set
            {
                _refreshing = value;
                OnPropertyChanged("Refreshing");
            }
        }

        public IEnumerable<SessionView> Sessions
        {
            private set
            {
                Invoke.OnDispatcher(
                    () =>
                    {
                        _sessions.Clear();
                        if (value != null)
                        {
                            _sessions.AddRange(value);
                        }
                        _sendSessionsCommand.RaiseCanExecuteChanged();
                        _exportSessionsCommand.RaiseCanExecuteChanged();
                    });
            }
            get { return _sessions; }
        }

        public bool AnySessionsPresent
        {
            get { return _sessions.Count > 0; }
        }

        public IEnumerable<SessionView> SelectedSessions
        {
            set
            {
                _selectedSessions.Clear();
                _selectedSessions.AddRange(value);
                // single selected session depends on selected sessions
                OnPropertyChanged("SingleSelectedSession");
                _deleteSessionsCommand.RaiseCanExecuteChanged();
            }
        }

        [CanBeNull]
        public SessionView SingleSelectedSession
        {
            get { return _selectedSessions.Count == 1 ? _selectedSessions.First() : null; }
        }

        #region ExportSessionsCommands implementation

        public DelegateCommand ExportSessionsCommand
        {
            get
            {
                return _exportSessionsCommand ??
                       (_exportSessionsCommand =
                           new DelegateCommand(param => ExportSessions(), param => AnySessionsPresent));
            }
        }

        public DelegateCommand SendSessionsCommand
        {
            get
            {
                return _sendSessionsCommand ??
                       (_sendSessionsCommand =
                           new DelegateCommand(param => SendSessions(), param => AnySessionsPresent));
            }
        }

        private void ExportSessions()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Log files (*.log)|*.log|All files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog().Equals(DialogResult.Cancel))
            {
                return;
            }
            var destFileName = saveFileDialog.FileName; // checks??
            var exportFileName = GenerateExportFile();
            File.Copy(exportFileName, destFileName, true);
            //_logFileManager.DeleteLogsOlderThan(_lastRefresh);
        }

        private void SendSessions()
        {
            var exportFileName = GenerateExportFile();
            //Upload("http://kave.st.informatik.tu-darmstadt.de:667/upload", Path.GetFileName(exportFileName), File.ReadAllBytes(exportFileName));

            // upload
            // delete
            //_logFileManager.DeleteLogsOlderThan(_lastRefresh);
        }

        private string GenerateExportFile()
        {
            var tmpFileName = Path.GetTempFileName();
            using (var writer = _logFileManager.NewLogWriter(tmpFileName))
            {
                foreach (var e in _sessions.SelectMany(session => session.Events).Select(eventlist => eventlist.Event))
                {
                    writer.Write(e);
                }
            }
            return tmpFileName;
        }

        private Stream Upload(string actionUrl, string paramString, byte[] paramFileBytes)
        {
            HttpContent bytesContent = new ByteArrayContent(paramFileBytes);
            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(bytesContent, "file", paramString);
                    var response = client.PostAsync(actionUrl, formData).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }
                    MessageBox.Show(response.Content.ReadAsStringAsync().Result);
                    return response.Content.ReadAsStreamAsync().Result;
                }
            }
        }

        #endregion

        // TODO remove these regions...
        #region DeleteSessionsCommand implementation

        public DelegateCommand DeleteSessionsCommand
        {
            get { return _deleteSessionsCommand ?? (_deleteSessionsCommand = CreateDeleteSessionsCommand()); }
        }

        private DelegateCommand CreateDeleteSessionsCommand()
        {
            Func<string> confirmationTitle = () => Messages.SessionDeleteConfirmTitle;
            Func<string> confirmationText = () =>
            {
                var numberOfSessions = _selectedSessions.Count;
                return numberOfSessions == 1
                    ? Messages.SessionDeleteConfirmSingular
                    : Messages.SessionDeleteConfirmPlural.FormatEx(numberOfSessions);
            };
            return ConfirmedCommand.Create(DeleteSessions, confirmationTitle, confirmationText, CanDeleteSessions);
        }

        private bool CanDeleteSessions()
        {
            return _selectedSessions.Count > 0;
        }

        private void DeleteSessions()
        {
            for (var i = 0; i < _sessions.Count; i++)
            {
                var session = _sessions[i];
                if (_selectedSessions.Contains(session))
                {
                    _sessions.Remove(session);
                    File.Delete(session.LogFileName);
                    i--;
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
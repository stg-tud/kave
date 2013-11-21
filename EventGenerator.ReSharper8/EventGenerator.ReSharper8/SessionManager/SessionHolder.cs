using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KaVE.EventGenerator.ReSharper8.SessionManager
{
    public class Session : INotifyPropertyChanged
    {
        private DateTime date;
        private int time;
        private string content;

        public Session(DateTime date, int time, string content)
        {
            Time = time;
            Date = date;
            Content = content;
        }

        public DateTime Date
        { 
            get { return date; }
            private set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        public int Time
        {
            get { return time; }
            private set
            {
                time = value;
                OnPropertyChanged("Time");
            }
        }

        public string Content
        {
            get { return content; }
            private set
            {
                content = value;
                OnPropertyChanged("Content");
            }
        }

        #region INotifyPropertyChanged Members
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

    public sealed class SessionHolder
    {
        private static readonly SessionHolder instance = new SessionHolder();

        private SessionHolder()
        {
            SessionList = new List<Session>
            {
                new Session(DateTime.Now, 10, "LIRUM LARUM LÖFFELSTIL"),
                new Session(DateTime.Now, 20, @"{
                                                ""glossary"": {
                                                    ""title"": ""example glossary"",
		                                            ""GlossDiv"": {
                                                        ""title"": ""S"",
			                                            ""GlossList"": {
                                                            ""GlossEntry"": {
                                                                ""ID"": ""SGML"",
					                                            ""SortAs"": ""SGML"",
					                                            ""GlossTerm"": ""Standard Generalized Markup Language"",
					                                            ""Acronym"": ""SGML"",
					                                            ""Abbrev"": ""ISO 8879:1986"",
					                                            ""GlossDef"": {
                                                                    ""para"": ""A meta-markup language, used to create markup languages such as DocBook."",
						                                            ""GlossSeeAlso"": [""GML"", ""XML""]
                                                                },
					                                            ""GlossSee"": ""markup""
                                                            }
                                                        }
                                                    }
                                                }
                                            }")
            };
        }

        public static SessionHolder Instance
        {
            get { return instance; }
        }

        public List<Session> SessionList { get; private set; }
    }
}

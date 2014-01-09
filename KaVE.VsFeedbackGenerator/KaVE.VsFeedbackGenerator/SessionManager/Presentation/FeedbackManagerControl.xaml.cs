using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KaVE.Utils;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    /// <summary>
    /// Interaktionslogik für SessionManagerControl.xaml
    /// </summary>
    public partial class SessionManagerControl
    {
        private readonly FeedbackView _feedbackView;
        private ScheduledAction _releaseTimer;

        public SessionManagerControl(FeedbackView holder)
        {
            _releaseTimer = ScheduledAction.NoOp;
            _feedbackView = holder;
            DataContext = holder;
            InitializeComponent();
        }

        private void FeedbackWindowControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            RefreshControl();
        }

        /// <summary>
        /// Makes the overflow dropdown button invisible.
        /// </summary>
        private void ToolBar_OnLoaded(object sender, RoutedEventArgs e)
        {
            var toolBar = sender as ToolBar;
            if (toolBar == null)
            {
                return;
            }
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Unfortunately, SelectedItems is not a bidirectional control property, hence, we cannot bind it to a
        /// property of out view model to access the set of selected items. Therefore, we react on selection changes
        /// here and update our model property manually.
        /// </summary>
        private void SessionListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _feedbackView.SelectedSessions = SessionListView.SelectedItems.Cast<SessionView>();
        }

        /// <summary>
        /// Unfortunately, SelectedItems is not a bidirectional control property, hence, we cannot bind it to a
        /// property of out view model to access the set of selected items. Therefore, we react on selection changes
        /// here and update our model property manually.
        /// </summary>
        private void EventListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_feedbackView.SingleSelectedSession != null)
            {
                _feedbackView.SingleSelectedSession.SelectedEvents = EventListView.SelectedItems.Cast<EventView>();
            }
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshControl();
        }

        private void VisitHomepageButton_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO insert correct url here
            System.Diagnostics.Process.Start("http://kave-project.de");
        }

        private void SessionManagerControl_OnLostFocus(object sender, RoutedEventArgs e)
        {
            lock (_feedbackView)
            {
                // release view data after 5 minutes of inactivity
                _releaseTimer = Invoke.Later(() => _feedbackView.Release(), 300000);
            }
        }

        private void SessionManagerControl_OnGotFocus(object sender, RoutedEventArgs e)
        {
            lock (_feedbackView)
            {
                _releaseTimer.Cancel();
                if (_feedbackView.Released)
                {
                    RefreshControl();
                }
            }
        }

        private void RefreshControl()
        {
            // TODO enable "loading" screen
            _feedbackView.RefreshSessions();
            // TODO disable "loading" screen
        }
    }
}
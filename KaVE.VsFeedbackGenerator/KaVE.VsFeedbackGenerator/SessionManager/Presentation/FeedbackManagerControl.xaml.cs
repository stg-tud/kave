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
        private readonly FeedbackViewModel _feedbackViewModel;
        private ScheduledAction _releaseTimer;

        public SessionManagerControl(FeedbackViewModel holder)
        {
            _releaseTimer = ScheduledAction.NoOp;
            _feedbackViewModel = holder;
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
            _feedbackViewModel.SelectedSessions = SessionListView.SelectedItems.Cast<SessionView>();
        }

        /// <summary>
        /// Unfortunately, SelectedItems is not a bidirectional control property, hence, we cannot bind it to a
        /// property of out view model to access the set of selected items. Therefore, we react on selection changes
        /// here and update our model property manually.
        /// </summary>
        private void EventListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_feedbackViewModel.SingleSelectedSession != null)
            {
                _feedbackViewModel.SingleSelectedSession.SelectedEvents = EventListView.SelectedItems.Cast<EventView>();
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
            lock (_feedbackViewModel)
            {
                // release view data after 5 minutes of inactivity
                _releaseTimer = Invoke.Later(() => _feedbackViewModel.Release(), 300000);
            }
        }

        private void SessionManagerControl_OnGotFocus(object sender, RoutedEventArgs e)
        {
            lock (_feedbackViewModel)
            {
                _releaseTimer.Cancel();
                if (_feedbackViewModel.Released)
                {
                    RefreshControl();
                }
            }
        }

        private void RefreshControl()
        {
            _feedbackViewModel.Refresh();
        }
    }
}
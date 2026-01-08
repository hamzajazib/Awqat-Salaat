using AwqatSalaat.Helpers;
using AwqatSalaat.ViewModels;
using AwqatSalaat.WinUI.Helpers;
using AwqatSalaat.WinUI.Media;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Serilog;
using System;
using Windows.Foundation;

namespace AwqatSalaat.WinUI.Views
{
    public sealed partial class WidgetSummary : UserControl
    {
        private const string NearNotificationTag = "NearNotification";
        private const string AdhanSoundTag = "Adhan";

#if DEBUG
        public static WidgetSummary Current { get; private set; }
#endif

        public static readonly DependencyProperty ElementsAlignmentProperty = DependencyProperty.Register(
            "ElementsAlignment",
            typeof(HorizontalAlignment),
            typeof(WidgetSummary),
            new PropertyMetadata(HorizontalAlignment.Center));

        public HorizontalAlignment ElementsAlignment
        {
            get => (HorizontalAlignment)GetValue(ElementsAlignmentProperty);
            set => SetValue(ElementsAlignmentProperty, value);
        }

        private bool shouldBeCompactHorizontally;
        private DisplayMode currentDisplayMode = DisplayMode.Default;
        private AudioPlayerSession currentAudioSession;

        private WidgetViewModel ViewModel => DataContext as WidgetViewModel;

        public event Action<DisplayMode> DisplayModeChanged;

        public WidgetSummary()
        {
            this.InitializeComponent();
#if DEBUG
            Current = this;
            Properties.Settings.Default.IsConfigured = false;
#endif
            this.Loaded += WidgetSummary_Loaded;
            this.Unloaded += WidgetSummary_Unloaded;
            ViewModel.WidgetSettings.Updated += WidgetSettings_Updated;
            ViewModel.WidgetSettings.Realtime.PropertyChanged += Settings_PropertyChanged;
            ViewModel.NearNotificationStarted += ViewModel_NearNotificationStarted;
            ViewModel.NearNotificationStopped += ViewModel_NearNotificationStopped;
            ViewModel.PrayerTimeEntered += ViewModel_PrayerTimeEntered;
            LocaleManager.Default.CurrentChanged += LocaleManager_CurrentChanged;
            ThemeHelper.ThemeChanged += ThemeHelper_ThemeChanged;
            Notification.NotificationManager.ShowWidgetRequested += NotificationManager_ShowWidgetRequested;
            Notification.NotificationManager.DismissReminderRequested += NotificationManager_DismissReminderRequested;
            Notification.NotificationManager.StopReminderSoundRequested += NotificationManager_StopReminderSoundRequested;
            Notification.NotificationManager.StopAdhanRequested += NotificationManager_StopAdhanRequested;

            UpdateDirection();
            UpdateNotificationSound();
        }

        private void NotificationManager_StopAdhanRequested()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (currentAudioSession?.Tag == AdhanSoundTag)
                {
                    Log.Information("Stopping adhan sound after interacting with toast notification");
                    currentAudioSession.End();
                }
            });
        }

        private void NotificationManager_StopReminderSoundRequested()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (currentAudioSession?.Tag == NearNotificationTag)
                {
                    Log.Information("Stopping reminder sound after interacting with toast notification");
                    currentAudioSession.End();
                }
            });
        }

        private void NotificationManager_DismissReminderRequested()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                Log.Information("Dismissing reminder after interacting with toast notification");
                ViewModel.DisplayedTime.DismissNotification.Execute(null);
            });
        }

        private void NotificationManager_ShowWidgetRequested()
        {
            Log.Information("Showing widget's panel after clicking on toast notification");
            DispatcherQueue.TryEnqueue(() => ToggleButton_Checked(toggle, null));
        }

        private void ThemeHelper_ThemeChanged()
        {
            DispatcherQueue.TryEnqueue(UpdateTheme);
        }

        private void UpdateTheme()
        {
            this.RequestedTheme = ThemeHelper.ButtonTheme;
            Log.Information($"Updated theme: {this.RequestedTheme}");
        }

        private void ReloadThemes()
        {
            ThemeHelper.ReloadElementTheme(this, this.RequestedTheme);

            var flyoutPresenter = flyout.GetPresenter();

            if (flyoutPresenter?.RequestedTheme is ElementTheme currentTheme)
            {
                ThemeHelper.ReloadElementTheme(flyoutPresenter, currentTheme);
            }
        }

        private void WidgetSummary_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Information("Widget summary loaded");
            UpdateTheme();
            UpdateDisplayMode();
        }

        private void WidgetSummary_Unloaded(object sender, RoutedEventArgs e)
        {
            Log.Information("Widget summary unloaded");
            ViewModel.WidgetSettings.Realtime.PropertyChanged -= Settings_PropertyChanged;
            ViewModel.WidgetSettings.Updated -= WidgetSettings_Updated;
            ViewModel.NearNotificationStarted -= ViewModel_NearNotificationStarted;
            ViewModel.NearNotificationStopped -= ViewModel_NearNotificationStopped;
            ViewModel.PrayerTimeEntered -= ViewModel_PrayerTimeEntered;
            LocaleManager.Default.CurrentChanged -= LocaleManager_CurrentChanged;
            ThemeHelper.ThemeChanged -= ThemeHelper_ThemeChanged;
            Notification.NotificationManager.ShowWidgetRequested -= NotificationManager_ShowWidgetRequested;
            Notification.NotificationManager.DismissReminderRequested -= NotificationManager_DismissReminderRequested;
            Notification.NotificationManager.StopReminderSoundRequested -= NotificationManager_StopReminderSoundRequested;
            Notification.NotificationManager.StopAdhanRequested -= NotificationManager_StopAdhanRequested;

            currentAudioSession?.End();
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(Properties.Settings.ShowCountdown) or nameof(Properties.Settings.UseCompactMode))
            {
                flyout?.DisableLightDismissTemporarily();
                UpdateDisplayMode();
            }
            else if (e.PropertyName is nameof(Properties.Settings.AutoAlignment) or nameof(Properties.Settings.DisplayLanguage))
            {
                TaskBarManager.InvalidateWidgetElementsAlignment(ViewModel.WidgetSettings.Realtime.AutoAlignment);
            }
            else if (e.PropertyName ==  nameof(Properties.Settings.ThemeAccent))
            {
                ReloadThemes();
            }
            else if (e.PropertyName == nameof(Properties.Settings.ShortTimePattern))
            {
                Bindings.Update();
            }
        }

        private void ViewModel_PrayerTimeEntered(PrayerTimeViewModel prayerTime, bool adhanRequested)
        {
            if (adhanRequested)
            {
                bool isFajrTime = prayerTime.Key == nameof(Data.PrayerTimes.Fajr);
                DispatcherQueue.TryEnqueue(() =>
                {
                    Log.Information("Adhan requested" + (isFajrTime ? " for fajr" : ""));
                    var file = isFajrTime
                            ? ViewModel.WidgetSettings.Settings.AdhanFajrSoundFilePath
                            : ViewModel.WidgetSettings.Settings.AdhanSoundFilePath;
                    var session = new AudioPlayerSession
                    {
                        File = file,
                        Tag = AdhanSoundTag,
                    };
                    PlaySound(session);
                });
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                if (prayerTime.Key != nameof(Data.PrayerTimes.Shuruq) && ViewModel.WidgetSettings.Settings.EnablePrayerTimeToast)
                {
                    Notification.NotificationManager.SendTimeEnteredToast(prayerTime.Key, adhanRequested);
                }
            });
        }

        private void ViewModel_NearNotificationStarted()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                bool repeat = ViewModel.WidgetSettings.Settings.RepeatNotificationSound;
                var file = ViewModel.WidgetSettings.Settings.NotificationSoundFilePath;
                var session = new AudioPlayerSession
                {
                    File = file,
                    Loop = repeat,
                    Tag = NearNotificationTag,
                };
                PlaySound(session);

                if (ViewModel.WidgetSettings.Settings.EnableReminderToast)
                {
                    var prayer = ViewModel.DisplayedTime.Key;
                    var time = ViewModel.DisplayedTime.Time;
                    var playingSound = currentAudioSession is not null;
                    Notification.NotificationManager.SendReminderToast(prayer, time, playingSound);
                }
            });
        }

        private void ViewModel_NearNotificationStopped()
        {
            DispatcherQueue.TryEnqueue(() => currentAudioSession?.End());
        }

        private void PlaySound(AudioPlayerSession session)
        {
            bool started = AudioPlayer.Play(session);

            if (started)
            {
                currentAudioSession = session;
                session.Ended += AudioSession_Ended;
            }
        }

        private void AudioSession_Ended()
        {
            currentAudioSession.Ended -= AudioSession_Ended;
            currentAudioSession = null;
        }

        private void WidgetSettings_Updated(bool apiSettingsUpdated)
        {
            UpdateNotificationSound();
        }

        private void LocaleManager_CurrentChanged(object sender, EventArgs e)
        {
            UpdateDirection();
        }

        private void Flyout_Opened(object sender, object e)
        {
            Log.Information("Flyout opened");
            //flyoutContent.Focus(FocusState.Programmatic);
        }

        private void Flyout_Closed(object sender, object e)
        {
            Log.Information("Flyout closed");

            toggle.IsChecked = false;

            if (ViewModel.WidgetSettings.IsOpen && ViewModel.WidgetSettings.Settings.IsConfigured)
            {
                ViewModel.WidgetSettings.Cancel.Execute(null);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var needCompactH = finalSize.Height < grid.MaxHeight;

            if (shouldBeCompactHorizontally != needCompactH)
            {
                shouldBeCompactHorizontally = needCompactH;
                DispatcherQueue.TryEnqueue(UpdateDisplayMode);
            }

            return base.ArrangeOverride(finalSize);
        }

        private void UpdateDisplayMode()
        {
            if (!this.IsLoaded)
            {
                return;
            }

            DisplayMode displayMode = DisplayMode.Default;

            if (shouldBeCompactHorizontally)
            {
                displayMode = ViewModel.WidgetSettings.Realtime.ShowCountdown
                    ? DisplayMode.CompactHorizontal
                    : DisplayMode.CompactHorizontalNoCountdown;
            }
            else if (!ViewModel.WidgetSettings.Realtime.ShowCountdown)
            {
                displayMode = DisplayMode.CompactNoCountdown;
            }
            else if (ViewModel.WidgetSettings.Realtime.UseCompactMode)
            {
                displayMode = DisplayMode.Compact;
            }

            if (displayMode == currentDisplayMode)
            {
                return;
            }

            bool success = VisualStateManager.GoToState(this, displayMode.ToString(), false);

            if (success)
            {
                currentDisplayMode = displayMode;
                DisplayModeChanged?.Invoke(displayMode);
            }
        }

        private void UpdateNotificationSound()
        {
            var filePath = ViewModel.WidgetSettings.Settings.NotificationSoundFilePath;

            // Do we have a running notification sound?
            if (currentAudioSession?.Tag == NearNotificationTag)
            {
                // Yes, we have

                // Did the file path change?
                if (!string.Equals(currentAudioSession.File, filePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Yes,

                    // Stop current sound
                    currentAudioSession.End();
                }
                else
                {
                    return;
                }
            }

            // Start playing if we have notification
            if (!string.IsNullOrEmpty(filePath) && ViewModel.DisplayedTime?.State == PrayerTimeState.Near)
            {
                ViewModel_NearNotificationStarted();
            }
        }

        private void UpdateDirection()
        {
            var dir = LocaleManager.Default.CurrentCulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            btngrid.FlowDirection = dir;
            flyoutContent.FlowDirection = dir;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }

    public enum DisplayMode
    {
        Default,
        Compact,
        CompactNoCountdown,
        CompactHorizontal,
        CompactHorizontalNoCountdown
    }
}

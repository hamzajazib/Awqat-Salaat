using AwqatSalaat.Configurations;
using AwqatSalaat.Data;
using AwqatSalaat.Helpers;
using AwqatSalaat.Services.GitHub;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Settings = AwqatSalaat.Properties.Settings;

namespace AwqatSalaat.ViewModels
{
    public class WidgetSettingsViewModel : ObservableObject
    {
        private bool isOpen = !Settings.Default.IsConfigured;
        private bool isCheckingNewVersion;

        public static Country[] AvailableCountries => CountriesProvider.GetCountries();

        public bool IsOpen { get => isOpen; set => SetProperty(ref isOpen, value); }
        public bool IsCheckingNewVersion { get => isCheckingNewVersion; set => SetProperty(ref isCheckingNewVersion, value); }
        public string CountdownFormat => Realtime.ShowSeconds ? "{0:hh\\:mm\\:ss}" : "{0:hh\\:mm}";
        public Settings Settings => Settings.Default;

        // realtime settings are binded to settings UI so changes are reflected immediately
        public Settings Realtime { get; } = Settings.Realtime;
        public RelayCommand Save { get; }
        public RelayCommand Cancel { get; }
        public LocatorViewModel Locator { get; }
        public CsvImporterViewModel CsvImporter { get; }
        public ObservableCollection<PrayerConfig> PrayerConfigs { get; }

        public event Action<bool> Updated;

        public WidgetSettingsViewModel()
        {
            Save = new RelayCommand(SaveExecute);
            Cancel = new RelayCommand(CancelExecute, o => Settings.IsConfigured);

            if (!Settings.IsConfigured)
            {
                Settings.Upgrade();
            }

            if (string.IsNullOrEmpty(Settings.DisplayLanguage))
            {
                Settings.DisplayLanguage = LocaleManager.Default.Current;
            }

            CopySettings(fromOriginal: true);

            Locator = new LocatorViewModel(Realtime);
            CsvImporter = new CsvImporterViewModel(Realtime);

            Realtime.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Realtime.ShowSeconds))
                {
                    OnPropertyChanged(nameof(CountdownFormat));
                }
            };

            PrayerConfigs = new ObservableCollection<PrayerConfig>
            {
                Realtime.GetPrayerConfig(nameof(PrayerTimes.Fajr)),
                Realtime.GetPrayerConfig(nameof(PrayerTimes.Shuruq)),
                Realtime.GetPrayerConfig(nameof(PrayerTimes.Dhuhr)),
                Realtime.GetPrayerConfig(nameof(PrayerTimes.Asr)),
                Realtime.GetPrayerConfig(nameof(PrayerTimes.Maghrib)),
                Realtime.GetPrayerConfig(nameof(PrayerTimes.Isha)),
            };
        }

        public async Task<Release> CheckForNewVersion(Version currentVersion, CancellationToken cancellationToken)
        {
            try
            {
                IsCheckingNewVersion = true;

                var latest = await GitHubClient.GetLatestRelease(cancellationToken);

                if (cancellationToken.IsCancellationRequested || latest is null)
                {
                    return null;
                }

                if (latest.IsDraft || latest.IsPreRelease)
                {
                    var allReleases = await GitHubClient.GetReleases(cancellationToken);

                    if (allReleases?.Length > 1)
                    {
                        latest = allReleases
                            .Where(r => !r.IsDraft && !r.IsPreRelease)
                            .DefaultIfEmpty(new Release { Tag = "0.0" })
                            .OrderByDescending(r => r.GetVersion())
                            .First();
                    }
                    else
                    {
                        return null;
                    }
                }

                return latest.GetVersion() > currentVersion ? latest : null;
            }
            finally
            {
                IsCheckingNewVersion = false;
            }
        }

        private void CopySettings(bool fromOriginal)
        {
            Settings source = fromOriginal ? Settings : Realtime;
            Settings destination = fromOriginal ? Realtime : Settings;

            foreach (SettingsProperty prop in Settings.Properties)
            {
                if (prop.Name == nameof(Settings.CustomPosition))
                {
                    // we are not interested in custom position here and we should not affect it
                    continue;
                }

                destination[prop.Name] = source[prop.Name];
            }
        }

        private void SaveExecute(object obj)
        {
            Log.Information("[Settings] Save invoked");
            var currentServiceSettings = (
                    Realtime.Service,
                    Realtime.School,
                    Realtime.MethodString,
                    Realtime.CountryCode,
                    Realtime.City,
                    Realtime.ZipCode,
                    Realtime.Latitude,
                    Realtime.Longitude,
                    Realtime.LocationDetection,
                    Realtime.QchCity,
                    Realtime.CSV_FilePath,
                    Realtime.CSV_Range,
                    Realtime.CSV_HasHeader,
                    Realtime.CSV_HasDateColumn,
                    Realtime.CSV_DateColumnSchema,
                    Realtime.CSV_Map_Fajr,
                    Realtime.CSV_Map_Shuruq,
                    Realtime.CSV_Map_Dhuhr,
                    Realtime.CSV_Map_Asr,
                    Realtime.CSV_Map_Maghrib,
                    Realtime.CSV_Map_Isha,
                    Realtime.CSV_Map_Date,
                    Realtime.CSV_Map_Day,
                    Realtime.CSV_Map_Month
                    );
            var previousServiceSettings = (
                    Settings.Service,
                    Settings.School,
                    Settings.MethodString,
                    Settings.CountryCode,
                    Settings.City,
                    Settings.ZipCode,
                    Settings.Latitude,
                    Settings.Longitude,
                    Settings.LocationDetection,
                    Settings.QchCity,
                    Settings.CSV_FilePath,
                    Settings.CSV_Range,
                    Settings.CSV_HasHeader,
                    Settings.CSV_HasDateColumn,
                    Settings.CSV_DateColumnSchema,
                    Settings.CSV_Map_Fajr,
                    Settings.CSV_Map_Shuruq,
                    Settings.CSV_Map_Dhuhr,
                    Settings.CSV_Map_Asr,
                    Settings.CSV_Map_Maghrib,
                    Settings.CSV_Map_Isha,
                    Settings.CSV_Map_Date,
                    Settings.CSV_Map_Day,
                    Settings.CSV_Map_Month
                    );
            bool serviceSettingsChanged = previousServiceSettings != currentServiceSettings;
            Realtime.IsConfigured = true;
            CopySettings(fromOriginal: false);
            Settings.Save();
            LogManager.InvalidateLogger();
            Cleanup();
            Updated?.Invoke(serviceSettingsChanged);
        }

        private void CancelExecute(object obj)
        {
            Log.Information("[Settings] Cancel invoked");
            CopySettings(fromOriginal: true);
            Cleanup();
        }

        private void Cleanup()
        {
            IsOpen = false;
            Locator.SearchQuery = null;
            Locator.CancelCheck.Execute(null);
            Cancel.RaiseCanExecuteChanged();
        }
    }
}
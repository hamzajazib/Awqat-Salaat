using AwqatSalaat.Helpers;
using AwqatSalaat.Properties;
using CsvHelper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace AwqatSalaat.ViewModels
{
    public class CsvImporterViewModel : ObservableObject
    {
        private readonly Settings settings;

        public bool RangeInYear
        {
            get => settings.CSV_Range == Configurations.CsvImportRange.Year;
            set
            {
                if (value)
                {
                    settings.CSV_Range = Configurations.CsvImportRange.Year;
                }
            }
        }
        public bool RangeInMonth
        {
            get => settings.CSV_Range == Configurations.CsvImportRange.Month;
            set
            {
                if (value)
                {
                    settings.CSV_Range = Configurations.CsvImportRange.Month;
                }
            }
        }
        public bool HasSingleDateColumn
        {
            get => settings.CSV_DateColumnSchema == Configurations.CsvImportDateColumnSchema.Single;
            set
            {
                if (value)
                {
                    settings.CSV_DateColumnSchema = Configurations.CsvImportDateColumnSchema.Single;
                }
            }
        }
        public bool HasDualDateColumn
        {
            get => settings.CSV_DateColumnSchema == Configurations.CsvImportDateColumnSchema.Dual;
            set
            {
                if (value)
                {
                    settings.CSV_DateColumnSchema = Configurations.CsvImportDateColumnSchema.Dual;
                }
            }
        }
        public bool AreColumnsLoaded => Columns.Count > 0;
        public ObservableCollection<KeyValuePair<int, string>> Columns { get; } = new ObservableCollection<KeyValuePair<int, string>>();
        public RelayCommand Load { get; }

        public CsvImporterViewModel(Settings settings)
        {
            this.settings = settings;
            settings.PropertyChanged += Settings_PropertyChanged;

            Load = new RelayCommand(LoadExecute, o => File.Exists(settings.CSV_FilePath));
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.CSV_Range))
            {
                OnPropertyChanged(nameof(RangeInYear));
                OnPropertyChanged(nameof(RangeInMonth));
            }
            else if (e.PropertyName == nameof(Settings.CSV_DateColumnSchema))
            {
                OnPropertyChanged(nameof(HasSingleDateColumn));
                OnPropertyChanged(nameof(HasDualDateColumn));
            }
            else if (e.PropertyName == nameof(Settings.CSV_FilePath))
            {
                Load.RaiseCanExecuteChanged();
                Columns.Clear();
                OnPropertyChanged(nameof(AreColumnsLoaded));
            }
        }

        private void LoadExecute(object obj)
        {
            Log.Debug($"Loading CSV file: {settings.CSV_FilePath}");

            try
            {
                if (Columns.Count > 0)
                {
                    Columns.Clear();
                    OnPropertyChanged(nameof(AreColumnsLoaded));
                }

                using (var reader = new StreamReader(settings.CSV_FilePath))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csv.Read();

                        if (settings.CSV_HasHeader)
                        {
                            Log.Debug("Reading header");
                            csv.ReadHeader();
                            Log.Debug($"Header record length: {csv.HeaderRecord.Length} {{@header}}", csv.HeaderRecord);

                            for (int i = 0; i < csv.HeaderRecord.Length; i++)
                            {
                                var header = csv.HeaderRecord[i];
                                Columns.Add(new KeyValuePair<int, string>(i, header));
                            }
                        }
                        else
                        {
                            Log.Debug($"Column count: {csv.ColumnCount}");

                            for (int i = 0; i < csv.ColumnCount; i++)
                            {
                                Columns.Add(new KeyValuePair<int, string>(i, "#" + (i + 1)));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Loading CSV file failed: {ex.Message}");
#if DEBUG
                throw;
#endif
            }

            OnPropertyChanged(nameof(AreColumnsLoaded));
        }
    }
}

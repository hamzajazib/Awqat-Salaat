using AwqatSalaat.Data;

namespace AwqatSalaat.Helpers
{
    public static class AdhanConverter
    {
        public static string AdhanSoundToFilePath(AdhanSound adhanSound, string currentFile, bool isFajr)
        {
            switch (adhanSound)
            {
                case AdhanSound.None:
                    return string.Empty;
                case AdhanSound.Adhan1:
                    return isFajr
                        ? @"Sounds\kholafa_13041446_full.mp3"
                        : @"Sounds\kholafa_13041446.mp3";
                case AdhanSound.Adhan2:
                    return isFajr
                        ? @"Sounds\kholafa_08041446_full.mp3"
                        : @"Sounds\kholafa_08041446.mp3";
                case AdhanSound.NasserAlQatami:
                    return isFajr
                        ? @"Sounds\Sheikh Nasser Al-Qatami Fajr.mp3"
                        : @"Sounds\Sheikh Nasser Al-Qatami.mp3";
                case AdhanSound.AbdulMajeedAlSuraihi:
                    return @"Sounds\Abdul Majeed Al-Suraihi.mp3";
            }

            return currentFile;
        }

        public static AdhanSound FilePathToAdhanSound(string filePath)
        {
            switch (filePath)
            {
                case null:
                case "":
                    return AdhanSound.None;
                case @"Sounds\kholafa_13041446.mp3":
                case @"Sounds\kholafa_13041446_full.mp3":
                    return AdhanSound.Adhan1;
                case @"Sounds\kholafa_08041446.mp3":
                case @"Sounds\kholafa_08041446_full.mp3":
                    return AdhanSound.Adhan2;
                case @"Sounds\Sheikh Nasser Al-Qatami.mp3":
                case @"Sounds\Sheikh Nasser Al-Qatami Fajr.mp3":
                    return AdhanSound.NasserAlQatami;
                case @"Sounds\Abdul Majeed Al-Suraihi.mp3":
                    return AdhanSound.AbdulMajeedAlSuraihi;
            }

            return AdhanSound.Custom;
        }
    }
}

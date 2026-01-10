using System;
using System.IO;

namespace AwqatSalaat.Helpers
{
    public static class FileHelper
    {
        private static readonly string s_assemblyDirectory;

        static FileHelper()
        {
            var assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            s_assemblyDirectory = Path.GetDirectoryName(assemblyPath);
        }

        public static string AbsolutePath(string file)
        {
            try
            {
                if (Path.IsPathRooted(file))
                {
                    // Use Path.GetFullPath to make sure we always have a drive letter
                    return Path.GetFullPath(file);
                }
                else
                {
                    // The path is relative
                    return Path.Combine(s_assemblyDirectory, file);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                throw;
#endif
                return null;
            }
        }
    }
}

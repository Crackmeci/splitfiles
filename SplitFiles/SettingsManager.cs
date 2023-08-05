using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MyNamespace
{
    public static class SettingsManager
    {
        private const string settingsFile = "settings.json";

        public static void AddOrUpdateFileType(string fileType, string fileTypes)
        {
            var existingSettings = ReadSettings();

            if (existingSettings != null)
            {
                if (existingSettings.ContainsKey(fileType))
                {
                    existingSettings[fileType] = fileTypes;
                }
                else
                {
                    existingSettings.Add(fileType, fileTypes);
                }
            }
            else
            {
                existingSettings = new Dictionary<string, string>();
                existingSettings.Add(fileType, fileTypes);
            }

            WriteSettings(existingSettings);
        }

        public static string GetFileTypeSettings(string fileType)
        {
            var existingSettings = ReadSettings();

            if (existingSettings.ContainsKey(fileType))
            {
                return existingSettings[fileType];
            }

            return null; // Tür bulunamadıysa null dönebilirsiniz veya farklı bir işlem yapabilirsiniz
        }

        private static Dictionary<string, string> ReadSettings()
        {
            if (File.Exists(settingsFile))
            {
                var json = File.ReadAllText(settingsFile);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }

            return new Dictionary<string, string>();
        }

        public static int RemoveFileType(string fileType)
        {
            var existingSettings = ReadSettings();

            if (existingSettings.ContainsKey(fileType))
            {
                existingSettings.Remove(fileType);
                WriteSettings(existingSettings);
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static List<string> GetAllFileTypes()
        {
            var existingSettings = ReadSettings();
            return existingSettings.Keys.ToList();
        }

        private static void WriteSettings(Dictionary<string, string> settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(settingsFile, json);
        }


    }
}

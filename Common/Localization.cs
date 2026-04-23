using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using ColossalFramework.Globalization;
using ColossalFramework.Plugins;
using static ColossalFramework.Globalization.Locale;

namespace SleepyCommon
{
    public class Localization
    {
        const string SYSTEM_DEFAULT = "System Default";

        private static readonly Dictionary<string, Locale> s_localeStore = new Dictionary<string, Locale>();
        private static bool s_bInitialised = false;

        public static string PreferredLanguage { get; set; } = SYSTEM_DEFAULT;

        // ----------------------------------------------------------------------------------------
        public static string Get(string sKey)
        {
            if (!s_bInitialised)
            {
                s_bInitialised = true;
                LoadAllLanguageFiles();
            }

            string sText = GetValue(sKey);
            if (string.IsNullOrEmpty(sText) || sText.Contains(sKey))
            {
                CDebug.Log("LOCALIZATION Couldn't find: " + sKey);
            }
            sText = sText.Replace("\\r\\n", "\r\n");
            return sText;
        }

        public static string[] GetLoadedLanguages()
        {
            List<string> languages = new List<string>();

            languages.Add(SYSTEM_DEFAULT);
            foreach (KeyValuePair<string, Locale> kvp in s_localeStore)
            {
                Locale locale = kvp.Value;
                string sCode = locale.Get(new Locale.Key { m_Identifier = "CODE" });
                string sName = locale.Get(new Locale.Key { m_Identifier = "NAME" });
                languages.Add(sName + " (" + sCode + ")");
            }

            return languages.ToArray();
        }

        public static List<string> GetLoadedCodes()
        {
            List<string> languages = new List<string>();

            languages.Add(SYSTEM_DEFAULT);
            foreach (KeyValuePair<string, Locale> kvp in s_localeStore)
            {
                Locale locale = kvp.Value;
                string sCode = locale.Get(new Locale.Key { m_Identifier = "CODE" });
                languages.Add(sCode);
            }

            return languages;
        }

        public static int GetLanguageIndexFromCode(string sCode)
        {
            List<string> languages = GetLoadedCodes();
            return Math.Max(0, languages.IndexOf(sCode));
        }

        private static void LoadAllLanguageFiles()
        {
            string sPath = LocalePath();
            if (!string.IsNullOrEmpty(sPath) && Directory.Exists(sPath))
            {
                // Load each file in directory and attempt to deserialise as a translation file.
                string[] localFiles = Directory.GetFiles(sPath);
                foreach (string file in localFiles)
                {
                    if (file.EndsWith(".csv"))
                    {
                        Locale locale = LocaleFromFile(file);

                        string sLanguage = locale.Get(new Locale.Key { m_Identifier = "CODE" });
                        if (!string.IsNullOrEmpty(sLanguage) && !s_localeStore.ContainsKey(sLanguage))
                        {
                            s_localeStore[sLanguage] = locale;
                        }
                    }
                }
                CDebug.Log("Locales loaded: " + s_localeStore.Count);
            }
            else
            {
                CDebug.Log("Locales directory not found: " + LocalePath());
            }

            // Load en from resources so we have at least 1 language
            if (s_localeStore.Count == 0)
            {
                s_localeStore.Add("en", LocaleFromResource(Assembly.GetExecutingAssembly().GetName().Name + ".Locales.en.csv"));
            }
        }

        private static Locale LocaleFromFile(string file)
        {
            var locale = new Locale();
            if (File.Exists(file))
            {
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                try
                {
                    using (FileStream fileStream = File.OpenRead(file)) 
                    {
                        if (fileStream is not null)
                        {
                            using (var reader = new StreamReader(fileStream))
                            {
                                string line;
                                while ((line = reader.ReadLine()) is not null)
                                {
                                    line = line.Trim();
                                    if (!string.IsNullOrEmpty(line) && line[0] != '#')
                                    {
                                        string[] fields = CSVParser.Split(line);
                                        if (fields.Length == 2)
                                        {
                                            string key = Trim(fields[0]);
                                            string value = Trim(fields[1]);

                                            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                                            {
                                                try
                                                {
                                                    locale.AddLocalizedString(new Locale.Key { m_Identifier = Trim(fields[0]) }, Trim(fields[1]));
                                                }
                                                catch (Exception e)
                                                {
                                                    CDebug.Log($"ERROR: Key: {key} Value: {value} - " + e.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            CDebug.LogError("Unable to open localization file: " + file);
                        }
                    }
                    
                }
                catch (Exception e)
                {
                    CDebug.LogError("Unable to load localization file: " + file + " Error: " + e.ToString());
                }
            }

            return locale;
        }

        private static Locale LocaleFromResource(string resource)
        {
            var locale = new Locale();
            var assembly = Assembly.GetExecutingAssembly();
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream is not null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) is not null)
                        {
                            string[] fields = CSVParser.Split(line);
                            if (fields.Length == 2)
                            {
                                locale.AddLocalizedString(new Locale.Key { m_Identifier = Trim(fields[0]) }, Trim(fields[1]));
                            }
                        }
                    }
                }
                else
                {
                    CDebug.Log("Resource locale file not found.");
                }
            }

            return locale;
        }

        private static string LocalePath()
        {
            PluginManager.PluginInfo pluginInfo = PluginManager.instance.FindPluginInfo(Assembly.GetExecutingAssembly());
            if (pluginInfo is not null && !string.IsNullOrEmpty(pluginInfo.modPath))
            {
                return Path.Combine(pluginInfo.modPath, "Locales/");
            }
            return "";
        }

        private static string GetValue(string id)
        {
            string sLanguage = PreferredLanguage;

            if (string.IsNullOrEmpty(sLanguage) || sLanguage == SYSTEM_DEFAULT)
            {
                sLanguage = LocaleManager.instance.language ?? "en";
            }

            if (s_localeStore.ContainsKey(sLanguage))
            {
                return s_localeStore[sLanguage].Get(new Locale.Key { m_Identifier = id });
            }

            // We should at least have english due to file in resources.
            if (s_localeStore.ContainsKey("en"))
            {
                return s_localeStore["en"].Get(new Locale.Key { m_Identifier = id });
            }

            return $"TRANSLATION_NOT_FOUND:{id}";
        }

        private static string Trim(string s)
        {
            return s.Trim().Trim('"');
        }
    }
}
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SleepyCommon
{
    public static class DependencyUtils
    {
        private static Dictionary<long, bool> s_pluginRunning = new Dictionary<long, bool>();
        private static bool? s_bNaturalDisastersDlcOwned = null;

        public static void LogPlugins()
        {
            string sPlugins = "";
            foreach (PluginManager.PluginInfo oPlugin in PluginManager.instance.GetPluginsInfo())
            {
                sPlugins += "\n    ";
                if (oPlugin.isEnabled)
                {
                    sPlugins += "* ";
                }
                else
                {
                    sPlugins += "  ";
                }
                    
                sPlugins += oPlugin.name;

                if (oPlugin.userModInstance is not null)
                {
                    sPlugins += $" {((IUserMod)oPlugin.userModInstance).Name}"; 
                }
                else
                {
                    Log.Error("Mod instance is null");
                }
            }

            Log.Info("Loaded Mods");
            Log.Separator();
            Log.Info(sPlugins);
        }

        public static bool IsPluginRunningNotCached(long pluginId, string sAssemblyName)
        {
            string sPluginId = pluginId.ToString();

            bool bRunning = false;
            foreach (PluginManager.PluginInfo oPlugin in PluginManager.instance.GetPluginsInfo())
            {
                if (oPlugin.isEnabled)
                {
                    if (!string.IsNullOrEmpty(sPluginId))
                    {
                        if (oPlugin.name == sPluginId)
                        {
                            bRunning = true;
                            break;
                        };
                    }

                    if (!string.IsNullOrEmpty(sAssemblyName))
                    {
                        foreach (Assembly assembly in oPlugin.GetAssemblies())
                        {
                            if (assembly.GetName().Name.Contains(sAssemblyName))
                            {
                                bRunning = true;
                                break;
                            }
                        }
                    }
                }
            }

            return bRunning;
        }

        public static bool IsPluginRunning(long pluginId, string sAssemblyName)
        {
            // Only cache result once map is loaded
            if (UserModBase.BaseInstance.IsLoaded)
            {
                if (!s_pluginRunning.TryGetValue(pluginId, out bool bRunning))
                {
                    bRunning = IsPluginRunningNotCached(pluginId, sAssemblyName);
                    s_pluginRunning[pluginId] = bRunning;
                }

                return bRunning;
            }
            else
            {
                return IsPluginRunningNotCached(pluginId, sAssemblyName);
            }
        }

        public static bool IsHarmonyRunning()
        {
            // We look for either Harmony 2.2-0 steam ID or CitiesHarmony assembly name
            return IsPluginRunningNotCached(2040656402, "CitiesHarmony");
        }

        public static bool IsSmarterFireFightersRunning()
        {
            return IsPluginRunning(2346565561, "SmarterFirefighters");
        }

        public static bool IsUnifiedUIRunning()
        {
            return IsPluginRunning(2255219025, "UnifiedUILib");
        }

        public static bool IsPloppableRICORunning()
        {
            return IsPluginRunning(2016920607, "ploppablerico");
        }

        public static bool IsRepainterRunning()
        {
            return IsPluginRunning(2101551127, "Painter");
        }

        public static bool IsAdvancedBuildingLevelRunning()
        {
            return IsPluginRunning(2133705267, "AdvancedBuildingLevelControl");
        }

        public static bool IsRONRunning()
        {
            return IsPluginRunning(2405917899, "RON");
        }

        public static bool IsAdvancedOutsideConnectionsRunning()
        {
            return IsPluginRunning(2053500739, "AdvancedOutsideConnection");
        }

        public static bool IsSeniorCitizenCenterModRunning()
        {
            return IsPluginRunning(2559105223, "SeniorCitizenCenterMod");
        }

        public static bool IsEmployOverEducatedWorkersRunning()
        {
            return IsPluginRunning(1674732053, "EmployOvereducatedWorkers");
        }

        public static bool IsEmployOverEducatedWorkersByAkiraRunning()
        {
            // Don't check string name as it is the same as pcfantasy's mod name which does work.
            return IsPluginRunning(569008960, "");
        }

        public static bool IsCombinedAISRunning()
        {
            return IsPluginRunning(3158078540, "CombinedAIS");
        }

        public static bool IsCommuterDestinationsRunning()
        {
            return IsPluginRunning(2475986859, "CSLShowCommuterDestination");
        }

        public static bool IsImprovedPublicTransport2Running()
        {
            return IsPluginRunning(928128676, "ImprovedPublicTransport2");
        }

        public static bool IsMoreTransferReasonsRunning()
        {
            return IsPluginRunning(2980644460, "MoreTransferReasons");
        }

        public static bool IsPrisonHelicopterRunning()
        {
            return IsPluginRunning(2559039910, "PrisonHelicopter");
        }

        public static bool IsNaturalDisastersDLC()
        {
            if (s_bNaturalDisastersDlcOwned is null)
            {
                s_bNaturalDisastersDlcOwned = SteamHelper.IsDLCOwned(SteamHelper.DLC.NaturalDisastersDLC);
            }

            return s_bNaturalDisastersDlcOwned.Value;
        }
    }
}

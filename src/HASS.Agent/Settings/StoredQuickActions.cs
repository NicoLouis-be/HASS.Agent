﻿using System.IO;
using HASS.Agent.Enums;
using HASS.Agent.Models.Internal;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Shared.Enums;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.Settings
{
    /// <summary>
    /// Handles loading and storing quickactions
    /// </summary>
    internal static class StoredQuickActions
    {
        /// <summary>
        /// Load all stored quickactions
        /// </summary>
        /// <returns></returns>
        internal static bool Load()
        {
            try
            {
                // add an empty list
                Variables.QuickActions = new List<QuickAction>();

                // check for existing file
                if (!File.Exists(Variables.QuickActionsFile))
                {
                    // none yet
                    Log.Information("[SETTINGS_QUICKACTIONS] Config not found, no entities loaded");
                    Variables.MainForm?.SetQuickActionsStatus(ComponentStatus.Stopped);
                    return true;
                }

                // read the content
                var quickActionsRaw = File.ReadAllText(Variables.QuickActionsFile);
                if (string.IsNullOrWhiteSpace(quickActionsRaw))
                {
                    Log.Information("[SETTINGS_QUICKACTIONS] Config empty, no entities loaded");
                    Variables.MainForm?.SetQuickActionsStatus(ComponentStatus.Stopped);
                    return true;
                }

                // deserialize
                Variables.QuickActions = JsonConvert.DeserializeObject<List<QuickAction>>(quickActionsRaw);

                // null-check
                if (Variables.QuickActions == null)
                {
                    Log.Error("[SETTINGS_QUICKACTIONS] Error loading entities: returned null object");
                    Variables.MainForm?.SetQuickActionsStatus(ComponentStatus.Failed);
                    Variables.QuickActions = new List<QuickAction>();
                    return false;
                }

                // all good
                Log.Information("[SETTINGS_QUICKACTIONS] Loaded {count} entities", Variables.QuickActions.Count);
                Variables.MainForm?.SetQuickActionsStatus(ComponentStatus.Ok);
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SETTINGS_QUICKACTIONS] Error loading entities: {err}", ex.Message);
                Variables.MainForm?.ShowMessageBox(string.Format(Languages.StoredQuickActions_Load_MessageBox1, ex.Message), true);

                Variables.MainForm?.SetQuickActionsStatus(ComponentStatus.Failed);
                return false;
            }
        }

        /// <summary>
        /// Store all current quickactions
        /// </summary>
        /// <returns></returns>
        internal static bool Store()
        {
            try
            {
                // check config dir
                if (!Directory.Exists(Variables.ConfigPath))
                {
                    // create
                    Directory.CreateDirectory(Variables.ConfigPath);
                }

                // serialize to file
                var quickActions = JsonConvert.SerializeObject(Variables.QuickActions, Formatting.Indented);
                File.WriteAllText(Variables.QuickActionsFile, quickActions);

                // done
                Log.Information("[SETTINGS_QUICKACTIONS] Stored {count} entities", Variables.QuickActions.Count);
                Variables.MainForm?.SetQuickActionsStatus(ComponentStatus.Ok);
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SETTINGS_QUICKACTIONS] Error storing entities: {err}", ex.Message);
                Variables.MainForm?.ShowMessageBox(string.Format(Languages.StoredQuickActions_Store_MessageBox1, ex.Message), true);

                Variables.MainForm?.SetQuickActionsStatus(ComponentStatus.Failed);
                return false;
            }
        }
    }
}

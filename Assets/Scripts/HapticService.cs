using System.Collections.Generic;
using Solo.MOST_IN_ONE;
using UnityEngine;
using ModularFW.Core.Locator;
using ModularFW.Core.SaveSystem;

namespace ModularFW.Core.HapticService
{
    public class HapticService : IService
    {
        private bool hapticEnabled = true;
        public bool HapticEnabled => hapticEnabled;

        public void SetHapticEnabled(bool enabled)
        {
            hapticEnabled = enabled;
            SaveSettings();
        }

        public void Initialize()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = SaveLoadService.Instance.GetData<SettingsData>(DataKey.Settings);
            if (settings != null)
                hapticEnabled = settings.HapticEnabled;
        }

        private void SaveSettings()
        {
            var settings = SaveLoadService.Instance.GetData<SettingsData>(DataKey.Settings) ?? new SettingsData();
            settings.HapticEnabled = hapticEnabled;
            SaveLoadService.Instance.Save(DataKey.Settings, settings, true);
        }

        public static HapticService Instance => SystemLocator.Instance.HapticService;

        public bool IsReady { get; private set; }

        private Dictionary<HapticType, Most_HapticFeedback.HapticTypes> _hapticFeedback =
            new Dictionary<HapticType, Most_HapticFeedback.HapticTypes>()
            {
                { HapticType.Success, Most_HapticFeedback.HapticTypes.Success },
                { HapticType.Warning, Most_HapticFeedback.HapticTypes.Warning },
                { HapticType.Failure, Most_HapticFeedback.HapticTypes.Failure },
            };

        public void PlayHaptic(HapticType hapticType)
        {
            if (!hapticEnabled) return;
            Most_HapticFeedback.Generate(_hapticFeedback[hapticType]);
        }
    }

    public enum HapticType
    {
        Success,
        Failure,
        Warning,
    }
}

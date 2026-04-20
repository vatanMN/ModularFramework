using System.Collections.Generic;

namespace ModularFW.Core.SaveSystem
{
    public class UnlockSaveData : ISaveData
    {
        public List<string> UnlockedDrops;

        public string GetSaveable()
        {
            string saveable = "";
            foreach (var dropId in UnlockedDrops)
            {
                saveable += dropId;
                if (dropId != UnlockedDrops[^1]) saveable += "/";
            }
            return saveable;
        }

        public void LoadData(string saved)
        {
            var segments = saved.Split('/');
            UnlockedDrops = new List<string>();
            foreach (var segment in segments)
            {
                if (string.IsNullOrEmpty(segment)) continue;
                UnlockedDrops.Add(segment);
            }
        }

        public void Update<T>(T input) where T : ISaveData
        {
            var typedInput = input as UnlockSaveData;
            var incomingDrops = new List<string>(typedInput.UnlockedDrops);
            foreach (var drop in incomingDrops)
            {
                if (!UnlockedDrops.Contains(drop))
                    UnlockedDrops.Add(drop);
            }
        }
    }
}

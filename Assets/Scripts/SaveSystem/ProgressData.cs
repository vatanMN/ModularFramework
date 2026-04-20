using System.Collections.Generic;

namespace ModularFW.Core.SaveSystem
{
    public class ProgressData : ISaveData
    {
        public int ActiveLevel;
        public int PlayerLevel;
        public int XP;

        public string GetSaveable()
        {
            return ActiveLevel + "_" + PlayerLevel + "_" + XP;
        }

        public void LoadData(string saved)
        {
            var parts = saved.Split('_');
            if (parts.Length < 3)
            {
                ActiveLevel = 1;
                PlayerLevel = 0;
                XP = 0;
                return;
            }
            ActiveLevel = int.Parse(parts[0]);
            PlayerLevel = int.Parse(parts[1]);
            XP = int.Parse(parts[2]);
        }

        public void Update<T>(T input) where T : ISaveData
        {
            var typedInput = input as ProgressData;
            PlayerLevel = typedInput.PlayerLevel;
            XP = typedInput.XP;
            ActiveLevel = typedInput.ActiveLevel;
        }
    }
}

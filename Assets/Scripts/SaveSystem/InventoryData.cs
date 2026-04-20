using System.Collections.Generic;

namespace ModularFW.Core.SaveSystem
{
    public class InventoryData : ISaveData
    {
        public Dictionary<int, int> OwnItems = new Dictionary<int, int>();

        public string GetSaveable()
        {
            var serialized = "";
            foreach (var item in OwnItems)
                serialized += item.Key + ":" + item.Value + ";";
            return serialized;
        }

        public void LoadData(string saved)
        {
            string[] entries = saved.Split(";");
            foreach (var entry in entries)
            {
                if (entry.Length > 0)
                {
                    string[] parts = entry.Split(":");
                    OwnItems[int.Parse(parts[0])] = int.Parse(parts[1]);
                }
            }
        }

        public void Update<T>(T input) where T : ISaveData
        {
            var typedInput = input as InventoryData;
            var incomingItems = new Dictionary<int, int>(typedInput.OwnItems);
            foreach (var item in incomingItems)
                OwnItems[item.Key] = item.Value;
        }
    }
}

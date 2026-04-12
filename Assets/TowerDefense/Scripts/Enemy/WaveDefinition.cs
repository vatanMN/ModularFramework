using System.Collections.Generic;
using UnityEngine;

namespace MiniGame.TowerDefense {
[System.Serializable]
public class WaveDefinition
{
    public List<int> ModelIds = new List<int>();
    public int Count = 10;
}
}

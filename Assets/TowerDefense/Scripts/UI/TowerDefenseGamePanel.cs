using UnityEngine;
using ModularFW.Core.PanelSystem;

namespace MiniGame.TowerDefense {
public class TowerDefenseGamePanel : BasePanel
{
    public override PanelType PanelType => PanelType.TowerDefenseGamePanel;

    public TowerDefenseEngine Engine;
}
}

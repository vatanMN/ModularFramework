using UnityEngine;

public class LoadingPanel : BasePanel
{
    public override PanelType PanelType => PanelType.LoadingPanel;

    public override void Show(params object[] parameters)
    {
        base.Show(parameters);
    }

    public override void Hide()
    {
        base.Hide();
    }
}

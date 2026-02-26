using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] Button Button;
    // Start is called before the first frame update
    void OnEnable()
    {
        Button.onClick.AddListener(() => {
            if (AudioService.Instance != null) AudioService.Instance.Play(AudioEnum.Tick);
            PanelService.Instance.Show(PanelType.InventoryPanel);
        });
    }

    // Update is called once per frame
    void OnDisable()
    {
        Button.onClick.RemoveListener(() => PanelService.Instance.Show(PanelType.InventoryPanel));
    }
}

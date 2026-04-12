using ModularFW.Core.InventorySystem;
using ModularFW.Core.PoolSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ModularFW.Core.PanelSystem {
public class InventoryPanel : BasePanel
{
    [SerializeField] GameObject Layout;
    [SerializeField] Button ResetButton;
    private List<InventoryObject> inventoryObjects = new List<InventoryObject>();

    public override PanelType PanelType => PanelType.InventoryPanel;

    public override void Show(params object[] parameters)
    {
        base.Show(parameters);
        ResetButton.onClick.AddListener(Reset);

        var ownedItems = InventoryService.Instance.GetOwnItems();
        foreach (var item in ownedItems)
        {
            bool isSet = false;
            foreach (var inventoryObject in inventoryObjects)
            {
                if(inventoryObject.Item.Id == item.Key)
                {
                    inventoryObject.UpdateValue(item.Value);
                    isSet = true;
                }
            }
            if (!isSet)
            {
               var newObj = PoolingService.Instance.Create<InventoryObject>(PoolEnum.InventoryObject, Layout.transform);
                newObj.Fill(InventoryService.Instance.GetItemModel(item.Key), item.Value);
                inventoryObjects.Add(newObj);
            }
        }
    }

    public override void Hide()
    {
        ResetButton.onClick.RemoveListener(Reset);
        base.Hide();
    }

    private void Reset()
    {
        InventoryService.Instance.ResetOwnedItems();
        foreach (var item in inventoryObjects)
        {
            PoolingService.Instance.Destroy(PoolEnum.InventoryObject, item.gameObject);
        }
        inventoryObjects.Clear();
        Show();
    }
}
}

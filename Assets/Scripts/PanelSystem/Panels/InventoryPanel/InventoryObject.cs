using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModularFW.Core.PanelSystem {
public class InventoryObject : MonoBehaviour
{
    [SerializeField] Image Icon;
    [SerializeField] TextMeshProUGUI Count;
    public ItemModel Item;

    public void Fill(ItemModel item, int count)
    {
        Item = item;
        Icon.sprite = item.Sprite;
        Count.text = count.ToString();
    }

    public void UpdateValue(int count)
    {
        Count.text = count.ToString();
    }
}
}

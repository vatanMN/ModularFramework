using ModularFW.Core.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

namespace ModularFW.Core.PanelSystem {
    public class GainItemPanel : BasePanel
    {
        [SerializeField] Image Icon;
        [SerializeField] TextMeshProUGUI Count;
        public override PanelType PanelType => PanelType.GainItemPanel;
	
        public override void Show(params object[] parameters)
        {
            base.Show(parameters);
	
            var id = (int)parameters[0];
            var count = InventoryService.Instance.GetOwnItemCount(id);
            Icon.sprite = InventoryService.Instance.GetItemModel(id).Sprite;
            if(count == 1)
            {
                Count.text = "NEW";
            }
            else
            {
                Count.text = (count - 1) + " -> " + count;
            }
        }
    }
    
    }

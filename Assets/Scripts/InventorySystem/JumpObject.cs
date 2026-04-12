using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ModularFW.Core.PoolSystem;

public class JumpObject : MonoBehaviour
{
    [SerializeField] Image Icon;
    private InventoryButton InventoryButton
    {
        get{
            if (inventoryButton == null)
            {
                inventoryButton = FindAnyObjectByType<InventoryButton>();
            }

            return inventoryButton;
        }

    }
    private InventoryButton inventoryButton;

    public void FillAndGo(Sprite sprite, Vector3 startPosition)
    {
        this.transform.position = startPosition;
        Icon.sprite = sprite;
        transform.DOJump(InventoryButton.transform.position, 3f, 1, 1f).OnComplete(() =>
        {
            PoolingService.Instance.Destroy(PoolEnum.JumpObject, gameObject);
            InventoryButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        });
    }
}

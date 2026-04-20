using ModularFW.Core.HapticService;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ModularFW.Core.AudioSystem;
using ModularFW.Core.Signal;

namespace ModularFW.Core.PanelSystem {
public abstract class BasePanel : MonoBehaviour
{
    [SerializeField] protected Button CloseButton;

    [Header("Animation")]
    public bool IsAnimated = true;
    [Range(0f, 2f), Tooltip("Duration of show/hide tween in seconds. Override per panel prefab in the Inspector.")]
    public float AnimationDuration = 0.18f;
    [Range(0.5f, 1.5f), Tooltip("Target scale when fully visible.")]
    public float ShowScale = 1.0f;
    [Range(0f, 1f), Tooltip("Scale when hidden; panel shrinks to this before deactivating.")]
    public float HideScale = 0.9f;

    private CanvasGroup canvasGroup;

    public abstract PanelType PanelType { get; }
    public virtual void Show(params object[] parameters)
    {
        gameObject.SetActive(true);
        CloseButton?.onClick.AddListener(Hide);

        if (IsAnimated)
        {
            if (canvasGroup == null) canvasGroup = GetOrAddCanvasGroup();
            canvasGroup.alpha = 0f;
            transform.localScale = Vector3.one * HideScale;
            canvasGroup.DOFade(1f, AnimationDuration).SetEase(Ease.OutSine);
            transform.DOScale(Vector3.one * ShowScale, AnimationDuration).SetEase(Ease.OutBack);
        }
    }

    public virtual void Hide()
    {
        CloseButton?.onClick.RemoveListener(Hide);

        if (IsAnimated)
        {
            if (AudioService.Instance != null)
                AudioService.Instance.Play(AudioEnum.Tick);
            if (ModularFW.Core.HapticService.HapticService.Instance != null)
                ModularFW.Core.HapticService.HapticService.Instance.PlayHaptic(HapticType.Success);
            if (canvasGroup == null) canvasGroup = GetOrAddCanvasGroup();
            // fade and scale down then deactivate
            canvasGroup.DOFade(0f, AnimationDuration).SetEase(Ease.InSine);
            transform.DOScale(Vector3.one * HideScale, AnimationDuration).SetEase(Ease.InBack).OnComplete(() => {
                gameObject.SetActive(false);
                SignalBus.Instance.Publish(new PanelHiddenSignal() { PanelType = PanelType });
            });
        }
        else
        {
            gameObject.SetActive(false);
            SignalBus.Instance.Publish(new PanelHiddenSignal() { PanelType = PanelType });
        }
    }

    protected virtual void OnDisable()
    {
        transform.DOKill();
        if (canvasGroup != null)
            canvasGroup.DOKill();
    }

    private CanvasGroup GetOrAddCanvasGroup()
    {
        var group = GetComponent<CanvasGroup>();
        if (group == null) group = gameObject.AddComponent<CanvasGroup>();
        return group;
    }
}
}

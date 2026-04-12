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
    public float AnimationDuration = 0.18f;
    public float ShowScale = 1.0f;
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

    private CanvasGroup GetOrAddCanvasGroup()
    {
        var cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
        return cg;
    }
}
}

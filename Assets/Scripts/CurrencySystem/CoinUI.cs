using ModularFW.Core.PoolSystem;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ModularFW.Core.Signal;

namespace ModularFW.Core.CurrencySystem {
public class CoinUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI CoinsText;

    private Canvas parentCanvas;

    void Start()
    {
        if (CoinsText == null) CoinsText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        CoinsText.text = CurrencyService.Instance != null ? CurrencyService.Instance.GetCoins().ToString() : "0";
        lastCoins = CurrencyService.Instance != null ? CurrencyService.Instance.GetCoins() : 0;
          SignalBus.Instance.Subscribe<CoinsChangedSignal>(OnCoinsChangedSignal);
        parentCanvas = GetComponentInParent<Canvas>();
    }

    void OnEnable()
    {
        if (CurrencyService.Instance != null) CurrencyService.Instance.RegisterCoinUI(this);
    }

    void OnDisable()
    {
        if (CurrencyService.Instance != null) CurrencyService.Instance.UnregisterCoinUI(this);
    }

    private int lastCoins = 0;
    private Tween countTween;
    private void OnCoinsChangedSignal(CoinsChangedSignal s)
    {
        if (CoinsText == null) return;
        int newCoins = s.Coins;
        if (countTween != null && countTween.IsActive()) countTween.Kill();
        int startValue = lastCoins;
        lastCoins = newCoins;
        if (startValue == newCoins)
        {
            CoinsText.text = newCoins.ToString();
            return;
        }
        countTween = DOTween.To(() => startValue, x => {
            CoinsText.text = x.ToString();
        }, newCoins, 0.5f).SetEase(Ease.OutQuad);
    }

    void OnDestroy()
    {
        SignalBus.Instance.Unsubscribe<CoinsChangedSignal>(OnCoinsChangedSignal);
    }

    // Spawn a flying coin animation from a world position to this UI coin target.
    public void SpawnFlyingCoin(Vector3 worldPosition, int amount = 1)
    {

        if (parentCanvas == null) return;
        Image fly = PoolingService.Instance.Create<Image>(PoolEnum.FlyingCoin, parentCanvas.transform);

        fly.transform.position = worldPosition;
        fly.transform.DOJump(transform.position + Vector3.left * 50f, 3f, 1, 1f).OnComplete(() =>
        {
            PoolingService.Instance.Destroy(PoolEnum.FlyingCoin, fly.gameObject);
        }); 
    }
}
}

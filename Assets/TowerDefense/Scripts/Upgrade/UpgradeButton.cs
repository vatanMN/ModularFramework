using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public TowerDefenseEngine.TowerUpgradeType Type;
    public Button Button;
    public TMPro.TextMeshProUGUI CostText;
    public TMPro.TextMeshProUGUI LevelText;
    public Color InsufficientColor = Color.red;
    public float FlashDuration = 0.5f;
    private Image buttonImage;
    private Color originalColor;
    bool isStarted = true;

    void Start()
    {
        if (Button == null) Button = GetComponent<Button>();
        if (Button != null) Button.onClick.AddListener(OnClick);
        if (Button != null) buttonImage = Button.GetComponent<Image>();
        if (buttonImage != null) originalColor = buttonImage.color;
        SignalBus.Instance.Subscribe<CoinsChangedSignal>(OnCoinsChangedSignal);
        SignalBus.Instance.Subscribe<UpgradesResetSignal>(OnUpgradesReset);
        UpdateInteractable(CurrencyService.Instance != null ? CurrencyService.Instance.GetCoins() : 0);
        isStarted = true;
    }
    void OnEnabled()
    {
        if (!isStarted) return;
        UpdateInteractable(CurrencyService.Instance != null ? CurrencyService.Instance.GetCoins() : 0);
    }

    private void OnClick()
    {
        if (AudioService.Instance != null) AudioService.Instance.Play(AudioEnum.Tick);
        var engine = FindObjectOfType<TowerDefenseEngine>();
        if (engine == null) return;
        // delegate upgrade attempt to engine (engine will spend currency)
        var ok = engine.TryUpgrade(Type);
        if (!ok) 
        {
            StartCoroutine(FlashInsufficient());
            if (HapticService.Instance != null) HapticService.Instance.PlayHaptic(HapticType.Failure);
        }
        else
        {
            // update interactable state after successful upgrade
            UpdateInteractable(CurrencyService.Instance != null ? CurrencyService.Instance.GetCoins() : 0);
        }
    }

    private void UpdateInteractable(int coins)
    {
        if (Button == null) return;
        var engine = FindObjectOfType<TowerDefenseEngine>();
        if (engine == null) { Button.interactable = false; return; }
        int cost = engine.GetUpgradeCost(Type);
        Button.interactable = coins >= cost;
        if (CostText != null) CostText.text = cost.ToString();
        if (LevelText != null) LevelText.text = (engine.GetUpgradeLevel(Type)+1).ToString();
    }

    private System.Collections.IEnumerator FlashInsufficient()
    {
        if (buttonImage == null) yield break;
        float t = 0f;
        buttonImage.color = InsufficientColor;
        while (t < FlashDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }
        buttonImage.color = originalColor;
    }

    void OnDestroy()
    {
        SignalBus.Instance.Unsubscribe<CoinsChangedSignal>(OnCoinsChangedSignal);
        SignalBus.Instance.Unsubscribe<UpgradesResetSignal>(OnUpgradesReset);
    }

    private void OnCoinsChangedSignal(CoinsChangedSignal s)
    {
        UpdateInteractable(s.Coins);
    }

    private void OnUpgradesReset(UpgradesResetSignal s)
    {
        UpdateInteractable(CurrencyService.Instance != null ? CurrencyService.Instance.GetCoins() : 0);
    }
}

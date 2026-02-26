using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TowerHealthUI : MonoBehaviour
{
    public TowerDefenseEngine Engine;
    public Slider HealthSlider;
    public TMPro.TextMeshProUGUI HealthText;
    public Image FillImage;
    public Image BackgroundImage;

    public Color HighColor = Color.green;
    public Color MidColor = Color.yellow;
    public Color LowColor = Color.red;


    public Color TakeDamageColor = Color.red;

    private Color originalBgColor = Color.white;
    private Vector3 baseScale;

    void Start()
    {
        if (HealthSlider != null) baseScale = HealthSlider.transform.localScale;
        if (BackgroundImage != null) originalBgColor = BackgroundImage.color;
        // try to auto-find fill image
        if (FillImage == null && HealthSlider != null && HealthSlider.fillRect != null)
            FillImage = HealthSlider.fillRect.GetComponent<Image>();
    }

    void Update()
    {
        if (Engine == null || HealthSlider == null || HealthText == null) return;
        float max = Engine.GetTowerMaxHealth();
        float cur = Engine.GetTowerHealth();
        if (max <= 0f) max = 1f;
        HealthSlider.maxValue = max;
        HealthSlider.value = Mathf.Clamp(cur, 0f, max);
        HealthText.text = string.Format("{0}/{1}", Mathf.CeilToInt(cur), Mathf.CeilToInt(max));

        // update fill color based on percent
        float pct = (max <= 0f) ? 0f : (cur / max);
        if (FillImage != null)
        {
            if (pct > 0.75f) FillImage.color = HighColor;
            else if (pct > 0.25f) FillImage.color = MidColor;
            else FillImage.color = LowColor;
        }
    }

    // Called when tower takes damage to show UI feedback
    public void PlayDamageFeedback(float damage)
    {
        if (HealthSlider == null) return;
        // small boing on slider
        HealthSlider.transform.DOKill();
        HealthSlider.transform.DOScale(baseScale * 1.07f, 0.08f).SetLoops(1, LoopType.Yoyo).SetEase(Ease.OutSine);

        // flash background red
        if (BackgroundImage != null)
        {
            BackgroundImage.DOKill();
            BackgroundImage.DOColor(TakeDamageColor, 0.08f).OnComplete(() => BackgroundImage.DOColor(originalBgColor, 0.18f));
        }
    }
}

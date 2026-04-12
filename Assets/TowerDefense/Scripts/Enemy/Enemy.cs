using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using ModularFW.Core.HapticService;
using ModularFW.Core.PoolSystem;
using ModularFW.Core.CurrencySystem;

namespace MiniGame.TowerDefense {
public enum EnemyType { Basic, Fast, Tank }

public class Enemy : MonoBehaviour{
    public SpawnManager SpawnManager;

    public EnemyType Type = EnemyType.Basic;
    public float Speed = 1f;
    public float Health = 20f;
    public float Damage = 10f;
    public int Reward = 1;
    private Transform target;
    private Tween boingTween;
    private Vector3 baseScale;
    private float hitRadius = 50f;
    private SpriteRenderer sr;
    private Image uiImage;
    private Color originalColor = Color.white;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        uiImage = GetComponent<Image>();
        if (uiImage == null) uiImage = GetComponentInChildren<Image>();
        if (sr != null) originalColor = sr.color;
        else if (uiImage != null) originalColor = uiImage.color;
        baseScale = transform.localScale;
    }

    public void StartBoing(float intensity = 0.05f, float period = 0.5f)
    {
        if (boingTween != null) boingTween.Kill();
        baseScale = transform.localScale;
        float targetScale = 1f + intensity;
        boingTween = transform.DOScale(baseScale * targetScale, period).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public void StopBoing()
    {
        if (boingTween != null) boingTween.Kill();
        boingTween = null;
        transform.localScale = baseScale;
    }

    void Update()
    {
        if (target == null) return;
        Vector3 dir = (target.position - transform.position).normalized;
        if (dir.sqrMagnitude > 0.0001f)
        {
            transform.up = dir; // face the tower (assumes sprite 'up' points forward)
        }
        transform.position += dir * Speed * Time.deltaTime;
    
        // check if we reached the tower
        if (Vector3.Distance(transform.position, target.position) <= hitRadius)
        {
            HitTower();
        }
    }

    public void ApplyDamage(float dmg)
    {
        // blink red on hit
        if (sr != null)
        {
            sr.DOKill();
            sr.DOColor(Color.red, 0.06f).OnComplete(() => sr.DOColor(originalColor, 0.15f));
        }
        else if (uiImage != null)
        {
            uiImage.DOKill();
            uiImage.DOColor(Color.red, 0.06f).OnComplete(() => uiImage.DOColor(originalColor, 0.15f));
        }

        Health -= dmg;
        if (Health <= 0f)
        {
            StartDeathSequence();
        }
    }

    // Called when reused from pool to reset transient state
    public void ResetForReuse()
    {
        transform.DOKill();
        if (boingTween != null) { boingTween.Kill(); boingTween = null; }
        if (baseScale == Vector3.zero) baseScale = Vector3.one;
        transform.localScale = baseScale;
        if (sr != null) sr.color = originalColor;
        if (uiImage != null) uiImage.color = originalColor;
        target = null;
    }

    private void StartDeathSequence()
    {
        StopBoing();
        if (SpawnManager != null)
            SpawnManager.NotifyEnemyDestroyed(this);
        // disable colliders so it doesn't interact while dying
        var col2d = GetComponent<Collider2D>(); if (col2d != null) col2d.enabled = false;
        var col = GetComponent<Collider>(); if (col != null) col.enabled = false;

        // shrink then give reward and return to pool or destroy
        transform.DOKill();
        GameObject go = this.gameObject;
        transform.DOScale(Vector3.zero, 0.18f).SetEase(Ease.InBack).OnComplete(() => {
            // give reward
            if (CurrencyService.Instance != null)
            {
                CurrencyService.Instance.AddCoins(Reward);
                // spawn flying coin visual if UI is registered
                var coinUI = CurrencyService.Instance.GetActiveCoinUI();
                if (coinUI != null)
                {
                    coinUI.SpawnFlyingCoin(transform.position, Reward);
                }
            }
            // haptic feedback for enemy destroyed
            if (HapticService.Instance != null) HapticService.Instance.PlayHaptic(HapticType.Success);
            if (PoolingService.Instance != null)
            {
                PoolingService.Instance.Destroy(PoolEnum.Enemy, go);
            }
            else
            {
                Destroy(go);
            }
        });
    }

    private void Die()
    {
        // kept for legacy calls; use StartDeathSequence for animated death
        if (CurrencyService.Instance != null)
        {
            CurrencyService.Instance.AddCoins(Reward);
        }
        StopBoing();
        Destroy(this.gameObject);
    }

    private void HitTower()
    {
        StopBoing();
        if (SpawnManager != null)
        {
            SpawnManager.NotifyEnemyDestroyed(this);
            var engine = SpawnManager.Engine;
            if (engine != null)
            {
                engine.ApplyDamageToTower(Damage);
            }
        }
        
        // haptic for tower hit
        if (HapticService.Instance != null) HapticService.Instance.PlayHaptic(HapticType.Failure);
        if (PoolingService.Instance != null)
        {
            PoolingService.Instance.Destroy(PoolEnum.Enemy, this.gameObject);
        }
        else Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        if (boingTween != null) boingTween.Kill();
    }
}
}

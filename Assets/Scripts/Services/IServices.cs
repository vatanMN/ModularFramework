using System;
using System.Collections.Generic;
using UnityEngine;
using ModularFW.Core.AudioSystem;
using ModularFW.Core.CurrencySystem;
using ModularFW.Core.PanelSystem;
using ModularFW.Core.PoolSystem;
using ModularFW.Core.SaveSystem;
using ModularFW.Core.HapticService;

namespace ModularFW.Core
{
    public interface IAudioService
    {
        bool AudioEnabled { get; }
        float MasterVolume { get; }
        float SfxVolume { get; }
        float MusicVolume { get; }
        void Play(AudioEnum audioType);
        void SetAudioEnabled(bool enabled);
        void SetMasterVolume(float volume);
        void SetSfxVolume(float volume);
        void SetMusicVolume(float volume);
    }

    public interface ICurrencyService
    {
        int GetCoins();
        void AddCoins(int amount);
        bool TrySpend(int amount);
        CoinUI GetActiveCoinUI();
        void RegisterCoinUI(CoinUI coinUI);
        void UnregisterCoinUI(CoinUI coinUI);
    }

    public interface IInventoryService
    {
        bool isOwned(int id);
        void GainItem(int id, int count);
        int GetOwnItemCount(int id);
    }

    public interface IPanelService
    {
        void Show(PanelType panelType, params object[] parameters);
        void Hide(PanelType panelType);
    }

    public interface IPoolingService
    {
        T Create<T>(PoolEnum poolEnum, Transform parent) where T : MonoBehaviour;
        void Destroy(PoolEnum poolEnum, GameObject gameObject);
    }

    public interface ISignalBus
    {
        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
        IDisposable SubscribeTracked<T>(Action<T> handler);
        void Publish<T>(T signal);
    }

    public interface IHapticService
    {
        bool HapticEnabled { get; }
        void SetHapticEnabled(bool enabled);
        void PlayHaptic(HapticType hapticType);
    }

    public interface ISaveLoadService
    {
        T GetData<T>(DataKey key) where T : ISaveData;
        void Save<T>(DataKey key, T data, bool immediate = false) where T : ISaveData;
        void Shutdown();
    }

    public interface IAnalyticsService
    {
        TimeSpan GetSessionDuration();
        void TrackEvent(string eventName, Dictionary<string, object> parameters = null);
        void TrackGameStart(string gameId);
        void TrackGameEnd(string gameId, string result);
        void TrackError(string message);
    }
}

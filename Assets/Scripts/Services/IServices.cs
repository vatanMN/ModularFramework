using System;
using UnityEngine;
using ModularFW.Core.AudioSystem;
using ModularFW.Core.CurrencySystem;
using ModularFW.Core.PanelSystem;
using ModularFW.Core.PoolSystem;

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
}

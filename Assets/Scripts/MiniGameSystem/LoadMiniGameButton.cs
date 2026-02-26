using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadMiniGameButton : MonoBehaviour
{

    public MiniGameType MiniGameType;
    [SerializeField] Button Button;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (Button != null) Button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void OnDisable()
    {
        if (Button != null) Button.onClick.RemoveListener(OnClick);
    }

    void OnClick()
    {
        if (AudioService.Instance != null) AudioService.Instance.Play(AudioEnum.Tick);
        MiniGameService.Instance.LoadMiniGame(MiniGameType);
    }
}

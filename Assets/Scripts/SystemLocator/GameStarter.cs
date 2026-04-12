using UnityEngine;

namespace ModularFW.Core.Locator
{
    public class GameStarter : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SystemLocator.Instance.enabled = true;
        }
    }
}

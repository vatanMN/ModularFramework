
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModularFW.Core.PoolSystem;

namespace ModularFW.Core.PoolSystem {

public class ParticlePoolObject : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    public PoolEnum PoolEnum;
    // Start is called before the first frame update
    void OnEnable()
    {
        var main = ParticleSystem.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        PoolingService.Instance.Destroy(PoolEnum, this.gameObject);
    }
}

}

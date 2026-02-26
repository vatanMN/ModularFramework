using UnityEngine;
using UnityEngine.UI;

public class TowerRangeVisualizer : MonoBehaviour
{
    public TowerDefenseEngine Engine;
    public Image RangeImage; // should be on a World Space Canvas or an Image with world-space transform
    public float ReferenceRadius = 1f; // radius (world units) represented by scale = 1 on the image
    public bool AutoFindEngine = true;

    void Start()
    {
        if (Engine == null && AutoFindEngine)
            Engine = FindObjectOfType<TowerDefenseEngine>();

        if (RangeImage == null)
            RangeImage = GetComponent<Image>();
    }

    void Update()
    {
        if (Engine == null || Engine.TowerPoint == null || RangeImage == null) { if (RangeImage != null) RangeImage.enabled = false; return; }
        float range = Engine.GetTowerRange();
        if (range <= 0f) { RangeImage.enabled = false; return; }
        RangeImage.enabled = true;

        // position the image at the tower's point
        RangeImage.transform.position = Engine.TowerPoint.position;

        // compute desired scale so that the image radius equals 'range'
        float refR = Mathf.Max(0.0001f, ReferenceRadius);
        float scale = range / refR;
        RangeImage.transform.localScale = new Vector3(scale, scale, 1f);
    }
}

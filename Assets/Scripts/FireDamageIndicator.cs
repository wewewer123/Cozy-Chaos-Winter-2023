using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class FireDamageIndicator : MonoBehaviour
{
    // [SerializeField] private GameObject effect;
    [SerializeField] private float effectTransationTime = 0.25f;

    [Header("Shader Property values")]
    [SerializeField] private float intensityMinValue = 0.5f;
    [SerializeField] private float intensityMaxValue = 1.0f;
    [SerializeField] private float voronoiPowerMinValue = 0.8f;
    [SerializeField] private float voronoiPowerMaxValue = 1.8f;

    private GameObject effect;
    private static string effectName = "FireDamageIndicator";
    private Material _effectMaterial;
    private static int voronoiPowerCached = Shader.PropertyToID("_VoronoiPower");
    private static int intensityCached = Shader.PropertyToID("_Intensity");

    private void Awake()
    {
        effect = Camera.main.transform.Find(effectName).gameObject;
        if (effect == null)
        {
            Debug.LogError("FireDamageIndicator: effect not found should be added under the main camera! Please get it from another level scene.");
            Destroy(gameObject); // Destroying the whole object to make sure whatever dummy is making this level is aware of the issue
        }
        _effectMaterial = effect.GetComponent<SpriteRenderer>().material;
        if (_effectMaterial == null)
        {
            Debug.LogError("FireDamageIndicator: effect material not found! Please readd it from another level scene.");
            Destroy(gameObject); // Destroying the whole object to make sure whatever dummy is making this level is aware of the issue
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _effectMaterial.DOKill();
            _effectMaterial.SetFloat(intensityCached, intensityMaxValue);
            _effectMaterial.SetFloat(voronoiPowerCached, voronoiPowerMaxValue);
            effect.SetActive(true);
            _effectMaterial.DOFloat(intensityMinValue, intensityCached, effectTransationTime);
            _effectMaterial.DOFloat(voronoiPowerMinValue, voronoiPowerCached, effectTransationTime);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _effectMaterial.DOKill();
            effect.SetActive(false);
            _effectMaterial.DOFloat(intensityMaxValue, intensityCached, effectTransationTime);
            _effectMaterial.DOFloat(voronoiPowerMaxValue, voronoiPowerCached, effectTransationTime);
        }
    }
}

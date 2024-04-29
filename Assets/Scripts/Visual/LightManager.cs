using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    #region Variables
    private MapManager _mm;

    [SerializeField] private GameObject _spotlightPrefab;
    private Light2D _globalLight;
    private float _dayNightDuration = 400.0f;
    private float _startTime;
    [SerializeField] private Gradient _gradient;

    // Struct to store light variation data
    public struct LightVariation
    {
        public float baseIntensity;
        public float baseRadius;
        public float timeMultiplier;
        public float intensityVariation;
        public float radiusVariation;
        public float timeVariation;
    }

    // Dictionary to store light variations for each spotlight
    private Dictionary<GameObject, LightVariation> _lightVariations = new Dictionary<GameObject, LightVariation>();
    #endregion

    #region UnityMethods
    private void Start()
    {
        _mm = FindAnyObjectByType<MapManager>();
        _globalLight = FindAnyObjectByType<Light2D>();
        _startTime = Time.time;
        _lightVariations.Clear();

        AddLightsToBuildings();
    }

    void Update()
    {
        // Loop through each light and variation pair
        foreach (var lightAndVariation in _lightVariations)
        {
            GameObject spotlight = lightAndVariation.Key;
            LightVariation variation = lightAndVariation.Value;

            float timeOffset = Time.time * variation.timeMultiplier;

            // Calculate oscillating values based on the stored base values
            float currentIntensity = variation.baseIntensity + Mathf.Sin(timeOffset) * variation.intensityVariation;
            float currentRadius = variation.baseRadius + Mathf.Cos(timeOffset) * variation.radiusVariation;

            spotlight.GetComponent<Light2D>().intensity = currentIntensity;
            spotlight.GetComponent<Light2D>().pointLightInnerRadius = currentRadius;
        }

        VaryGlobalLight();
    }
    #endregion

    #region Methods
    // Create spotlight at given position with random variations
    public void InstantiateSpotLight(Vector3Int position)
    {
        if (_spotlightPrefab != null)
        {
            Vector3 worldPosition = _mm.Map.CellToWorld(position) + new Vector3(0.5f, 0.5f, 0.0f);
            GameObject newLight = Instantiate(_spotlightPrefab, worldPosition, Quaternion.identity);

            newLight.transform.parent = FindAnyObjectByType<LightManager>().transform; // Set parent

            // Generate random variations for the light
            LightVariation variation = new()
            {
                baseIntensity = Mathf.Clamp(Random.value, 0.7f, 1.2f),
                baseRadius = Mathf.Clamp(Random.value, 0.6f, 1.0f),
                intensityVariation = Mathf.Clamp(Random.value, 0.2f, 0.45f),
                radiusVariation = Mathf.Clamp(Random.value, 0.07f, 0.1f),
                timeMultiplier = Mathf.Clamp(Random.value, 0.6f, 0.8f)
        };

            // Add light and its variation to the dictionary
            _lightVariations.Add(newLight, variation);
        }
        else
        {
            Debug.LogError("Spot light prefab is not assigned!");
        }
    }

    // Instantiate a spotlight wherever there's a building
    public void AddLightsToBuildings()
    {
        foreach (var pos in _mm.Map.cellBounds.allPositionsWithin)
        {
            TerrainDataSO posTile = _mm.GetTileData(pos);
            if (posTile != null && posTile.TerrainType == ETerrains.Building)
            {
                InstantiateSpotLight(pos);
            }
        }
    }

    // Day and night logic
    public void VaryGlobalLight()
    {
        float timeElapsed = Time.time - _startTime;
        float percentage = Mathf.Sin(timeElapsed / _dayNightDuration * Mathf.PI * 2) * 0.5f + 0.5f;
        percentage = Mathf.Clamp01(percentage);

        _globalLight.color = _gradient.Evaluate(percentage);
    }
    #endregion
}
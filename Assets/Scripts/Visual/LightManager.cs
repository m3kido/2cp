using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    #region Variables
    private MapManager _mm;

    [SerializeField] private GameObject _spotlightPrefab;
    private Light2D _globalLight;

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
                baseIntensity = Mathf.Clamp(Random.value, 0.8f, 1.2f),
                baseRadius = Mathf.Clamp(Random.value, 0.7f, 1.0f),
                intensityVariation = Mathf.Clamp(Random.value, 0.2f, 0.45f),
                radiusVariation = Mathf.Clamp(Random.value, 0.1f, 0.2f),
                timeMultiplier = Mathf.Clamp(Random.value, 0.6f, 1.0f)
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
        // Total duration (seconds) of the day/night cycle
        float dayNightLength = 240.0f;

        // Speed of day cycle
        float timeMultiplier = 2.0f;

        // Normalized time within the day/night cycle
        float timeOfDay = Mathf.Repeat(Time.time * timeMultiplier, dayNightLength) / dayNightLength;

        // Define day and night intensity and color thresholds
        float dayIntensity = 1.0f;
        float nightIntensity = 0.45f;
        Color nightColor = new(159f / 255f, 111f / 255f, 229f / 255f);
        Color dayColor = Color.white;
        Color dawnColor = new(255f / 255f, 185f / 255f, 129f / 255f);
        Color duskColor = new(241f / 255f, 121f / 255f, 128f / 255f);

        // Define dawn and dusk intervals
        float dawnStart = 0.2f;
        float dawnEnd = 0.4f;
        float duskStart = 0.6f;
        float duskEnd = 0.8f;

        // Calculate intensity and color based on time of day
        float globalIntensity;
        Color globalColor;

        if (timeOfDay < dawnStart)
        {
            // Night time before dawn starts
            globalIntensity = nightIntensity;
            globalColor = nightColor;
        }
        else if (timeOfDay < dawnEnd)
        {
            // Gradually increase intensity and transition to dawn color during dawn
            float dawnProgress = Mathf.InverseLerp(dawnStart, dawnEnd, timeOfDay);
            float dawnEase = Mathf.Pow(dawnProgress, 4.0f); // Ease-in using power function
            globalIntensity = Mathf.Lerp(nightIntensity, dayIntensity, dawnEase);
            globalColor = Color.Lerp(nightColor, dawnColor, dawnEase);
        }
        else if (timeOfDay < duskStart)
        {
            // Day time, maintain full day intensity and color
            globalIntensity = dayIntensity;
            globalColor = dayColor;
        }
        else if (timeOfDay < duskEnd)
        {
            // Gradually decrease intensity and transition to dusk color during dusk
            float duskProgress = Mathf.InverseLerp(duskStart, duskEnd, timeOfDay);
            float duskEase = 1.0f - Mathf.Pow(1.0f - duskProgress, 4.0f); // Ease-out using power function
            globalIntensity = Mathf.Lerp(dayIntensity, nightIntensity, duskEase);
            globalColor = Color.Lerp(dayColor, duskColor, duskEase);
        }
        else
        {
            // Maintain night intensity and color after dusk ends
            globalIntensity = nightIntensity;
            globalColor = nightColor;
        }

        // Apply intensity and color
        _globalLight.intensity = globalIntensity;
        _globalLight.color = globalColor;
    }
    #endregion
}
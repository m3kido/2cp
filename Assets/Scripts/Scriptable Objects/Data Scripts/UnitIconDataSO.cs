using UnityEngine;

[CreateAssetMenu(fileName = "UnitIcons", menuName = "Unit Icons")]
public class UnitIconDataSO : ScriptableObject
{
    // Icon prefabs
    [SerializeField] private GameObject _H; // Health icon
    [SerializeField] private Sprite[] _healthIcons;
    [SerializeField] private GameObject _P; // In need of provisions icon
    [SerializeField] private GameObject _E; // In need of energy icon
    [SerializeField] private GameObject _C; // Capturing icon
    [SerializeField] private GameObject _L; // Loading icon

    public GameObject H => _H;
    public Sprite[] HealthIcons => _healthIcons;
    public GameObject P => _P;
    public GameObject E => _E;
    public GameObject C => _C;
    public GameObject L => _L;
}

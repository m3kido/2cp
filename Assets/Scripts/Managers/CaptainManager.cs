using System.Collections.Generic;
using UnityEngine;


public class CaptainManager : MonoBehaviour
{
    GameManager _gm;
    public static List<Captain> CaptainList=new();

    [SerializeField] List<CaptainData> _captainSOList = new();



    private void Awake()
    {
        _gm = FindObjectOfType<GameManager>();
        CaptainList.Add(new Captain(_captainSOList[0]));
        CaptainList.Add(new Captain(_captainSOList[1]));

    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

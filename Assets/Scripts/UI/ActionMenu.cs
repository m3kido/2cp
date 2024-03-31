using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour
{
    CursorController Cc;
    GameManager Gm;
    UnitManager Um;
    BuildingManager Bm;
    AttackManager Am;

    public Sprite Cursor;
    public GameObject Options;
    List<GameObject> OptionsList;
    int SelectedOption;

    [SerializeField] GameObject WaitOption;
    [SerializeField] GameObject AttackOption;
    //public GameObject FireOption;
    // public GameObject CaptureOption;

    private GameObject WaitOptionInstance;
    private GameObject AttackOptionInstance;

    private AttackingUnit attacker;

    //public GameObject FireOption;
    //public GameObject CaptureOption;
    private void Awake()
    {
        Cc = FindAnyObjectByType<CursorController>();
        Um = FindAnyObjectByType<UnitManager>();
        Gm = FindAnyObjectByType<GameManager>();
        Bm = FindAnyObjectByType<BuildingManager>();
        Am = FindAnyObjectByType<AttackManager>();

        OptionsList = new List<GameObject>();

        WaitOptionInstance = Instantiate(WaitOption, Options.transform);
        WaitOptionInstance.SetActive(false);
        AttackOptionInstance = Instantiate(AttackOption, Options.transform);
        AttackOptionInstance.SetActive(false);

    }
    private void OnEnable()
    {
        if (Bm.Buildings == null) { return; }
        CalculateOptions();

    }
    private void OnDisable()
    {
        if (OptionsList.Count == 0) { return; }
        OptionsList[SelectedOption].transform.GetChild(0).GetComponent<Image>().color = Color.clear;
        foreach (GameObject option in OptionsList) { option.SetActive(false); }
        OptionsList.Clear();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Gm.GameState = EGameStates.Selecting;
            Um.SelectedUnit.transform.position = Cc.SaveTile;
            if (Um.Path.Count != 0)
            {
                Cc.HoverTile = Um.Path.Last();
            }

            Um.SelectUnit(Um.SelectedUnit);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {

            if (OptionsList[SelectedOption].name.Contains("Wait"))
            {
                Um.EndMove();
                Gm.GameState = EGameStates.Idle;
            }
            //Logique to do when the player choose to attack 
            else if (OptionsList[SelectedOption].name.Contains("Attack"))
            {

                if (Um.SelectedUnit is AttackingUnit)
                {
                    attacker = Um.SelectedUnit as AttackingUnit;
                    Gm.GameState = EGameStates.Attacking;
                    Am.attacker = attacker;
                    //attacker.HighlightTargets(attacker);
                    attacker.HasAttacked = true;
                    Debug.Log("We're attacking");
                    Am.InitiateAttack(attacker);
                    Gm.GameState = EGameStates.ActionMenu;
                    //BEFORE RESETING UNIT NEED TO 
                    //SELECT THE TARGET => A FUNCTION TO SELECT TARGET : Unit target = SelectTarget(attacker);
                    //SHOW AN ATTACKING SCENE => ShowAttackingScene(attacker, target);
                    //APPLY DAMMAGE => attacker.ApplyDamage(target, attacker); 


                    StartCoroutine(EndMoveAfterDelay(1.0f, attacker));
                    
                       

                }

                ;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OptionsList[SelectedOption].transform.GetChild(0).GetComponent<Image>().color = Color.clear;
            SelectedOption = (SelectedOption - 1 + OptionsList.Count) % OptionsList.Count;
            OptionsList[SelectedOption].transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OptionsList[SelectedOption].transform.GetChild(0).GetComponent<Image>().color = Color.clear;
            SelectedOption = (SelectedOption + 1) % OptionsList.Count;
            OptionsList[SelectedOption].transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }



    }
    private IEnumerator EndMoveAfterDelay(float delay , AttackingUnit attacker ) 
    {
        yield return new WaitForSeconds(delay);

        Um.EndMove();
        attacker.UnHighlightTargets(attacker); 
        Gm.GameState = EGameStates.Idle;
    }
    private void CalculateOptions()
    {
        //Hna n3amr ola list te3 option 

        CheckAttackOption();
        CheckCapture();
        

        if (OptionsList.Count > 0)
        {
            SelectedOption = 0;
            OptionsList[SelectedOption].transform.GetChild(0).GetComponent<Image>().color = Color.white;

        }

    }
    private void CheckAttackOption()
    {
        if (Um.SelectedUnit == null)
        {
            Debug.LogWarning("SelectedUnit is null. Unable to check attack option.");
            return;
        }

        if (Um.SelectedUnit is AttackingUnit && Um.SelectedUnit != null)
        {
            attacker = Um.SelectedUnit as AttackingUnit;
            if (attacker == null)
            {
                Debug.Log("Attacker is null");
            }
            else
            {
                if (Am == null) Debug.Log("ONIICHAAAAAAAAAN");
                if (attacker.CanAttack(attacker) )
                {
                    AttackOptionInstance.SetActive(true);
                    OptionsList.Add(AttackOptionInstance);
                }
            }
        }
        else
        {
            Debug.LogWarning("SelectedUnit is not an AttackingUnit. Unable to check attack option.");
        }
    }


    private void CheckCapture()
    {
        if (Bm.Buildings == null) { return; }
        var building = Bm.Buildings.ContainsKey(Cc.HoverTile) ? Bm.Buildings[Cc.HoverTile] : null;
        if (building != null && building.Owner != Gm.PlayerTurn)
        {
            //OptionsList.Add( Instantiate(CaptureOption, Options.transform));
        }
        else
        {
            WaitOptionInstance.SetActive(true);
            OptionsList.Add(WaitOptionInstance);


        }

    }
}

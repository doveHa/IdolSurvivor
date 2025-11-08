using System.Collections;
using Script;
using Script.Manager;
using Script.Stage;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : ManagerBase<DiceManager>
{
    [SerializeField] private Button diceRollButton;

    private OwnedDiceHandler ownedDices;

    protected override void Awake()
    {
        base.Awake();
        ownedDices = new OwnedDiceHandler();
    }

    public IEnumerator DiceRoll()
    {
        for (int i = 0; i < StageManager.Manager.CurrentStage.eventCount; i++)
        {
            diceRollButton.interactable = true;
            yield return new WaitUntil(() => diceRollButton.interactable == false);
            ownedDices.InputDice(DiceRoller.lastRollResult);
        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
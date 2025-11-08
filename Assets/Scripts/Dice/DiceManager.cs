using System.Collections;
using Script;
using Script.Manager;
using Script.Stage;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : ManagerBase<DiceManager>
{
    [SerializeField] public DiceSlotHandler DiceSlot;
    [SerializeField] private Button diceRollButton;
    
    private OwnedDiceHandler ownedDices;

    protected override void Awake()
    {
        base.Awake();
        ownedDices = new OwnedDiceHandler();
    }

    void Start()
    {
        DiceSlot.CreateSlots();
    }

    public IEnumerator InitialDiceSet()
    {
        for (int i = 0; i < StageManager.Manager.CurrentStage.eventCount; i++)
        {
            diceRollButton.interactable = true;
            yield return new WaitUntil(() => diceRollButton.interactable == false);
            ownedDices.InputDice(DiceRoller.lastRollResult);
        }
        diceRollButton.gameObject.SetActive(false);
        StageDialogManager.Manager.InitialDiceEndDescription();
    }
}
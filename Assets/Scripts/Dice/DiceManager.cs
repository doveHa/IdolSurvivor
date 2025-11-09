using System.Collections;
using Script;
using Script.Manager;
using Script.Stage;
using Script.Stage.Event;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Manager { get; private set; }
    [SerializeField] public DiceSlotHandler DiceSlot;
    [SerializeField] private Button diceRollButton;

    private OwnedDiceHandler ownedDices;

    void Awake()
    {
        if (Manager == null)
        {
            Manager = this;
        }

        ownedDices = new OwnedDiceHandler();
    }

    void Start()
    {
        DiceSlot.CreateSlots();
    }

    public IEnumerator InitialDiceSet()
    {
        for (int i = 0; i < Config.Event.EventCount; i++)
        {
            diceRollButton.interactable = true;
            yield return new WaitUntil(() => diceRollButton.interactable == false);
            ownedDices.InputDice(DiceRoller.lastRollResult);
        }

        diceRollButton.gameObject.SetActive(false);
        StageDialogManager.Manager.InitialDiceEndDescription();
    }

    public void AddDraggableScript()
    {
        DiceSlot.AddDraggableScript();
    }

    public void ShowDiceSlot()
    {
        DiceSlot.gameObject.SetActive(true);
    }

    public void HideDiceSlot()
    {
        DiceSlot.gameObject.SetActive(false);
    }
}
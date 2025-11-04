using System;
using Script.Manager;
using TMPro;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] statText;

    void Start()
    {
        statText[0].text = CharacterSelectManager.Manager.Player.Stat.Sing.Value.ToString();
        statText[1].text = CharacterSelectManager.Manager.Player.Stat.Dance.Value.ToString();
        statText[2].text = CharacterSelectManager.Manager.Player.Stat.Appearance.Value.ToString();
        statText[3].text = CharacterSelectManager.Manager.Player.Stat.Charm.Value.ToString();
        statText[4].text = CharacterSelectManager.Manager.Player.Name;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
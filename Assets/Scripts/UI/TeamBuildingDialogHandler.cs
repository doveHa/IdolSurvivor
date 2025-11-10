using System.Data;
using System.Resources;
using Script;
using Script.TeamBuilding;
using TMPro;
using UnityEngine;
using ResourceManager = Script.Manager.ResourceManager;

namespace UI
{
    public class TeamBuildingDialogHandler : MonoBehaviour
    {
        private TextMeshProUGUI titleText;

        void Awake()
        {
            titleText = GetComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            titleText.text = "팀빌딩";
        }
    }
}
using Script;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PositionSelectDialogHandler : MonoBehaviour
    {
        private TextMeshProUGUI titleText;

        void Awake()
        {
            titleText = GetComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            titleText.text = "공연 역할, 파트 배분";
        }
    }
}
using Script;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PracticeDialogHandler : MonoBehaviour
    {
        private TextMeshProUGUI titleText;

        void Awake()
        {
            titleText = GetComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            titleText.text = "공연 연습";
        }
    }
}
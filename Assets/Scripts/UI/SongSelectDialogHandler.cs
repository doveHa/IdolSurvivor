using Script;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SongSelectDialogHandler : MonoBehaviour
    {
        private TextMeshProUGUI titleText;

        void Awake()
        {
            titleText = GetComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            titleText.text = "공연 공연곡 선택";
        }
    }
}
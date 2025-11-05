using UnityEngine;
using UnityEngine.UI;


namespace Script.ButtonClick
{
    public abstract class ButtonOnClick : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        protected abstract void OnClick();
    }
}
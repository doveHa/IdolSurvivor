using System.Collections;
using Script.DataDefinition.Enum;
using UnityEngine;

namespace Script.UI
{
    public class UIAnimationHandler : MonoBehaviour
    {
        [SerializeField] private UIAnimatorNumber number;
        [SerializeField] private int waitTimeBeforeAnimation;
        private Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            animator.SetInteger("UINumber", (int)number);
        }

        public void StartAnimation()
        {
            if (animator.GetBool("Pause"))
            {
                animator.SetBool("Pause", false);
            }
            else
            {
                StartCoroutine(WaitBeforeAnimation());
            }
        }

        public void PauseAnimation()
        {
            animator.SetBool("Pause", true);
        }

        public void EndAnimation()
        {
            animator.enabled = false;
        }

        private IEnumerator WaitBeforeAnimation()
        {
            yield return new WaitForSeconds(waitTimeBeforeAnimation);
            animator.SetBool("IsPlay", true);
        }
    }
}
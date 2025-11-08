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
            StartCoroutine(WaitBeforeAnimation());
        }

        public void EndAnimation()
        {
            animator.SetTrigger("Exit");
        }

        private IEnumerator WaitBeforeAnimation()
        {
            yield return new WaitForSeconds(waitTimeBeforeAnimation);
            animator.SetBool("IsPlay", true);
        }
    }
}
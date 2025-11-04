using UnityEngine;

namespace Script.Manager
{
    public class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T>
    {
        public static T Manager { get; private set; }

        protected virtual void Awake()
        {
            if (Manager == null)
            {
                Manager = (T)this;
            }
            else
            {
                Destroy(gameObject);
            }

            GameObject manager = GameObject.Find("Manager");
            if (manager == null)
            {
                manager = new GameObject("Manager");
            }

            DontDestroyOnLoad(manager);
            transform.SetParent(manager.transform);
        }
    }
}
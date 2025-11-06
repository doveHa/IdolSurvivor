using UnityEngine;

namespace Script.Manager
{
    public class ResourceManager
    {
        public static T Load<T>(string path) where T : Object
        {
            T resource = Resources.Load<T>(path);
            if (resource != null)
            {
                return resource;
            }
            else
            {
                return null;
            }
        }

        public static T[] LoadAll<T>(string path) where T : Object
        {
            T[] resources = Resources.LoadAll<T>(path);
            return resources;
        }
    }
}
using UnityEngine;

namespace Script.UI.DragDrop.DropFunction
{
    public abstract class IDrop : MonoBehaviour
    {
        public abstract void Click();
        public abstract void Drop(DroppableObject drop);
    }
}
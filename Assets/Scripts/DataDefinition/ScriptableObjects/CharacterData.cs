using UnityEngine;

namespace Script.DataDefinition.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "Character")]
    public class CharacterData : ScriptableObject
    {
        public string name;
        [TextArea] public string description;
        public Sprite standingImage;
    }
}
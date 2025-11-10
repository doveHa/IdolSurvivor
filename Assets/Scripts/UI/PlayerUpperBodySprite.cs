using System.Resources;
using Script;
using UnityEngine;
using UnityEngine.UI;
using Script.Manager;
using ResourceManager = Script.Manager.ResourceManager;

namespace UI
{
    public class PlayerUpperBodySprite : MonoBehaviour
    {
        [SerializeField] private Image image;

        void Start()
        {
            image.sprite = ResourceManager.Load<Sprite>(Config.Resource.CharacterData.PlayerCharacterUpperBodyPath());
        }
    }
}
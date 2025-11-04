using Script.Manager;
using TMPro;
using UnityEngine;

namespace Script.UI.DragDrop.DropFunction
{
    public class SetCharacterStat : IDrop
    {
        public override void Drop(DroppableObject drop)
        {
            TextMeshProUGUI textMesh = GetComponentInChildren<TextMeshProUGUI>();
            GetComponent<RectTransform>().transform.position = drop.GetComponent<RectTransform>().transform.position;
            string statType = drop.GetComponentInChildren<TextMeshProUGUI>().text;
            string point = textMesh.text;
            textMesh.text = $"{statType} <{point}>";

            CharacterSelectManager.Manager.SetStat(drop.name, int.Parse(point));
            Destroy(GetComponent<DraggableObject>());
        }
    }
}
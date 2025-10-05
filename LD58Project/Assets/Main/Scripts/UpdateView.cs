using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main.Scripts
{
    public class UpdateView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _image;

        private Action actionOnClick;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.localScale = Vector3.one * 1.15f; 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = Vector3.one;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            actionOnClick?.Invoke();
        }

        public void Initialize(Sprite sprite, string Description, Action action)
        {
            _text.text = Description;
            _image.sprite = sprite;
            actionOnClick = action;
        }
    }
}
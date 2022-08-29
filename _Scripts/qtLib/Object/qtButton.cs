using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.qtLib
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class qtButton : MonoBehaviour
    {
        //Property
        private Text _text;

        public Text text
        {
            get
            {
                if (_text == null)
                {
                    if (transform.childCount == 0)
                    {
                        return null;
                    }
                    else
                    {
                        _text = GetComponentInChildren<Text>(true);
                    }
                }

                return _text;
            }
        }

        private TextMeshProUGUI _tmpText;

        public TextMeshProUGUI tmpText
        {
            get
            {
                if (_tmpText == null)
                {
                    if (transform.childCount == 0)
                    {
                        return null;
                    }
                    else
                    {
                        _tmpText = GetComponentInChildren<TextMeshProUGUI>(true);
                    }
                }

                return _tmpText;
            }
        }

        private Button _button;
        public Button button
        {
            get
            {
                if (_button == null)
                {
                    _button = GetComponent<Button>();
                }

                return _button;
            }
        }

        //Event
        private qtPointerEvent _onPointerClick;

        public qtPointerEvent onPointerClick => _onPointerClick ??= new qtPointerEvent();
        public Button.ButtonClickedEvent onClick => button.onClick;

        public Color selectColor = Color.cyan;
        public Color norColor = Color.white;
        public Color disableColor = Color.gray;

        private bool _isActive;
        public bool isActive
        {
            get => _isActive;
            set
            {
                button.interactable = value;
                button.image.color = value ? disableColor : norColor;
                _isActive = value;
            }
        }

        private bool _isSelect;
        public bool isSelect
        {
            get => _isSelect;
            set
            {
                _button.image.color = value ? selectColor : norColor;
                _isSelect = value;
            }
        }

        public Color color
        {
            get => _button.image.color;
            set => _button.image.color = value;
        }

    }

    #region ----- Internal Class -----

    public class qtPointerEvent : UnityEvent<PointerEventData>
    {
    }

    #endregion
}
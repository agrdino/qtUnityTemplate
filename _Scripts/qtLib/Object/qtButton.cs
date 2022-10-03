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
    public class qtButton : qtPoolingObject
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
        public Color isUsedColor = Color.green;

        public Color color
        {
            get => button.image.color;
            set => button.image.color = value;
        }

        private bool _isActive;
        public bool isActive
        {
            get => _isActive;
            set
            {
                button.interactable = value;
                color = value ? norColor : disableColor;
                _isActive = value;
                _unTouchable = !value;
            }
        }

        private bool _isSelect;
        public bool isSelect
        {
            get => _isSelect;
            set
            {
                color = value ? selectColor : norColor;
                _isSelect = value;
            }
        }


        private bool _isUsed;
        public bool isUsed
        {
            get => _isUsed;
            set
            {
                _isUsed = value;
                color = value ? isUsedColor : norColor;
            }
        }

        private bool _unTouchable;

        public bool unTouchable
        {
            get => _unTouchable;
            set
            {
                button.interactable = !value;
                _unTouchable = value;
            }
        }

        public override void FactoryReset()
        {
            base.FactoryReset();
            isSelect = false;
            isUsed = false;
            isActive = true;
            onClick.RemoveAllListeners();
            onPointerClick.RemoveAllListeners();
        }
    }

    #region ----- Internal Class -----

    public class qtPointerEvent : UnityEvent<PointerEventData>
    {
    }

    #endregion
}
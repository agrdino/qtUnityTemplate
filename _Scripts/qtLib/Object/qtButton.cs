using System;
using _Scripts.qtLib.Extension;
using _Scripts.System;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.qtLib
{
    [Serializable]
    public enum BtnTransitionType
    {
        Color = 1,
        Image = 2
    }
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    [Serializable]
    public class qtButton : qtPoolingObject
    {
        public BtnTransitionType TransitionType = BtnTransitionType.Color;
        
        [HideInInspector] public Color selectColor = Color.cyan;
        [HideInInspector] public Color norColor = Color.white;
        [HideInInspector] public Color disableColor = Color.gray;
        [HideInInspector] public Color isUsedColor = Color.green;

        [HideInInspector] public Sprite selectImage;
        [HideInInspector] public Sprite norImage;
        [HideInInspector] public Sprite disableImage;
        [HideInInspector] public Sprite isUsedImage;

        [HideInInspector] public Color textSelectColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1);
        [HideInInspector] public Color textDisableColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1);
        
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
        private Button.ButtonClickedEvent _onButtonClick;

        public qtPointerEvent onPointerClick => _onPointerClick ??= new qtPointerEvent();
        public Button.ButtonClickedEvent onClick => _onButtonClick ??= new Button.ButtonClickedEvent();

        public Color color
        {
            get => button.image.color;
            set => button.image.color = value;
        }

        public Image image
        {
            get => button.image;
            set => button.image = value;
        }

        public Color textColor
        {
            get => text.color;
            set => text.color = value;
        }
        
        public Color tmpTextColor
        {
            get => tmpText.color;
            set => tmpText.color = value;
        }

        private bool _isActive;
        public bool isActive
        {
            get => _isActive;
            set
            {
                button.interactable = value;
                
                if (TransitionType == BtnTransitionType.Color)
                {
                    color = value ? norColor : disableColor;
                }
                else if(TransitionType == BtnTransitionType.Image)
                {
                    image.sprite = value ? norImage : disableImage;
                }
                
                if(text != null) textColor = value ? textSelectColor : textDisableColor;
                if(tmpText != null) tmpTextColor = value ? textSelectColor : textDisableColor;
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
                if (TransitionType == BtnTransitionType.Color)
                {
                    color = value ? selectColor : norColor;
                }
                else if(TransitionType == BtnTransitionType.Image)
                {
                    image.sprite = value ? selectImage : norImage;
                } 
                
                if(text != null) textColor = value ? textSelectColor : textDisableColor;
                if(tmpText != null) tmpTextColor = value ? textSelectColor : textDisableColor;

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
                if (TransitionType == BtnTransitionType.Color)
                {
                    color = value ? isUsedColor : norColor;
                }
                else if(TransitionType == BtnTransitionType.Image)
                {
                    image.sprite = value ? isUsedImage : norImage;
                }
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

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                _onButtonClick?.Invoke(); 
            });
        }
    }

    #region ----- Internal Class -----

    public class qtPointerEvent : UnityEvent<PointerEventData>
    {
    }

    #endregion
}
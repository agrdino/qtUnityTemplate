using System;
using System.Collections.Generic;
using _Scripts.Helper;
using _Scripts.qtLib.Extension;
using DG.Tweening;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.qtLib
{
    public class qtDynamicSrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region ----- VARIABLE -----

        private enum MoveDirection
        {
            Negative = -1,
            None = 0,
            Positive = 1
        }

        private MoveDirection _movesDirection;
        private Vector3 _vectorDirection{
            get
            {
                if (scrollSide == ScrollSide.Horizontal)
                {
                    return Vector3.right;
                }
                else
                {
                    return Vector3.down;
                }
            }
        }
        
        [Header("Scale of item")]
        public int width;
        public int height;

        private int _size
        {
            get
            {
                if (scrollSide == ScrollSide.Horizontal)
                {
                    return width;
                }
                else
                {
                    return height;
                }
            }
        }
        
        [Header("Scroll view property")]
        public int spacing;

        private float _containerWidth;
        private float _containerHeight;
        private float _containerSize
        {
            get
            {
                if (scrollSide == ScrollSide.Horizontal)
                {
                    return _containerWidth;
                }
                else
                {
                    return _containerHeight;
                }
            }
        }
        private int _objectInView;
        public bool isGrid;
        public int constrainNumber;
        
        public enum ScrollSide
        {
            Vertical,
            Horizontal,
        }

        public ScrollSide scrollSide = ScrollSide.Horizontal;
        
        [SerializeField] private RectTransform _container;
        private List<qtObjectScroll> _objectScrolls;
        private List<qtObjectModelScroll> _data;
        private qtObjectScroll _endScroll;
        private qtObjectScroll _beginScroll;

        private int _itemCache = 2;
        #endregion

        #region ----- Initialize -----
        
        public void Initialize<C,T>(string path, List<C> content) where C : qtObjectModelScroll where T : qtObjectScroll
        {
            _itemCache = 2;
            _beginScroll = null;
            _endScroll = null;
            _containerHeight = GetComponent<RectTransform>().rect.height;
            _containerWidth = GetComponent<RectTransform>().rect.width;

            _data = new List<qtObjectModelScroll>(content);

            if (_container == null)
            {
                qtLogging.LogError($"Container in {name} is NULL");
                return;
            }

            Vector3 startPosition;
            if (scrollSide == ScrollSide.Horizontal)
            {
                startPosition = _container.position - _container.rect.width/2 * Vector3.right;
            }
            else
            {
                startPosition = _container.position + _container.rect.height/2 * Vector3.left;
            }

            _objectScrolls ??= new List<qtObjectScroll>();
            _objectScrolls.ForEach(objectScroll => objectScroll.gameObject.SetActive(false));
            _objectScrolls.Clear();

            for (int i = 0; i < content.Count; i++)
            {
                var temp = System.DataManager.Pooling<T>(path, _container, true);
                temp.gameObject.name = i.ToString();
                temp.index = i;
                temp.Initialize(content[i], i, Vector3.zero).state = qtObjectScroll.ObjectScrollState.InView;
                if (i == 0)
                {
                    _beginScroll = temp;
                    temp.rect.position = startPosition + _size/2f * _vectorDirection;
                }
                else
                {
                    temp.rect.position = _objectScrolls[i - 1].rect.position + (_size + spacing) * _vectorDirection;
                }
                _objectScrolls.Add(temp);
                temp.gameObject.SetActive(true);
                if (i * (_size + spacing) >= _containerSize)
                {
                    temp.state = qtObjectScroll.ObjectScrollState.Cache;
                    _itemCache--;
                    if (_itemCache == 0)
                    {
                        temp.rect.position = _beginScroll.rect.position - (_size + spacing) * _vectorDirection;
                        _beginScroll = temp;
                        break;
                    }
                }

                _endScroll = temp;
            }
        }
        
        #endregion

        #region ----- DRAG EVENT -----

        private ScrollRect x;

        private Vector3 _inputMousePosition;

        public void OnBeginDrag(PointerEventData eventData)
        {
            _inputMousePosition = Input.mousePosition;
        }

        private float _cacheDistance;
        public void OnDrag(PointerEventData eventData)
        {
            _container.DOKill(false);
            float tempDistance;
            if (scrollSide == ScrollSide.Horizontal)
            {
                tempDistance = Input.mousePosition.x - _inputMousePosition.x;
            }
            else
            {
                tempDistance = Input.mousePosition.y - _inputMousePosition.y;
            }

            if (Mathf.Abs(tempDistance) < Mathf.Abs(_cacheDistance))
            {
                _container.DOKill();
                _cacheDistance = 0;
                _inputMousePosition = Input.mousePosition;
            }
            else
            {
                _cacheDistance = tempDistance;
            }

            //Move right
            if (tempDistance > 0)
            {
                _movesDirection = MoveDirection.Negative;
            }
            //Move left
            else if (tempDistance < 0)
            {
                _movesDirection = MoveDirection.Positive;
            }

            if (scrollSide == ScrollSide.Horizontal)
            {
                _container.DOMoveX(_container.position.x + tempDistance, Mathf.Abs(tempDistance) / 400)
                    .OnComplete(() => _movesDirection = MoveDirection.None)
                    .OnUpdate(() =>
                    {
                        _objectScrolls.ForEach(x =>
                        {
                            if (!x.rect.IsVisibleFrom(Camera.main) &&
                                x.state == qtObjectScroll.ObjectScrollState.InView)
                            {
                                OutSizeEvent(x);
                                return;
                            }

                            if (x.rect.IsVisibleFrom(Camera.main))
                            {
                                x.state = qtObjectScroll.ObjectScrollState.InView;
                            }
                        });
                    });
            }
            else
            {
                _container.DOMoveY(_container.position.y + tempDistance, Mathf.Abs(tempDistance) / 400).OnUpdate(() =>
                {
                    _objectScrolls.ForEach(x =>
                    {
                        if (!x.rect.IsVisibleFrom(Camera.main) && x.state == qtObjectScroll.ObjectScrollState.InView)
                        {
                            OutSizeEvent(x);
                        }

                        if (x.rect.IsVisibleFrom(Camera.main) && x.state == qtObjectScroll.ObjectScrollState.Cache)
                        {
                            InSizeEvent(x.index);
                            x.state = qtObjectScroll.ObjectScrollState.InView;
                        }
                    });
                });
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _cacheDistance = 0;
        }
        private void OutSizeEvent(qtObjectScroll target)
        {
            target.state = qtObjectScroll.ObjectScrollState.Cache;             
            var temp = target;
            switch (_movesDirection)
            {
                case MoveDirection.None:
                {
                    return;
                }
                case MoveDirection.Positive:
                {
                    int newIndex;
                    if (!temp.Equals(_beginScroll))
                    {
                        temp = _beginScroll;
                        temp.rect.position= _endScroll.rect.position + (_size + spacing) * _vectorDirection;
                        newIndex = _endScroll.index + 1;
                        _endScroll = temp;
                        
                    }
                    else
                    {
                        _beginScroll.rect.position = _endScroll.rect.position + (_size + spacing) * _vectorDirection;
                        newIndex = _endScroll.index + 1;
                        _endScroll = _beginScroll;
                    }
                    _beginScroll = _objectScrolls[(temp.index + 1) % _objectScrolls.Count];
                    _endScroll.Initialize(_data[newIndex % _data.Count], newIndex, Vector3.zero).state = qtObjectScroll.ObjectScrollState.Cache;
                    break;
                }
                case MoveDirection.Negative:
                {
                    int newIndex;
                    if (!temp.Equals(_endScroll))
                    {
                        temp = _endScroll;
                        temp.rect.position = _beginScroll.rect.position - (_size + spacing) * _vectorDirection;
                        newIndex = _beginScroll.index - 1;
                        _beginScroll = temp;
                    }
                    else
                    {
                        _endScroll.rect.position = _beginScroll.rect.position - (_size + spacing) * _vectorDirection;
                        newIndex = _beginScroll.index - 1;
                        _beginScroll = _endScroll;
                    }
                    _endScroll = _objectScrolls[(temp.index + _objectScrolls.Count - 1) % _objectScrolls.Count];
                    _beginScroll.Initialize(_data[newIndex % _data.Count], newIndex, Vector3.zero).state = qtObjectScroll.ObjectScrollState.Cache;
                    break;
                }
            }
        }
        private void InSizeEvent(int index)
        {
            
        }
        #endregion
    }
    
    public abstract class qtObjectModelScroll{}
}

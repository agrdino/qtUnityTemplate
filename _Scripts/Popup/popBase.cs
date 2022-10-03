using System;
using _Scripts.Helper;
using _Scripts.qtLib;
using _Scripts.qtLib.Extension;
using _Scripts.System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Prefab.Popup
{
    [DisallowMultipleComponent]
    public abstract class popBase : MonoBehaviour
    {
        [SerializeField] private Image imgBackground;
        [SerializeField] private qtButton btnClose;
        [SerializeField] protected TextMeshProUGUI txtContent;

        public enum AnimType
        {
            Left,
            Right,
            Up,
            Down,
            ZoomIn,
            ZoomOut
        }

        [SerializeField] protected AnimType _moveIn = AnimType.Left;
        [SerializeField] protected AnimType _moveOut = AnimType.Right;

        protected bool keepShowing;
        protected virtual void OnEnable()
        {
            btnClose?.onClick.AddListener(OnButtonCloseClick);
        }

        protected virtual void OnDisable()
        {
            btnClose?.onClick.RemoveAllListeners();
        }

        protected virtual void OnButtonCloseClick()
        {
            Hide();
        }

        public void Hide()
        {
            if (keepShowing)
            {
                keepShowing = false;
                return;
            }
            MoveOut();
            UIManager.Instance.HidePopup(this);
            UIManager.Instance.Fading(false);
        }

        public popBase Show(Action beforeShow = null)
        {
            MoveIn();
            return this;
        }

        public popBase KeepShowing()
        {
            ForceUpdateScale();
            transform.DOKill();
            keepShowing = true;
            return this;
        }

        protected virtual void MoveIn()
        {
            gameObject.SetActive(true);
            switch (_moveIn)
            {
                case AnimType.Left:
                {
                    transform.position = -Screen.width / 2f * Vector3.right + Screen.height/2f * Vector3.up;
                    transform.DOLocalMoveX(0, 0.25f).SetEase(Ease.InOutBounce);
                    break;
                }
                case AnimType.Right:
                {
                    transform.position = 3 * Screen.width / 2f * Vector3.right + Screen.height/2f * Vector3.up;
                    transform.DOLocalMoveX(0, 0.25f).SetEase(Ease.InOutBounce);
                    break;
                }
                case AnimType.Up:
                {
                    transform.position = Screen.width / 2f * Vector3.right - Screen.height/2f * Vector3.up;
                    transform.DOLocalMoveY(0, 0.25f).SetEase(Ease.InOutBounce);
                    break;
                }
                case AnimType.Down:
                {
                    transform.position = Screen.width / 2f * Vector3.right + 3 * Screen.height/2f * Vector3.up;
                    transform.DOLocalMoveY(0, 0.25f).SetEase(Ease.InOutBounce);
                    break;
                }
                case AnimType.ZoomIn:
                case AnimType.ZoomOut:
                {
                    transform.localScale = Vector3.zero;
                    transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.InOutBounce);
                    break;
                }
            }
        }

        protected virtual void MoveOut()
        {
            switch (_moveOut)
            {
                case AnimType.Left:
                {
                    transform.DOLocalMoveX(-Screen.width / 2f, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                    });
                    break;
                }
                case AnimType.Right:
                {
                    transform.DOLocalMoveX(3 * Screen.width / 2f, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                    });
                    break;
                }
                case AnimType.Up:
                {
                    transform.DOLocalMoveY(3 * Screen.height / 2f, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                    });
                    break;
                }
                case AnimType.Down:
                {
                    transform.DOLocalMoveY(-Screen.height / 2f, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                    });
                    break;
                }
                case AnimType.ZoomOut:
                case AnimType.ZoomIn:
                {
                    transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                    });
                    break;
                }
            }
        }

        private void Reset()
        {
            _moveIn = AnimType.Left;
            _moveOut = AnimType.Right;
        }

        protected void ForceUpdateScale()
        {
            (transform.GetChild(0) as RectTransform)?.ForceUpdateScale();
        }
    }
}
using System;
using DG.Tweening;
using UnityEngine;

public class qtSlider : MonoBehaviour
{
   [SerializeField] private RectTransform container;
   [SerializeField]  private RectTransform imgCurrent;
   [SerializeField]  private RectTransform imgNew;

   private float _oldValue, _newValue, _maxValue;
   
   public void SetValue(float oldValue, float newValue, float maxValue)
   {
      _oldValue = oldValue;
      _newValue = newValue;
      _maxValue = maxValue;
      
      var rect = container.rect;
      imgCurrent.sizeDelta  = new Vector2(rect.width * _oldValue / _maxValue, rect.height);
      imgNew.sizeDelta  = new Vector2(rect.width * _newValue / _maxValue, rect.height);
   }

   public void UpdateValue(Action onComplete)
   {
      var rect = container.rect;
      imgCurrent.DOSizeDelta(new Vector2(rect.width * _newValue / _maxValue, rect.height), 0.25f)
         .OnComplete(() => onComplete?.Invoke());
   }
}

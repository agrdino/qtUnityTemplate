using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class qtToggle : MonoBehaviour
{
    [SerializeField] private GameObject checkMark;
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private Color disableColor;
    [SerializeField] private Color enableColor;
    private Toggle _togger;

    private void Awake()
    {
        if (_togger == null)
            _togger = GetComponent<Toggle>();
    }

    private void OnEnable()
    {
        UpdateUI(_togger.isOn);
        _togger.onValueChanged.AddListener(UpdateUI);
    }

    private void OnDisable()
    {
        _togger.onValueChanged.RemoveAllListeners();
    }

    private void UpdateUI(bool isOn)
    {
        checkMark.SetActive(isOn);
        txtName.color = isOn ? enableColor : disableColor;
    }
}

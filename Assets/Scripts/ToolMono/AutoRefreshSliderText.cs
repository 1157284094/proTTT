using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AutoRefreshSliderText : MonoBehaviour
{
    private Slider slider = null;
    private TextMeshProUGUI text = null;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        text = slider.handleRect.GetComponentInChildren<TextMeshProUGUI>();

        slider.onValueChanged.AddListener(onSliderValueChange);
    }

    private void onSliderValueChange(float value)
    {
        text.text = string.Format("дя╤х{0}", value);
    }
}

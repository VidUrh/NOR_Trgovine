using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class kolicina : MonoBehaviour
{
    public Slider slider;
    private float sliderValue;

    public Text uiText;

    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        //sliderValue = 1;
    }

    private void OnSliderValueChanged(float value)
    {
        // Store the slider value in a variable
        sliderValue = value;
        //Debug.Log("Slider value is: " + sliderValue);
    }

    private void Update()
    {
        // Change the text of the UI element every frame
        uiText.text = sliderValue.ToString();
    }
}

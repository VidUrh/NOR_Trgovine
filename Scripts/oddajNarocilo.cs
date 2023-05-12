using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class oddajNarocilo : MonoBehaviour
{
    // delovanje gumba Oddaj narocilo
    public List<List<string>> narocila = new List<List<string>>();
    public GameObject Canvas;
    public Button oddaj;
    public Dropdown izdelekDropdown;
    public Slider kolicinaSlider;

    // Start is called before the first frame update
    void Start()
    {
        // Canvas.SetActive(true);
        oddaj.onClick.AddListener(ButtonClicked);
    }

    public void ButtonClicked()
    {
        // Code to execute when the button is clicked
        //Debug.Log("Button clicked!");
        int izdelek = izdelekDropdown.value;
        float kolicina = kolicinaSlider.value;
        Canvas.SetActive(false);
    }
}

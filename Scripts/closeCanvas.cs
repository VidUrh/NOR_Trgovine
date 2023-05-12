using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class closeCanvas : MonoBehaviour
{
    // delovanje gumba Oddaj narocilo
    public GameObject Canvas;
    public Button oddajNarocilo;

    private void Start()
    {
        // Disable the game object at the start of the game
        //Canvas.SetActive(true);
        oddajNarocilo.onClick.AddListener(ButtonClicked);
    }

    public void ButtonClicked()
    {
        // Code to execute when the button is clicked
        //Debug.Log("Button clicked!");
        Canvas.SetActive(false);
    }
}

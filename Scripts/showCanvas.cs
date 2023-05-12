using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showCanvas : MonoBehaviour
{
    // delovanje gumba Novo narocilo
    public GameObject Canvas;
    public Button novoNarocilo;

    private void Start()
    {
        // Disable the game object at the start of the game
        Canvas.SetActive(false);
        novoNarocilo.onClick.AddListener(ButtonClicked);
    }

    public void ButtonClicked()
    {
        // Code to execute when the button is clicked
        //Debug.Log("Button clicked!");
        Canvas.SetActive(true);
    }
}

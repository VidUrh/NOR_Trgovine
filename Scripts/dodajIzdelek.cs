using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dodajIzdelek : MonoBehaviour
{
    // delovanje gumba Dodaj izdelek
    public Button dodaj;
    public int num = 1;

    public GameObject izdelek;
    public Transform contentParentPanel;
    // Start is called before the first frame update
    void Start()
    {
        dodaj.onClick.AddListener(ButtonClickedAdd);
    }

    public void ButtonClickedAdd()
    {
        num += 1;
        GameObject copy = Instantiate(izdelek);
        copy.transform.SetParent(contentParentPanel);
        copy.transform.localScale = izdelek.transform.localScale;
    }
}

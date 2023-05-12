using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dodajInOddaj : MonoBehaviour
{
    public Button dodaj;
    public int num = 1;

    public GameObject izdelek;
    public Transform contentParentPanel;

    //public List<List<string>> narocila = new List<List<string>>();
    public List<List<int>> narocila2 = new List<List<int>>();
    public Narocila narocilaScript;
    public GameObject Canvas;
    public Button oddaj;
    public Dropdown izdelekDropdown;
    public Slider kolicinaSlider;

    //public Dropdown cloneDropdown;
    //public Slider cloneSlider;

    public GameObject currentIzdelek;
    public Dropdown currentDropdown;
    public Slider currentSlider;

    // Start is called before the first frame update
    void Start()
    {
        dodaj.onClick.AddListener(dodajIzdelekClicked);
        oddaj.onClick.AddListener(oddajNarociloClicked);
    }
    
    public void dodajIzdelekClicked()
    {
        num += 1;
        GameObject copy = Instantiate(izdelek);
        copy.name = "Izdelek" + num.ToString();
        copy.transform.SetParent(contentParentPanel);
        copy.transform.localScale = izdelek.transform.localScale;
    }

    public void oddajNarociloClicked()
    {
        //Debug.Log("Button clicked!");
        int izdelek;
        float kolicina;


        //izdelek = izdelekDropdown.value;
        //kolicina = kolicinaSlider.value;
        //Debug.Log("izdelek: " + izdelek + " kolicina: " + kolicina);
        //num -= 1;

        string name;
        //List<float> izdelekList = new List<float>();
        List<int> novoNarocilo = new List<int>();
        for (int i = 1; i <= num; i++)
        {
            name = "Izdelek" + i.ToString();
            //currentDropdown = Dropdown.Find("Dropdown");
            currentIzdelek = GameObject.Find(name);
            //Debug.Log("trenutni izdelek: " + currentIzdelek.name);
            currentDropdown = currentIzdelek.transform.GetChild(0).GetComponent<Dropdown>();
            //currentDropdown = currentIzdelek.GetComponents(Dropdown);
            //Debug.Log("objekt"+currentDropdown.name);
            izdelek = (int)currentDropdown.value;
            //Debug.Log("value" + currentDropdown.value);
            currentSlider = currentIzdelek.transform.GetChild(2).GetComponent<Slider>();
            kolicina = (int)currentSlider.value;
            for (int j = 0; j < kolicina; j++)
            {
                novoNarocilo.Add(izdelek - 1);
            }
            if (i == 1)
            {
                currentDropdown.value = 0;
                currentSlider.value = 0;
            }
            else
            {
                Destroy(currentIzdelek);
            }
        }
        /*Debug.Log("Novo narocilo:");
        string x;
        for (int i = 0; i < novoNarocilo.Count; i++ )
        {
            x = "Izdelek " + i.ToString();
            Debug.Log(x + ": " + novoNarocilo[i]);
        }*/
        num = 1;
        narocilaScript.narocila.Add(novoNarocilo);

        //while (num > 0)
        //{
        //    izdelek = cloneDropdown.value;
        //    kolicina = cloneSlider.value;
        //    Debug.Log("izdelek: " + izdelek + " kolicina: " + kolicina);
        //    num -= 1;
        //}

        Canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

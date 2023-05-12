/*
    OBSOLETE
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Narocila_skupno : MonoBehaviour
{
    public GameObject Canvas;
    public Button novoNarocilo;

    public List<List<string>> narocila = new List<List<string>>();
    public pathfinding pathScript;
    public Transform MobilnaPlatforma;
    public Dictionary<string, int> izdelki;
    public Vector3 pozicijaPlatforme;
    public Transform pozicijaIzdelka;
    public pobiranjeIzdelkov skriptaPobiranje;

    public Button dodaj;
    public int num = 1;
    public GameObject izdelek;
    public Transform contentParentPanel;

    // Start is called before the first frame update
    void Start()
    {
        Canvas.SetActive(false);
        novoNarocilo.onClick.AddListener(novoNarociloClicked);

        dodaj.onClick.AddListener(dodajIzdelekClicked);

        izdelki = new Dictionary<string, int>(){
			{"ananas", 9},
			{"banana", 10},
			{"pomaranca", 11},
			{"guinness", 12},
			{"kozarec", 13},
			{"toaster", 14},
			{"lonec", 15},
			{"jajca", 16},
			{"krof", 17},
			{"jabolka", 18},
			{"avocado", 19},
			{"cebula", 20},
			{"korenje", 21},
			{"paprika", 22},
			{"brokoli", 23},
			{"kumare", 24}
		};

        List<string> tempNarocilo = new List<string>();
        //tempNarocilo.Add("ananas");
        tempNarocilo.Add("jabolka");
        tempNarocilo.Add("jabolka");
        tempNarocilo.Add("jabolka");
        tempNarocilo.Add("avocado");
        tempNarocilo.Add("cebula");
        tempNarocilo.Add("cebula");
        tempNarocilo.Add("cebula");
        tempNarocilo.Add("korenje");
        tempNarocilo.Add("paprika");
        tempNarocilo.Add("brokoli");
        tempNarocilo.Add("kumare");
        narocila.Add(tempNarocilo);
    }

    public void novoNarociloClicked()
    {
        // Code to execute when the button is clicked
        //Debug.Log("Button clicked!");
        Canvas.SetActive(true);
    }

    public void dodajIzdelekClicked()
    {
        num += 1;
        GameObject copy = Instantiate(izdelek);
        copy.transform.SetParent(contentParentPanel);
        copy.transform.localScale = izdelek.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        int point;
        if (narocila.Count != 0)
        {
            List<string> trenutnoNarocilo = narocila[0];
            if (trenutnoNarocilo.Count != 0)
            {

                point = izdelki[trenutnoNarocilo[0]];
                pathScript.whichPoint = point;
                pozicijaPlatforme = MobilnaPlatforma.transform.position;
                pozicijaIzdelka = pathScript.points[point];

                float firstRotationY = MobilnaPlatforma.rotation.eulerAngles.y;
                float secondRotationY = pozicijaIzdelka.rotation.eulerAngles.y;

                float difference = Mathf.Abs(firstRotationY - secondRotationY) - 90;


                if (Vector3.Distance(pozicijaIzdelka.position, pozicijaPlatforme) < 5f && (Mathf.Abs(difference) < 1 || Mathf.Abs(difference - 180) < 1))
                {

                    // Stoj na mestu in �akaj da poberemo izdelek in ga damo v ko�aro
                    pathScript.moving = false;

                    if (skriptaPobiranje.pobrano)
                    {
                        skriptaPobiranje.poberi = false;
                        // Izdelek je dodan v ko�aro, pojdi na naslednjega
                        Debug.Log("[IZDELEK] Pobran izdelek: " + trenutnoNarocilo[0]);
                        trenutnoNarocilo.RemoveAt(0);
                        skriptaPobiranje.pobrano = false;
                    }
                    else
                    {
                        skriptaPobiranje.kajPoberi = izdelki[trenutnoNarocilo[0]];
                        skriptaPobiranje.poberi = true;
                    }

                }
                else
                {
                    pathScript.moving = true;
                }
            }
        }
    }
}

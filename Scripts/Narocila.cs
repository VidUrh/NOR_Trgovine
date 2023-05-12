using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Narocila : MonoBehaviour
{
    // Prebere narocilo in posilja platformo od enega izdelka do drugega
	public List<List<int>> narocila = new List<List<int>>();
	
	public pathfinding pathScript;

	public Transform MobilnaPlatforma;

	public Dictionary<string, int> izdelki;
	
	public Vector3 pozicijaPlatforme;
	public Transform pozicijaIzdelka;

	public pobiranjeIzdelkov skriptaPobiranje;

	
    // Start is called before the first frame update
    void Start()
    {
		izdelki = new Dictionary<string, int>(){
			{"ananas", 0},
			{"banana", 1},
			{"pomaranca", 2},
			{"jabolka", 3},
			{"avocado", 4},
			{"cebula", 5},
			{"korenje", 6},
			{"paprika", 7},
			{"brokoli", 8},
			{"kumare", 9},
			{"guinness", 10},
			{"kozarec", 11},
			{"toaster", 12},
			{"lonec", 13},
			{"jajca", 14},
			{"krof", 15}
		};
		

		List<int> tempNarocilo = new List<int>();
		for(int i = 2; i < 16; i++)
		{
			tempNarocilo.Add(i);
		}
		narocila.Add(tempNarocilo);

    }
	
    // Update is called once per frame
    void Update()
    {
		Debug.Log("Stevilo narocil: " + narocila.Count);
		int point;
		if (narocila.Count != 0)
		{
			List<int> trenutnoNarocilo = narocila[0];
			if (trenutnoNarocilo.Count != 0)
			{
				
				//point = izdelki[trenutnoNarocilo[0]];
                point = trenutnoNarocilo[0];
				pathScript.whichPoint = point;
				pozicijaPlatforme = MobilnaPlatforma.transform.position;
				pozicijaIzdelka = pathScript.points[point];

				float firstRotationY = MobilnaPlatforma.rotation.eulerAngles.y;
				float secondRotationY = pozicijaIzdelka.rotation.eulerAngles.y;

				float difference = Mathf.Abs(firstRotationY - secondRotationY) - 90;
				
				Debug.Log("Izdelek: " + trenutnoNarocilo[0]);

				if (Vector3.Distance(pozicijaIzdelka.position, pozicijaPlatforme) < 5f && (Mathf.Abs(difference) < 1 || Mathf.Abs(difference-180) < 1))
				{

					// Stoj na mestu in �akaj da poberemo izdelek in ga damo v ko�aro
					pathScript.moving = false;

					if (skriptaPobiranje.pobrano)
					{
						skriptaPobiranje.poberi = false;
						// Izdelek je dodan v ko�aro, pojdi na naslednjega
						trenutnoNarocilo.RemoveAt(0);
						skriptaPobiranje.pobrano = false;
					}
					else
					{
						//skriptaPobiranje.kajPoberi = izdelki[trenutnoNarocilo[0]] ;
                        skriptaPobiranje.kajPoberi = trenutnoNarocilo[0];
						skriptaPobiranje.poberi = true;
					}

				}
                else
                {
					pathScript.moving = true;
                }
			}
			else{
				narocila.RemoveAt(0);
			}
		}
    }
}

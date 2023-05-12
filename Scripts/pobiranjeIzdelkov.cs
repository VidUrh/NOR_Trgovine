using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pobiranjeIzdelkov : MonoBehaviour
{
    public Transform vrhRobota;
    public Transform targetPos;
    public Transform NadPlatformo;
    public Transform robotBase;
    public Transform kosarica;

    public bool pobrano = false;
    public bool poberi = false;
    public int kajPoberi = 0; 

    public Transform izdelkiParent;
    public List<List<Transform>> pozicijeIzdelkov;

    private Transform KateriIzdelekSePobira;
    
    [SerializeField] private int stanje = -1;

    // Start is called before the first frame update
    void Start()
    {
        pozicijeIzdelkov = new List<List<Transform>>();
        // Zanka �ez vsako vrsto izdelkov
        for (int i = 0; i < izdelkiParent.transform.childCount; i++)
        {
            Transform izdelkiDoloceneVrste = izdelkiParent.GetChild(i);
            List<Transform> izdelkiVrsteList = new List<Transform>();

            // Zanka �ez vsak izdelek dolo�ene vrste
            for (int j = 0; j < izdelkiDoloceneVrste.childCount; j++)
            {

                Transform couldBeCrate = izdelkiDoloceneVrste.GetChild(j);
                if (couldBeCrate.name.StartsWith("Crate of"))
                {
                    // Zanka �ez vsako �katlo dolo�ene vrste
                    for (int k = 0; k < couldBeCrate.childCount; k++)
                    {
                        Transform child = couldBeCrate.GetChild(k);
                        if (child.gameObject.activeSelf == true)
                        {
                            izdelkiVrsteList.Add(child);
                        }

                    }
                }
                else
                {
                    // Zanka �ez vsak podelement dolo�ene vrste
                    if (couldBeCrate.gameObject.activeSelf == true)
                    {
                        izdelkiVrsteList.Add(couldBeCrate);
                    }

                }

            }

            pozicijeIzdelkov.Add(izdelkiVrsteList);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(poberi==true)KateriIzdelekSePobira = getNajblizjiIzdelek(kajPoberi); //pozicijeIzdelkov[kajPoberi][0];
        if (poberi == true && stanje == -1)
        {
            targetPos.position = Vector3.Lerp(targetPos.position, KateriIzdelekSePobira.position, 3);
            targetPos.position += Vector3.up*50;
            targetPos.rotation = Quaternion.Euler(90, 0, 0);
            stanje = 0;
        }

        if (Vector3.Distance(targetPos.position, vrhRobota.position) < 5f && stanje == 0)
        {
            targetPos.position = KateriIzdelekSePobira.position;
            stanje++;
        }
        if(Vector3.Distance(targetPos.position, vrhRobota.position) < 5f && stanje == 1)
        {
            KateriIzdelekSePobira.parent = vrhRobota;
            targetPos.position += Vector3.up*50;
            stanje++;
        }
        if (Vector3.Distance(targetPos.position, vrhRobota.position) < 5f && stanje == 2)
        {
            targetPos.position = NadPlatformo.position;
            stanje++;
        }
        if (Vector3.Distance(targetPos.position, vrhRobota.position) < 5f && stanje == 3)
        {
            targetPos.position -= Vector3.up*20;
            stanje++;
        }
        if (Vector3.Distance(targetPos.position, vrhRobota.position) < 5f && stanje == 4)
        {
            KateriIzdelekSePobira.parent = kosarica;
            
            // Add a CapsuleCollider component to this game object
            CapsuleCollider capsuleCollider = KateriIzdelekSePobira.gameObject.AddComponent<CapsuleCollider>();

            // Set the radius and height of the capsule collider based on the game object's scale
            float radius = KateriIzdelekSePobira.transform.lossyScale.magnitude / 2f; // Assumes uniform scale in all axes
            float height = KateriIzdelekSePobira.transform.lossyScale.y - (2f * radius);
            capsuleCollider.radius = radius;
            capsuleCollider.height = height;

            // Set the center of the capsule collider
            capsuleCollider.center = new Vector3(0f, height / 2f, 0f);



            KateriIzdelekSePobira.gameObject.AddComponent<Rigidbody>();
            pobrano = true;
            stanje = -1;
        }
        
    }
    Transform getNajblizjiIzdelek(int indexIzdelka)
    {
        if(!(pozicijeIzdelkov.Count > 0)) { return null; }

        Debug.Log("indexIzdelka: " + indexIzdelka);
        
        Transform najblizji = pozicijeIzdelkov[indexIzdelka][0];
        float mini = float.PositiveInfinity;

        for(int i = 0; i < pozicijeIzdelkov[indexIzdelka].Count; i++)
        {
            float dist = Vector3.Distance(robotBase.position, pozicijeIzdelkov[indexIzdelka][i].position);
            if (dist < mini && pozicijeIzdelkov[indexIzdelka][i].parent.name != "kosarica"){
                mini = dist;
                najblizji = pozicijeIzdelkov[indexIzdelka][i];
            }
        }

        return najblizji;
    }
}

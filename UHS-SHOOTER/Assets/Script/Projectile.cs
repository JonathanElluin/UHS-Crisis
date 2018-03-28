using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public int damages;
    private const int speed = 50;

    public GameObject _parent;


    // Use this for initialization
    void Start () {
        //Détruit le projectile après 10 secondes s'il ne rencontre pas d'obstacles
        Destroy(gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void SetParent(GameObject parent)
    {
        _parent = parent;
    }


    void OnTriggerEnter(Collider other)
    {
        // Si le missile a touché le personnage qui l'a tiré il ne prends pas de dégats
        if (other.transform == _parent.transform)
        {
            return;
        }

        Debug.Log(other.tag);

        if (other.tag == "CheckPoint" || other.tag == "Trigger")
        {

        }

        // If the projectile hits an enemy or the player, it deals damages and disappear
        else if (other.tag == "Enemy" || other.tag == "Player")
        {
            other.gameObject.GetComponent<HealthManager>().TakeDammage(damages);
            Destroy(gameObject);
        }

        // If it hits a wall, it disappear. 
        else if (other.tag == "Mur")
        {
            Destroy(gameObject);
        }

        // Si on touche le boss
        else if(other.tag == "Boss")
        {
            other.gameObject.GetComponent<HealthManager>().TakeDammage(damages);
            Destroy(gameObject);
        }

        else
        {
        }
    }
}

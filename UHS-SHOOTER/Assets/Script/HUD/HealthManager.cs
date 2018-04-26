using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {

    public int MaxHealth;
    private int LifePoints;
    public bool isAlive = true;
    public Scrollbar LifeBar;

    public GameObject prefabConfettis;

    // Use this for initialization
    void Start () {
        LifePoints = MaxHealth;
    }
    

    public void TakeDammage(int damages, GameObject character)
    {
        LifePoints -= damages;
        LifeBar.size = (float)LifePoints * 1 / MaxHealth;

        // Si le personnage n'a plus de points de vie
        if (LifePoints <= 0)
        {
            isAlive = false;
            Destroy(gameObject);

            // Fais apparaitre des confettis qui disparaitrons apres 5 secondes
            Destroy(Instantiate(prefabConfettis, transform.position, Quaternion.identity), 5f);
        }

        // Si c'est le boss
        if (character.tag == "Boss")
        {
            character.GetComponent<Enemy>().BossTookDamages(damages);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour {

    public GameObject[] spawnPoints;
    public CheckPoint[] checkPoints;
    public GameObject prefabEnemy;
    public GameObject ptDecouvert;
    public int enemyLife;
    private int EnemiesAlive = 0;
    private Player playerScript;
    public Camera CamTactic;

    // Use this for initialization
    void Start ()
    {

    }
    
    // Update is called once per frame
    void Update ()
    {
        
    }

    // lorsqu'un objet entre en collision avec le spawnpoint
    void OnTriggerEnter(Collider other)
    {
        // Si l'objet est le joueur
        if (other.tag == "Player")
        {
            if (!playerScript)
            {
                //Récupère la position du joueur
                playerScript = other.gameObject.GetComponent<Player>();
                playerScript.CamMngr.SetTPSCam(CamTactic.transform);
            }

            // Récupère le temps depuis le début du jeu et gère la vie des ennemis
            int lifeCalculated = GetTimeAndSetEnemyLife();

            Enemy scriptEnemy;
            HealthManager scriptHealthManager;

            // Fais spawn des ennemis
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                GameObject _enemy = Instantiate(prefabEnemy, spawnPoints[i].transform.position, Quaternion.identity);
                _enemy.name = "Enemy" + i;
                scriptEnemy =_enemy.GetComponent<Enemy>();
                scriptHealthManager = _enemy.GetComponent<HealthManager>();

                
                scriptHealthManager.MaxHealth = enemyLife;
                scriptEnemy.spawnPointsScript = this;
                scriptEnemy.SetDestination(checkPoints[i].transform);
                scriptEnemy.checkPoint = checkPoints[i];
                scriptEnemy.target = other.gameObject;
                EnemiesAlive++;
            }
            
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    /// <summary>
    /// Récupère le temps depuis le début de la partie, et calcule la vie des 
    /// </summary>
    /// <returns></returns>
    private int GetTimeAndSetEnemyLife()
    {
        //throw new NotImplementedException();

        return 0;
    }

    /// <summary>
    /// Si un enemie a été tué
    /// </summary>
    public void EnemyDied()
    {
        EnemiesAlive--;
        
        if ((EnemiesAlive == 0) && (playerScript))
        {
            playerScript.GoToNextPosition();
        }
    }
}

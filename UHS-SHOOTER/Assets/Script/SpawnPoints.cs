using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour {

    // Pour faire spawn les enemies ou le boss
    public GameObject[] spawnPoints;
    public CheckPoint[] checkPoints;

    // Pour faire spawn les sbires si c'est la vague du boss
    public GameObject[] minionsSpawnPoints;
    public CheckPoint[] minionsCheckPoints;

    public GameObject prefabEnemy;
    public GameObject prefabMinion;
    public GameObject ptDecouvert;
    public int enemyLife;
    private int enemiesAlive = 0;
    private Player playerScript;
    public Camera CamTactic;

    private int bossDamagesTaken = 0;
    private int minionWavesPassed = 0;

    private bool prefabIsBoss = false;

    private GameObject enemy;
    private Enemy scriptEnemy;
    private GameObject target;

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

                target = other.gameObject;

                // Si le prefab a comme tag "Boss" alors c'est le boss
                if (prefabEnemy.tag == "Boss")
                {
                    prefabIsBoss = true;
                }
            }
            
            HealthManager scriptHealthManager;

            // Fais spawn des ennemis
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                enemy = Instantiate(prefabEnemy, spawnPoints[i].transform.position, Quaternion.identity);
                enemy.name = "Enemy" + i;
                scriptEnemy = enemy.GetComponent<Enemy>();
                scriptHealthManager = enemy.GetComponent<HealthManager>();

                
                scriptHealthManager.MaxHealth = enemyLife;
                scriptEnemy.spawnPointsScript = this;
                scriptEnemy.SetDestination(checkPoints[i].transform);
                scriptEnemy.checkPoint = checkPoints[i];
                scriptEnemy.target = target;
                enemiesAlive++;
            }
            
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    /// <summary>
    /// Se déclenche lorsque le boss prends des dégats
    /// </summary>
    internal void BossTookDamages(int damages)
    {
        bossDamagesTaken += damages;

        // Si le boss a pris un certain nombre de dégats
        if (bossDamagesTaken >= (20 * minionWavesPassed + 20) && minionWavesPassed < 3)
        {
            minionWavesPassed++;
            scriptEnemy.SetDestination(spawnPoints[0].transform);
            scriptEnemy.MoveToThisPoint(spawnPoints[0].transform);

            SpawnMinionsWave();
        }
    }

    /// <summary>
    /// Fais spawn une vague de sbires
    /// </summary>
    private void SpawnMinionsWave()
    {
        GameObject minion;
        HealthManager healthManager;
        Enemy scriptMinion;
        
        // Fais spawn des ennemis
        for (int i = 0; i < minionsSpawnPoints.Length; i++)
        {
            minion = Instantiate(prefabMinion, minionsSpawnPoints[i].transform.position, Quaternion.identity);
            minion.name = "Enemy" + i;
            scriptMinion = minion.GetComponent<Enemy>();
            healthManager = minion.GetComponent<HealthManager>();


            healthManager.MaxHealth = 3;
            scriptMinion.spawnPointsScript = this;
            scriptMinion.SetDestination(minionsCheckPoints[i].transform);
            scriptMinion.checkPoint = minionsCheckPoints[i];
            scriptMinion.target = target;
            enemiesAlive++;
        }
    }

    /// <summary>
    /// Se déclenche lorsqu'un sbire meurt
    /// </summary>
    public void MinionDied()
    {

    }

    /// <summary>
    /// Si un enemie a été tué
    /// </summary>
    public void EnemyDied()
    {
        enemiesAlive--;
        
        // Si on a tué tous les ennemies on se déplace vers la prochaine position
        if ((enemiesAlive == 0) && (playerScript))
        {
            playerScript.GoToNextPosition();
        }

        // Si il ne reste qu'un seul ennemie et que le dernier survivant est le boss on le fait revenir
        if (prefabIsBoss && enemiesAlive == 1)
        {
            scriptEnemy.SetDestination(checkPoints[0].transform);
            scriptEnemy.MoveToThisPoint(checkPoints[0].transform);
        }

        Debug.Log(playerScript.GetTimeElapsed());
    }
}

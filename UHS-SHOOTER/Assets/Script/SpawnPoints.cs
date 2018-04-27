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

    private int waves = 0;

    private int bossDamagesTaken = 0;
    private int minionWavesPassed = 0;
    private bool waveDied = false;

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
            SpawnEnemies(other);
            
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    /// <summary>
    /// Fais spawn les enemis
    /// </summary>
    private void SpawnEnemies(Collider other)
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
    }

    /// <summary>
    /// Se déclenche lorsque le boss prends des dégats
    /// </summary>
    internal void BossTookDamages(int damages)
    {
        bossDamagesTaken += damages;

        // Si le boss a pris un certain nombre de dégats
        if (bossDamagesTaken >= (35 * minionWavesPassed + 35))
        {
            minionWavesPassed++;
            scriptEnemy.SetDestination(spawnPoints[0].transform);
            scriptEnemy.MoveToThisPoint(spawnPoints[0].transform);

            SpawnMinionsWave();
            waveDied = false;
        }

        // Si on a passé les 3 vagues et que le temps est écoulé le boss meurt au prochain coup
        if (playerScript.GetTimeElapsed().TotalSeconds >= 290 && waveDied)
        {
            Destroy(enemy);
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


            healthManager.MaxHealth = 5;
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

        if (enemiesAlive == 0)
        {
            // Si on a tué tous les ennemis et qu'il reste des vagues
            if (waves < 2)
            {
                SpawnEnemies(playerScript.GetComponent<Collider>());
                waves++;
            }
            // Si on a tué tous les ennemies on se déplace vers la prochaine position
            else if (playerScript)
            {
                playerScript.GoToNextPosition();
            }
        }

        // Si il ne reste qu'un seul ennemie et que le dernier survivant est le boss on le fait revenir
        if (prefabIsBoss && enemiesAlive == 1)
        {
            scriptEnemy.SetDestination(checkPoints[0].transform);
            scriptEnemy.MoveToThisPoint(checkPoints[0].transform);

            waveDied = true;
        }

        Debug.Log(playerScript.GetTimeElapsed());
    }
}

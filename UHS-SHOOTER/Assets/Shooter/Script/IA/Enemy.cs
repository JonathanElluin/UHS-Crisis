using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Humanoid {

    private float waitTimeUncovered = 11;
    private float waitTimeCovered = 0.1f;
    private float waitTimeShoot = 2;

    public SpawnPoints spawnPointsScript;
    public CheckPoint checkPoint;

    private bool coroutineShot = false;

    // Use this for initialization
    void Start ()
    {
        Init();

        agent.enabled = true;
        MoveToThisPoint(true);
    }
    
    // Update is called once per frame
    void Update ()
    {

        if (!IsAlive())
        {
            return;
        }
        
        switch (HumanState)
        {

            // Si le joueur arrive à destination, on passe dans l'étape "Arrived"
            case Etape.Moving:

                // Lorsque le joueur arrive, on enlève sa position dans la list et on passe dans l'étape arrivée
                if (HasArrived())
                {
                    SwitchState(Etape.Arrived);
                }

                break;

            // Si le joueur est arrivé, on fait spawn les ennemis et on passe dans l'étape "Covered"
            case Etape.Arrived:
                
                SwitchState(Etape.GoCovered);

                break;

            case Etape.GoCovered:

                if (HasArrived())
                {
                    SwitchState(Etape.Covered);
                    StartCoroutine("WaitCovered");
                }

                break;

            // Si le joueur est à couvert, un appuie sur le bouton haut nous fait passer dans l'étape "Uncovered"
            case Etape.Covered:

                iTween.RotateTo(gameObject, new Vector3(checkPoint.transform.rotation.x, checkPoint.transform.rotation.y, checkPoint.transform.rotation.z), 1);

                break;

            // si appuie sur touche "haut", déplacement vers le point de découvert
            case Etape.GoUncovered:
                
                if (HasArrived())
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, GetDestination().rotation, 10 * Time.deltaTime);
                    SwitchState(Etape.Uncovered);
                    StartCoroutine("WaitUncovered");
                }
                break;

            // Lorsque que l'enemy arrive au point de destination, il est à découvert et peut tirer
            case Etape.Uncovered:

                //Look enemy and shoot
                if (target)
                {
                    LookToTarget();
                    
                    // Si on a pas encore tiré de projectile
                    if (!coroutineShot)
                    {
                        StartCoroutine("Shoot");
                    }
                }

                break;
        }
    }

    /// <summary>
    /// Est appelé lorsque le boss prend des dégats
    /// </summary>
    /// <param name="damages"></param>
    public void BossTookDamages(int damages)
    {
        spawnPointsScript.BossTookDamages(damages);
    }

    /// <summary>
    /// Lorsque l'enemy n'a plus de vie on le détruit
    /// </summary>
    void OnDestroy()
    {
        if (spawnPointsScript)
        {
            spawnPointsScript.EnemyDied();
        }
    }


    IEnumerator WaitCovered()
    {
        yield return new WaitForSeconds(waitTimeCovered);

        SwitchState(Etape.GoUncovered);

        // Si pas de point à découvert on ne va pas vers ce point 
        if (checkPoint.ptDecouvert != null)
        {
            // Se déplace vers le point à découvert
            SetDestination(checkPoint.ptDecouvert.transform);
            MoveToThisPoint(false);
        }
    }

    IEnumerator WaitUncovered()
    {
        yield return new WaitForSeconds(waitTimeUncovered);


        // Si le CheckPoint n'a pas de point à découvert, on ne se met pas à couvert
        if (checkPoint.ptDecouvert != null)
        {
            col.enabled = false;
            SwitchState(Etape.GoCovered);

            // Se déplace vers le point à couvert
            SetDestination(checkPoint);
            MoveToThisPoint(false);
        }
    }

    IEnumerator Shoot()
    {
        coroutineShot = true;

        yield return new WaitForSeconds(waitTimeShoot);

        if (HumanState == Etape.Uncovered)
        {
            Fire();
        }

        coroutineShot = false;
    }
}

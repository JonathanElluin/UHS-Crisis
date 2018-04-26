using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Humanoid : MonoBehaviour {


    public List<CheckPoint> destination;

    //Projectile
    public GameObject Projectile;
    public int damages;

    //Agent
    public NavMeshAgent agent;
    HealthManager healthManager;
    public Collider col;

    //Goal
    public GameObject target;
    Transform Destination;

    //State
    public enum Etape { Tuto, Moving, Arrived, GoCovered, Covered, GoUncovered, Uncovered }
    public Etape HumanState;

    private float bulletSpawnDistance = 0;

    // Use this for initialization
    public void Init ()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        healthManager = gameObject.GetComponent<HealthManager>();
        col = gameObject.GetComponent<Collider>();

        /*// Le boss doit tirer de plus loin car il est plus gros
        if (transform.tag.Contains("Boss"))
        {
            bulletSpawnDistance = 3f;
        }
        else
        {
            bulletSpawnDistance = 2.5f;
        }
        
         DESACTIVATION DE L'AVANCEE DE LA BALLE : Le projectile apparait maintenant dans le personnage, sans lui faire de dégats
         
         */
    }

    /// <summary>
    /// Change l'état dans lequel se trouve le personnage 
    /// </summary>
    /// <param name="_state"></param>
    public virtual void SwitchState(Etape _state)
    {
        HumanState = _state;
        switch (HumanState)
        {
            case Etape.Tuto:
                break;

            case Etape.Moving:
                break;

            case Etape.GoCovered:
                break;

            case Etape.Covered:
                col.enabled = false;
                break;

            case Etape.GoUncovered:
                break;

            case Etape.Uncovered:
                col.enabled = true;
                break;
        }
    }


    /// <summary>
    /// Envoie un projectile
    /// </summary>
    public void Fire()
    {
        Vector3 positionOutsideObject = transform.position;
        positionOutsideObject += bulletSpawnDistance * (transform.forward);
        positionOutsideObject += 0.5f * (transform.up);

        GameObject _projectile = Instantiate(Projectile, positionOutsideObject, transform.rotation);

        _projectile.GetComponent<Projectile>().SetParent(this.gameObject);
        _projectile.GetComponent<Projectile>().damages = damages;
    }


    // PARTIE MOUVEMENT

    //Set destination of IA
    public void SetDestination(CheckPoint _destination)
    {
        destination.Add(_destination);

        Destination = _destination.transform;
    }

    public void SetDestination(Transform _destination)
    {
        Destination = _destination.transform;
    }

    public Transform GetDestination()
    {
        return Destination;
    }

    // Move to the goal point
    public void MoveToThisPoint(bool moveToNewDestination)
    {
        if (Destination)
        {
            if (!agent)
            {
                agent = gameObject.GetComponent<NavMeshAgent>();
            }

            agent.isStopped = false;
            agent.SetDestination(Destination.position);

            // On passe dans l'étape "Moving" uniquement si l'on se rends vers une nouvelle destination
            if (moveToNewDestination)
            {
                SwitchState(Etape.Moving);
            }
        }
    }

    // Return true if the destination is reach
    public bool HasArrived()
    {
        if (agent.remainingDistance <= 0.5f)
        {
            agent.isStopped = true;

            return true;
        }
        else
        {
            return false;
        }
    }

    //State
    void GoCovered()
    {

    }

    void GoUncovered()
    {

    }

    // Regarde vers l'ennemi
    public void LookToTarget()
    {
         transform.LookAt(target.transform);
    }

    public bool IsAlive()
    {
        return healthManager.isAlive;
    }
}
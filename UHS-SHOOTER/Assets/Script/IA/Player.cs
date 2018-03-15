using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player : Humanoid 
{
    
    private CheckPoint actualPosition;
    private const int coverPos = 5;
    private KeyCode btnTir = KeyCode.Space;
    public CamManager CamMngr;



    public int LONGUEURMAX = 5;//min


    
    //Enemy
    bool EnemiesFind = false;
    List<GameObject> Enemies = new List<GameObject>();
    int indexEnemies = 0;

    //Tuto
    public TutoManager TutoMngr;

    private bool haveWaited5Sec = false;

	// Use this for initialization
	void Start()
    {
        //get script camm manager on it
        CamMngr = gameObject.GetComponent<CamManager>();
        Init();
        //Set Player Destination
        GoToNextPosition();


        //DeactivateMeshRenderer("PtDecouvert");
        //DeactivateMeshRenderer("CheckPoint");
        //DeactivateMeshRenderer("Respawn");
    }

    private void DeactivateMeshRenderer(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach(GameObject obj in objects)
        {
            obj.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
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
                    if (destination.Count > 0)
                    {
                        actualPosition = destination[0];
                        destination.RemoveAt(0);
                    }
                    SwitchState(Etape.Arrived);
                }

                break;

            // Si le joueur est arrivé, on fait spawn les ennemis et on passe dans l'étape "Covered"
            case Etape.Arrived:

                if (!EnemiesFind)
                {
                    FindEnemies();
                }
                
                SwitchState(Etape.GoCovered);

                // Se déplace vers le check point
                this.SetDestination(actualPosition.transform);
                MoveToThisPoint(false);

                //switch cam position
                CamMngr.SwitchPosCam("TPS");

                break;

            //Se déplace vers le point pour se mettre à couvert
            case Etape.GoCovered:

                if (HasArrived())
                {
                    this.transform.rotation = GetDestination().rotation;

                    if (haveWaited5Sec)
                    {
                        SwitchState(Etape.Covered);
                        StartCoroutine("WaitCovered");
                    }
                    else
                    {
                        StartCoroutine("Wait2Second");
                    }
                }


                break;

            // Si le joueur est à couvert, un appuie sur le bouton haut nous fait passer dans l'étape "Uncovered"
            case Etape.Covered:

                transform.rotation = Quaternion.Slerp(transform.rotation, GetDestination().rotation, 10 * Time.deltaTime);

                break;

            // si appuie sur touche "haut", déplacement vers le point de découvert
            case Etape.GoUncovered:
                
                if (HasArrived())
                {
                    transform.rotation = GetDestination().rotation;
                    SwitchState(Etape.Uncovered);
                    target = ChooseTarget(1);
                }
                break;

            // Lorsque que le joueur arrive au point de destination, il est à découvert et peut tirer
            case Etape.Uncovered:
                
                //Look enemy and shoot
                if (target)
                {
                    LookToTarget();

                    if (Input.GetKeyDown(btnTir))
                    {
                        Fire();
                    }
                }
                
                //Choose Enemy
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
					target = ChooseTarget(-1);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    target = ChooseTarget(1);
                }

               
                //Go to Covered state if there is an uncover point
                if (Input.GetKeyDown(KeyCode.DownArrow) && actualPosition.ptDecouvert != null)
                {
                    col.enabled = false;
                    SwitchState(Etape.GoCovered);

                    // Se déplace vers le point à couvert
                    this.SetDestination(actualPosition.transform);
                    MoveToThisPoint(false);
                }

                break;
        
        }
    }

    // Se déplace vers la prochaine position
    public void GoToNextPosition()
    {
        EnemiesFind = false;
        
        if (destination[0].GetTransform())
        {
            SetDestination(destination[0].GetTransform());
            MoveToThisPoint(true);
        }
    }


    public void FindEnemies()
    {
        GameObject[] _enemies;
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Enemies = new List<GameObject>();
        
        for (int i = 0; i < _enemies.Length; i++)
        {
            Enemies.Add(_enemies[i]);
        }
        EnemiesFind = true;

        //Player targeting enemies
        for (int i = 0; i < Enemies.Count; i++)
        {
            try
            {
                Enemies[i].GetComponent<TargetManager>().Targeting();
            }
            catch
            {
                Debug.Log("prooblem");
            }
        }
    }

    //return a target
    GameObject ChooseTarget(int _direction)
    {
        GameObject _enemyCloser = null;
        Vector3 _relativeCloser = Vector3.zero;
        for (int i = 0; i < Enemies.Count; i++)
        {
            //si l'enemi est mort on l'enleve de la liste
            if (!Enemies[i])
            {
                Enemies.Remove(Enemies[i]);
                
            }

            //si il est different de la cible on test ou il est 
            else if (Enemies[i] != target)
            {
                Vector3 relativePoint = transform.InverseTransformPoint(Enemies[i].transform.position);
                //si le joueur a choisi gauche
                if (relativePoint.x < 0.0f && _direction == -1)
                {
                    //on prend l'ennemi le plus proche a gauche
                    if (_relativeCloser.x < relativePoint.x || _relativeCloser.x == 0.0f)
                    {
                        _relativeCloser = relativePoint;
                        _enemyCloser = Enemies[i];
                    }

                }

                //si le joueur a choisi droite
                if (relativePoint.x > 0.0f && _direction == 1)
                {
                    //on prend l'ennemi le plus proche a droite
                    if (_relativeCloser.x > relativePoint.x || _relativeCloser.x == 0.0f)
                    {
                        _relativeCloser = relativePoint;
                        _enemyCloser = Enemies[i];
                    }
                }
            }
        }

        // Si on a pas trouvé d'ennemis on refait avec l'autre direction
        if (!_enemyCloser)
        {
            try
            {
                if (_direction == 1)
                {
                    _enemyCloser = ChooseTarget(-1);
                }
                else
                {
                    _enemyCloser = ChooseTarget(1);
                }
            }
            catch
            {

            }
        }
        if (!_enemyCloser)
        {
            return target;
        }
        return _enemyCloser;
    }

    IEnumerator WaitCovered()
    {
        //switch cam position
        CamMngr.SwitchPosCam("TPS");

        yield return new WaitForSeconds(2);

        SwitchState(Etape.GoUncovered);

        //switch cam position
        CamMngr.SwitchPosCam("FPS");

        // Si il n'y a pas de point à découvert on ne bouge pas
        if (actualPosition.ptDecouvert != null)
        {
            // Se déplace vers le point à découvert
            this.SetDestination(actualPosition.ptDecouvert.transform);
            MoveToThisPoint(false);
        }
    }

    IEnumerator Wait2Second()
    {
        haveWaited5Sec = false;

        yield return new WaitForSeconds(2);

        haveWaited5Sec = true;
    }
}


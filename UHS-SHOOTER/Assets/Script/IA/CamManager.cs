using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour {
	//cam
	public Camera MainCam;

	//destinations
	public Transform CamFPS;
	Transform CamTPS;

	public int time = 10;

	//current destination
	Transform DestinationCam;

	bool moveCam = false;
	// Use this for initialization
	void Start () {

	}

    // Update is called once per frame
    void Update()
    {

        if (DestinationCam)
        {
            //Move and rotate PlayerCam to its destination
            if (moveCam)
            {
                MainCam.transform.rotation = Quaternion.Slerp(MainCam.transform.rotation, DestinationCam.rotation, time * Time.deltaTime);
                MainCam.transform.position = Vector3.Lerp(MainCam.transform.position, DestinationCam.position, time * Time.deltaTime);
            }

            if (!moveCam) return;


            //Stop move and rotate
            if (MainCam.transform.rotation == DestinationCam.rotation & MainCam.transform.position == DestinationCam.position)
            {
                moveCam = false;
            }
        }
    }

	public void SetTPSCam(Transform _cam)
	{
		CamTPS = _cam;
	}

	public void SwitchPosCam(string _choice)
	{
		moveCam = true;
		DestinationCam = (_choice == "TPS") ? CamTPS : CamFPS;

		StartCoroutine("Wait2Second");
	}

	public bool IsMoving()
	{
		return moveCam;
	}


	// Après 1.5 secondes, on dit que la cam est alignée
	IEnumerator Wait2Second()
	{
		yield return new WaitForSeconds(1.5f);

		moveCam = false;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : MonoBehaviour
{

    public int timer;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(StartTimer());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator StartTimer()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timer--;
        }
        Destroy(this.gameObject);
    }
}

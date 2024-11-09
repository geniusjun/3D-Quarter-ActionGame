using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    public GamaManager gamaManager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            gamaManager.StageStart();
        }
    }
}

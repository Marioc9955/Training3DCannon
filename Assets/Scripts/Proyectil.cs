using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cinemachine;

public class Proyectil : MonoBehaviour
{
    public int numCol;

    public CannonAgent cannonAgent { get; set; }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("TargetCannonAgent"))
        {
            cannonAgent.AddReward(-.0111f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("TargetCannonAgent"))
        {
            cannonAgent.AddReward(+10f);
            cannonAgent.Win();
            cannonAgent.EndEpisode();
            Destroy(gameObject);
        }
        else
        {
            cannonAgent.AddReward(-0.0333f);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            cannonAgent.AddReward(-0.25f);
            Destroy(gameObject);
        }

    }

}

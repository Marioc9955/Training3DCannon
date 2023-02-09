using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public int numCol;

    public CannonAgent cannonAgent { get; set; }

    public Transform target;

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("TargetCannonAgent"))
        {
            cannonAgent.AddReward(+10f);
            cannonAgent.Win();
            cannonAgent.EndEpisode();
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            cannonAgent.AddReward(-.01f);
            Destroy(gameObject);
        }

    }

    private void Update()
    {
        float distance = Vector3.Distance(target.localPosition, transform.localPosition);
        float rewardByDistance = DistanceToReward(distance);
        print("distance "+ distance + " reward by dist " + rewardByDistance );
        cannonAgent.AddReward(rewardByDistance);
    }

    float DistanceToReward(float d)
    {
        //return -Mathf.Sqrt(d) + 3;
        return Mathf.Atan(-d + 10);
    }
}

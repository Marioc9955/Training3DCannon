using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public int numCol;

    public CannonAgent cannonAgent { get; set; }

    bool chocaConPiso;

    float minTimeActive = 15f;
    float timeActive = 0;

    private void Start()
    {
        chocaConPiso = false;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("TargetCannonAgent"))
        {
            cannonAgent.SetReward(1f);
            if (!chocaConPiso)
            {
                cannonAgent.AddReward(0.5f);
                print("Big reward");
            }
            cannonAgent.Win();
            BackToPool();
        }
        else
        {
            cannonAgent.AddReward(-.000111f);
            chocaConPiso = true;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            BackToPool();
        }
    }

    private void Update()
    {
        //float distance = Vector3.Distance(target.localPosition, transform.localPosition);
        //float rewardByDistance = DistanceToReward(distance);
        ////print("distance "+ distance + " reward by dist " + rewardByDistance +" of proyectil no "+cannonAgent.shotsMade
        ////+" in env "+ transform.parent.name);
        //cannonAgent.AddReward(rewardByDistance);
        timeActive += Time.deltaTime;
        if (GetComponent<Rigidbody>().velocity.sqrMagnitude < 1f && timeActive>=minTimeActive)
        {
            BackToPool();
        }
    }

    private void OnEnable()
    {
        chocaConPiso = false;
    }

    public float DistanceToReward(float d)
    {
        float r = Mathf.Clamp(1.1f - 0.9f * Mathf.Log10(d * 0.7f), -0.5f, 0.5f);
        //r = Mathf.InverseLerp(-0.5f, 0.5f, r);
        //r = Mathf.Lerp(-0.005f, 0.01f, r);

        return r;
        
    }

    [ContextMenu("Back projectile to pool")]
    public void BackToPool()
    {
        timeActive= 0;
        cannonAgent.DeleteShootedProjectile(this);
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity= false;
        gameObject.SetActive(false);
    }
}

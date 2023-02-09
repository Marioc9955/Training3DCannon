using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CannonAgent : Agent
{
    [SerializeField] private GameObject parentMultipleTargets;
    [SerializeField] private Transform currentTarget;
    bool currentTargetDead;

    [SerializeField] private float reloadCannonTime;

    [Header("Variables para disparar")]
    [SerializeField] private GameObject projectilPrefab;
    [SerializeField] private Transform cannonTip;
    [SerializeField] private float force;
    [SerializeField] private ParticleSystem psCannonExplosion;
    [SerializeField] private int totalShots;

    int shotsMade;

    private Proyectil _proyectil;

    [Header("Variables para rotacion")]
    [SerializeField] private Transform rotacionVertical; //eje X
    [SerializeField] private Transform rotacionHorizontal; //eje Y
    [SerializeField] private float rotationFactor;

    private Quaternion rotacionVerticalInicial;
    private Quaternion rotacionHorizontalInicial;

    [Header("Visualizar entrenamiento")]
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorRenderer;

    bool canAim, canShoot;

    public override void OnEpisodeBegin()
    {
        canAim = true;
        shotsMade = 0;
        rotacionVertical.localRotation = rotacionVerticalInicial;
        rotacionHorizontal.localRotation = rotacionHorizontalInicial;
       
    }

    private void Start()
    {
        canAim = true;
        canShoot = true;
        shotsMade = 0;
        currentTargetDead = true;
        rotacionHorizontalInicial = rotacionHorizontal.localRotation;
        rotacionVerticalInicial = rotacionVertical.localRotation;
    }

    void ChooseNewTarget()
    {
        int i = Random.Range(0, parentMultipleTargets.transform.childCount);
        currentTarget = parentMultipleTargets.transform.GetChild(i);
        currentTargetDead = false;

        print(parentMultipleTargets.transform.GetChild(i).name);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rotacionHorizontal.localRotation);
        sensor.AddObservation(rotacionVertical.localRotation);
        sensor.AddObservation(currentTarget.localPosition);

        //if (currentTargetDead)
        //{
        //    ChooseNewTarget();
        //}
        //else
        //{
        //    sensor.AddObservation(currentTarget.localPosition);
        //}
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;

        continousActions[0] = Input.GetAxis("Vertical");
        continousActions[1] = Input.GetAxis("Horizontal");

        ActionSegment<int> discreteAction = actionsOut.DiscreteActions;

        discreteAction[0] = Input.GetKey(KeyCode.F) ? 1 : 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        //if (_proyectil != null)
        //{
        //    //if (Vector3.Distance(currentTarget.position, _proyectil.transform.position) < 3f)
        //    //{
        //    //    AddReward(1f);
        //    //}
        //    //else {
        //    //    AddReward(-1f);
        //    //}

        //    float rewardByDistance = DistanceToReward(Vector3.Distance(currentTarget.position, _proyectil.transform.position));
        //    print("reward by dist " + rewardByDistance);
        //    AddReward(rewardByDistance);
        //}

        if (canAim)
        {
            var verticalMove = actions.ContinuousActions[0];
            //print("Vertical move action " + verticalMove);
            Vector3 ear = rotacionVertical.transform.rotation.ToEulerAngles();
            if ((verticalMove > 0 && ear.z < 0f) ||
            (verticalMove < 0 && ear.z > -1.11f))
            {
                rotacionVertical.Rotate(0, 0, verticalMove * rotationFactor);
            }

            var horizontalMove = actions.ContinuousActions[1];
            rotacionHorizontal.Rotate(0, horizontalMove * rotationFactor, 0);
        }
        if (canShoot && actions.DiscreteActions[0] > 0 && shotsMade < totalShots)
        {
            Invoke("Disparar", .111f);
            canAim = false;
            canShoot = false;
            Invoke("ActivateAim", .5f);
            Invoke("ActivateShoot", reloadCannonTime);
            shotsMade++;
            //Disparar();
        }
        if (shotsMade >= totalShots)
        {
            Lose();
            EndEpisode();
        }
    }

    void ActivateAim()
    {
        canAim = true;
    }

    void ActivateShoot()
    {
        canShoot = true;
    }

    [ContextMenu("Disparar")]
    void Disparar()
    {
        _proyectil = Instantiate(projectilPrefab, cannonTip.parent.position, Quaternion.identity, transform.parent)
            .GetComponent<Proyectil>();
        Vector3 _dirShoot = cannonTip.position - cannonTip.parent.position;

        psCannonExplosion.Play();
        _proyectil.target = currentTarget;
        _proyectil.cannonAgent = this;
        _proyectil.GetComponent<Rigidbody>().AddForce(_dirShoot * force, ForceMode.Impulse);
        _proyectil.GetComponent<Rigidbody>().useGravity = true;
    }

    public void Win()
    {
        floorRenderer.material = winMaterial;
    }

    public void Lose()
    {
        floorRenderer.material = loseMaterial;
    }

}

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

    [SerializeField] private float aimTime;
    float timetoShoot, timeDecidingShot;

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

    public override void OnEpisodeBegin()
    {
        //print("episode begin " + Time.time);
        timeDecidingShot = 0;
        shotsMade = 0;
        rotacionVertical.localRotation = rotacionVerticalInicial;
        rotacionHorizontal.localRotation = rotacionHorizontalInicial;
       
    }

    private void Start()
    {
        shotsMade = 0;
        currentTargetDead = true;
        timetoShoot = Time.time + aimTime;
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

        discreteAction[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //for (int i = 0; i < actions.ContinuousActions.Length; i++)
        //{
        //    if (actions.ContinuousActions[i] > 0)
        //    {
        //        print("Continous action " + i + " " + actions.ContinuousActions[i]);
        //    }

        //}
        //print("Discrete action " + actions.DiscreteActions[0]);

        if (Time.time > timetoShoot)
        {
            print("Deciding to shoot " );
            timeDecidingShot += Time.deltaTime;
            if (timeDecidingShot > aimTime*.333f)
            {
                timetoShoot = aimTime + Time.time;
            }
            print("discrete action " + actions.DiscreteActions[0]);
            if (actions.DiscreteActions[0] > 0 && shotsMade < totalShots)
            {
                shotsMade++;
                Disparar();
                
                timetoShoot = aimTime + Time.time;
            } else if (shotsMade >= totalShots)
            {
                Lose();
                EndEpisode();
            }
        }
        else
        {
            timeDecidingShot = 0;

            print("aiming");

            var verticalMove = actions.ContinuousActions[0];
            //print("Vertical move action " + verticalMove);
            Vector3 ear = rotacionVertical.transform.rotation.ToEulerAngles();
            if ((verticalMove > 0 && ear.z < 0f) ||
            (verticalMove < 0 && ear.z > -1.11f))
            {
                rotacionVertical.Rotate(0, 0, verticalMove * rotationFactor);
            }

            var horizontalMove = actions.ContinuousActions[1];

            if (horizontalMove > 0)
            {
                print(" horizontal > 0");
            }
            else
            {
                print("horizontal <= 0");
            }

            //print("Horizontal move action " + horizontalMove);
            rotacionHorizontal.Rotate(0, horizontalMove * rotationFactor, 0);   
        }

    }

    [ContextMenu("Disparar")]
    void Disparar()
    {
        _proyectil = Instantiate(projectilPrefab, cannonTip.parent.position, Quaternion.identity).GetComponent<Proyectil>();
        Vector3 _dirShoot = cannonTip.position - cannonTip.parent.position;

        psCannonExplosion.Play();
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

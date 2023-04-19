using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CannonAgent : Agent
{
    [SerializeField] private Transform currentTarget;
    [SerializeField] private float reloadCannonTime, radioTargetRandomPosicion;

    [Header("Variables para disparar")]
    [SerializeField] private Transform cannonTip;
    [SerializeField] private float force;
    //[SerializeField] private ParticleSystem psCannonExplosion;
    [SerializeField] private int totalShots;
    [SerializeField] private float shootDelay = 15.0f;
    [SerializeField] private ObjectPool projectilePool;

    public int shotsMade;

    private List<Proyectil> shootedProyectiles;

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
    private float timeSinceLastShot = 0f;

    float lowerD = 100f;
    Proyectil nearestProyectil = null;

    private BufferSensorComponent bufferSensor;
    private VectorSensorComponent vectorSensorGoal;

    public override void OnEpisodeBegin()
    {
        timeSinceLastShot = 0.0f;
        canAim = true;
        //canShoot = true;
        shotsMade = 0;
        rotacionVertical.localRotation = rotacionVerticalInicial;
        rotacionHorizontal.localRotation = rotacionHorizontalInicial;
        //print("Episode begin at "+Time.time+transform.parent.name);
    }

    private void Start()
    {
        canAim = true;
        canShoot = true;
        shotsMade = 0;
        rotacionHorizontalInicial = rotacionHorizontal.localRotation;
        rotacionVerticalInicial = rotacionVertical.localRotation;
        projectilePool = transform.parent.GetComponentInChildren<ObjectPool>();
        shootedProyectiles = new List<Proyectil>();
        bufferSensor = GetComponent<BufferSensorComponent>();
        vectorSensorGoal = GetComponent<VectorSensorComponent>();
    }

    private void Update()
    {

        // Update timer since last shot
        timeSinceLastShot += Time.deltaTime;

        // If enough time has passed without shooting, apply negative reward and allow shooting again
        if (timeSinceLastShot > shootDelay)
        {
            print("in " + transform.parent.name + " Negative reward for not shooting in " + ((int)timeSinceLastShot));

            timeSinceLastShot = 0.0f;
            AddReward(-0.0999f);
        }

        //if (shootedProyectiles.Count>0)
        //{
        //    print("altura primer proyectil disparado "+ shootedProyectiles[0].transform.position.y);
        //}
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        vectorSensorGoal.GetSensor().AddObservation(NormalizedTargetPosition(currentTarget.localPosition));

        Vector2 targetVel = Vector2.zero;
        if (currentTarget.TryGetComponent(out ObjetivoMovil om))
        {
            targetVel = new Vector2(om.velocity.x / om.speed, om.velocity.z / om.speed);
        }
        vectorSensorGoal.GetSensor().AddObservation(targetVel);

        float angleHorizontalRotation = rotacionHorizontal.localRotation.eulerAngles.y / 360f;
        sensor.AddObservation(angleHorizontalRotation);
        float angleVerticalRotation = Vector3.Angle(transform.forward, cannonTip.position - cannonTip.parent.position);
        //print("hor: "+ angleHorizontalRotation*360+" ver "+angleVerticalRotation);
        angleVerticalRotation = angleVerticalRotation / 90.0f;
        sensor.AddObservation(angleVerticalRotation);


        foreach (Proyectil p in shootedProyectiles)
        {
            var d = Vector3.Distance(currentTarget.localPosition, p.transform.localPosition);
            if (d < lowerD)
            {
                lowerD = d;
                nearestProyectil = p;
            }
            Vector3 pRelPos = NormalizedProjectilePositon(p.transform.localPosition);
            //Vector3 pRelPos = currentTarget.localPosition - p.transform.localPosition;
            //Vector3 pRelPos = p.transform.localPosition;
            //Vector3 pVel = p.GetComponent<Rigidbody>().velocity;
            //Vector3 pVel = NormalizedProjectileVelocity(p.GetComponent<Rigidbody>().velocity, 24.25f);
            float[] projectileObservation = new float[] { pRelPos.x, pRelPos.y, pRelPos.z/*, pVel.x, pVel.y, pVel.z */};
            bufferSensor.AppendObservation(projectileObservation);
        }
        #region Observaciones anteriores
        //if (nearestP != null)
        //{
        //    //Vector3 v = nearestP.GetComponent<Rigidbody>().velocity;
        //    var r = nearestP.DistanceToReward(lowerD);
        //    //print("nearest projectil of " + transform.parent.name + ": " + nearestP.name
        //    //    + " with velocity " + v
        //    //    + " distance " + lowerD
        //    //    + " Reward by dist " + r);
        //    //sensor.AddObservation(v);
        //    //sensor.AddObservation(NormalizedProjectileVelocity(v, 24.25f));
        //    AddReward(r);
        //}
        //else
        //{
        //    sensor.AddObservation(Vector3.zero);
        //} 
        #endregion

        Vector3 _dirShoot = cannonTip.position - cannonTip.parent.position;
        Vector3 _dirTarget = currentTarget.localPosition - transform.localPosition;

        //angulo entre la direccion de apuntado y direccion del objeivos
        float angle = Vector3.Angle(_dirShoot, _dirTarget);
        //print(angle);
        sensor.AddObservation(angle / 180.0f);//normalizo angulo entre [0,1]
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

        PointAndShoot(actions);

        if (nearestProyectil != null)
        {
            var r = nearestProyectil.DistanceToReward(lowerD);
            AddReward(r);
        }

        if (shotsMade >= totalShots)
        {
            Lose();
        }

    }

    void PointAndShoot(ActionBuffers actions)
    {
        AddReward(-0.0005f);
        if (canAim)
        {
            var verticalMove = actions.ContinuousActions[0];
            //print("Vertical move action " + verticalMove);
            Vector3 ear = rotacionVertical.transform.rotation.ToEulerAngles();
            if ((verticalMove > 0 && ear.z < 0f) ||
            (verticalMove < 0 && ear.z > -1.45333f))
            {
                rotacionVertical.Rotate(0, 0, verticalMove * rotationFactor);
            }

            var horizontalMove = actions.ContinuousActions[1];
            rotacionHorizontal.Rotate(0, horizontalMove * rotationFactor, 0);
        }
        if (canShoot && actions.DiscreteActions[0] > 0 && shotsMade < totalShots)
        {
            Invoke(nameof(Disparar), .111f);
            canAim = false;
            canShoot = false;
            Invoke(nameof(ActivateAim), .5f);
            Invoke(nameof(ActivateShoot), reloadCannonTime);
            shotsMade++;
            timeSinceLastShot = 0.0f;
            //Disparar();
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

    void Disparar()
    {
        Proyectil _proyectil = projectilePool.GetPooledObject().GetComponent<Proyectil>();
        _proyectil.transform.position = cannonTip.parent.position;
        _proyectil.gameObject.SetActive(true);
        Vector3 _dirShoot = cannonTip.position - cannonTip.parent.position;

        //psCannonExplosion.Play();
        _proyectil.cannonAgent = this;
        _proyectil.GetComponent<Rigidbody>().AddForce(_dirShoot * force, ForceMode.Impulse);
        _proyectil.GetComponent<Rigidbody>().useGravity = true;
        shootedProyectiles.Add(_proyectil);
    }

    public void DeleteShootedProjectile(Proyectil p)
    {
        if (shootedProyectiles.Contains(p))
        {
            //if (shootedProyectiles.Remove(p))
            //{
            //    print(p.name + " deja de estar en shooted proyectils ");
            //}
            //else
            //{
            //    print(p.name + " no se ha eliminado de shooted proyectils ");
            //}
            shootedProyectiles.Remove(p);
        }
        //else
        //{
        //    print(p.name + " no esta en shooted proyectils ");
        //}

    }

    public void Win()
    {
        ReturnAllShotsToPool();
        EndEpisode();

        //bool trueOrFalse = Random.value > 0.5f;
        //if (trueOrFalse)
        //{
        //    ChangeTarget();
        //}
        ChangeTarget();

        currentTarget.transform.localPosition = RandomPos();
        floorRenderer.material = winMaterial;
    }

    public void ChangeTarget()
    {
        int i;
        if (currentTarget.TryGetComponent(out ObjetivoMovil om))
        {
            om.enabled = false;
            i = 0;
        }
        else
        {
            i = 1;
        }
        currentTarget.gameObject.SetActive(false);
        currentTarget = currentTarget.parent.GetChild(i);
        currentTarget.gameObject.SetActive(true);
        if (i == 1)
        {
            currentTarget.GetComponent<ObjetivoMovil>().enabled = true;
        }
    }

    public void Lose()
    {
        //AddReward(-1);
        SetReward(-1);
        ReturnAllShotsToPool();
        EndEpisode();
        floorRenderer.material = loseMaterial;
    }

    [ContextMenu("Print posicion aleatoria")]
    void PrintRandomPos()
    {
        Vector3 p = RandomPos();
        print(p);
        currentTarget.transform.localPosition = p;
    }

    Vector3 RandomPos()
    {
        Vector3 pos = Random.insideUnitCircle;
        pos = new Vector3(pos.x, 0, pos.y);
        if (radioTargetRandomPosicion <= 12)
        {
            return pos * 12;
        }
        pos *= radioTargetRandomPosicion;
        float dist = Vector3.Distance(transform.position, pos);
        if (dist < 12)
        {
            return RandomPos();
        }
        else
        {
            return pos;
        }
    }

    void ReturnAllShotsToPool()
    {
        while (shootedProyectiles.Count > 0)
        {
            shootedProyectiles[0].BackToPool();
        }
    }

    Vector2 NormalizedTargetPosition(Vector3 targetPos)
    {
        float value = targetPos.x;
        float normalizedValueX = value / radioTargetRandomPosicion;
        value = targetPos.z;
        float normalizedValueZ = value / radioTargetRandomPosicion;
        return new Vector2(normalizedValueX, normalizedValueZ);
    }

    Vector3 NormalizedProjectilePositon(Vector3 projectilePos)
    {
        float value = projectilePos.x;
        float normalizedValueX = value / radioTargetRandomPosicion;
        value = projectilePos.z;
        float normalizedValueZ = value / radioTargetRandomPosicion;
        float normalizedValueY = projectilePos.y / 37.2f; //37.2 altura maxima de un projectile disparado hacia arriba con el mayor angulo permitido
        return new Vector3(normalizedValueX, normalizedValueY, normalizedValueZ);
    }

    Vector3 NormalizedProjectileVelocity(Vector3 projectileVel, float maxVel)
    {
        //obtener velocidad maxima maxVel por medio de una prueba en ejecucion, disparando un proyectil en ejecucion
        return new Vector3(projectileVel.x / maxVel, projectileVel.y / maxVel, projectileVel.z / maxVel);
    }
}

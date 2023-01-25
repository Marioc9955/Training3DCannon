using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class CannonShoot : MonoBehaviour
{
    [SerializeField] private Proyectil esferaPrefab;
    [SerializeField] private float fuerzaMax;
    [SerializeField] private LayerMask layerCanon, layerGround;
    [SerializeField] private Transform cannonTip;
    //[SerializeField] private Projection _projection;

    private Proyectil _esfera;
    private Vector3 _dirShoot;

    [SerializeField] private ParticleSystem psCannonExplosion;

    private void OnMouseDown()
    {
        _esfera = Instantiate(esferaPrefab, GetMouseWorldPosition(layerCanon), Quaternion.identity);
    }

    private void OnMouseDrag()
    {
        Vector3 pos = GetMouseWorldPosition(layerCanon);
        if (pos != Vector3.zero)
        {
            pos = PuntoMasCercanoRectaDisparo(pos);
            _esfera.transform.position = pos;
            _dirShoot = (cannonTip.position - _esfera.transform.position);
            //_projection.SimulateTrajectory(_esfera, pos, (_dirShoot * fuerzaMax));
            _esfera.transform.position = pos;
        }
        
    }

    private void OnMouseUp()
    {
        _dirShoot = (cannonTip.position - _esfera.transform.position);

        psCannonExplosion.Play();

        _esfera.GetComponent<Rigidbody>().AddForce(_dirShoot * fuerzaMax, ForceMode.Impulse);
        _esfera.GetComponent<Rigidbody>().AddTorque(500, 10, 20);
        _esfera.GetComponent<Rigidbody>().useGravity = true;
    }

    private Vector3 GetMouseWorldPosition(LayerMask lm)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, lm))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 PuntoMasCercanoRectaDisparo(Vector3 positionEsfera)
    {
        //obtengo direccion recta
        Vector3 ct = cannonTip.position;
        Vector3 dirRec = ct - cannonTip.parent.position;

        //obtengo ecuacion plano Ax + By + Cz = D Donde ABC dir recta y (x, y z) punto esfera para obtener D
        float D = Vector3.Dot(dirRec, positionEsfera);

        //float landa = (D - dirRec.x * ct.x - dirRec.y * ct.y) /(dirRec.x * dirRec.x);
        float landa = (D - Vector3.Dot(dirRec, ct)) / Vector3.Dot(dirRec, dirRec);

        return ct + landa * dirRec;
    }

}


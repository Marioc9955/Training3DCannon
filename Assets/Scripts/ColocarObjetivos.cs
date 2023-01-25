using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColocarObjetivos : MonoBehaviour
{
    [SerializeField] private int numObjetivos;
    [SerializeField] private GameObject prefabObjetivo;

    [SerializeField] private float largo, ancho;


    [ContextMenu("Crear objetivos")]
    public void CrearObjetivos()
    {
        GameObject padre = new GameObject("Objetivos");
        //padre = Instantiate(padre, transform.position, Quaternion.identity);
        for (int i = 0; i < numObjetivos; i++)
        {
            var x = Random.Range(-largo / 2f, largo / 2f);
            var z = Random.Range(-ancho / 2f, ancho / 2f);
            Vector3 pos = new Vector3(x, 0, z);

            Instantiate(prefabObjetivo, pos + transform.position, Quaternion.identity, padre.transform);
        }
        
    }
}

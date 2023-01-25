using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Animator animator;

    private Rigidbody[] rbsRagdoll;
    private Collider[] collRagdoll;

    private Collider colliderPadre;
    private Rigidbody rbPadre;

    //public float velMax;
    void Start()
    {
        colliderPadre = GetComponent<Collider>();
        rbPadre = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();

        rbsRagdoll = transform.GetComponentsInChildren<Rigidbody>();
        collRagdoll = transform.GetComponentsInChildren<Collider>();

        foreach (Collider coll in collRagdoll)
        {
            Physics.IgnoreCollision(coll, colliderPadre);
        }

        SetEnabled(false);
    }

    [ContextMenu("Habilitar")]
    public void Habilitar()
    {
        SetEnabled(true);
    }

    [ContextMenu("Deshabilitar")]
    public void Deshabilitar()
    {
        SetEnabled(false);
    }


    public void SetEnabled(bool enabled)
    {
        bool isKinematic = !enabled;
        foreach (Rigidbody rigidbody in rbsRagdoll)
        {
            rigidbody.isKinematic = isKinematic;
        }

        animator.enabled = !enabled;

        rbPadre.isKinematic = enabled;

        colliderPadre.enabled = !enabled;

    }

    void Update()
    {

        //if (Keyboard.current.rKey.wasReleasedThisFrame)
        //{
        //    print("r");
        //    SetEnabled(true);
        //}
        //if (Keyboard.current.tKey.wasPressedThisFrame)
        //{
        //    print("t");
        //    SetEnabled(false);
        //}

        //if (rbPadre.velocity.magnitude > velMax || rbPadre.velocity.magnitude < -velMax)
        //{
        //    SetEnabled(true);
        //}
    }



}
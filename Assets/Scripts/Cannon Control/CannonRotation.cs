using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CannonRotation : MonoBehaviour
{
    [SerializeField] private float rotationFactor;
    [SerializeField] private Transform objectToRotate;
    [SerializeField] private bool rotationVertical;


    //public void Rotate(InputAction.CallbackContext ctx)
    //{
    //    print(ctx.ReadValue<Vector2>());
    //}

    private Vector3 mouseCurrentPosition, mouseLastPosition, mouseDeltaPosition;


    //private void Update()
    //{
    //    mouseCurrentPosition = Input.mousePosition;
        
        
    //    if (rotationVertical)
    //    {
    //        var verticalMove = Input.GetAxis("Vertical");
    //        Vector3 ear = objectToRotate.transform.rotation.ToEulerAngles();
    //        if ((verticalMove > 0 && ear.z < 0.25f) ||
    //        (verticalMove < 0 && ear.z > -1.0f))
    //        {
    //            objectToRotate.Rotate(0, 0, verticalMove * rotationFactor);
    //        }
    //    }
    //    else
    //    {
    //        var horizontalMove = Input.GetAxis("Horizontal");
    //        objectToRotate.Rotate(0, horizontalMove * rotationFactor, 0);
    //    }
    //}

    private void OnMouseDrag()
    {
        mouseLastPosition = Input.mousePosition;
        float deltaPosition;
        
        //print(objectToRotate.transform.rotation.ToEulerAngles() + " " + objectToRotate.name);
        float angle;
        Vector3 axis;

        Quaternion quat = objectToRotate.transform.rotation.normalized;
        quat.ToAngleAxis(out angle, out axis);
        //print(angle + " " + axis);
        if (rotationVertical)
        {
            //deltaPosition = Input.GetTouch(0).deltaPosition.y;
            if (Touchscreen.current != null && Application.isMobilePlatform)
            {
                deltaPosition = UnityEngine.InputSystem.Touchscreen.current.delta.ReadValue().y;
            }
            else
            {
                //deltaPosition = Mouse.current.delta.ReadValue().y;
                mouseDeltaPosition = mouseCurrentPosition - mouseLastPosition;
                deltaPosition = mouseDeltaPosition.y;
            }
            
            Vector3 ear = objectToRotate.transform.rotation.ToEulerAngles();
            if ((deltaPosition > 0 && ear.z < 0.25f) ||
            (deltaPosition < 0 && ear.z > -1.0f))
            {
                objectToRotate.Rotate(0, 0, deltaPosition * rotationFactor);
            }
        }
        else
        {
            if (Touchscreen.current != null && Application.isMobilePlatform)
            {
                deltaPosition = Touchscreen.current.delta.ReadValue().x;
            }
            else
            {
                //deltaPosition = Mouse.current.delta.ReadValue().x;
                mouseDeltaPosition = mouseCurrentPosition - mouseLastPosition;
                deltaPosition = mouseDeltaPosition.x;
            }
                
            //deltaPosition = Input.GetTouch(0).deltaPosition.x;
            //if ((deltaPosition > 0 && objectToRotate.transform.rotation.eulerAngles.y < 220) ||
            //    (deltaPosition < 0 && objectToRotate.transform.rotation.eulerAngles.y > 140))
            {
                objectToRotate.Rotate(0, deltaPosition * rotationFactor, 0);
            }
        }

       
        
    }
}

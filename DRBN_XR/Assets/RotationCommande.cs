using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationCommande : MonoBehaviour
{
    public GameObject Rotation;
    public string axe;
    public int Sens = 1;


    void Update()
    {
        //float input = Input.GetAxis("Fleche");
        float input = Input.GetAxis(axe);
        EtatRotation rotationState = MoveStateForInput(input);
        RotationControleur controller = Rotation.GetComponent<RotationControleur>();
        controller.rotationState = rotationState;

        
    }

    //EtatRotation MoveStateForInput(float input)
    //{
    //    if (input > 0)
    //    {
    //        return EtatRotation.MovingUp;
    //    }
    //    else if (input < 0)
    //    {
    //        return EtatRotation.MovingDown;
    //    }
    //    else
    //    {
    //        return EtatRotation.Fixed;
    //    }
    //}

    EtatRotation MoveStateForInput(float input)
    {
        if (Sens == 1)
        {
            if (input > 0)
            {
                return EtatRotation.MovingUp;
            }
            else if (input < 0)
            {
                return EtatRotation.MovingDown;
            }
            else
            {
                return EtatRotation.Fixed;
            }
        }
        else if (Sens == -1)
        {
            if (input < 0)
            {
                return EtatRotation.MovingUp;
            }
            else if (input > 0)
            {
                return EtatRotation.MovingDown;
            }
            else
            {
                return EtatRotation.Fixed;
            }
        }
        else
        {
            return EtatRotation.Fixed;
        }
    }
}

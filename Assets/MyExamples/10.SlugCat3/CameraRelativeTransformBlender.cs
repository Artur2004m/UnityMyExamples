using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


[Serializable]
public class CameraRelativeTransformBlenderTarget
{
    public Vector3 targetDirection;
    public Transform transform;
    public Vector3 newPosition;
    public Vector3 newRotation;
}
public class CameraRelativeTransformBlender : MonoBehaviour
{
    public bool test;
    public AnimationCurve interpolationCurve;
    public CameraRelativeTransformBlenderTarget[] targets;

    private void OnValidate()
    {
        if (test)
        {

            test = false;
        }
    }
}

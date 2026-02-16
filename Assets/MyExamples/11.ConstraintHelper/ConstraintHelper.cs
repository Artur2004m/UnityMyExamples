using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyExamples.ConstraintHelper
{
    public class ConstraintHelper : MonoBehaviour
    {
        [Header("buttons----")]
        public bool moveToEditModeTransform;
        public bool moveToPlayModeTransform;
        public bool setPlayModeTransform;
        public bool setEditModeTransform;
        [Header("properties----")]
        public Transform parent;
        public Transform constraintTarget;
        [Header("read-only----")]
        public Vector3 editModeLocalPosition;      
        public Vector3 editModeLocalRotation;      
        public Vector3 playModeLocalPosition;
        public Vector3 playModeLocalRotation;

        public ConstraintHelperMirror mirror;
        public ConstraintHelperCopy copy;
        public ConstraintHelperGizmos gizmos;
        private void OnValidate()
        {
            if (setPlayModeTransform)
            {
                if (parent != null)
                {
                    playModeLocalPosition = parent.InverseTransformPoint(transform.position);
                    playModeLocalRotation = (Quaternion.Inverse(parent.rotation) * transform.rotation).eulerAngles;
                }
                else
                {
                    playModeLocalPosition = transform.position;
                    playModeLocalRotation = transform.rotation.eulerAngles;
                }
                Debug.Log("play mode transform setted (local to parent)");
                setPlayModeTransform = false;
            }
            if (setEditModeTransform)
            {
                if (parent != null)
                {
                    editModeLocalPosition = parent.InverseTransformPoint(transform.position);
                    editModeLocalRotation = (Quaternion.Inverse(parent.rotation) * transform.rotation).eulerAngles;
                }
                else
                {
                    editModeLocalPosition = transform.position;
                    editModeLocalRotation = transform.rotation.eulerAngles;
                }
                Debug.Log("edit mode transform setted (local to parent)");
                setEditModeTransform = false;
            }
            if (moveToEditModeTransform)
            {
                if (parent != null)
                {
                    transform.position = parent.TransformPoint(editModeLocalPosition);
                    transform.rotation = parent.rotation * Quaternion.Euler(editModeLocalRotation);
                }
                else
                {
                    transform.position = editModeLocalPosition;
                    transform.rotation = Quaternion.Euler(editModeLocalRotation);
                }
                Debug.Log("current transform setted to editModeVariant");
                moveToEditModeTransform = false;
            }
            if (moveToPlayModeTransform)
            {
                if (parent != null)
                {
                    transform.position = parent.TransformPoint(playModeLocalPosition);
                    transform.rotation = parent.rotation * Quaternion.Euler(playModeLocalRotation);
                }
                else
                {
                    transform.position = playModeLocalPosition;
                    transform.rotation = Quaternion.Euler(playModeLocalRotation);
                }
                Debug.Log("current transform setted to playModeVariant");
                moveToPlayModeTransform = false;
            }
            copy.OnValidate(this);
            mirror.OnValidate(this);
        }

        private void OnDrawGizmosSelected()
        {
            gizmos.OnDrawGizmosSelected(
                editModeLocalPosition,
                editModeLocalRotation,
                transform.position,
                transform.rotation.eulerAngles,
                playModeLocalPosition,
                playModeLocalRotation,
                parent.transform
                );
        }

        public void MoveToPlayModeTransform()
        {
            if (constraintTarget == null) return;

            if (parent != null)
            {
                constraintTarget.position = parent.TransformPoint(playModeLocalPosition);
                constraintTarget.rotation = parent.rotation * Quaternion.Euler(playModeLocalRotation);
            }
            else
            {
                constraintTarget.position = playModeLocalPosition;
                constraintTarget.rotation = Quaternion.Euler(playModeLocalRotation);
            }
        }
        public void MoveToEditModeTransform()
        {
            if (constraintTarget == null) return;

            if (parent != null)
            {
                constraintTarget.position = parent.TransformPoint(editModeLocalPosition);
                constraintTarget.rotation = parent.rotation * Quaternion.Euler(editModeLocalRotation);
            }
            else
            {
                constraintTarget.position = editModeLocalPosition;
                constraintTarget.rotation = Quaternion.Euler(editModeLocalRotation);
            }
        }
    }
}
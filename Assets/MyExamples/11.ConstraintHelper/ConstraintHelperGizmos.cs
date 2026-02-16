using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MyExamples.ConstraintHelper
{
    [Serializable]
    public class ConstraintHelperGizmos
    {
        public bool enable = true;
        [SerializeField] bool drawText = true;
        [SerializeField] bool drawPositions = true;
        [SerializeField] bool drawRotations = true;
        [SerializeField] bool drawCurrent = true;
        [SerializeField] bool drawEditMode = true;
        [SerializeField] bool drawPlayMode = true;

        [SerializeField] Vector3 textOffset = new Vector3(0, 0.15f, 0);
        [SerializeField] float directionLineLenght = 1f;

        Color currentColor = Color.gray;
        Color editModeColor = Color.blue;
        Color playModeColor = Color.red;
        public void OnDrawGizmosSelected(
            Vector3 editModePosition,
            Vector3 editModeRotation,
            Vector3 currentPosition,
            Vector3 currentRotation,
            Vector3 playModePosition,
            Vector3 playModeRotation,
            Transform parent
            )
        {
            if (!enable) return;

            Vector3 WorldPosition(Vector3 localPos) => parent ? parent.TransformPoint(localPos) : localPos;
            Quaternion WorldRotation(Vector3 localEuler) => parent ? parent.rotation * Quaternion.Euler(localEuler) : Quaternion.Euler(localEuler);

            if (drawEditMode)
            {
                Vector3 worldPos = WorldPosition(editModePosition);
                Quaternion worldRot = WorldRotation(editModeRotation);

                if (drawPositions)
                {
                    Gizmos.color = editModeColor;
                    Gizmos.DrawSphere(worldPos, 0.1f); 
                }

                if (drawRotations)
                {
                    Handles.color = editModeColor;
                    Handles.ArrowHandleCap(0, worldPos, worldRot, directionLineLenght, EventType.Repaint);
                }

                if (drawText)
                {
                    Handles.color = editModeColor;
                    Handles.Label(worldPos + textOffset, "EditModeTransform");
                }
            }

            if (drawPlayMode)
            {
                Vector3 worldPos = WorldPosition(playModePosition);
                Quaternion worldRot = WorldRotation(playModeRotation);

                if (drawPositions)
                {
                    Gizmos.color = playModeColor;
                    Gizmos.DrawSphere(worldPos, 0.1f);
                }

                if (drawRotations)
                {
                    Handles.color = playModeColor;
                    Handles.ArrowHandleCap(0, worldPos, worldRot, directionLineLenght, EventType.Repaint);
                }

                if (drawText)
                {
                    Handles.color = playModeColor;
                    Handles.Label(worldPos + textOffset, "PlayModeTransform");
                }
            }

            if (drawCurrent)
            {
                Vector3 worldPos = currentPosition;
                Quaternion worldRot = Quaternion.Euler(currentRotation);

                if (drawPositions)
                {
                    Gizmos.color = currentColor;
                    Gizmos.DrawSphere(worldPos, 0.1f);
                }

                if (drawRotations)
                {
                    Handles.color = currentColor;
                    Handles.ArrowHandleCap(0, worldPos, worldRot, directionLineLenght, EventType.Repaint);
                }

                if (drawText)
                {
                    Handles.color = currentColor;
                    Handles.Label(worldPos + textOffset, "CurrentTransform");
                }
            }
        }
    }
}
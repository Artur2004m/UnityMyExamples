using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyExamples.ConstraintHelper
{
    [Serializable]
    public class ConstraintHelperMirror
    {   
        public TransformStateSelection transformStateSelection = TransformStateSelection.PlayMode; //Current, PlayMode, EditMode
        [Header("buttons----")]
        public bool mirror_x_rotation;
        public bool mirror_y_rotation;
        public bool mirror_z_rotation;

        public void OnValidate(ConstraintHelper current)
        {
            if (mirror_x_rotation)
            {
                MirrorRotation(current, Axis.X);
                mirror_x_rotation = false;
            }
            if (mirror_y_rotation)
            {
                MirrorRotation(current, Axis.Y);
                mirror_y_rotation = false;
            }
            if (mirror_z_rotation)
            {
                MirrorRotation(current, Axis.Z);
                mirror_z_rotation = false;
            }
        }

        private enum Axis { X, Y, Z }

        private void MirrorRotation(ConstraintHelper current, Axis axis)
        {
            Vector3 targetEuler;
            switch (transformStateSelection)
            {
                case TransformStateSelection.Current:
                    targetEuler = current.parent != null
                        ? current.transform.localEulerAngles
                        : current.transform.eulerAngles;
                    break;
                case TransformStateSelection.EditMode:
                    targetEuler = current.editModeLocalRotation;
                    break;
                case TransformStateSelection.PlayMode:
                    targetEuler = current.playModeLocalRotation;
                    break;
                default:
                    return;
            }

            Vector3 mirrored = MirrorEuler(targetEuler, axis);

            switch (transformStateSelection)
            {
                case TransformStateSelection.Current:
                    if (current.parent != null)
                        current.transform.localEulerAngles = mirrored;
                    else
                        current.transform.eulerAngles = mirrored;
                    break;
                case TransformStateSelection.EditMode:
                    current.editModeLocalRotation = mirrored;
                    break;
                case TransformStateSelection.PlayMode:
                    current.playModeLocalRotation = mirrored;
                    break;
            }

            // Отмечаем объект как изменённый для сохранения в редакторе
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(current);
#endif
        }
        private Vector3 MirrorEuler(Vector3 euler, Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return new Vector3(euler.x, -euler.y, -euler.z);
                case Axis.Y: return new Vector3(-euler.x, euler.y, -euler.z);
                case Axis.Z: return new Vector3(-euler.x, -euler.y, euler.z);
                default: return euler;
            }
        }
    }
}


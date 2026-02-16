using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyExamples.ConstraintHelper
{
    public enum CopyTypeSelection
    {
        Both,
        Rotation,
        Position
    }
    public enum TransformStateSelection
    {
        Current,
        EditMode,
        PlayMode
    }

    [Serializable]
    public class ConstraintHelperCopy
    {
        public bool copyButton;
        public ConstraintHelper copySource;
        public CopyTypeSelection copySelection = CopyTypeSelection.Rotation;
        public TransformStateSelection transformStateSelection = TransformStateSelection.PlayMode;
        public void OnValidate(ConstraintHelper current)
        {
            if (copyButton)
            {
                if (copySource == null)
                {
                    Debug.Log("first off select reference");
                }
                if (copySource != null)
                {
                    if (copySelection == CopyTypeSelection.Both)
                    {
                        if (transformStateSelection == TransformStateSelection.Current)
                        {
                            current.transform.position = copySource.transform.position;
                            current.transform.rotation = copySource.transform.rotation;
                        }
                        if (transformStateSelection == TransformStateSelection.PlayMode)
                        {
                            current.playModeLocalPosition = copySource.playModeLocalPosition;
                            current.playModeLocalRotation = copySource.playModeLocalRotation;
                        }
                        if (transformStateSelection == TransformStateSelection.EditMode)
                        {
                            current.editModeLocalPosition = copySource.editModeLocalPosition;
                            current.editModeLocalRotation = copySource.editModeLocalRotation;
                        }
                        Debug.Log($"[{transformStateSelection}] succesfully copied rotation and position");
                    }
                    if (copySelection == CopyTypeSelection.Position)
                    {
                        if (transformStateSelection == TransformStateSelection.Current)
                        {
                            current.transform.position = copySource.transform.position;
                        }
                        if (transformStateSelection == TransformStateSelection.PlayMode)
                        {
                            current.playModeLocalPosition = copySource.playModeLocalPosition;
                        }
                        if (transformStateSelection == TransformStateSelection.EditMode)
                        {
                            current.editModeLocalPosition = copySource.editModeLocalPosition;
                        }
                        Debug.Log($"[{transformStateSelection}] succesfully copied position");
                    }
                    if (copySelection == CopyTypeSelection.Rotation)
                    {
                        if (transformStateSelection == TransformStateSelection.Current)
                        {
                            current.transform.rotation = copySource.transform.rotation;
                        }
                        if (transformStateSelection == TransformStateSelection.PlayMode)
                        {
                            current.playModeLocalRotation = copySource.playModeLocalRotation;
                        }
                        if (transformStateSelection == TransformStateSelection.EditMode)
                        {
                            current.editModeLocalRotation = copySource.editModeLocalRotation;
                        }
                        Debug.Log($"[{transformStateSelection}] succesfully copied rotation");
                    }
                }
                copyButton = !copyButton;
            }
        }
    }
}


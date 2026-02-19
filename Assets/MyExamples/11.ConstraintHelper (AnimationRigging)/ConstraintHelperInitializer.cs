using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyExamples.ConstraintHelper
{
    public class ConstraintHelperInitializer : MonoBehaviour
    {
        [Header("buttons----")]
        public bool findInChilds;
        public bool setParentForAll;
        public bool autoSetCurrentTransforms;
        public bool enableGizmosForAll;
        public bool disableGizmosForAll;
        [Header("[careful]buttons----")]
        public bool moveAllToPlayModeTransforms;
        public bool moveAllToEditModeTransforms;
        [Header("references----")]
        public Transform parent;
        [Header("read-only----")]
        public List<ConstraintHelper> constraintHelperMap;

        private void OnValidate()
        {
            if (findInChilds)
            {
                if (constraintHelperMap == null) constraintHelperMap = new List<ConstraintHelper>();
                else constraintHelperMap.Clear();

                ConstraintHelper[] foundHelpers = GetComponentsInChildren<ConstraintHelper>(true);

                constraintHelperMap.AddRange(foundHelpers);

                if (foundHelpers.Length > 0)
                {
                    Debug.Log($"found: {foundHelpers.Length} ConstraintHelper in childs");
                }
                else
                {
                    Debug.Log("ConstraintHelper not found");
                }

                findInChilds = !findInChilds;
            }
            if (setParentForAll)
            {
                if (constraintHelperMap == null) return;
                foreach (var helper in constraintHelperMap)
                {
                    helper.parent = parent;
                }
                setParentForAll = !setParentForAll;
            }
            if (enableGizmosForAll)
            {
                if (constraintHelperMap == null) return;
                foreach (var helper in constraintHelperMap)
                {
                    helper.gizmos.enable = true;
                }
                Debug.Log("Enabled gizmos for all");
                enableGizmosForAll = !enableGizmosForAll;
            }
            if (disableGizmosForAll)
            {
                if (constraintHelperMap == null) return;
                foreach (var helper in constraintHelperMap)
                {
                    helper.gizmos.enable = false;
                }
                Debug.Log("Disabled gizmos for all");
                disableGizmosForAll = !disableGizmosForAll;
            }
            if (autoSetCurrentTransforms)
            {
                if (constraintHelperMap == null) return;
                foreach (var helper in constraintHelperMap)
                {
                    helper.constraintTarget = helper.transform;
                }
                autoSetCurrentTransforms = !autoSetCurrentTransforms;
            }

            if (moveAllToPlayModeTransforms)
            {
                if (constraintHelperMap == null) return;
                foreach (var helper in constraintHelperMap)
                {
                    helper.MoveToPlayModeTransform();
                }
                Debug.Log("moved all to play mode transform variant");
                moveAllToPlayModeTransforms = false;
            }
            if (moveAllToEditModeTransforms)
            {
                if (constraintHelperMap == null) return;
                foreach (var helper in constraintHelperMap)
                {
                    helper.MoveToEditModeTransform();
                }
                Debug.Log("moved all to edit mode transform variant");
                moveAllToEditModeTransforms = false;
            }
        }
        private void Start()
        {
            if (constraintHelperMap == null) return;
            foreach (var helper in constraintHelperMap)
            {
                helper.MoveToPlayModeTransform();
            }
        }
    }

}
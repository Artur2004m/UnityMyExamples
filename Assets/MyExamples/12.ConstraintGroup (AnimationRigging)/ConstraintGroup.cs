using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace MyExamples.ConstraintGroup
{
    public class ConstraintGroup : MonoBehaviour
    {
        [Range(0, 1)] public float weight;
        public bool findInChilds;
        public List<MonoBehaviour> constraints;
        public List<IRigConstraint> constraintsRuntime;

        public void OnValidate()
        {
            if (findInChilds)
            {
                var compontents = GetComponentsInChildren<IRigConstraint>();
                constraints.Clear();

                foreach (var compontent in compontents)
                {
                    if (compontent is MonoBehaviour mono) constraints.Add(mono);
                }

                findInChilds = false;
            }
        }
        public void Awake()
        {
            FillRuntimeMap();
        }
        public void Update()
        {
            if (constraintsRuntime != null) foreach (var c in constraintsRuntime) c.weight = weight;
        }
        private void FillRuntimeMap()
        {
            constraintsRuntime = new();
            foreach (var constraint in constraints)
            {
                if (constraint is IRigConstraint correct) constraintsRuntime.Add(correct);
            }
        }
    }
}
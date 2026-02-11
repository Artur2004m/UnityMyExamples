using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyExamples.SlugCat
{
    public class PosSetter : MonoBehaviour
    {
        public Transform target;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        private void Start()
        {
            target.transform.position = position;
            target.transform.rotation = Quaternion.Euler(rotation);
            target.transform.localScale = scale;
        }
    }
}
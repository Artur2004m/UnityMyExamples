using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyExamples.HitboxHurtbox
{
    public class Hurtbox : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Hitbox") return;

            Hitbox hitboxComponent = other.GetComponent<Hitbox>();
            
        }
    }
}
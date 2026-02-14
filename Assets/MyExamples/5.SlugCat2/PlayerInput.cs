using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyExamples.SlugCat2
{


    [Serializable]
    public class PlayerInput
    {
        public KeyCode crouch = KeyCode.Q;
        public bool crouch_down;
        public bool crouch_up;
        public bool crouch_held;

        public void Update()
        {
            if (Input.GetKey(crouch)) crouch_held = true;
            else crouch_held = false;
            if (Input.GetKeyDown(crouch)) crouch_down = true;
            else crouch_down = false;
            if (Input.GetKeyUp(crouch)) crouch_up = true;
            else crouch_up = false;
        }
    }
}
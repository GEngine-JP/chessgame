﻿using UnityEngine;

namespace Assets.Scripts.Game.toubao
{
    public class SelectAnimation : MonoBehaviour
    {

        public GameObject SelectBig;
       public void Update () {
            SelectBig.transform.Rotate(0, 0, -400 * Time.deltaTime);
        }
    }
}

﻿using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public class SwatAnim : MonoBehaviour
    {

        public System.Action Onfinish;

        public void OnFinish()
        {
            gameObject.SetActive(false);
            if (Onfinish != null)
            {
                Onfinish();
            }
        }

        public void ShowSwatAnim()
        {
            gameObject.SetActive(true);
        }
    }
}
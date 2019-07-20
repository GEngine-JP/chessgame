using Assets.Scripts.Game.LX.SixLionCommon;
using Assets.Scripts.Game.lswc.Core;
using Assets.Scripts.Game.lswc.Data;
using UnityEngine;
using System.Collections.Generic;

public class LSChangeParticalColor:InstanceControl
{
    private static LSChangeParticalColor _instance;

    public static LSChangeParticalColor Instance
    {
        get { return _instance; }
    }
        public float ChangeInterval;
        public float ChangeTime;
        public float ChangeForm, ChangeTo, Delaytime;
        public Texture[] normal;
        public Texture[] specal;
        bool ChangeColorIsOver = false;
        LSColorType LastestColor = LSColorType.DEFAULT, CurruntColor = LSColorType.DEFAULT;

        void Awake()
        {

            _instance = this;

          
        }
        void OnEnable()
        {
            BeginToShowEffect();
            ChangeColorIsOver = false;
            LastestColor = LSColorType.DEFAULT;
        }

        int dout = 0;

        void changeColor()
        {

            if (ChangeColorIsOver)
            {

                if (CurruntColor == LastestColor)
                    return;
                else
                {
                    CurruntColor = LastestColor;
                }


            }
            else
            {

                int c = ++dout % 3 + 1;

                LSColorType ct = (LSColorType)c;
                CurruntColor = ct;
            }


        }

        float t = 0;

        bool TimeOut()
        {

            t += Time.deltaTime;
            if (t >= ChangeInterval)
            {

                t = 0;
                return true;
            }
            return false;

        }

        public void ReSetPosition(float space)
        {
        }

        public void BeginToShowEffect()
        {
        }

        int cycle = 0;

        void CompleteOne()
        {
            cycle++;

            if (cycle % 2 == 0)
            {


                changeColor();

            }


        }

        void OnDisable()
        {

            //iTween.StopByName("BeginToShowEffect");
        }

        public void OveredChange(LSColorType ct)
        {

            ChangeColorIsOver = true;
            LastestColor = ct;
        }

    public override void OnExit()
    {
        _instance = null;
    }
}

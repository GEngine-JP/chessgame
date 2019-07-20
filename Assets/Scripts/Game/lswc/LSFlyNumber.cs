using Assets.Scripts.Game.lswc.Control.System;
using Assets.Scripts.Game.lswc.Data;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Game.lswc
{
    public class LSFlyNumber : MonoBehaviour
    {
        public void Reset()
        {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// NGUI的tween不知道出什么问题了？，还是用dotween吧
        /// </summary>
        public void PlayAnimation()
        {
            Sequence mySequence = DOTween.Sequence();
            gameObject.SetActive(true);
            //
            //Vector3 moTo=new Vector3(0,transform.localPosition.y-75,0);
            //TweenPosition tPos=TweenPosition.Begin(gameObject, 1,moTo);
            //LSSystemControl.Instance.PlayVoice(LSConstant.LabelDown);
            //tPos.PlayForward();
            //tPos.AddOnFinished(
            //    delegate()
            //        {
            //            Destroy(tPos);
            //            Quaternion roTo=new Quaternion(0,180,-30,0);
            //            TweenRotation tRota = TweenRotation.Begin(gameObject, 2, roTo);
            //            tRota.AddOnFinished(
            //                delegate()
            //                    {
            //                        Destroy(tRota);
            //                        Vector3 moTo2 = (LSCameraManager.Instance.transform.position - transform.position);
            //                        TweenPosition tPos2 = TweenPosition.Begin(gameObject, 3, moTo2);
            //                        LSSystemControl.Instance.PlayVoice(LSConstant.LabelOut);
            //                        tPos2.PlayForward();
            //                    });

            //        });
            Vector3 moTo=new Vector3(0,transform.localPosition.y-75,0);
            Tweener t1 = transform.DOLocalMove(moTo, 1);
            t1.SetEase(Ease.InOutElastic);
            t1.OnStart(PlayLabelDownVoice);
            Vector3 roTo = new Vector3(0, 180, -30);
            Tweener t2 = transform.DORotate(roTo, 2);
            Vector3 moTo2 = (LSCameraManager.Instance.transform.position - transform.position) * Mathf.Cos(15 * Mathf.Deg2Rad) * 1.1f + this.transform.position;
            Tweener t3 = transform.DOMove(moTo2, 3);
            t3.SetEase(Ease.OutExpo);
            t3.OnStart(PlayLabelOutVoice);
            t3.OnComplete(Reset);
            mySequence.Append(t1);
            mySequence.Append(t2);
            mySequence.AppendInterval(1);
            mySequence.Append(t3);
        }

        private void PlayLabelDownVoice()
        {
            LSSystemControl.Instance.PlayVoice(LSConstant.LabelDown);
        }

        private void PlayLabelOutVoice()
        {
            LSSystemControl.Instance.PlayVoice(LSConstant.LabelOut);
        }

    }
}

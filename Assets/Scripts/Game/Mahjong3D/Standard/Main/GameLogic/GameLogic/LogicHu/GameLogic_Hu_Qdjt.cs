﻿using System;
using Sfs2X.Entities.Data;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Hu : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.MJReqTypeLastCd, GameKey = GameMisc.QdjtKey)]
        public void OnHu_LastCd_Qdjt(ISFSObject data)
        {
            SetHuMusicFunc();
            OnHu_LastCd(data);
        }

        [S2CResponseHandler(NetworkProtocol.MJReqTypeZiMo, GameKey = GameMisc.QdjtKey)]
        public void OnHu_Zimo_Qdjt(ISFSObject data)
        {
            SetHuMusicFunc();
            OnHu_Zimo(data);
        }

        [S2CResponseHandler(NetworkProtocol.MJRequestTypeHu, GameKey = GameMisc.QdjtKey)]
        public void OnHu_Dianpao_Qdjt(ISFSObject data)
        {
            SetHuMusicFunc();
            OnHu_Dianpao(data);
        }

        private void SetHuMusicFunc()
        {
            if (HuMusicFunc == null)
            {
                HuMusicFunc = (value) =>
                {
                    string AudioName = "";
                    List<QdjtHuType> EnHuTypeList = new List<QdjtHuType>();
                    foreach (QdjtHuType item in Enum.GetValues(typeof(QdjtHuType)))
                    {
                        if (((int)item & value) == (int)item)
                        {
                            EnHuTypeList.Add(item);
                        }
                    }
                    if (EnHuTypeList.Contains(QdjtHuType.qingyise))
                    {
                        if (EnHuTypeList.Count != 2)
                        {
                            AudioName = QdjtHuType.qingyise.ToString();
                        }
                        else
                        {
                            int typeValue = 0;
                            foreach (QdjtHuType type in EnHuTypeList)
                            {
                                typeValue += (int)type;
                            }
                            switch (typeValue)
                            {
                                case (int)QdjtHuType.qingyise + (int)QdjtHuType.kawuxing:
                                    AudioName = QdjtHuType.qingyise.ToString() + QdjtHuType.kawuxing;
                                    break;
                                case (int)QdjtHuType.qingyise + (int)QdjtHuType.longqidui:
                                    AudioName = QdjtHuType.qingyise.ToString() + QdjtHuType.longqidui;
                                    break;
                                case (int)QdjtHuType.qingyise + (int)QdjtHuType.mingsigui:
                                    AudioName = QdjtHuType.qingyise.ToString() + QdjtHuType.mingsigui;
                                    break;
                                case (int)QdjtHuType.qingyise + (int)QdjtHuType.pengpenghu:
                                    AudioName = QdjtHuType.qingyise.ToString() + QdjtHuType.pengpenghu;
                                    break;
                                case (int)QdjtHuType.qingyise + (int)QdjtHuType.qidui:
                                    AudioName = QdjtHuType.qingyise.ToString() + QdjtHuType.qidui;
                                    break;
                                case (int)QdjtHuType.qingyise + (int)QdjtHuType.ansigui:
                                    AudioName = QdjtHuType.qingyise.ToString() + QdjtHuType.ansigui;
                                    break;
                                default:
                                    AudioName = QdjtHuType.qingyise.ToString();
                                    break;
                            }
                        }
                    }
                    else if (EnHuTypeList.Count != 1)
                    {
                        AudioName = "";
                    }
                    else
                    {
                        AudioName = EnHuTypeList[0].ToString();
                        AudioName = AudioName == "jiansi" ? "" : AudioName;
                    }
                    return AudioName;
                };
            }
        }

        public enum QdjtHuType
        {
            jiansi = 1,
            shuanlongqidui = 1 << 2,
            longqidui = 1 << 3,
            ansigui = 1 << 4,
            qingyise = 1 << 5,
            qidui = 1 << 6,
            kawuxing = 1 << 10,
            pengpenghu = 1 << 11,
            mingsigui = 1 << 12,
            xiaosanyuan = 1 << 15,
            dasanyuan = 1 << 14,
            gangshangkaihua = 1 << 16,
            qiangganghu = 1 << 31,
            gangshangpao = 1 << 18,
        }
    }
}

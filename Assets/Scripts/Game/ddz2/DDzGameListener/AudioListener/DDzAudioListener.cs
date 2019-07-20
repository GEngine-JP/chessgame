using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.AudioListener
{
    public class DDzAudioListener : ServEvtListener
    {
        /// <summary>
        /// 语言种类，p是普通话，""空字符是方言
        /// </summary>
        const string LanguageType = "p";

        //用于播放音效的声音列表
        [SerializeField] 
        protected AudioSource[] AudioSourcesList = new AudioSource[5];

        /// <summary>
        /// 标记声音大小
        /// </summary>
        private float _soundVolume;

        // Use this for initialization
        void Awake () {
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGrab, OnTypeGrab);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypePass, OnTypePass);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);

            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeDoubleOver, OnDoubleOver);
        }

        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        protected virtual void OnDoubleOver(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var rates = data.GetIntArray("jiabei");
            var len = rates.Length;

            var landSeat = App.GetGameData<GlobalData>().LandSeat;
            for (int i = 0; i < len; i++)
            {
                var userData = App.GetGameData<GlobalData>().GetUserInfo(i);
                if(userData==null) continue;
                //if(i==landSeat)continue;//忽略地主喊叫

                var sex = userData.GetShort(NewRequestKey.KeySex);
                if (sex != 0 && sex != 1) sex = 0;
                string soundName = "";
                if (rates[i] > 1)
                {
                    soundName = LanguageType + sex + ".jiabei";
                }
                else
                {
                    soundName = LanguageType + sex + ".bujiabei";
                }

                PlayAudioOneShot(soundName);
            }
        }

        void Start()
        {
            App.GetGameData<GlobalData>().OnhandCdsNumChanged = OnHandCdsChanged;

            _soundVolume = MusicManager.Instance.EffectVolume;//PlayerPrefs.GetFloat(SettingCtrl.SoundValueKey, 1);

            SettingCtrl.OnSoundValueChangeEvt = OnSoundValueChange;
        }

        /// <summary>
        /// 当有调试声音大小时
        /// </summary>
        /// <param name="value"></param>
        void OnSoundValueChange(float value)
        {
            _soundVolume = value;
        }


        public override void RefreshUiInfo()
        {
           // throw new System.NotImplementedException();
        }

        /// <summary>
        /// 当有人叫了分了
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        protected void OnTypeGrab(object obj, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            //分数
            var score = data.GetInt(GlobalConstKey.C_Score);
            var seat = data.GetInt(GlobalConstKey.C_Sit);//座位
            var sex = App.GetGameData<GlobalData>().GetUserInfo(seat).GetShort(NewRequestKey.KeySex);
            if (sex != 0 && sex != 1) sex = 0;
            string soundName = "";
            if (score > 0)
            {
                soundName = LanguageType + sex + "." + score + "fen";
            }
            else
            {
                soundName = LanguageType + sex + "." + "bujiao";
            }


            PlayAudioOneShot(soundName);
        }


        /// <summary>
        /// 当是上家出牌时，之前说的话要消失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var sex = App.GetGameData<GlobalData>().GetUserInfo(data.GetInt(RequestKey.KeySeat)).GetShort(NewRequestKey.KeySex);
            if (sex != 0 && sex != 1) sex = 0;
            var cards = data.GetIntArray(RequestKey.KeyCards);

            string soundName = LanguageType + sex + "." + GetAudioName(cards);
            PlayAudioOneShot(soundName);
        }

        /// <summary>
        /// 有人不要
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var seat = data.GetInt(GlobalConstKey.C_Sit);//座位
            var sex = App.GetGameData<GlobalData>().GetUserInfo(seat).GetShort(NewRequestKey.KeySex);
            if (sex != 0 && sex != 1) sex = 0;
            string soundName = LanguageType + sex + "." + "buyao";
            PlayAudioOneShot(soundName);
        }

        /// <summary>
        /// 当有人手牌数量剩1张时
        /// </summary>
        /// <param name="userSeat"></param>
        /// <param name="cdsLeftNum"></param>
        private void OnHandCdsChanged(int userSeat,int cdsLeftNum)
        {
            if (cdsLeftNum != 1 && cdsLeftNum!=2) return;

            var user = App.GetGameData<GlobalData>().GetUserInfo(userSeat);
            short sex = 0;
            if (user.ContainsKey(NewRequestKey.KeySex)) sex = user.GetShort(NewRequestKey.KeySex);
            if (sex != 0 && sex != 1) sex = 0;
            string soundName = LanguageType + sex + "." + cdsLeftNum.ToString(CultureInfo.InvariantCulture)+ "zhang";
            PlayAudioOneShot(soundName);

            PlayAudioOneShot("Special_alert");
        }

        /// <summary>
        /// 标记取到的audioSource
        /// </summary>
        private short _avlaudioI=0;
        /// <summary>
        /// 用一个极有可能没有被占用的AudioSource播放声音clip
        /// </summary>
        public void PlayAudioOneShot(string soundName){

            var soundClip = App.GetGameData<GlobalData>().GetSoundClip(soundName);
            if (soundClip == null) return;
            if (_avlaudioI >= AudioSourcesList.Length) _avlaudioI = 0;

            var listi = _avlaudioI++;
            AudioSourcesList[listi].volume = _soundVolume;
            AudioSourcesList[listi].PlayOneShot(soundClip);
        }


        /// <summary>
        /// 获得声音文件的标记名字部分
        /// </summary>
        /// <param name="cards">获得的牌值（包括花色的）</param>
        /// <returns></returns>
        public static string GetAudioName(int[] cards)
        {

      
            var str = new string[cards.Length];
            for (int i = 0; i < cards.Length; i++)
            {
                str[i] = cards[i].ToString("x").Substring(1, 1);

            }
            System.Array.Sort(str);
            switch (cards.Length)
            {
                case 1:
                    if (cards[0] == 81)
                    {

                        return "xiaowang";
                    }
                    if (cards[0] == 97)
                    {
                        return "dawang";
                    }
                    return str[0];
                case 2:
                    if (cards[0] == 97 || cards[1] == 97 || cards[0] == 81 || cards[1] == 81)
                    {
                        return "huojian";
                    }
                    if (str[0] == str[1] || HaveMagic(cards))
                    {
                        if (HaveMagic(cards))
                        {
                            return "1dui" + str[1];
                        }
                        return "1dui" + str[0];

                    }
                    else
                    {
                        return "huojian";
                    }
                case 3:
                    return "3zhang" + str[1];
                case 4:
                    if (HaveMagic(cards))
                    {
                        if (str[1] == str[2] && str[2] == str[3])
                        {
                            return "zhadan";
                        }
                    }
                    if (str[0] != str[3])
                    {
                        return "3dai1";
                    }
                    return "zhadan";
                case 5:
                    if (HaveMagic(cards))
                    {
                        if (str[1] == str[2] && str[2] == str[3] && str[3] == str[4])
                        {

                            return "zhadan";
                        }
                    }
                  
                    if (str[0] != str[1] && str[2] != str[3])
                    {

                        return "shunzi";
                    }
                    if (str[0] == str[1] && str[3] == str[4])
                    {
                        return "3dai2";
                    }
                    return "4dai1";
                case 6:
                
                    if (str[0] != str[1] && str[1] != str[2] && str[2] != str[3])
                    {
                        return "shunzi";
                    }

                    if (str[0] == str[1] && str[2] == str[3] && str[4] == str[5] && str[1] != str[2] && str[3] != str[4])
                    {
                        return "shuangshun";
                    }
                    if (str[0] == str[1] && str[1] == str[2] && str[3] == str[4] && str[4] == str[5])
                    {
                        return "feiji";
                    }
                    return "4dai2";
                case 8:
                    if (str[2] == str[3] && str[3] == str[4] && str[4] == str[5])
                    {
                        return "4dai2dui";
                    }
                    if (str[4] == str[5] && str[5] == str[6] && str[6] == str[7])
                    {
                        return "4dai2dui";
                    }
                    if (str[0] == str[1] && str[1] == str[2] && str[2] == str[3])
                    {
                        return "4dai2dui";
                    }
                    if (str[0] == str[1] && str[1] == str[2])
                    {
                        return "feijidaichibang";
                    }
                    if (str[0] != str[1])
                    {
                        if (str[1] == str[2] && str[2] == str[3])
                        {
                            return "feijidaichibang";
                        }
                    }
                    if (str[0] != str[1] && str[1] != str[2])
                    {
                        if (str[2] == str[3] && str[2] == str[4])
                        {
                            return "feijidaichibang";
                        }
                    }
                    if (str[0] == str[1] && str[2] == str[3] && str[0] != str[2])
                    {
                        return "shuangshun";
                    }
                    return "shunzi";
                case 10:
                    if (str[0] == str[1] && str[1] == str[2])
                    {
                        return "feijidaichibang";
                    }
                    if (str[1] != str[2])
                    {
                        if (str[2] == str[3] && str[2] == str[4])
                        {
                            return "feijidaichibang";
                        }
                    }
                    if (str[7] == str[8] && str[7] == str[9])
                    {
                        return "feijidaichibang";
                    }
                    if (str[0] == str[1])
                    {
                        return "shuangshun";
                    }
                    return "shunzi";
                default:
                    if (str.Length % 2 == 0)
                    {
                        if (str[0] == str[1])
                        {
                            return "shuangshun";
                        }
                        else
                        {
                            return "shunzi";
                        }
                    }
                    else
                    {
                        if (str[0] == str[1] && str[0] == str[2])
                        {
                            return "sanshun";
                        }

                        else
                        {
                            return "shunzi";
                        }
                    }
            }
        }
        private static bool HaveMagic(IEnumerable<int> cards)
        {
            return cards.Any(s => s == 113);
        }
    }

}

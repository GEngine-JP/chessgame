using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.brnn3d
{
    public class NoteUI : MonoBehaviour
    {
        public static NoteUI Instance;
        public Text NoteText;
        protected void Awake()
        {
            Instance = this;
        }

        public void Note(string str)
        {
            if (NoteText.gameObject.activeSelf)
                NoteText.gameObject.SetActive(false);
            NoteText.gameObject.SetActive(true);
            NoteText.text = str;
        }
    }

}

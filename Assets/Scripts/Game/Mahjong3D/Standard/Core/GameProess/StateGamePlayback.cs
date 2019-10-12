namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class StateGamePlayback : FsmState
    {
        public override void OnEnter(FsmStateArgs args)
        {
            GameCenter.Playback.InitReplayData();
            GameCenter.Playback.StartupTask();
            GameCenter.Lifecycle.ContinuePlaybackCycle();
            GameCenter.Playback.OnReset();
            GameCenter.Playback.InitPlaybackScene();

            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.PlaybackRestart, PlaybackRestart);
        }

        public void PlaybackRestart(EvtHandlerArgs args)
        {
            GameCenter.Scene.PlaybackRestart();
            GameCenter.Playback.PlaybackRestart();
            GameCenter.Playback.ReplayData.OnResetFrameDatas();
        }

        public override void OnLeave(bool isShutdown) { }
    }
}
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.Common.WebView
{
#if UNITY_EDITOR
    
    public class UniWebViewPlugin
    {
        private static bool _connected;

        public delegate void UnityCallbackDelegate(IntPtr objectName, IntPtr methodName, IntPtr parameter);

        [DllImport("UniWebView")]
        private static extern void _ConnetCallback([MarshalAs(UnmanagedType.FunctionPtr)]UnityCallbackDelegate callbackMethod);

        [DllImport("UniWebView")]
        private static extern void _UniWebViewInit(string name, int top, int left, int bottom, int right, int screenWidth, int screenHeight);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewChangeInsets(string name, int top, int left, int bottom, int right, int screenWidth, int screenHeight);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewLoad(string name, string url);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewReload(string name);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewStop(string name);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewShow(string name, bool fade, int direction, float duration);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewHide(string name, bool fade, int direction, float duration);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewEvaluatingJavaScript(string name, string javascript, bool callback);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewCleanCache(string name);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewCleanCookie(string name, string key);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewDestroy(string name);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewTransparentBackground(string name, bool transparent);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewSetBackgroundColor(string name, float r, float g, float b, float a);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewSetSpinnerShowWhenLoading(string name, bool show);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewSetSpinnerText(string name, string text);
        [DllImport("UniWebView")]
        private static extern bool _UniWebViewCanGoBack(string name);
        [DllImport("UniWebView")]
        private static extern bool _UniWebViewCanGoForward(string name);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewGoBack(string name);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewGoForward(string name);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewLoadHTMLString(string name, string htmlString, string baseUrl);
        [DllImport("UniWebView")]
        private static extern string _UniWebViewGetCurrentUrl(string name);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewAddUrlScheme(string name, string scheme);
        [DllImport("UniWebView")]
        private static extern string _UniWebViewRemoveUrlScheme(string name, string scheme);

        [DllImport("UniWebView")]
        private static extern void _UniWebViewInputEvent(string name, int x, int y, float deltaY,
                                                         bool buttonDown, bool buttonPress, bool buttonRelease,
                                                         bool keyPress, short keyCode, string keyChars, int textureId);
        [DllImport("UniWebView")]
        private static extern int _UniWebViewGetId(string name);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewSetUserAgent(string userAgent);
        [DllImport("UniWebView")]
        private static extern string _UniWebViewGetUserAgent(string name);
        [DllImport("UniWebView")]
        private static extern float _UniWebViewGetAlpha(string name);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewSetAlpha(string name, float alpha);
        [DllImport("UniWebView")]
        private static extern IntPtr _UniWebViewGetRenderEventFunc();
        [DllImport("UniWebView")]
        private static extern int _UniWebViewScreenScale();
        [DllImport("UniWebView")]
        private static extern void _UniWebViewSetHeaderField(string name, string key, string value);
        [DllImport("UniWebView")]
        private static extern bool _UniWebViewGetOpenLinksInExternalBrowser(string name);
        [DllImport("UniWebView")]
        private static extern void _UniWebViewSetOpenLinksInExternalBrowser(string name, bool value);

        public static void Init(string name, int top, int left, int bottom, int right)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if (!_connected)
                {
                    ConnectNativeBundle();
                    SetUserAgent("Mozilla/5.0 (iPhone; CPU iPhone OS 9_0 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13A344 Safari/601.1");
                }
                _UniWebViewInit(name, top, left, bottom, right, Screen.width, Screen.height);
            }
            else
            {
                Debug.LogWarning("Windows Editor is not supported yet in UniWebView. Please build it to devices or use a Mac Editor.");
            }
        }
        /// <summary>
        /// 更改显示范围
        /// </summary>
        /// <param name="name"></param>
        /// <param name="top"></param>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        public static void ChangeInsets(string name, int top, int left, int bottom, int right)
        {
            switch(Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                    _UniWebViewChangeInsets(name, top, left, bottom, right, Screen.width, Screen.height);
                    break;
            }
        }

        public static void Load(string name, string url)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewLoad(name, url);
            }
        }

        public static void LoadHTMLString(string name, string htmlString, string baseUrl)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewLoadHTMLString(name, htmlString, baseUrl);
            }
        }


        public static void Reload(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewReload(name);
            }
        }

        public static void Stop(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewStop(name);
            }
        }

        public static void Show(string name, bool fade, int direction, float duration)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if (fade || direction != 0)
                {
                    Debug.Log("Transition with animation is not supported in Editor yet.");
                }

                _UniWebViewShow(name, fade, direction, duration);
            }
        }

        public static void Hide(string name, bool fade, int direction, float duration)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if (fade || direction != 0)
                {
                    Debug.Log("Transition with animation is not supported in Editor yet.");
                }

                _UniWebViewHide(name, fade, direction, duration);
            }
        }

        public static void EvaluatingJavaScript(string name, string javaScript, bool callback = true)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewEvaluatingJavaScript(name, javaScript, callback);
            }
        }

        public static void AddJavaScript(string name, string javaScript)
        {
            EvaluatingJavaScript(name, javaScript, false);
        }

        public static void CleanCache(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewCleanCache(name);
            }
        }

        public static void CleanCookie(string name, string key)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewCleanCookie(name, key);
            }
        }

        public static void Destroy(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewDestroy(name);
            }
        }

        public static void TransparentBackground(string name, bool transparent)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewTransparentBackground(name, transparent);
            }
        }

        public static void SetBackgroundColor(string name, float r, float g, float b, float a)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                Debug.LogWarning("This method is limited in OSX Editor. You can only set white/clear background with this method in Editor.");
                _UniWebViewSetBackgroundColor(name, r, g, b, a);
            }
        }

        public static void SetSpinnerShowWhenLoading(string name, bool show)
        {
            Debug.Log("UniWebViewSetSpinnerShowWhenLoading will do nothing in Editor");
        }

        public static void SetSpinnerText(string name, string text)
        {
            Debug.Log("UniWebViewSetSpinnerText will do nothing in Editor");
        }

        public static bool CanGoBack(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return _UniWebViewCanGoBack(name);
            }
            return false;
        }

        public static bool CanGoForward(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return _UniWebViewCanGoForward(name);
            }
            return false;
        }

        public static void GoBack(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewGoBack(name);
            }
        }

        public static void GoForward(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewGoForward(name);
            }
        }

        public static void InputEvent(string name, int x, int y, float deltaY,
                                 bool buttonDown, bool buttonPress, bool buttonRelease,
                                 bool keyPress, short keyCode, string keyChars, int textureId)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewInputEvent(name, x, y, deltaY,
                                      buttonDown, buttonPress, buttonRelease,
                                      keyPress, keyCode, keyChars, textureId);
            }
        }

        public static int GetId(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return _UniWebViewGetId(name);
            }
            return 0;
        }

        public static string GetCurrentUrl(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return _UniWebViewGetCurrentUrl(name);
            }
            return "";
        }

        private static void ConnectNativeBundle()
        {
            _ConnetCallback((objectName, methodName, parameter) => {
                string objectNameStr = Marshal.PtrToStringAuto(objectName);
                string methodNameStr = Marshal.PtrToStringAuto(methodName);
                string parameterStr = Marshal.PtrToStringAuto(parameter);

                GameObject foundGO = GameObject.Find(objectNameStr);
                if (foundGO != null)
                {
                    foundGO.SendMessage(methodNameStr, parameterStr);
                }
            });
            _connected = true;
        }

        public static void AddUrlScheme(string name, string scheme)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewAddUrlScheme(name, scheme);
            }
        }

        public static void RemoveUrlScheme(string name, string scheme)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewRemoveUrlScheme(name, scheme);
            }
        }

        public static void SetUserAgent(string userAgent)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewSetUserAgent(userAgent);
            }
        }

        public static string GetUserAgent(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return _UniWebViewGetUserAgent(name);
            }
            return "";
        }

        public static float GetAlpha(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                Debug.LogWarning("Alpha is not available in editor version.");
            }
            return 1.0f;
        }

        public static void SetAlpha(string name, float alpha)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                Debug.LogWarning("Alpha is not available in editor version.");
            }
        }

        public static IntPtr GetRenderEventFunc()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return _UniWebViewGetRenderEventFunc();
            }
            return IntPtr.Zero;
        }

        public static int ScreenScale()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return _UniWebViewScreenScale();
            }
            return 1;
        }

        public static void SetHeaderField(string name, string key, string value)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewSetHeaderField(name, key, value);
            }
        }

        public static void SetVerticalScrollBarShow(string name, bool show)
        {
            // Not implemented.
        }

        public static void SetHorizontalScrollBarShow(string name, bool show)
        {
            //Not implemented.
        }

        public static bool GetOpenLinksInExternalBrowser(string name)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return _UniWebViewGetOpenLinksInExternalBrowser(name);
            }
            return false;
        }

        public static void SetOpenLinksInExternalBrowser(string name, bool value)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _UniWebViewSetOpenLinksInExternalBrowser(name, value);
            }
        }
    }
#endif

}
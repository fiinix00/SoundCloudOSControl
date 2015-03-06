using ChromeAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoundCloudOSControl
{
    public static class ChromeRemote
    {
        private static Chrome _chrome;
        private static Chrome Chrome
        {
            get
            {
                if (_chrome == null)
                {
                    _chrome = new Chrome("http://localhost:9222");
                    var sessions = _chrome.GetAvailableSessions();

                    var soundcloud = sessions.FirstOrDefault(session => new Uri(session.url).Host == "soundcloud.com");

                    if (soundcloud != null && soundcloud.webSocketDebuggerUrl != null)
                    {
                        _chrome.SetActiveSession(soundcloud.webSocketDebuggerUrl);
                    }
                    else
                    {
                        _chrome = null;
                    }
                }

                return _chrome;
            }
        }

        public static void Click(string _class)
        {
            Send("document.querySelectorAll('" + _class + "')[0].click();");
        }

        public static async void Send(string data)
        {
            var cts = new CancellationTokenSource();
            
            try
            {
                cts.CancelAfter(5000);

                await Task.Run(() =>
                {
                    var chrome = Chrome;
                    if (chrome == null)
                    {
                        cts.Cancel();
                    }
                    else
                    {
                        chrome.Eval(data);
                    }
                });
            }
            catch (OperationCanceledException)
            {
                _chrome = null; //reset session
            }
        }
    }
}

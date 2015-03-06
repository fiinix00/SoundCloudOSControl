using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using WebSocket4Net;
using System.Net;

namespace ChromeAutomation
{
    public class Chrome
    {
        const string JsonPostfix = "/json";

        string remoteDebuggingUri;
        string sessionWSEndpoint;

        public Chrome(string remoteDebuggingUri)
        {
            this.remoteDebuggingUri = remoteDebuggingUri;
        }

        public TRes SendRequest<TRes>()
        {
            var req = (HttpWebRequest)WebRequest.Create(remoteDebuggingUri + JsonPostfix);
            var resp = req.GetResponse();
            var respStream = resp.GetResponseStream();

            return Deserialise<TRes>(respStream);
        }

        public List<RemoteSessionsResponse> GetAvailableSessions()
        {
            var res = this.SendRequest<List<RemoteSessionsResponse>>();
            return (from r in res
                    where r.devtoolsFrontendUrl != null
                    select r).ToList();
        }

        public string NavigateTo(string uri)
        {
            // Page.navigate is working from M18
            //var json = @"{""method"":""Page.navigate"",""params"":{""url"":""http://www.seznam.cz""},""id"":1}";

            // Instead of Page.navigate, we can use document.location
            var json = @"{""method"":""Runtime.evaluate"",""params"":{""expression"":""document.location='" + uri + @"'"",""objectGroup"":""console"",""includeCommandLineAPI"":true,""doNotPauseOnExceptions"":false,""returnByValue"":false},""id"":1}";
            return this.SendCommand(json);
        }

        public string GetElementsByTagName(string tagName)
        {
            // Page.navigate is working from M18
            //var json = @"{""method"":""Page.navigate"",""params"":{""url"":""http://www.seznam.cz""},""id"":1}";

            // Instead of Page.navigate, we can use document.location
            var json = @"{""method"":""Runtime.evaluate"",""params"":{""expression"":""document.getElementsByTagName('" + tagName + @"')"",""objectGroup"":""console"",""includeCommandLineAPI"":true,""doNotPauseOnExceptions"":false,""returnByValue"":false},""id"":1}";
            return this.SendCommand(json);
        }

        public string Eval(string cmd)
        {
            var json = @"{""method"":""Runtime.evaluate"",""params"":{""expression"":""" + cmd + @""",""objectGroup"":""console"",""includeCommandLineAPI"":true,""doNotPauseOnExceptions"":false,""returnByValue"":false},""id"":1}";
            return this.SendCommand(json);
        }

        public string SendCommand(string cmd)
        {
            var socker = new WebSocket(this.sessionWSEndpoint);
            var waitEvent = new ManualResetEvent(false);
            var closedEvent = new ManualResetEvent(false);
            string message = "";
            byte[] data;

            Exception exc = null;
            socker.Opened += delegate(System.Object o, EventArgs e)
            {
                socker.Send(cmd);
            };

            socker.MessageReceived += delegate(System.Object o, MessageReceivedEventArgs e)
            {
                message = e.Message;
                waitEvent.Set();
            };

            socker.Error += delegate(System.Object o, SuperSocket.ClientEngine.ErrorEventArgs e)
            {
                exc = e.Exception;
                waitEvent.Set();
            };

            socker.Closed += delegate(System.Object o, EventArgs e)
            {
                closedEvent.Set();
            };

            socker.DataReceived += delegate(System.Object o, DataReceivedEventArgs e)
            {
                data = e.Data;
                waitEvent.Set();
            };

            socker.Open();

            waitEvent.WaitOne();
            if (socker.State == WebSocket4Net.WebSocketState.Open)
            {
                socker.Close();
                closedEvent.WaitOne();
            }
            if (exc != null)
                throw exc;

            return message;
        }

        private T Deserialise<T>(Stream json)
        {
            T obj = Activator.CreateInstance<T>();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(json);
            return obj;
        }

        public void SetActiveSession(string sessionWSEndpoint)
        {
            // Sometimes binding to localhost might resolve wrong AddressFamily, force IPv4
            this.sessionWSEndpoint = sessionWSEndpoint.Replace("ws://localhost", "ws://127.0.0.1");
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;
using Remotus.Base;
using Remotus.Base.Interfaces.Net;
using Remotus.Base.Net;

namespace Remotus.Core.Net.Client
{
    public class CustomHubAgent : HubAgent, ICustomHubAgent
    {
        //private static readonly ILog _log = LogManager.GetLoggerFor(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName);
        
        public CustomHubAgent(string hubName, IHubProxy hubProxy, IHubConnector hubConnector, IMessageCache messageCache)
            : base(hubName, hubProxy, hubConnector, messageCache)
        {
        }


        public override Task Invoke(IHubMessage message)
        {
            var msg = message;
            var task = base.Invoke(msg);
            return task;
        }

        public Task InvokeCustom(CustomHubMessage message)
        {
            var args = message?.Args?.Length > 0
                ? message.Args.ToArray()
                : new object[0];

            var inner = new CustomHubMessage();
            inner.HubName = HubName;
            inner.Method = message?.Method;
            inner.Args = args;
            inner.Queuable = message?.Queuable ?? false;
            inner.Groups = message?.Groups;

            var msg = new CustomHubMessage();
            msg.HubName = "CustomHub";
            msg.Method = "InvokeCustom";
            msg.Groups = null;
            //msg.Groups = new [] { HubName };
            //msg.Args = args;
            msg.Args = new object[] { inner };
            msg.Queuable = inner.Queuable;

            var task = base.Invoke(msg);
            return task;
        }


        //public override IHubSubscription Subscribe(string eventName)
        //{
        //    var subscription = new HubSubscription();
        //    subscription.HubName = HubName;
        //    subscription.EventName = eventName;

        //    Action<IList<JToken>> HubProxySub_OnReceived = list =>
        //    {
        //        subscription.Invoke(list);
        //    };

        //    var sub = _hubProxy.Subscribe(eventName);
        //    sub.Received += HubProxySub_OnReceived;

        //    // todo: able to unsubscribe via IDisposable


        //    Task task = null;
        //    try
        //    {
        //        var msg = new HubMessage
        //        {
        //            Method = "AddToGroup",
        //            Args = new object[] { CustomHubIdentifier },
        //            Queuable = true,
        //        };
        //        var timeout = TimeSpan.FromSeconds(15);
        //        task = Invoke(msg);
        //        task.Wait(timeout);
        //    }
        //    catch (Exception ex)
        //    {
                
        //    }

        //    return subscription;
        //}

    }
}

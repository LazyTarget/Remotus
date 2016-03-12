﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FullCtrl.Base;
using RestSharp;

namespace FullCtrl.API.v1
{
    public partial class FullCtrlAPI
    {
        public async Task<IResponseBase<IEnumerable<IPlugin>>> GetRemotePlugins(string clientID)
        {
            var uri = new Uri("remote/plugins", UriKind.Relative);
            var request = BuildRequest(uri, Method.GET);
            request.AddQueryParameter("clientID", clientID);

            var response = await GetResponse<IEnumerable<IPlugin>>(request);
            return response;
        }


        public async Task<IResponseBase<IFunctionResult>> ExecuteRemoteFunction(string clientID, string pluginName, string functionName, IFunctionArguments arguments)
        {
            var uri = new Uri("remote/execute/function", UriKind.Relative);
            var request = BuildRequest(uri, Method.POST);
            request.AddQueryParameter("clientID", clientID);
            request.AddQueryParameter("pluginName", pluginName);
            request.AddQueryParameter("functionName", functionName);
            request.AddJsonBody(arguments);

            var response = await GetResponse<IFunctionResult>(request);
            return response;
        }

    }
}

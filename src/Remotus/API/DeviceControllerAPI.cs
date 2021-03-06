﻿using System;
using System.Threading.Tasks;
using Remotus.Base;
using RestSharp;

namespace Remotus.API
{
    public class DeviceControllerAPI : IDeviceControllerAPI
    {
        public DeviceControllerAPI()
        {
            BaseUri = new Uri("http://localhost:9000/api/v1/");
        }

        public Uri BaseUri { get; set; }


        protected virtual IRestClient GetClient()
        {
            var jsonSerializer = new CustomJsonSerializer();
            jsonSerializer.Container.Bind(typeof(IResponseBase), typeof(DefaultResponseBase<>));
            jsonSerializer.Container.Bind(typeof(IResponseBase<>), typeof(DefaultResponseBase<>));
            jsonSerializer.Container.Bind(typeof(IError), typeof(DefaultError));
            jsonSerializer.Container.Bind(typeof(ILink), typeof(DefaultLink));
            jsonSerializer.Container.Bind(typeof(IProcessDto), typeof(ProcessDto));

            var client = new RestClient(BaseUri);
            client.AddHandler("application/json", jsonSerializer);
            client.AddHandler("text/json", jsonSerializer);
            client.AddHandler("text/x-json", jsonSerializer);
            client.AddHandler("*+json", jsonSerializer);
            client.AddHandler("*", jsonSerializer);

            //client.AddDefaultHeader("Accept", "application/json");
            return client;
        }

        protected virtual IRestRequest BuildRequest(Uri resource, Method method)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("accept", "application/json");
            return request;
        }

        protected async Task<IResponseBase<TResult>> GetResponse<TResult>(IRestRequest request)
        {
            IResponseBase<TResult> response;
            try
            {
                var client = GetClient();
                var httpResponse = await client.ExecuteTaskAsync<IResponseBase<TResult>>(request);
                response = httpResponse.Data;
            }
            catch (Exception ex)
            {
                response = new DefaultResponseBase<TResult>
                {
                    Error = DefaultError.FromException(ex),
                };
            }
            return response;
        }



        public async Task<IResponseBase<object>> TakeScreenshot()
        {
            var uri = new Uri("device/take/screenshot", UriKind.Relative);
            var request = BuildRequest(uri, Method.POST);

            var response = await GetResponse<object>(request);
            return response;
        }
    }
}

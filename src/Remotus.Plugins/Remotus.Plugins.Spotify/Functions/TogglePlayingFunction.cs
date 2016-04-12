﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotus.Base;

namespace Remotus.Plugins.Spotify
{
    public class TogglePlayingFunction : IFunction<StatusResponse>
    {
        private ModelConverter _modelConverter = new ModelConverter();

        public TogglePlayingFunction()
        {
            
        }

        public IFunctionDescriptor GetDescriptor()
        {
            return new Descriptor();
        }

        async Task<IFunctionResult> IFunction.Execute(IExecutionContext context, IFunctionArguments arguments)
        {
            var result = await Execute(context, arguments);
            return result;
        }

        public async Task<IFunctionResult<StatusResponse>> Execute(IExecutionContext context, IFunctionArguments arguments)
        {
            try
            {
                var connected = Worker.ConnectLocalIfNotConnected();
                if (!connected)
                {
                    throw new Exception("Unable to connect to Spotify");
                }
                var status = Worker.LocalApi.GetStatus();
                if(status==null)
                    throw new Exception("Could not get status from Spotify");
                else if (status.Playing)
                    Worker.LocalApi.Pause();
                else
                    Worker.LocalApi.Play();
                status = Worker.LocalApi.GetStatus();
                var res = _modelConverter.FromStatusResponse(status);

                var result = new FunctionResult<StatusResponse>();
                result.Arguments = arguments;
                result.Result = res;
                return result;
            }
            catch (Exception ex)
            {
                var result = new FunctionResult<StatusResponse>();
                result.Arguments = arguments;
                result.Error = DefaultError.FromException(ex);
                return result;
            }
        }


        public class Descriptor : IFunctionDescriptor
        {
            public string ID => "2E23C7FD-BABE-4E29-A30F-C20ACB35D572";
            public string Name => "Toggle playing";
            public string Version => "1.0.0.0";

            public IParameterCollection GetParameters()
            {
                IParameterCollection res = null;
                return res;
            }

            IFunction IComponentInstantiator<IFunction>.Instantiate()
            {
                return Instantiate();
            }

            public IFunction<StatusResponse> Instantiate()
            {
                return new TogglePlayingFunction();
            }
        }

        public void Dispose()
        {
            
        }
    }
}

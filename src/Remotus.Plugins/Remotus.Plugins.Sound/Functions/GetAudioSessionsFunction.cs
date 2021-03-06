﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio;
using Lux;
using Lux.Interfaces;
using Remotus.Base;

namespace Remotus.Plugins.Sound
{
    public class GetAudioSessionsFunction : IFunction, IFunction<IList<AudioSession>>
    {
        private CoreAudioController _audioController = SoundPlugin.AudioController;
        private ModelConverter _modelConverter = new ModelConverter();
        private IConverter _converter = new Converter();

        public GetAudioSessionsFunction()
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

        public async Task<IFunctionResult<IList<AudioSession>>> Execute(IExecutionContext context, IFunctionArguments arguments)
        {
            try
            {
                AudioSwitcher.AudioApi.CoreAudio.CoreAudioDevice device;
                var deviceID = arguments?.Parameters.GetOrDefault<string>(ParameterKeys.DeviceID)?.Value;
                var guid = _converter.Convert<Guid>(deviceID);
                if (guid != Guid.Empty)
                    device = await _audioController.GetDeviceAsync(guid);
                else
                    device = _audioController.DefaultPlaybackDevice;
                if (device == null)
                    throw new Exception("Device not found");
                
                var sessions = device?.SessionController?.All();
                var res = sessions?.Select(_modelConverter.FromAudioSession).Where(x => x != null).ToList();

                var result = new FunctionResult<IList<AudioSession>>();
                result.Arguments = arguments;
                result.Result = res;
                return result;
            }
            catch (Exception ex)
            {
                var result = new FunctionResult<IList<AudioSession>>();
                result.Arguments = arguments;
                result.Error = DefaultError.FromException(ex);
                return result;
            }
        }


        public class Descriptor : IFunctionDescriptor
        {
            public string ID => "D5F99DAC-03A7-4E33-878C-20CA93726DE9";
            public string Name => "Get audio sessions";
            public string Version => "1.0.0.0";

            IParameterCollection IFunctionDescriptor.GetParameters()
            {
                return GetParameters();
            }

            public Parameters GetParameters()
            {
                var res = new Parameters();
                return res;
            }

            IFunction IComponentInstantiator<IFunction>.Instantiate()
            {
                return Instantiate();
            }

            public IFunction<IList<AudioSession>> Instantiate()
            {
                return new GetAudioSessionsFunction();
            }
        }

        public class Parameters : ParameterCollection
        {
            public Parameters()
            {
                DeviceID = new Parameter<string>
                {
                    Name = ParameterKeys.DeviceID,
                    Required = false,
                    Type = typeof(string),
                    Value = null,
                };
            }

            public IParameter<string> DeviceID
            {
                get { return this.GetOrDefault<string>(ParameterKeys.DeviceID); }
                private set { this[ParameterKeys.DeviceID] = value; }
            }
        }

        public static class ParameterKeys
        {
            public const string DeviceID = "DeviceID";
        }

        public void Dispose()
        {
            //_audioController?.Dispose();
            //_audioController = null;
        }

    }
}

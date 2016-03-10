﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Session;
using FullCtrl.Base;

namespace FullCtrl.API.v1.Controllers
{
    public class AudioController : BaseController
    {
        private static readonly CoreAudioController _audioController;

        static AudioController()
        {
            _audioController = new CoreAudioController();
        }

        [HttpGet, HttpPost, HttpPut]
        [Route("api/v1/audio/sessions")]
        [Route("api/v1/audio/sessions/list")]
        public IResponseBase<IEnumerable<AudioSession>> GetSessions(string deviceID)
        {
            Guid guid;
            CoreAudioDevice device = null;
            if (string.IsNullOrEmpty(deviceID))
            {
                device = _audioController.DefaultPlaybackDevice;
            }
            else if(Guid.TryParse(deviceID, out guid))
            {
                device = _audioController.GetDevice(guid);
            }


            if (device == null)
            {
                throw new Exception("Device not found");
            }

            var sessions = device?.SessionController?.All();
            var res = sessions?.Select(FromAudioControllerState).Where(x => x != null).ToList();

            IEnumerable<AudioSession> result = res;
            var response = CreateResponse(result);
            return response;
        }


        [HttpGet, HttpPost, HttpPut]
        [Route("api/v1/audio/devices")]
        [Route("api/v1/audio/devices/list")]
        public IResponseBase<IEnumerable<AudioDevice>> GetDevices()
        {
            var response = GetDevices(null);
            return response;
        }
        
        [HttpGet, HttpPost, HttpPut]
        [Route("api/v1/audio/devices/list/{deviceType}")]
        public IResponseBase<IEnumerable<AudioDevice>> GetDevices(DeviceType? deviceType)
        {
            var response = GetDevices(deviceType, null);
            return response;
        }

        [HttpGet, HttpPost, HttpPut]
        [Route("api/v1/audio/devices/list/{deviceType}/{deviceState}")]
        public IResponseBase<IEnumerable<AudioDevice>> GetDevices(DeviceType? deviceType, DeviceState? deviceState)
        {
            IEnumerable<CoreAudioDevice> devices;
            if (deviceType.HasValue && deviceState.HasValue)
            {
                devices = _audioController.GetDevices(deviceType.Value, deviceState.Value);
            }
            else if (deviceType.HasValue)
            {
                devices = _audioController.GetDevices(deviceType.Value);
            }
            else if (deviceState.HasValue)
            {
                devices = _audioController.GetDevices(deviceState.Value);
            }
            else
            {
                devices = _audioController.GetDevices();
            }
            var res = devices?.Select(FromAudioDevice).Where(x => x != null).ToList();

            IEnumerable<AudioDevice> result = res;
            var response = CreateResponse(result);
            return response;
        }


        [HttpPost, HttpPut]
        [Route("api/v1/audio/device/set")]
        [Route("api/v1/audio/device/set/{deviceID}")]
        public IResponseBase<object> SetDefaultDevice(string deviceID)
        {
            object result = null;
            Guid guid;
            if (Guid.TryParse(deviceID, out guid))
            {
                var device = _audioController.GetDevice(guid);
                if (device != null)
                {
                    result = _audioController.SetDefaultDevice(device);
                }
                else
                {
                    result = -1;
                }
            }
            else
            {
                result = -1;
            }
            
            var response = CreateResponse(result);
            return response;
        }


        [HttpPost, HttpPut]
        [Route("api/v1/audio/device/togglemute")]
        [Route("api/v1/audio/device/togglemute/{deviceID}")]
        public IResponseBase<object> ToggleDeviceMute(string deviceID)
        {
            object result = null;
            Guid guid;
            if (Guid.TryParse(deviceID, out guid))
            {
                var device = _audioController.GetDevice(guid);
                if (device != null)
                {
                    result = device.Mute(!device.IsMuted);
                }
                else
                {
                    result = -1;
                }
            }
            else
            {
                result = -1;
            }
            
            var response = CreateResponse(result);
            return response;
        }




        


        private AudioSession FromAudioControllerState(IAudioSession state)
        {
            if (state == null)
                return null;

            var icon = GetIcon(state.IconPath, state.ExecutablePath);
            var result = new AudioSession
            {
                ID = state.Id,
                DeviceID = state.Device.Id,
                Name = state.DisplayName,
                IconPath = state.IconPath,
                IconRaw = icon,
                Muted = state.IsMuted,
                Volume = state.Volume,
                IsSystemSession = state.IsSystemSession,
            };
            return result;
        }

        private AudioDevice FromAudioDevice(CoreAudioDevice state)
        {
            if (state == null)
                return null;

            var icon = GetIcon(state.IconPath, null);
            var result = new AudioDevice
            {
                ID = state.Id,
                FriendlyName = state.Name,
                InterfaceName = state.InterfaceName,
                Muted = state.IsMuted,
                Volume = state.Volume,
                DeviceType = (AudioDeviceType)Enum.Parse(typeof(AudioDeviceType), ((int)state.DeviceType).ToString()),
                State = (AudioDeviceState)Enum.Parse(typeof(AudioDeviceState), ((int)state.State).ToString()),
                DeviceIconType = (AudioDeviceIcon)Enum.Parse(typeof(AudioDeviceIcon), ((int)state.Icon).ToString()),
                IconPath = state.IconPath,
                IconRaw = icon,
                DefaultDevice = state.IsDefaultDevice,
                IsDefaultCommunicationsDevice = state.IsDefaultCommunicationsDevice,
            };
            return result;
        }




        [DllImport("shell32.dll", EntryPoint = "ExtractIcon")]
        private extern static IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        private Bitmap GetIcon(string iconReference, string exePath)
        {
            Bitmap icon = null;
            if (string.IsNullOrWhiteSpace(iconReference) && string.IsNullOrWhiteSpace(exePath))
                return null;

            if (!string.IsNullOrWhiteSpace(iconReference))
            {
                try
                {
                    var iconPath = iconReference;
                    var iconIdx = 0;
                    if (iconPath.IndexOf(',') >= 0)
                    {
                        var i = iconPath.IndexOf(',');
                        iconPath = iconReference.Substring(0, i).Trim('@').Trim();
                        iconPath = Environment.ExpandEnvironmentVariables(iconPath);
                        int.TryParse(iconReference.Substring(i + 1).Trim(), out iconIdx);
                    }
                    
                    if (File.Exists(iconPath))
                    {
                        var ptr = ExtractIcon(IntPtr.Zero, iconPath, iconIdx);
                        Icon ico = Icon.FromHandle(ptr);
                        icon = ico?.ToBitmap();
                    }
                }
                catch (Exception ex)
                {

                }

                if (icon != null)
                    return icon;
                else
                {
                    
                }
            }
            
            if (!string.IsNullOrWhiteSpace(exePath))
            {
                try
                {
                    var ico = Icon.ExtractAssociatedIcon(exePath);
                    icon = ico?.ToBitmap();
                }
                catch (Exception ex)
                {

                }

                if (icon != null)
                    return icon;
                else
                {
                    
                }
            }
            return icon;
        }

    }
}

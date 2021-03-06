﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lux.IO;
using Remotus.Base;
using Remotus.Base.Scripting;

namespace Remotus.Plugins.Scripting
{
    public class MemoryScriptDataStore : IScriptDataStore
    {
        public MemoryScriptDataStore()
        {
            Scripts = new List<Script>();

            var testScript = new Script
            {
                ID = "3FE0E633-771F-4B75-BFB7-BE520EC11FF1",
                Name = "Mute and Pause Spotify script",
                Version = "1.0.0.0",
                Tasks = new[]
                {
                    new ExecuteFunctionScriptTask
                    {
                        PluginID = "ABA6417A-65A2-4761-9B01-AA9DFFC074C0",      // Sound
                        FunctionID = "78324B37-0F93-40C2-AC56-5B1D714CFC41",    // Toggle mute device
                        Arguments = new FunctionArguments
                        {
                            Parameters = new ParameterCollection
                            {
                                {
                                    "DeviceID",
                                    new Parameter<string>
                                    {
                                        Name = "DeviceID",
                                        Required = false,
                                        Type = typeof(string),
                                        Value = null,
                                    }
                                },
                            },
                        },
                    },

                    new ExecuteFunctionScriptTask
                    {
                        PluginID = "79A54741-590C-464D-B1E9-0CC606771493",      // Spotify
                        //FunctionID = "2E23C7FD-BABE-4E29-A30F-C20ACB35D572",  // Toggle playing
                        FunctionID = "B525CAD9-27E6-4709-B29B-96A32438C7D0",    // Pause
                        Arguments = null,
                    },
                },
            };
            Scripts.Add(testScript);

#if DEBUG
            Type[] extraTypes = new[]
            {
                typeof(ExecuteFunctionScriptTask), 
                //typeof (FunctionArguments), typeof (IFunctionDescriptor), typeof (IFunctionResult),
                //typeof (IParameterCollection), typeof (IParameter), typeof (IComponentDescriptor),
                //typeof (IComponentInstantiator<>)
            };

            var fileSystem = new FileSystem();
            //var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Script), extraTypes);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Script));
            var directory = PathHelper.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Scripts");
            string filePath = PathHelper.Combine(directory, "TestScript.xml");
            fileSystem.CreateDir(directory);
            fileSystem.DeleteFile(filePath);
            using (var stream = fileSystem.OpenFile(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                serializer.Serialize(stream, testScript);
            }
#endif
        }

        public List<Script> Scripts { get; set; }

        public IEnumerable<Script> GetScripts()
        {
            return Scripts?.AsEnumerable() ?? new Script[0];
        }
    }
}
﻿using System.Collections.Generic;
using FullCtrl.Base;

namespace Remotus.Plugins.Process
{
    public class ProcessManagerPlugin : IFunctionPlugin
    {
        public string Name => nameof(ProcessManagerPlugin);
        public string Version => "1.0.0.0";

        public IEnumerable<IFunctionDescriptor> GetFunctions()
        {
            yield return new GetProcessFunction();
            yield return new GetProcessesFunction();
            yield return new StartProcessFunction();
            yield return new KillProcessFunction();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsDataTools
{
    public class ToolBase
    {

        public void Run(CommonOptions options)
        {
            if (options.DebugBreak)
            {
                System.Diagnostics.Debugger.Launch();
            }
        }

    }
}

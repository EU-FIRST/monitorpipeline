//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

namespace MonitorPipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            ZeroMqReceiverComponent zmqRcv = new ZeroMqReceiverComponent();
            TestComponent tc = new TestComponent();
            zmqRcv.Subscribe(tc);
            zmqRcv.Start();
        }
    }
}

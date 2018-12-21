using System.Linq;
using System.Threading;

namespace Utility
{
    /// <summary>
    /// 线程帮助类
    /// </summary>
    public static class ThreadHelper
    {
        private static Thread t;

        public static Thread InitThread(ThreadStart start)
        {
            t = new Thread(start);
            t.IsBackground = true;
            t.Start();
            return t;
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        public static void Stop()
        {
            t.Join();
        }
    }
}

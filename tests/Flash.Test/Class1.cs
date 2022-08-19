using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Flash.Extensions;

namespace Flash.Test
{
    [TestClass]
    public class Class1
    {
        [TestMethod]
        public void Test1()
        {
            decimal value = 123456789.23m;
            var dfdf = value.ToUpper();

            var s = "1101019003079691";
            var d = s.ValidCardId();
            var f = s.ToMaskCardId();
        }


        [TestMethod]
        public void Test2()
        {
            for (int i = 0; i < 500; i++)
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    if (LeakyBucket.Grant())
                    {
                        Console.WriteLine("执行业务逻辑");
                    }
                    else
                    {
                        Console.WriteLine("限流");
                    }
                });
            }
        }

        [TestMethod]
        public void Test3()
        {
            var x = 0.0666200060006;
            var y = 0.0000400000000 ;

            var x1 = Math.Log(x, 10);
            var y1 = Math.Log(y, 10);

            Console.WriteLine(x1);
            Console.WriteLine(y1);

            Console.WriteLine(x1 > y1);
        }

    }

    /// <summary>
    /// 漏桶算法
    /// </summary>
    public static class LeakyBucket
    {
        /// <summary>
        /// 时间刻度
        /// </summary>
        private static long time = DateTime.Now.Ticks;
        /// <summary>
        /// 桶里面现在的水
        /// </summary>
        private static int water = 0;
        /// <summary>
        /// 桶大小
        /// </summary>
        private static int size = 10;
        /// <summary>
        /// 出水速率
        /// </summary>
        private static int rate = 3;

        public static bool Grant()
        {
            var now = DateTime.Now.Ticks;
            var outNumber = ((now - time) * 1.0) * rate;
            water = (int)Math.Max(0, water - outNumber);

            time = now;

            if (water + 1 < size)
            {
                ++water;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

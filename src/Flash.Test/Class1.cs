using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

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
        }
    }
}

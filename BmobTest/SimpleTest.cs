using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cn.bmob.io;
using System.Text.RegularExpressions;

namespace BmobTest
{
    [TestClass]
    public class SimpleTest
    {

        [TestMethod()]
        public void BmobDateTest()
        {
            BmobDate d = DateTime.Now;
            Console.WriteLine(d);
        }

        [TestMethod()]
        public void RegexTest()
        {
            var ss = "HTTP/1.1 400 abc.txt";
            ss = Regex.Replace(ss, @"[^ ]* (\d*) .*", "$1");
            Console.WriteLine(ss);
        }

        [TestMethod()]
        public void BmobIntTest()
        {
            // set
            BmobInt i = new BmobInt(1);
            BmobInt i2 = 1; // 有隐士转换

            // get
            int iv = i.Get();
        }

    }
}

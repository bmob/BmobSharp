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

    }
}

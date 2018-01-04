//|---------------------------------------------------------------|
//|                         AZURE GRAPH DB                        |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                             Copyright ©2018 - Present         |
//|---------------------------------------------------------------|
//|                         AZURE GRAPH DB                        |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace AzureGraphDBSample
{
    class Program
    {
        public static void Main(string[] args)
        {
            new GremlinEmployeeGraphProcessor().ProcessEmployeeGraph();
            Console.WriteLine("Press any key to exit !");
            Console.ReadKey();
        }

    }
}

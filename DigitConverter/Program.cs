using System;
using System.Runtime.InteropServices;

namespace DigitConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var res = LibraryImport.ProcessData();
            
            // import = LibraryImport.Select();
            //import.ProcessData(1, 10, s => Console.WriteLine(s));
            Console.WriteLine($"Hello C++! {res}");
        }
    }

    public class LibraryImport
    {
        [DllImport("C:\\Users\\Asus\\Desktop\\transform.dll", CharSet = CharSet.Auto)]
        public static extern int ProcessData();

        //[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        //public delegate void Notification(string value);

        //[DllImport("transform.dll", CallingConvention = CallingConvention.StdCall)]
        //public static extern int ProcessData(int start, int count, Notification notification);

    }
}

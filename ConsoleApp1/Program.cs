using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Converter cvrt = new Converter(
                inputPath: "C:\\Users\\Asus\\Desktop\\data.txt",
                outputPath: "C:\\Users\\Asus\\Desktop\\output.txt");

            cvrt.Execute();
    
            //const uint mask = (1 << 24) - 1; // 0x3FF
            //uint last24 = input1 & mask;
        }
    }

    public class Digit 
    {
        public int letter { get; set; } = 0;
        public int order { get; set; } = 0;
        public int originNum { get; set; }

        public string mes { get; set; }
    
    }

    public class Converter 
    {
        private Stopwatch _stopWatch;
        private string _inputPath;
        private string _outputPath;
        private List<Digit> _list;
        private List<Digit> _sortedList;
        private int _counter = 0;
        private object locker = new object();

        public Converter(string inputPath, string outputPath)
        {
            _stopWatch = new Stopwatch();
            _inputPath = inputPath;
            _outputPath = outputPath;
            _list = new List<Digit>();
            _sortedList = new List<Digit>();
        }

        public void Execute() 
        {
            _stopWatch.Start();
            var tasks = ReadFile();
            Task.WaitAll(tasks);

            //var nss = _list.Where(x => x == null);
            var _clearList = _list.Where(x => x != null).ToList();
            _sortedList = _clearList.OrderBy(x => x.order).ToList();


            WriteToFile();
               


        }

        private void WriteToFile()
        {
            File.WriteAllLines(_outputPath, _sortedList.Select(x => $" {x.order} : {x.letter}"));
            _stopWatch.Stop();

            TimeSpan ts = _stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            File.AppendAllText(_outputPath, $"RunTime {elapsedTime}");
            File.AppendAllText(_outputPath, $"Total output records: {_list.Count}" );
            File.AppendAllText(_outputPath, $"Total input records : {_counter}");
        }

        private Task[] ReadFile()
        {
            int i = 0;
            Task[] pending = new Task[500000];

            foreach (string line in File.ReadLines(_inputPath))
            {
                Task tsk = new Task(() => Transform(line));
                pending[i] = tsk;
                tsk.Start();
                _counter++;
                i++;
            }
            return pending;
        }

        private void Transform(string strNum) 
        {
            //await Task.Run(() =>
            //{
                uint input = Convert.ToUInt32(strNum);
                int res = (int)LibraryImport.Transform(input);
                Digit dgt = new Digit() { originNum = res };
                _list.Add(dgt);

                var letter = GetBitsSum(res, 0, 8);
                var order = GetBitsSum(res, 8, 24);
                dgt.letter = letter;
                dgt.order = order;
            //});


            //}
            //catch (Exception ex)
            //{
            //    _list.Add(new Digit() { mes = ex.Message });
            //}

        }

        private int GetBitsSum(int input, int from, int to)
        {
            BitArray array = new BitArray(new[] { input });

            if (array.Length >= 32)
            {
                var byteArr = Enumerable.Range(from, to).Select(i => array[i]).ToArray();
                var res = GetResult(byteArr, from);
                //int[] array1 = new int[1]; array.CopyTo(array1, 0);
                return res;
            }
            return -1;

        }

        private int GetResult(bool[] arr, int from)
        {
            double sum = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i])
                {
                    sum += Math.Pow(2, from);
                }
                from++;
            }
            return (int)sum;

        }

//        public void ReadFile() 
//        {
//            using (FileStream fstream = File.OpenRead(_inputPath))
//            {
//                // выделяем массив для считывания данных из файла
//                byte[] buffer = new byte[fstream.Length];
//                // считываем данные
//                fstream.ReadAsync(buffer, 0, buffer.Length);
//                // декодируем байты в строку
//                string textFromFile = Encoding.UTF8.GetString(buffer);               

//                //var res1 = BitConverter.GetBytes(textFromFile);


//                File.WriteAllText(_inputPath, textFromFile);
//                //Console.WriteLine($"Текст из файла: {textFromFile}");
//                //WriteToFile(outputPath, textFromFile);
//            }
//        }

//        private void WriteToFile(string path, string text)
//        {
//            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
//            {
//                // преобразуем строку в байты
//                byte[] buffer = Encoding.Default.GetBytes(text);
//                // запись массива байтов в файл
//                 fstream.WriteAsync(buffer, 0, buffer.Length);
////                Console.WriteLine("Текст записан в файл");
//            }
//        }
    }


    public class LibraryImport
    {
        [DllImport("C:\\Users\\Asus\\Desktop\\transform.dll", 
            CharSet = CharSet.Auto, 
            SetLastError = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 Transform(UInt32 hWnd);

        //[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        //public delegate void Notification(string value);

        //[DllImport("transform.dll", CallingConvention = CallingConvention.StdCall)]
        //public static extern int ProcessData(int start, int count, Notification notification);
    }
    
}

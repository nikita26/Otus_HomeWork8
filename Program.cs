using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

#region Init array
Console.WriteLine("Генерация массива ...");
var arrayCount = 100000; 
var array = new int[arrayCount];

var rnd = new Random();
for (int i = 0; i < arrayCount; i++)
    array[i] = rnd.Next(-10000, 10000);
#endregion


Console.WriteLine($"Массив размером {arrayCount} элементов готов");

Console.WriteLine(Summ(array));
Console.WriteLine(SummThread(array));
Console.WriteLine(SummPLINQ(array));

Console.ReadLine();

int Summ(int[] arr)
{
    var timer1 = new Stopwatch();
    timer1.Start();
    var sum = 0;
    for (int i = 0; i < arr.Length; i++)
        sum += arr[i];
    timer1.Stop();
    Console.WriteLine($"Последовательное выполнение {timer1.ElapsedMilliseconds / (double)1000} сек");
    return sum;
}

int SummThread(int[] arr)
{
    var sum = 0;
    var sumLock = new object();
    var list = arr.ToList();

    var threadCount = 10;


    var timer2 = new Stopwatch();
    timer2.Start();

    int step = list.Count / threadCount;

    var threadData = new List<int[]>();
    for (int t = 0; t < threadCount; t++)
        threadData.Add(list.GetRange(step * t, step).ToArray());

    if (list.Count > step * threadCount)
    {
        threadData.Add(list.GetRange(step * threadCount, list.Count - (step * threadCount)).ToArray());
        threadCount++;
    }

    var b = new Barrier(threadCount+1);

    foreach (var data in threadData)
        new Thread(() =>
        {
            var s = 0;
            foreach (var d in data)
                s += d;
            sum += s;
            b.SignalAndWait();
        })
        .Start();

    b.SignalAndWait();
    timer2.Stop();
    Console.WriteLine($"Многопоточное выполнение (Thread) {timer2.ElapsedMilliseconds / (double)1000} сек");
    return sum;
}

int SummPLINQ(int[] arr)
{
    var timer3 = new Stopwatch();
    timer3.Start();
    var sum = arr.AsParallel().Sum(x => x);
    timer3.Stop();
    Console.WriteLine($"Многопоточное выполнение (PLINQ) {timer3.ElapsedMilliseconds / (double)1000} сек");

    return sum;
}

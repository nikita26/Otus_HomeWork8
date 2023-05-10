using System.Diagnostics;

#region Init array
var arrayCount = 10000000;
var array = new int[arrayCount];

for (int i = 0; i < arrayCount; i++)
    array[i] = new Random().Next(-10000, 10000);
#endregion


Console.WriteLine($"Размер {arrayCount}");

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
    var me = new ManualResetEvent(false);
    var timer2 = new Stopwatch();
    var threadCount = arr.Length-1;
    var threads = new List<Thread>();
    var sumLock = new object();

    timer2.Start();
    var sum = 0;
    foreach (var item in arr)
        new Thread(() => {
            lock (sumLock)
                sum += item;
            if (Interlocked.Decrement(ref threadCount) == 0)
                me.Set();
        }).Start();


    me.WaitOne();
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
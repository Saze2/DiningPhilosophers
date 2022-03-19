using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiningPhilosophers
{
    internal class Program
    {
        public static int _numberOfPhilosophers;
        public static int _thinkingTime;
        public static int _eatingTime;
        static List<Thread> _philosophers = new List<Thread>();
        static List<Fork> _forks = new List<Fork>();
        static Random _random;
        static bool _running = true;
        static Counter _counter;
        

        static void Main(string[] args)
        { 
            _random = new Random();
            _counter = new Counter();

            string input = Console.ReadLine();
            List<int> inputValues = input.Split(" ").Select(int.Parse).ToList();
            _numberOfPhilosophers = inputValues[0];
            _thinkingTime = inputValues[1];  
            _eatingTime = inputValues[2];

            

            for (int i = 0; i < _numberOfPhilosophers; i++)
            {
                _forks.Add(new Fork(i));
            }

            for (int j = 0; j < _numberOfPhilosophers; j++)
            {
                _philosophers.Add(new Thread(ThinkEatRepeat));
                _philosophers[j].Start(j);
            }
           
            string dummyInput = Console.ReadLine();
            _running = !string.IsNullOrWhiteSpace(dummyInput);
            Console.WriteLine("SIGNAL SHUTDOWN");
            Console.WriteLine("~ " + _counter._timeInMiliseconds/1000 + " seconds spent waiting");
     
        }

        private static void ThinkEatRepeat(object indexObject)
        {
            int index = (int)indexObject;
            int first;
            int second;

            if (index % 2 !=  0)
            {
                first = index;
                second = (index + 1) % _numberOfPhilosophers;
            }
            else
            {
                first = (index + 1) % _numberOfPhilosophers;
                second = index;
            }
                  
            while (_running)
            {
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                int thinkingTime = _random.Next(0, _thinkingTime);
                Thread.Sleep(thinkingTime);
                Console.WriteLine($"Philosopher {index} finished thinking");

                lock (_forks[first])
                {
                    _forks[first]._isUsed  = true;
                    Console.WriteLine($"Philosoper {index} took first fork: {_forks[first]._index}");

                    lock (_forks[second])
                    {
                        sw.Stop();                      
                        var time = sw.ElapsedMilliseconds;
                        lock (_counter)
                        {
                            _counter._timeInMiliseconds += time;
                        }
                        

                        _forks[second]._isUsed = true;
                        Console.WriteLine($"Philosoper {index} took second fork: {_forks[second]._index}");

                        int eatingTime = _random.Next(0, _eatingTime);
                        Thread.Sleep(eatingTime);
                        Console.WriteLine($"Philosopher {index} finished eating");

                        
                        _forks[second]._isUsed = false;
                        _forks[first]._isUsed  = false;
                        Console.WriteLine($"Philosoper {index} put back forks: {index} & {(index + 1) % _numberOfPhilosophers}");
                    }             
                }

                if (_running == false)
                {                                   
                    Console.WriteLine($"Philosopher {index} stopped working!");
                    _philosophers[index].Join();                  
                } 
            }

        }
    }

    public class Fork
    {
        public bool _isUsed;
        public int _index;
        public Fork(int index)
        {
            _isUsed = false;
            _index = index;
        }
    }
    public class Counter
    {
        public long _timeInMiliseconds;
        public Counter()
        {
            _timeInMiliseconds = 0;
        }
    }
}



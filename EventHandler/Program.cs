using System;
using ConsoleApplication1;
using ConsoleApplication2;
using ConsoleApplication3;


namespace ConsoleApplication
{
    
    class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Choose No Data (1), Data (2), or Delegate(3)");
            var choice = Console.ReadKey(true).KeyChar;
            if (choice == '1')
            {
                ConsoleApplication1.Counter c = new ConsoleApplication1.Counter(new Random().Next(10));
                c.ThresholdReached += c_ThresholdReached1;

                Console.WriteLine("v1. press 'a' key to increase total");
                while (Console.ReadKey(true).KeyChar == 'a')
                {
                    Console.WriteLine("adding one");
                    c.Add(1);
                }
            }
            else if (choice == '2')
            {
                ConsoleApplication2.Counter c = new ConsoleApplication2.Counter(new Random().Next(10));
                c.ThresholdReached += c_ThresholdReached2;

                Console.WriteLine("v2. press 'a' key to increase total");
                while (Console.ReadKey(true).KeyChar == 'a')
                {
                    Console.WriteLine("adding one");
                    c.Add(1);
                }
            }
            else if (choice == '3')
            {
                // Creates one delegate for each method. For the instance method, an
                // instance (mySC) must be supplied. For the static method, use the
                // class name.
                mySampleClass mySC = new mySampleClass();
                //method
                myMethodDelegate myD1 = new myMethodDelegate( mySC.myStringMethod );
                //Static method
                myMethodDelegate myD2 = new myMethodDelegate( mySampleClass.mySignMethod );

                // Invokes the delegates.
                Console.WriteLine( "{0} is {1}; use the sign \"{2}\".", 5, myD1( 5 ), myD2( 5 ) );
                Console.WriteLine( "{0} is {1}; use the sign \"{2}\".", -3, myD1( -3 ), myD2( -3 ) );
                Console.WriteLine( "{0} is {1}; use the sign \"{2}\".", 0, myD1( 0 ), myD2( 0 ) );
            
            }
           
        }
        static void c_ThresholdReached1(object sender, EventArgs e)
        {
            Console.WriteLine("The threshold was reached.");
            Environment.Exit(0);
        }
        static void c_ThresholdReached2(Object sender, ThresholdReachedEventArgs e)
        {
            Console.WriteLine("The threshold of {0} was reached at {1}.", e.Threshold, e.TimeReached);
            Environment.Exit(0);
        }
    }
}
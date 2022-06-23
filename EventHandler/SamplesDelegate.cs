using System;

namespace ConsoleApplication3
{
    

    // Declares a delegate for a method that takes in an int and returns a string.
    public delegate string myMethodDelegate( int myInt );

    // Defines some methods to which the delegate can point.
    public class mySampleClass  {

        // Defines an instance method.
        public string myStringMethod ( int myInt )  {
            if ( myInt > 0 )
                return( "positive" );
            if ( myInt < 0 )
                return( "negative" );
            return ( "zero" );
        }

        // Defines a static method.
        public static string mySignMethod ( int myInt )  {
            if ( myInt > 0 )
                return( "+" );
            if ( myInt < 0 )
                return( "-" );
            return ( "" );
        }
    }


    
}


/*
This code produces the following output:

5 is positive; use the sign "+".
-3 is negative; use the sign "-".
0 is zero; use the sign "".
*/
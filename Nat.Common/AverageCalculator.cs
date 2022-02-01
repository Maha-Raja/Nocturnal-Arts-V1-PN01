using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMethods 
{
    public class AverageCalculator
    { 
        public double NewRating(double Average_Rating_Value, int Number_Of_Ratings, double New_Rating) {
            double newAvg = ((Average_Rating_Value * Number_Of_Ratings) + New_Rating)/ Convert.ToInt32(Number_Of_Ratings + 1);
            return newAvg= Math.Round(newAvg, 2);
        }

    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FearStudy
{
    class Program
    {
        static void Main(string[] args)
        {

            Experiment.Run("archive\\FearStudyData.csv");
            Console.WriteLine("\nÇıkış için herhangi bir tuşa basınız...");
            Console.ReadKey();
        }
    }
}

using System;


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

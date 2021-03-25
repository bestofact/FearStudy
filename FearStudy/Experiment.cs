using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace FearStudy
{

    class Experiment
    {

        //Static Variables.
        private static List<Individual> Subjects = new List<Individual>();
        private static String StudyCsvPath;
        private static List<String> KnownFears = new List<string>();
        private static Dictionary<String, float> OvercomeSuccess = new Dictionary<String, float>();


        /// <summary>
        /// Only this function will be called by main program.
        /// </summary>
        /// <param name="study_csv_path">
        /// Study csv path relative to solution path.
        /// </param>
        public static void Run(string study_csv_path)
        {
            StudyCsvPath = study_csv_path;

            ParseStudy();
            CalculateKnownFears();
            FilterScreen();
            CalculateOvercomeSuccess();
            OvercomeSuccessScreen();
        }





        /// <summary>
        /// Interaction screen with user that allows user to filter fears.
        /// </summary>
        private static void FilterScreen()
        {
            String Input;
            List<String> AcceptedInputs;

            // Include mode will allow us to exlude or include filtered fears.
            // Ask for an input until user choses 0 or 1 as an input.
            Input = "A";
            AcceptedInputs = new List<String>{ "0","1" };
            while(!AcceptedInputs.Contains(Input))
            {
                Console.WriteLine("0 = exclude selected fears, 1 = include only selected fears");
                Console.WriteLine("Empty input with Exlude Selected filter mode will select all of the fears.");
                Console.Write("Select a filter mode : ");
                Input = Console.ReadLine();
                Console.Clear();
            }
            // Clear accepted inputs since we will have new rules for next input session.
            AcceptedInputs.Clear();
            Boolean IncludeMode = Input == "1";


            // A table like string for all known fears.
            // Every index is an accepted input for this input session. So we must add them to our list.
            String FearListAsString = "";
            foreach (String fear in KnownFears)
            {
                FearListAsString = ($"{FearListAsString} {KnownFears.IndexOf(fear)} : {fear}\n");
                AcceptedInputs.Add($"{KnownFears.IndexOf(fear)}");
            }

            // When we extract accepted inputs from selected fear indexes ( user input ), if we get a list with at least one member
            //  it means that our selected fear indexes has an unaccepted input. So continue the loop.
            List<String> SelectedFearIndexes = new List<String> {""};
            while( SelectedFearIndexes.Except(AcceptedInputs).Count() != 0)
            {
                Console.WriteLine("All known fears in this survey are listed below.You can select a mode and filter the fears.");
                Console.WriteLine("-------------------------");
                Console.Write(FearListAsString);
                Console.WriteLine("-------------------------");
                Console.Write("(Example Input : 1,5,4,12,6) - Select fears with coma between them : ");
                Input = Console.ReadLine();
                SelectedFearIndexes = new List<String>(Input.Split(','));

                // When no input is given, (direclty pressed to 'Enter'), make program select none index.
                if (Input == "") { SelectedFearIndexes.Clear(); }
                Console.Clear();
            }
            // This function recreates the KnownFears list with filtered values.
            FilterKnownFears(SelectedFearIndexes, IncludeMode);
        }

        /// <summary>
        /// Result screen at the end of the program. Shows filtered fears and theirs overcome success percentages.
        /// </summary>
        private static void OvercomeSuccessScreen()
        {
            foreach(KeyValuePair<String,float> overcome_rate in OvercomeSuccess)
            {
                Console.WriteLine($"Fear, Overcome Success = {overcome_rate.Key} , {overcome_rate.Value}%");
            }
        }







        /// <summary>
        /// Parses the Study Csv file and creates a list of Subjects which contains 
        /// individuals as individual classes.
        /// This should be called after setting  Study Csv file path with SetStudyCsvPath function.
        /// </summary>
        private static void ParseStudy()
        {
            using (var reader = new StreamReader(StudyCsvFullPath()))
            {
                //Line index for csv file. I assumed first line index is = 1
                int X = 0;
                while (!reader.EndOfStream)
                {
                    var SurveyResultForIndividual_X = reader.ReadLine();

                    if (X != 0)
                    {
                        Individual individual = new Individual(X, SurveyResultForIndividual_X);
                        AddIndividual(individual);
                    }
                    ++X;
                }
            }
        }

        /// <summary>
        /// Finds Known Fear from individuals that stored in Subjects list.
        /// This should be called after Parsing Study Csv file since Subject list
        /// is filled after parsing the Study Csv file.
        /// </summary>
        private static void CalculateKnownFears()
        {
            foreach(Individual individual in Subjects)
            {
                AddKnownFear_Unique(individual.GetFear());
            }
        }

        /// <summary>
        /// Calculates Overcome Success of the fears that are in KnownFears list from Subjects Individual data.
        /// This function directly sets the OvercomeSuccess Dictionary and should be called after
        /// FindKnownFears function.
        /// </summary>
        private static void CalculateOvercomeSuccess()
        {
            foreach(String fear in KnownFears)
            {
                int FearDensity = 0;
                int OvercomeDensity = 0;
                foreach(Individual individual in Subjects)
                {
                    if(individual.GetFear() != fear) { continue; }
                    ++FearDensity;
                    if(individual.Overcome) { ++OvercomeDensity; }
                }
                
                float OvercomeSuccess = ((float)OvercomeDensity / (float)FearDensity) * 100;
                AddOvercomeSuccess(fear, OvercomeSuccess);
            }
        }

        /// <summary>
        /// With selected filter mode and selected fears, this function recreates the KnownFears list.
        /// Called in FilterScreen function. I know it is a bit messy but it was an easy way out due to
        /// the parameters.
        /// </summary>
        /// <param name="selected_fear_indexes">
        /// Indexes of selected fears in KnownFears list.
        /// </param>
        /// <param name="include_mode">
        /// If true, it will recreate KnownFears list with only selected fears.
        /// </param>
        private static void FilterKnownFears(List<String> selected_fear_indexes, Boolean include_mode)
        {
            if (include_mode)
            {
                List<String> Temp = new List<string>(KnownFears);
                KnownFears = new List<String>();
                foreach(String index in selected_fear_indexes)
                {
                    AddKnownFear_Unique(Temp[Int32.Parse(index)]);
                }
            }
            else
            {
                foreach(String index in selected_fear_indexes)
                {
                    RemoveKnownFear(KnownFears[Int32.Parse(index)]);
                }
            }
        }







        //Bunch of Add/Remove functions that I used for our static variables. They are not necessary but I believe
        // using this methods will be a good practise when I start working with big projects.
        private static void AddIndividual(Individual individual) { Subjects.Add(individual); }
        private static void RemoveIndividual(Individual individual) { if (Subjects.Contains(individual)) { Subjects.Remove(individual); } }

        private static void AddKnownFear_Unique(String fear) { if(!KnownFears.Contains(fear)) { KnownFears.Add(fear); } }
        private static void RemoveKnownFear(String fear) { if (KnownFears.Contains(fear)) { KnownFears.Remove(fear); } }

        private static void AddOvercomeSuccess(String fear, float overcome_success) { OvercomeSuccess.Add(fear, overcome_success); }


        // Returns full path of StudyCsvPath assuming StudyCsvPath is relative to solution path.
        private static String StudyCsvFullPath()
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"..\\..\\..\\{StudyCsvPath}"));
        }       
    }


    // I used an Individual class to make it more easy to work with. So that I don't use dictionaries to hold individuals 
    //  surveys like json files.
    class Individual
    {
        public int Index;
        public String Fear;
        public String Greatest;
        public int Impact;
        public Boolean Past;
        public int Encounter;
        public Boolean Overcome;
        public Boolean Embarrassed;

        /// <summary>
        /// Creates an individual with survey_result.
        /// </summary>
        /// <param name="survey_index">
        /// It is like id of this individual. 
        /// Not necessery for the main purpose of our program but it is cleaner for me to have it.
        /// </param>
        /// <param name="survey_result">
        /// Order must be : {'Fear', 'Greatest', 'Impact', 'Past', 'Encounter', 'Overcome', 'Embarrassed'}
        /// </param>
        public Individual(int survey_index, string survey_result)
        {
            Index = survey_index;

            var result_list = survey_result.Split(',');
            Fear = result_list[0];
            Greatest = result_list[1];
            Impact = Int32.Parse(result_list[2]);
            Past = result_list[3] == "Yes";
            Encounter = Int32.Parse(result_list[4]);
            Overcome = result_list[5] == "Yes";
            Embarrassed = result_list[6] == "Yes";
        }

        /// <summary>
        /// Returns the fear or greatest (when fear is 'other') of Individual. 
        /// </summary>
        public String GetFear()
        {
            return Fear == "Other" ? Greatest : Fear;
        }

        /// <summary>
        /// A info function for me to use when i debug individual class to see if it works or not.
        /// Not necessary for program to work now but good to keep it in here.
        /// </summary>
        public String Info()
        {
            var Title = $"==================\nInfo for Person {Index}\n------------------";
            var FearLine = $"Fear : {Fear}";
            var GreatestLine = $"Greatest : {Greatest}";
            var ImpactLine = $"Impact : {Impact}";
            var PastLine = $"Past : {Past}";
            var EncounterLine = $"Encounter : {Encounter}";
            var OvercomeLine = $"Overcome : {Overcome}";
            var EmbarrassedLine = $"Embarrassed : {Embarrassed} \n";

            var Info = $"{Title}\n{FearLine}\n{GreatestLine}\n{ImpactLine}\n{PastLine}\n{EncounterLine}\n{OvercomeLine}\n{EmbarrassedLine}";
            
            return Info;
        }
    }
}

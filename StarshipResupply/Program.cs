using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using StarshipResupply.Models;

namespace StarshipResupply
{
    /*
        This is the main program for the assingment for Kneat Software - Coding Challenge
    */
    /// <summary>
    /// This console application takes in a unit of distance measure in mega lights.
    /// Then it calculates the number of stops needed for each ship in the data.
    /// </summary>
    /// <example>
    /// Please enter distance in mega lights (MGLT):100000
    /// 
    /// CR90 corvette: 0
    /// Star Destroyer: 0
    /// Sentinel-class landing craft: 1
    /// Death Star: 0
    /// Millennium Falcon: 0
    /// Y-wing: 7
    /// X-wing: 5
    /// </example>
    public class CalculateShipResupplyStops
    {
        private const string URL = "https://swapi.dev/api/starships/";

        static void Main(string[] args)
        {
            // Using stopwatch in an attempt to limit the application up time 
            // when in an idle state. 
            // The Console input locks application.
            Stopwatch timer = new Stopwatch();

            while (timer.Elapsed.TotalMinutes < 10) 
            {
                timer.Start();
                long mglt = GetUserInput();


                CalculateShipStops(mglt);

            }
            ExitApplication();
            timer.Stop();
        }

        /* GetUserInput is a method to put the code for asking the user for input.*/
        /// <summary>
        /// Here I encapsulate the functionality of getting the user input.
        /// Idealy this could be improved upon to create a better UI.
        /// </summary>
        /// <returns>The user input distance value in mega lights; long value type</returns>
        public static long GetUserInput()
        {
            long mglt = 0;
            Console.WriteLine("");
            Console.Write("Please enter distance in mega lights (MGLT) or 'exit' or 'quit': ");
            string userInput = Console.ReadLine();
            Console.WriteLine("");

            if (userInput.ToLower().Equals("exit") || userInput.ToLower().Equals("quit"))
            {
                ExitApplication();
            }
            else if (!Int64.TryParse(userInput, out mglt))
            {
                Console.WriteLine("Input must be a number. '{0}' is invalid.", userInput);
                return GetUserInput();
            }
            else if (mglt < 0)
            {
                Console.WriteLine("Value must be positive. '{0}' is invalid", mglt);
                return GetUserInput();
            }
            
            return mglt;
        }

        /* CalculateShipStops calculates resupply stops using inputed mega lights */
        /// <summary>
        /// Taking mega lights long value, this method calls GetStarshipData method to Starship data and 
        /// then uses the Consumables and Mega Lights values for each ship to calculate the number of stops.
        /// A ship's mega lights value is the number of mega lights it can go in 1 hour.
        /// a shop's consumable value is then number of days, weeks, months, or years, that it has of 
        /// consumables before resupply is required
        /// <c>megalights distance / (ship consumableHours * ship megaLights)</c>
        /// </summary>
        /// <param name="mglt"></param>
        public static void CalculateShipStops(long mglt)
        {
            List<Starship> starships = GetStarshipData();

            foreach(var ship in starships)
            {
                long resupplyStops;
                long consumableHours = 0;
                if (!Int64.TryParse(ship.MGLT,out long megaLights)) 
                {
                    megaLights = -1;
                }

                if (ship.consumables.Equals("unknown"))
                {
                    consumableHours = -1;
                }
                else
                {
                    string[] consumables = ship.consumables.Split(' ');

                    switch (consumables[1])
                    {
                        case "day":
                        case "days":
                            consumableHours = Int64.Parse(consumables[0]) * 24;
                            break;
                        case "week":
                        case "weeks":
                            consumableHours = Int64.Parse(consumables[0]) * 168;
                            break;
                        case "month":
                        case "months":
                            consumableHours = Int64.Parse(consumables[0]) * 730;
                            break;
                        case "year":
                        case "years":
                            consumableHours = Int64.Parse(consumables[0]) * 8760;
                            break;

                    }
                }
                if (megaLights > 0 && consumableHours > 0)
                {
                    resupplyStops = mglt / (consumableHours * megaLights);

                    Console.WriteLine(ship.name + ": " + resupplyStops);
                }
                else
                {
                    string ml = (megaLights < 0) ? "unknown" : megaLights.ToString();
                    string consumables = (consumableHours < 0) ? "unknown" : consumableHours.ToString();

                    Console.WriteLine("{0}: (missing data)(MegaLights: {1}, Consumables: {2})", ship.name, ml, consumables);
                }
            }

        }

        /* GetStarshipData retrieves the starship data from SWAPI : The Star Wars API */
        /// <summary>
        /// This calls out to the SWAPI can retreives all the Starship data.
        /// It uses the call to find out the total count of ships and then pulls the data in pages
        /// of 10 until it has all the ships data.
        /// 
        /// NOTE: I went on a bit of a rabbit hole here as things did not work as I expected.
        /// I also was tempted and even tested out using some of the Helper libraries in the SWAPI Documentation.
        /// I did not find them as usefull as I wanted. Partly I think because I did not understand the code and did 
        /// not want to take the time to fully dig into their code.
        /// 
        /// NOTE: This is a bit bare bones. I would put this into its own class and have expect more error checking 
        /// and exception handling in a bigger app.
        /// </summary>
        /// <returns>This returns a List of StarshipResupply.Models.Starship objects</returns>
        public static List<Starship> GetStarshipData()
        {
            List<Starship> starships = new List<Starship>();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            bool recievedAllData = false;
            int page = 1;
            while (!recievedAllData) { 
                HttpResponseMessage response = client.GetAsync("?page=" + page).Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<dynamic>(json);
                    int count = data.count;
                    starships.AddRange(data.results.ToObject<List<Starship>>());
                    if (starships.Count < count)
                        page++;
                    else
                        recievedAllData = true;
                }
                else
                {
                    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
            }

            return starships;
        }

        /* ExitApplication() holds code to be used to exit the application*/
        /// <summary>
        /// This is used for when an event happens or is fired that asks the application to exit.
        /// It was intented to be used in both the GetUserInput when a user types 'exit' or 'quit'
        /// as well as the timer on the loop for the main function, but I decided to avoid another
        /// rabbit hole.
        /// </summary>
        private static void ExitApplication()
        {
            Console.WriteLine("");
            Console.WriteLine("Thank you for your request. Have a nice day, Citizen!");
            Environment.Exit(0);
        }
    }
}
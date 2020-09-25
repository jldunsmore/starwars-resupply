using System;
using System.Collections.Generic;

namespace StarshipResupply.Models
{
    /*
        This is the Starship Class derived from the API SWAPI (https://swapi.dev/api/starships)
    */
    /// <summary>
    /// This only needed to have the class data fields.
    /// </summary>
    /// <remarks>
    /// I used the online tool https://json2csharp.com/ and selected the json of a starship from API mentioned above
    /// </remarks>
    public class Starship
	{
        public string name { get; set; }
        public string model { get; set; }
        public string manufacturer { get; set; }
        public string cost_in_credits { get; set; }
        public string length { get; set; }
        public string max_atmosphering_speed { get; set; }
        public string crew { get; set; }
        public string passengers { get; set; }
        public string cargo_capacity { get; set; }
        public string consumables { get; set; }
        public string hyperdrive_rating { get; set; }
        public string MGLT { get; set; }
        public string starship_class { get; set; }
        public List<string> pilots { get; set; }
        public List<string> films { get; set; }
        public DateTime created { get; set; }
        public DateTime edited { get; set; }
        public string url { get; set; }
    }
}

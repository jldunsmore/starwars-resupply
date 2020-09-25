using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

using StarshipResupply;
using StarshipResupply.Models;
using System.Linq;

namespace StarshipResupplyUnitTests
{
    [TestClass]
    public class StarshipResupplyTests
    {
        [TestMethod]
        public void GetUserInput_valid_input()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader("1000000");
            Console.SetIn(input);

            long userInput = CalculateShipResupplyStops.GetUserInput();

            Assert.AreEqual(userInput, 1000000);
        }

        [TestMethod]
        public void GetUserInput_invalid_2_times_input()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader("sadf");
            Console.SetIn(input);
            input = new StringReader("rty");
            Console.SetIn(input);
            input = new StringReader("2000000");
            Console.SetIn(input);

            long userInput = CalculateShipResupplyStops.GetUserInput();

            Assert.AreEqual(userInput, 2000000);
        }

        [TestMethod]
        public void GetUserInput_invalid_negative_number()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader("-1000000");
            Console.SetIn(input);
            Console.SetOut(output);
            input = new StringReader("3000000");
            Console.SetIn(input);

            long userInput = CalculateShipResupplyStops.GetUserInput();

            List<string> lines = output.ToString().Split(Environment.NewLine).ToList();

            Assert.AreEqual(3000000, userInput);
        }

        [TestMethod]
        public void GetStarshipData_TestApiCall()
        {
            List<Starship> starships = CalculateShipResupplyStops.GetStarshipData();
            var MillenniumFalcon = starships.Find(ship => ship.name.Equals("Millennium Falcon"));
            var TradeFederationCruiser = starships.Find(ship => ship.name.Equals("Trade Federation cruiser"));

            Assert.AreEqual(36, starships.Count);
            Assert.AreEqual("75", MillenniumFalcon.MGLT);
            Assert.AreEqual("1050", TradeFederationCruiser.max_atmosphering_speed);
        }

        [TestMethod]
        public void CalculateShipStops_valid_MGLT()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            CalculateShipResupplyStops.CalculateShipStops(1000000);

            var lines = output.ToString().Split(Environment.NewLine).ToList();
            var MillenniumFalcon = lines.Find(ship => ship.Contains("Millennium Falcon"));
            var TradeFederationCruiser = lines.Find(ship => ship.Contains("Trade Federation cruiser"));

            Assert.AreEqual(37, lines.Count);
            Assert.AreEqual("Millennium Falcon: 9", MillenniumFalcon);
            Assert.AreEqual("Trade Federation cruiser: (missing data)(MegaLights: unknown, Consumables: 35040)", TradeFederationCruiser);
        }
    }
}
 
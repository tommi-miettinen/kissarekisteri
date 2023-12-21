using Kissarekisteri.Models;
using System.Collections.Generic;

namespace Kissarekisteri.Database
{
    public static class CatBreedSeed
    {
        public static List<CatBreed> GetSeedData()
        {
            var breeds = new List<CatBreed>
            {
                new() { Name = "Siamese" },
                new() { Name = "Persian" },
                new() { Name = "Maine Coon" }
            };

            return breeds;
        }
    }
}

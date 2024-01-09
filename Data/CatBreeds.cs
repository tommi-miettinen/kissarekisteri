using Kissarekisteri.Models;
using System.Collections.Generic;

namespace Kissarekisteri.Data
{
    public static class CatBreeds
    {
        public static readonly List<CatBreed> Breeds =
        [
            new() { Name = "Siamese" },
            new() { Name = "Persian" },
            new() { Name = "Maine Coon" }
        ];
    }
}

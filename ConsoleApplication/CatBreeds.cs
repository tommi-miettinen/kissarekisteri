using Kissarekisteri.Models;

namespace Kissarekisteri.Data
{
    public static class CatBreeds
    {
        public static readonly List<CatBreed> Breeds =
        [
            new() { Name = "Siamese" },
            new() { Name = "Persian" },
            new() { Name = "Maine Coon" },
            new() { Name = "Ragdoll" },
            new() { Name = "Bengal" },
            new() { Name = "Sphynx" },
            new() { Name = "British Shorthair" },
            new() { Name = "Abyssinian" },
            new() { Name = "Scottish Fold" },
            new() { Name = "Burmese" }
        ];
    }
}

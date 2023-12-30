using Microsoft.EntityFrameworkCore;

namespace Kissarekisteri.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class CatBreed
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

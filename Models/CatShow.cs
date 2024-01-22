using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.Models
{
    public class CatShow
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string ImageUrl { get; set; }

        public List<CatShowCats> Cats { get; set; }
        public List<CatShowPhoto> Photos { get; set; }

        public List<CatShowResult> Results { get; set; }
    }
}

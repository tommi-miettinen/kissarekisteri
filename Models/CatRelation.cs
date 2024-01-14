using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.Models
{
    public class CatRelation : IValidatableObject
    {
        public int Id { get; set; }

        public int ParentId { get; set; }
        public int KittenId { get; set; }
        public string ParentType { get; set; }

        public Cat ParentCat { get; set; }
        public Cat ChildCat { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ParentId == KittenId)
            {
                yield return new ValidationResult("Parent and child cannot be the same cat");
            }
        }
    }
}

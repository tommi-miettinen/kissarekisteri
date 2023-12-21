using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.Models
{
    public class CatParent : IValidatableObject
    {
        public int Id { get; set; }
        public int ParentCatId { get; set; }

        public Cat ParentCat { get; set; }
        public int ChildCatId { get; set; }
        public Cat ChildCat { get; set; }
        public string ParentType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ParentCatId == ChildCatId)
            {
                yield return new ValidationResult("Parent and child cannot be the same cat");
            }
        }
    }
}

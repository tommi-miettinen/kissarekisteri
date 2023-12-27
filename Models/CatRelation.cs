using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.Models
{
    public class CatRelation : IValidatableObject
    {
        public int Id { get; set; }
        public int ParentCatId { get; set; }
        public int ChildCatId { get; set; }
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

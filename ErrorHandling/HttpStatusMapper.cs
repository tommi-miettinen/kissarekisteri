using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Kissarekisteri.ErrorHandling
{


    public static class HttpStatusMapper
    {
        public static ActionResult Map(List<Error> errors)
        {
            foreach (var error in errors)
            {
                //CatErrors
                if (error == CatErrors.NotFound) return new NotFoundObjectResult(error.Message);
                if (error == CatErrors.PhotoUploadError) return new BadRequestObjectResult(error.Message);
                if (error == CatErrors.MotherNotFound) return new BadRequestObjectResult(error.Message);
                if (error == CatErrors.FatherNotFound) return new BadRequestObjectResult(error.Message);

                //CatShowErrors
                if (error == CatShowErrors.NotFound) return new NotFoundObjectResult(error.Message);





            }


            return new ObjectResult("Internal server error")
            {
                StatusCode = 500
            };
        }
    }
}
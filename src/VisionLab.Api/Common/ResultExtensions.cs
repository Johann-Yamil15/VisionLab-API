using Microsoft.AspNetCore.Mvc;
using VisionLab.Application.Common;

namespace VisionLab.Api.Common;

public static class ResultExtensions
{
    /// <summary>Convierte un error de negocio en una respuesta ProblemDetails (RFC 9457).</summary>
    public static ActionResult ToProblem(this ControllerBase controller, Error error)
    {
        var status = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        return controller.Problem(detail: error.Message, statusCode: status);
    }
}

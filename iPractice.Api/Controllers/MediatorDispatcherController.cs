using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using iPractice.Domain.Pipeline;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iPractice.Api.Controllers;

public abstract class MediatorDispatcherController : ControllerBase
{
    private readonly IMediator mediator;
    private readonly ILogger logger;

    protected MediatorDispatcherController(
        IMediator mediator,
        ILogger<MediatorDispatcherController> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    protected async Task<ActionResult> DispatchAndRespondAsync<TRequest>(TRequest request)
    {
        object objResponse;

        try
        {
            objResponse = await mediator.Send(request);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                $"Exception while executing {request.GetType().Name}.");

            return StatusCode(500, new { errors = new[] { "An internal server error occurred." } } );
        }

        if (objResponse is Response response)
        {
            if (response.Errors.Any())
            {
                int mostCommonHttpCode = response.Errors
                    .GroupBy(x => x.Code)
                    .OrderByDescending(x => x.Key)
                    .First()
                    .Key;

                return StatusCode(
                    mostCommonHttpCode,
                    new { errors = response.Errors.Select(x => $"{x.Field} {x.Message}").ToArray() });
            }

            if (response.IsCreated)
            {
                // TODO this should return the URL of the created
                // resource, but that's assigned in the DB and I
                // couldn't work out how to retrieve it. :-)
                // Ideally entities would be keyed by GUID which
                // are provided by the code, not originated in
                // the db.
                return StatusCode((int)HttpStatusCode.Created); 
            }
        }

        return NoContent();
    }
}

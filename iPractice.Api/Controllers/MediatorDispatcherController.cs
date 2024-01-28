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
    readonly IMediator mediator;
    readonly ILogger logger;

    protected MediatorDispatcherController(
        IMediator mediator,
        ILogger<MediatorDispatcherController> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    protected async Task<ActionResult> DispatchAndRespondAsync<TRequest>(
        TRequest request)
    {
        Response response;

        try
        {
            response = await mediator.Send(request) as Response;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                $"Exception while executing {request.GetType().Name}.");

            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                new { errors = new[] { "An internal server error occurred." } } );
        }

        if (response == null)
        {
            return NoContent();
        }

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

        if (response.Payload != null)
        {
            return Ok(response.Payload);
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

        return NoContent();
    }
}

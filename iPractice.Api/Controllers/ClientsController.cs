using iPractice.Api.Models;
using iPractice.Domain.Features.Client.Timeslots;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace iPractice.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientsController : MediatorDispatcherController
{
    public ClientsController(
        IMediator mediator,
        ILoggerFactory loggerFactory)
        : base(
            mediator,
            loggerFactory.CreateLogger<MediatorDispatcherController>())
    {
    }

    /// <summary>
    /// The client can see when his psychologists are available.
    /// Get available slots from his two psychologists.
    /// </summary>
    /// <param name="clientId">The client ID</param>
    /// <returns>All time slots for the selected client</returns>
    [HttpGet("{clientId}/timeslots")]
    [ProducesResponseType(typeof(IEnumerable<TimeSlot>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetAvailableTimeSlots(long clientId)
    {
        Get.Query q = new Get.Query { ClientId = clientId };

        return await DispatchAndRespondAsync(q);
    }

    /// <summary>
    /// Create an appointment for a given availability slot
    /// </summary>
    /// <param name="clientId">The client ID</param>
    /// <param name="timeSlot">Identifies the client and availability slot</param>
    /// <returns>Ok if appointment was made</returns>
    [HttpPost("{clientId}/appointment")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CreateAppointment(long clientId, [FromBody] TimeSlot timeSlot)
    {
        throw new NotImplementedException();
    }
}
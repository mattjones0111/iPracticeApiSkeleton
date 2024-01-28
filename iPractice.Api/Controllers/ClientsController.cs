using iPractice.Contracts;
using iPractice.Domain.Features.Client.Appointments;
using iPractice.Domain.Features.Client.Timeslots;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetAvailableTimeSlots(long clientId)
    {
        Get.Query q = new() { ClientId = clientId };

        return await DispatchAndRespondAsync(q);
    }

    /// <summary>
    /// Create an appointment for a given availability slot
    /// </summary>
    /// <param name="clientId">The client ID</param>
    /// <param name="timeSlot">Identifies the client and availability slot</param>
    /// <returns>Status 201 Created if appointment was made</returns>
    [HttpPost("{clientId}/appointments")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<ActionResult> CreateAppointment(
        long clientId,
        [FromBody] TimeSlot timeSlot)
    {
        Create.Command command = new()
        {
            ClientId = clientId,
            PsychologistId = timeSlot.PsychologistId,
            Start = timeSlot.Start,
            End = timeSlot.End
        };

        return await DispatchAndRespondAsync(command);
    }
}

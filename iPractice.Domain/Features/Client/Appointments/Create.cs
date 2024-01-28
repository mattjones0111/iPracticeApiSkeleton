using FluentValidation;
using iPractice.DataAccess;
using iPractice.Domain.Aggregates;
using iPractice.Domain.Aspects.Validation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using iPractice.Domain.Pipeline;

namespace iPractice.Domain.Features.Client.Appointments;

public class Create
{
    public class Command : IRequest<Response>
    {
        public long ClientId { get; set; }
        public long PsychologistId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        readonly ApplicationDbContext db;

        public Validator(ApplicationDbContext db)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            this.db = db;

            RuleFor(x => x.ClientId)
                .MustAsync(ExistAsClientAsync)
                .WithMessage("Client not found.")
                .WithHttpStatusCode(HttpStatusCode.NotFound)
                .MustAsync(ClientHasPsychologistAsync)
                .WithMessage("Client is not associated with this psychologist.")
                .WithHttpStatusCode(HttpStatusCode.NotFound);

            RuleFor(x => x.PsychologistId)
                .MustAsync(ExistsAsPsychologistAsync)
                .WithMessage("Psychologist not found.")
                .WithHttpStatusCode(HttpStatusCode.NotFound);

            RuleFor(x => x.End)
                .GreaterThan(x => x.Start)
                .WithMessage("Start date must occur before end date.");

            RuleFor(x => x.PsychologistId)
                .MustAsync(BeAvailableAsync)
                .WithHttpStatusCode(HttpStatusCode.Conflict);
        }

        async Task<bool> BeAvailableAsync(
            Command command,
            long psychologistId,
            CancellationToken cancellationToken)
        {
            DataAccess.Models.Psychologist data = await db.Psychologists
                .AsNoTracking()
                .Include(x => x.Availability)
                .Include(x => x.Clients)
                .SingleOrDefaultAsync(x => x.Id == command.PsychologistId, cancellationToken);

            PsychologistAggregate aggregate = new(data);
            return aggregate.IsAvailable(command.Start, command.End);
        }

        Task<bool> ExistsAsPsychologistAsync(
            long psychologistId,
            CancellationToken cancellationToken)
        {
            return db.Psychologists
                .AnyAsync(x => x.Id == psychologistId, cancellationToken);
        }

        Task<bool> ExistAsClientAsync(
            long clientId,
            CancellationToken cancellationToken)
        {
            return db.Clients
                .AnyAsync(x => x.Id == clientId, cancellationToken);
        }

        async Task<bool> ClientHasPsychologistAsync(
            Command command,
            long clientId,
            CancellationToken cancellationToken)
        {
            DataAccess.Models.Client client = await db.Clients
                .AsNoTracking()
                .Include(x => x.Psychologists)
                .SingleAsync(x => x.Id == clientId, cancellationToken);

            ClientAggregate clientAggregate = new(client);
            return clientAggregate.HasPsychologist(command.PsychologistId);
        }
    }

    public class Handler : IRequestHandler<Command, Response>
    {
        readonly ApplicationDbContext db;

        public Handler(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<Response> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            DataAccess.Models.Client data = await db.Clients
                .Include(x => x.Appointments)
                .SingleOrDefaultAsync(
                    x => x.Id == request.ClientId,
                    cancellationToken);

            ClientAggregate aggregate = new(data);

            aggregate.BookAppointment(
                request.Start,
                request.End,
                request.PsychologistId);

            await db.SaveChangesAsync(cancellationToken);

            return Response.Created();
        }
    }
}

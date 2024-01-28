using FluentValidation;
using iPractice.DataAccess;
using iPractice.Domain.Aggregates;
using iPractice.Domain.Aspects.Validation;
using iPractice.Domain.Pipeline;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace iPractice.Domain.Features.Psychologist.Availability;

public class Create
{
    public class Command : IRequest<Response>
    {
        public long PsychologistId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly ApplicationDbContext db;

        public Validator(ApplicationDbContext db)
        {
            this.db = db;

            RuleFor(x => x.End)
                .GreaterThan(x => x.Start)
                .WithMessage("Start date must occur before end date.");

            RuleFor(x => x.PsychologistId)
                .MustAsync(Exist)
                .WithMessage("Psychologist not found.")
                .WithHttpStatusCode(HttpStatusCode.NotFound)
                .MustAsync(NotOverlapExistingAvailability)
                .WithMessage("Requested availability overlaps an existing availability.")
                .WithHttpStatusCode(HttpStatusCode.Conflict);
        }

        async Task<bool> NotOverlapExistingAvailability(
            Command command,
            long psychologistId,
            CancellationToken cancellationToken)
        {
            DataAccess.Models.Psychologist dataModel = await db.Psychologists
                .Include(x => x.Availability)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == psychologistId, cancellationToken);

            if (dataModel == null || !dataModel.Availability.Any())
            {
                return true;
            }

            PsychologistAggregate aggregate = new(dataModel);

            return !aggregate.OverlapsAnyAvailability(command.Start, command.End);
        }

        Task<bool> Exist(
            long psychologistId,
            CancellationToken cancellationToken)
        {
            return db.Psychologists
                .AnyAsync(x => x.Id == psychologistId, cancellationToken);
        }
    }

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly ApplicationDbContext db;

        public Handler(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<Response> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            DataAccess.Models.Psychologist fromDb = await db
                .Psychologists
                .Include(x => x.Availability)
                .SingleAsync(x => x.Id == request.PsychologistId, cancellationToken);

            PsychologistAggregate aggregate = new PsychologistAggregate(fromDb);

            aggregate.AddAvailability(request.Start, request.End);

            await db.SaveChangesAsync(cancellationToken);

            return Response.Created();
        }
    }
}

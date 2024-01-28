using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using iPractice.Contracts;
using iPractice.DataAccess;
using iPractice.Domain.Aggregates;
using iPractice.Domain.Aspects.Validation;
using iPractice.Domain.Pipeline;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPractice.Domain.Features.Client.Timeslots;

public class Get
{
    public class Query : IRequest<Response>
    {
        public long ClientId { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        readonly ApplicationDbContext db;

        public Validator(ApplicationDbContext db)
        {
            this.db = db;

            RuleFor(x => x.ClientId)
                .MustAsync(ExistAsync)
                .WithMessage("Client not found.")
                .WithHttpStatusCode(HttpStatusCode.NotFound);
        }

        Task<bool> ExistAsync(long clientId, CancellationToken cancellationToken)
        {
            return db.Clients.AnyAsync(x => x.Id == clientId, cancellationToken);
        }
    }

    public class Handler : IRequestHandler<Query, Response>
    {
        readonly ApplicationDbContext db;

        public Handler(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<Response> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            DataAccess.Models.Client data = await db.Clients
                .Include(x => x.Psychologists)
                .ThenInclude(x => x.Availability)
                .SingleOrDefaultAsync(
                    x => x.Id == request.ClientId,
                    cancellationToken);

            List<TimeSlot> result = new();

            foreach (DataAccess.Models.Psychologist p in data.Psychologists)
            {
                PsychologistAggregate aggregate = new(p);

                result.AddRange(
                    aggregate.GetAvailableTimeSlots()
                        .Select(x => new TimeSlot
                        {
                            PsychologistId = p.Id,
                            Start = x.From,
                            End = x.To
                        }));
            }

            return Response.WithPayload(result.ToArray());
        }
    }
}

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using iPractice.Api.Models;
using iPractice.DataAccess;
using iPractice.Domain.Aspects.Validation;
using iPractice.Domain.Pipeline;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iPractice.Domain.Features.Client.Timeslots;

public class Get
{
    public class Query : IRequest<Response<TimeSlot[]>>
    {
        public long ClientId { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        private readonly ApplicationDbContext db;

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
        public Task<Response> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(Response.Created());
        }
    }
}

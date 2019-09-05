using System;
using MediatR;
using AutoMapper;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;
using Persistence;
using Application.Errors;

namespace Application.Comments
{
  public class Create
  {
    public class Command : IRequest<CommentDto>
    {
      public string Body { get; set; }
      public Guid ActivityID { get; set; }
      public string Username { get; set; }
    }

    public class Handler : IRequestHandler<Command, CommentDto>
    {
      private readonly DataContext context;
      private readonly IMapper mapper;
      public Handler(DataContext Context, IMapper Mapper) { this.mapper = Mapper; this.context = Context; }

      public async Task<CommentDto> Handle(Command request, CancellationToken cancellationToken)
      {
        var activity = await context.Activities.FindAsync(request.ActivityID);
        if (activity == null) throw new RestException(HttpStatusCode.NotFound, new {Activity = "Not found"});

        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == request.Username);

        var comment = new Comment { ID = new Guid(), Body = request.Body, Author = user, Activity = activity, CreatedAt = DateTime.Now };
        activity.Comments.Add(comment);
        var commentToReturn = mapper.Map<CommentDto>(comment);

        var success = await context.SaveChangesAsync() > 0;
        if (!success) throw new Exception("Problem saving changes"); else return commentToReturn;
      }
    }
  }
}

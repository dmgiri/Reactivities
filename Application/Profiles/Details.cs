using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Profiles
{
  public class Details
  {
    public class Query : IRequest<Profile> { public string Username { get; set; } }

    public class Handler : IRequestHandler<Query, Profile>
    {
      private readonly IProfileReader profileReader;
      public Handler(IProfileReader ProfileReader) => this.profileReader = ProfileReader;

      public async Task<Profile> Handle(Query request, CancellationToken cancellationToken) => await profileReader.ReadProfile(request.Username);
    }
  }
}
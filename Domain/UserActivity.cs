using System;

namespace Domain
{
  public class UserActivity
  {
    public string AppUserID { get; set; }      
    public virtual AppUser AppUser { get; set; }
    public Guid ActivityID { get; set; }
    public virtual Activity Activity { get; set; }
    public DateTime DateJoined { get; set; }
    public bool IsHost { get; set; }
  }
}
namespace Domain
{
  public class UserFollowing
  {
    public string ObserverID { get; set; }
    public virtual AppUser Observer { get; set; }
    public string TargetID { get; set; }
    public virtual AppUser Target { get; set; }
  }
}
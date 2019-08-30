using FluentValidation;

namespace Application.Validators
{
  public static class ValidatorExtensions
  {
    public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder) 
    {
      var options = ruleBuilder
        .NotEmpty()
        .MinimumLength(6).WithMessage("Password be at least 6 characters")
        .Matches("[A-Z]").WithMessage("Password must contain at least one upper case character")
        .Matches("[a-z]").WithMessage("Password must contain at least one lower case character")
        .Matches("[0-9]").WithMessage("Password must contain at least one number")
        .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one nonalphanumeric character");
      
      return options;
    }
  }
}
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityProvider.ViewModels.Authorization;

public class LogoutViewModel
{
    [BindNever]
    public string RequestId { get; set; }
}

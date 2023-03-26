using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IdentityProvider.ViewModels.Shared;

public class ErrorViewModel
{
    [Display(Name = "Error")]
    public string Error { get; set; }

    [Display(Name = "Description")]
    public string ErrorDescription { get; set; }
}

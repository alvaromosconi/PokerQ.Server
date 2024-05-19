using System.ComponentModel.DataAnnotations;

namespace Entities;
public class QRCodeModel
{
    [Display(Name = "Enter QRCode Text")]
    public string QRCodeText { get; set; }
}

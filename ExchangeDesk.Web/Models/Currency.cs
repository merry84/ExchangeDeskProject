using System.ComponentModel.DataAnnotations;

namespace ExchangeDesk.Web.Models
{
    public class Currency
    {
        // Използваме кода като ключ, за да е кратко и удобно (EUR, USD...)
        [Key]
        [StringLength(3)]
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}

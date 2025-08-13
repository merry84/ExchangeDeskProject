using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeDesk.Web.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }

        [Required]
        public int OfficeId { get; set; }
        public Office Office { get; set; } = null!; // навигационно свойство

        [Required]
        [StringLength(3)]
        public string CurrencyCode { get; set; } = null!;
        public Currency Currency { get; set; } = null!; // навигационно свойство

        [Column(TypeName = "decimal(18,4)")]
        public decimal RateToBGN { get; set; } // централният курс (БНБ)

        [Column(TypeName = "decimal(18,4)")]
        public decimal BuyRate { get; set; } // курс за купува

        [Column(TypeName = "decimal(18,4)")]
        public decimal SellRate { get; set; } // курс за продава

        public DateTime AsOf { get; set; } // дата/час на валидност

        public string Source { get; set; } = "BNB";
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UserController.Models
{
    [Table("addresses")]
    [Index(nameof(CityId), Name = "IX_addresses_city_id")]
    [Index(nameof(CountryId), Name = "IX_addresses_country_id")]
    [Index(nameof(DistrictId), Name = "IX_addresses_district_id")]
    [Index(nameof(Title), Name = "IX_addresses_title", IsUnique = true)]
    [Index(nameof(UserId), Name = "IX_addresses_user_id")]
    public partial class Address
    {
        [Key]
        [Column("address_id")]
        public int AddressId { get; set; }
        [Column("user_id")]
        public string UserId { get; set; } = null!;
        [Column("country_id")]
        public int CountryId { get; set; }
        [Column("city_id")]
        public int CityId { get; set; }
        [Column("district_id")]
        public int DistrictId { get; set; }
        [Column("title")]
        public string Title { get; set; } = null!;
        [Column("address")]
        public string Address1 { get; set; } = null!;
        [Column("name")]
        public string Name { get; set; } = null!;
        [Column("surname")]
        public string Surname { get; set; } = null!;
        [Column("phone")]
        public string Phone { get; set; } = null!;
        [Column("is_default")]
        public bool IsDefault { get; set; }
        [Column("is_invoice_use")]
        public bool IsInvoiceUse { get; set; }
        [Column("invoice_type")]
        public int InvoiceType { get; set; }
        [Column("company_name")]
        public string? CompanyName { get; set; }
        [Column("tax_number")]
        public string? TaxNumber { get; set; }
        [Column("tax_office")]
        public string? TaxOffice { get; set; }
        [Column("is_e_invoice")]
        public bool? IsEInvoice { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey(nameof(CityId))]
        [InverseProperty("Addresses")]
        public virtual City City { get; set; } = null!;
        [ForeignKey(nameof(CountryId))]
        [InverseProperty("Addresses")]
        public virtual Country Country { get; set; } = null!;
        [ForeignKey(nameof(DistrictId))]
        [InverseProperty("Addresses")]
        public virtual District District { get; set; } = null!;
        [ForeignKey(nameof(UserId))]
        [InverseProperty("Addresses")]
        public virtual User User { get; set; } = null!;
    }
}

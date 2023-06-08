using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UserController.Models
{
    [Table("countries")]
    [Index(nameof(Code), Name = "IX_countries_code", IsUnique = true)]
    public partial class Country
    {
        public Country()
        {
            Addresses = new HashSet<Address>();
            Cities = new HashSet<City>();
        }

        [Key]
        [Column("country_id")]
        public int CountryId { get; set; }
        [Column("name")]
        public string Name { get; set; } = null!;
        [Column("code")]
        public string Code { get; set; } = null!;
        [Column("sort_order")]
        public int SortOrder { get; set; }
        [Column("status")]
        public int Status { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [InverseProperty(nameof(Address.Country))]
        public virtual ICollection<Address> Addresses { get; set; }
        [InverseProperty(nameof(City.Country))]
        public virtual ICollection<City> Cities { get; set; }
    }
}

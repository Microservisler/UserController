using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UserController.Models
{
    [Table("cities")]
    [Index(nameof(Code), Name = "IX_cities_code", IsUnique = true)]
    [Index(nameof(CountryId), Name = "IX_cities_country_id")]
    public partial class City
    {
        public City()
        {
            Addresses = new HashSet<Address>();
            Districts = new HashSet<District>();
        }

        [Key]
        [Column("city_id")]
        public int CityId { get; set; }
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

        [ForeignKey(nameof(CountryId))]
        [InverseProperty("Cities")]
        public virtual Country Country { get; set; } = null!;
        [InverseProperty(nameof(Address.City))]
        public virtual ICollection<Address> Addresses { get; set; }
        [InverseProperty(nameof(District.City))]
        public virtual ICollection<District> Districts { get; set; }
    }
}

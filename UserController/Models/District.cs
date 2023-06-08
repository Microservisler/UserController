using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UserController.Models
{
    [Table("districts")]
    [Index(nameof(CityId), Name = "IX_districts_city_id")]
    [Index(nameof(Code), Name = "IX_districts_code", IsUnique = true)]
    public partial class District
    {
        public District()
        {
            Addresses = new HashSet<Address>();
        }

        [Key]
        [Column("district_id")]
        public int DistrictId { get; set; }
        [Column("city_id")]
        public int CityId { get; set; }
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

        [ForeignKey(nameof(CityId))]
        [InverseProperty("Districts")]
        public virtual City City { get; set; } = null!;
        [InverseProperty(nameof(Address.District))]
        public virtual ICollection<Address> Addresses { get; set; }
    }
}

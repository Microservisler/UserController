using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UserController.Models
{
    [Index(nameof(NormalizedEmail), Name = "EmailIndex")]
    public partial class User
    {
        public User()
        {
            Addresses = new HashSet<Address>();
        }

        [Key]
        public string Id { get; set; } = null!;
        [StringLength(256)]
        public string? UserName { get; set; }
        [StringLength(256)]
        public string? NormalizedUserName { get; set; }
        [StringLength(256)]
        public string? Email { get; set; }
        [StringLength(256)]
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public string Code { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? TcKimlik { get; set; }
        public string Telefon { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public int Newsletter { get; set; }
        public int PrivateDiscountType { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        [InverseProperty(nameof(Address.User))]
        public virtual ICollection<Address> Addresses { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHive.Services.DTOService;

public class OwnerDTO
{
    public int Id { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string IdentityName { get; set; }

    public string IdentityNumber { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string Sex { get; set; }

    public string Nationality { get; set; }

    public string PlaceOfOrigin { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string GoogleMapUrl { get; set; }

    public string Status { get; set; }

    public string PlaceOfResidence { get; set; }

    public DateOnly? IdentityExpiredDate { get; set; }

    public DateOnly? IdentityCreatedDate { get; set; }

    public string IdentityFile { get; set; }

    public string LicenseName { get; set; }

    public string LicenseNumber { get; set; }

    public string LicenseAddress { get; set; }

    public decimal? CharterCapital { get; set; }

    public string LicenseFile { get; set; }

    public string Facebook { get; set; }

    public string Instagram { get; set; }

    public string Tiktok { get; set; }

    public string PhoneStatus { get; set; }
    public int? IsBan { get; set; }
}

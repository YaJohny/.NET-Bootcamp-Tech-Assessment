using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.CustomValidations;

namespace Models.Entities
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

		[ValidPublicationYear(ErrorMessage = "Publication year must be a 4-digit number between 1000 and the current year.")]
		public int PublicationYear { get; set; }

        [MaxLength(50)]
        public string? AuthorName { get; set; }

        public int ViewCount { get; set; } = 0;

        public bool IsDeleted { get; set; } = false; //soft delete

        [NotMapped]
        public int BookAgeInYears => DateTime.Now.Year - PublicationYear;

        [NotMapped]
        public double PopularityScore => (ViewCount * 0.5) + (BookAgeInYears * 2);
	}
}

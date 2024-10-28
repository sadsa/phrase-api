using System.ComponentModel.DataAnnotations;

namespace PhraseApi.Core.Entities
{
    public class Phrase
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Text field is required")]
        public string Text { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
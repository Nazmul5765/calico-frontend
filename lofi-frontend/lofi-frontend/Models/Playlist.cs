using lofi_frontend.Models;
using lofi_frontend.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace lofi_frontend.Models
{
    public class Playlist
    {
        [Required]
        public string Id { get; set; } = "";
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public Mood Mood { get; set; } = Mood.Chill;
        [Required]
        public Genre Genre { get; set; } = Genre.LoFi;
        public List<Music> Songs { get; set; } = new List<Music>();
    }
}

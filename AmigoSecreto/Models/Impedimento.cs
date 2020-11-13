using System.ComponentModel.DataAnnotations;

namespace AmigoSecreto.Models
{
    public class Impedimento
    {
        public int ID { get; set; }
        [Display(Name = "Nome Contém")]
        public string NomeLike { get; set; }
        [Display(Name = "Amigo")]
        public int AmigoID { get; set; }
        
        public virtual Amigo Amigo { get; set; }

    }
}


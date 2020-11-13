using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AmigoSecreto.Models
{
    public class Amigo
    {
        public int ID { get; set; }
        [Display(Name = "Nome do Amigo")]
        public string Nome { get; set; }
        [Display(Name = "Telemóvel")]
        public string NTelemovel { get; set; }
        [Display(Name = "Data Último SMS")]
        public DateTime UltimoEnvioSMS { get; set; }
        [Display(Name = "Último Resultado SMS")]
        public string ResultadoUltimoEnvioSMS { get; set; }

        public string obs { get; set; }

        public virtual ICollection<Impedimento> Impedimentos { get; set; }

        [Display(Name = "Impedimentos")]
        public string StringImpedimentos
        {
            get 
            {
                if (Impedimentos == null) return "";
                if (Impedimentos.Count <= 0) return "";

                string stringImpedimentos = "";

                foreach (var impedimento in Impedimentos)
                {
                    stringImpedimentos = stringImpedimentos + "( " + impedimento.NomeLike + " )";
                }

                return stringImpedimentos;
            }        
        }

    }
}


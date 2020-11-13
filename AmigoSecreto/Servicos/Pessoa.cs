using System.Collections.Generic;

using AmigoSecreto.Models;

namespace AmigoSecreto.Servicos
{
    public class Pessoa
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Telemovel { get; set; }
        public int NAmigosPossiveis { get; set; }

        public IList<string> Impedimentos { get; set; }

        public AmigoPossivel AmigoSorteado;

        public IList<AmigoPossivel> AmigosPossiveis;
    }
}
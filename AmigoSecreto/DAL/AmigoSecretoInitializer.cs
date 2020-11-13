using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using AmigoSecreto.Models;

namespace AmigoSecreto.DAL
{
    public class AmigoSecretoInitializer : DropCreateDatabaseIfModelChanges<AmigoSecretoContext>
    {
        protected override void Seed(AmigoSecretoContext context)
        {
            var amigos = new List<Amigo>
            {
                new Amigo{ Nome = "Amigo1", NTelemovel = "111111111", UltimoEnvioSMS = DateTime.Parse("1900-01-01"), ResultadoUltimoEnvioSMS = "" },
                new Amigo{ Nome = "Amigo2", NTelemovel = "222222222", UltimoEnvioSMS = DateTime.Parse("1900-01-01"), ResultadoUltimoEnvioSMS = "" },
                new Amigo{ Nome = "Amigo3", NTelemovel = "333333333", UltimoEnvioSMS = DateTime.Parse("1900-01-01"), ResultadoUltimoEnvioSMS = "" },
            };

            amigos.ForEach(s => context.Amigos.Add(s));

            var amigo = amigos.SingleOrDefault(a => a.Nome.Contains("Amigo1"));
            if (amigo != null)
            {
                var impedimentos = new List<Impedimento>
                {
                    new Impedimento{ Amigo = amigo, NomeLike = "Amigo2" }
                };

                impedimentos.ForEach(i => context.Impedimentoes.Add(i));
            }

            context.SaveChanges();
        }
    }
}
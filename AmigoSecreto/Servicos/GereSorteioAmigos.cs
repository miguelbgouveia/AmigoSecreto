using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

using AmigoSecreto.DAL;
using AmigoSecreto.Models;

namespace AmigoSecreto.Servicos
{
    public static class GereSorteioAmigos
    {
        public static string SortearAmigosSecretos(AmigoSecretoContext db, bool IsTest)
        {
            var pessoas = GetPessoasForSorteio(db);

            AddAmigosPossiveisToPessoas(pessoas, db);

            SorteiaAmigoSecreto(pessoas);


            if (ExistPessoaSemAmigoSecreto(pessoas))
                return "Não foi possível sortear amigo secreto para todas as pessoas!";
            else
            {
                if (IsTest)
                {
                    AtualizaMensagemAmigoSorteado(pessoas, db);
                }
                else
                {
                    EnviaMensagensDeResultadoDoSorteio(pessoas, db);
                }
            }
            
            return "OK";
        }

        private static IList<Pessoa> GetPessoasForSorteio(AmigoSecretoContext db)
        {
            var pessoas = new List<Pessoa>();

            if (db == null) return pessoas;

            var amigos = db.Amigos.ToList();

            if (amigos == null) return pessoas;

            foreach (var amigo in amigos)
            {
                pessoas.Add(GetPessoaFromAmigo(amigo));
            }

            return pessoas;
        }

        private static Pessoa GetPessoaFromAmigo(Amigo amigo)
        {
            var pessoa = new Pessoa();

            if (amigo == null) return pessoa;

            pessoa.Id = amigo.ID;
            pessoa.Nome = amigo.Nome;
            pessoa.Telemovel = amigo.NTelemovel;
            pessoa.AmigoSorteado = null;
            pessoa.Impedimentos = GetImpedimentosForAmigo(amigo);

            return pessoa;
        }

        private static IList<string> GetImpedimentosForAmigo(Amigo amigo)
        {
            var impedimentos = new List<string>();

            if (amigo == null) return impedimentos;

            foreach (var impedimento in amigo.Impedimentos)
            {
                if (impedimento.NomeLike != null && impedimento.NomeLike.Length > 0)
                    impedimentos.Add(impedimento.NomeLike);
            }

            return impedimentos;
        }

        private static void AddAmigosPossiveisToPessoas(IList<Pessoa> pessoas, AmigoSecretoContext db)
        {
            if (pessoas == null) return;

            foreach (var pessoa in pessoas)
            {
                AddAmigosPossiveisToPessoa(pessoa, db);
            }
        }

        private static void AddAmigosPossiveisToPessoa(Pessoa pessoa, AmigoSecretoContext db)
        {
            if (pessoa == null) return;

            pessoa.AmigosPossiveis = new List<AmigoPossivel>();
            pessoa.NAmigosPossiveis = 0;

            var amigos = db.Amigos.Where(a => a.ID != pessoa.Id).ToList();

            if (amigos == null) return;

            foreach (var amigo in amigos)
            {
                if (!ExistsImpedimentoParaAmigo(pessoa.Impedimentos, amigo))
                {
                    pessoa.AmigosPossiveis.Add(new AmigoPossivel { Id = amigo.ID, Nome = amigo.Nome });
                    pessoa.NAmigosPossiveis ++; 
                }
            }
        }

        private static bool ExistsImpedimentoParaAmigo(IList<string> impedimentos, Amigo amigo)
        {
            if (amigo == null) return false;

            if (impedimentos == null) return false;

            foreach (var impedimento in impedimentos)
            {
                if (amigo.Nome.Contains(impedimento)) return true;
            }

            return false;
        }

        private static void SorteiaAmigoSecreto(IList<Pessoa> pessoas)
        {
            if (pessoas == null) return;

            int randomIndex;
            var random = new Random();
            foreach (var pessoa in pessoas.OrderBy(p => p.NAmigosPossiveis))
            {
                if (pessoa.AmigosPossiveis.Count > 0)
                {
                    randomIndex = random.Next(0, pessoa.AmigosPossiveis.Count);
                    pessoa.AmigoSorteado = pessoa.AmigosPossiveis[randomIndex];
                    RemoveAmigoDoSorteio(pessoas, pessoa.AmigoSorteado);
                }
            }
        }

        private static void RemoveAmigoDoSorteio(IList<Pessoa> pessoas, AmigoPossivel amigoPossivel)
        {
            if (pessoas == null) return;

            foreach (var pessoa  in pessoas)
            {

                var amigoPossivelToRemove = pessoa.AmigosPossiveis.SingleOrDefault( ap => ap.Id == amigoPossivel.Id);

                if (amigoPossivelToRemove != null)
                    pessoa.AmigosPossiveis.Remove(amigoPossivelToRemove);
            }
        }

        private static bool ExistPessoaSemAmigoSecreto(IList<Pessoa> pessoas)
        {
            if (pessoas == null) return false;

            foreach (var pessoa in pessoas)
            {
                if (pessoa.AmigoSorteado == null) return true;
            }

            return false;
        }

        private static void AtualizaMensagemAmigoSorteado(IList<Pessoa> pessoas, AmigoSecretoContext db)
        {
            if (pessoas == null || db == null) return;

            foreach (var pessoa in pessoas)
            {
                AtualizaMensagemAmigoSorteadoInPessoa(pessoa, db);
            }

            db.SaveChanges();
        }

        private static void AtualizaMensagemAmigoSorteadoInPessoa(Pessoa pessoa, AmigoSecretoContext db)
        {
            if (pessoa == null || db == null) return;

            var amigo = db.Amigos.SingleOrDefault(a => a.ID == pessoa.Id);
            amigo.ResultadoUltimoEnvioSMS = "Amigo secreto sorteado em testes: " + (pessoa.AmigoSorteado == null? "" : pessoa.AmigoSorteado.Nome);
            db.Entry(amigo).State = EntityState.Modified;
        }

        public static void EnviaMensagensDeResultadoDoSorteio(IList<Pessoa> pessoas, AmigoSecretoContext db)
        {
            if (pessoas == null) return;

            string smsResult;
            foreach (var pessoa in pessoas)
            {
                if (pessoa.AmigoSorteado != null &&
                    pessoa.AmigoSorteado.Nome != null &&
                    pessoa.AmigoSorteado.Nome.Length > 0)
                {
                    smsResult = SMSService.SendSMS(pessoa.Telemovel, GetMensagemAmigoSecreto(pessoa));

                    AtualizaSMSResultInPessoa(smsResult, pessoa.Id, db);
                }
            }

            db.SaveChanges();
        }

        public static string GetMensagemAmigoSecreto(Pessoa pessoa)
        {
            string nomePessoa = pessoa == null? 
                                "": 
                                pessoa.Nome == null? 
                                    "": 
                                    pessoa.Nome;  

            string nomeAmigo = pessoa == null ?
                                "":
                                pessoa.AmigoSorteado == null ?
                                    "":
                                    pessoa.AmigoSorteado.Nome == null?
                                        "":
                                        pessoa.AmigoSorteado.Nome;

            return "Ola " + nomePessoa + ", o seu amigo secreto e o(a) " + nomeAmigo + ". Boas compras e Feliz Natal!";
        }

        private static void AtualizaSMSResultInPessoa(string smsResult, int amigoId, AmigoSecretoContext db)
        {
            if (amigoId <= 0) return;

            var amigo = db.Amigos.SingleOrDefault(a => a.ID == amigoId);

            if (amigo == null) return;

            amigo.UltimoEnvioSMS = DateTime.Now;
            amigo.ResultadoUltimoEnvioSMS = smsResult;
            db.Entry(amigo).State = EntityState.Modified;
        }
    }
}
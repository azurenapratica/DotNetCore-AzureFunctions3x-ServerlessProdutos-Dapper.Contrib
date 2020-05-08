using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Dapper;
using Dapper.Contrib.Extensions;
using ServerlessProdutos.Models;

namespace ServerlessProdutos.Data
{
    public static class ProdutoRepository
    {
        public static Produto Get(string codigoBarras)
        {
            codigoBarras = codigoBarras?.Trim().ToUpper();
            if (!String.IsNullOrWhiteSpace(codigoBarras))
            {
                using (var conexao = new SqlConnection(
                    Environment.GetEnvironmentVariable("BaseProdutos")))
                {
                    return conexao.Get<Produto>(codigoBarras);
                }
            }
            else
                return null;
        }

        public static List<Produto> GetAll()
        {
            using (var conexao = new SqlConnection(
                Environment.GetEnvironmentVariable("BaseProdutos")))
            {
                return conexao.GetAll<Produto>().ToList();
            }
        }

        public static void Insert(Produto produto)
        {
            using (var conexao = new SqlConnection(
                Environment.GetEnvironmentVariable("BaseProdutos")))
            {
                conexao.Insert(produto);
            }
        }

        public static bool Update(Produto produto)
        {
            using (var conexao = new SqlConnection(
                Environment.GetEnvironmentVariable("BaseProdutos")))
            {
                return conexao.Update(produto);
            }
        }

        public static void UpdatePrecos(ReajustePreco reajuste)
        {
            using (var conexao = new SqlConnection(
                Environment.GetEnvironmentVariable("BaseProdutos")))
            {
                conexao.Execute("UPDATE dbo.Produtos SET " +
                               $"Preco = Preco * @IndiceReajuste, " +
                               $"ObservacaoReajustePreco = @ObservacaoReajustePreco, " +
                                "UltimaAtualizacao = @UltimaAtualizacao",
                                new
                                {
                                    IndiceReajuste = reajuste.IndiceReajuste.Value,
                                    ObservacaoReajustePreco = reajuste.ObservacaoReajustePreco,
                                    UltimaAtualizacao = DateTime.Now
                                });
            }
        }

        public static bool Delete(Produto produto)
        {
            using (var conexao = new SqlConnection(
                Environment.GetEnvironmentVariable("BaseProdutos")))
            {
                return conexao.Delete(produto);
            }
        }
    }
}
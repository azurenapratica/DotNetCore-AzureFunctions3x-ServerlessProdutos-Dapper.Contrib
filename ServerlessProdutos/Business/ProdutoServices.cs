using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ServerlessProdutos.Models;
using ServerlessProdutos.Data;

namespace ServerlessProdutos.Business
{
    public static class ProdutoServices
    {
        public static IActionResult Get(string codigoBarras)
        {
            List<Produto> listaProdutos;
            if (String.IsNullOrWhiteSpace(codigoBarras))
                listaProdutos = ProdutoRepository.GetAll();
            else
            {
                listaProdutos = null;
                var produto = ProdutoRepository.Get(codigoBarras);
                if (produto != null)
                {
                    listaProdutos = new List<Produto>();
                    listaProdutos.Add(produto);
                }
            }

            if (listaProdutos?.Count() > 0)
                return new OkObjectResult(listaProdutos);
            else
                return new NotFoundResult();
        }

        public static IActionResult Insert(string strDadosCadastroProduto)
        {
            var dadosProduto = DeserializeDadosProduto(strDadosCadastroProduto);
            var resultado = DadosValidos(dadosProduto);
            resultado.Acao = "Inclusão de Produto";

            if (resultado.Inconsistencias.Count == 0 &&
                ProdutoRepository.Get(dadosProduto.CodigoBarras) != null)
            {
                resultado.Inconsistencias.Add(
                    "Código de Barras já cadastrado");
            }

            if (resultado.Inconsistencias.Count == 0)
            {
                var produto = new Produto();
                produto.CodigoBarras = dadosProduto.CodigoBarras;
                produto.Nome = dadosProduto.Nome;
                produto.Preco = dadosProduto.Preco.Value;
                produto.UltimaAtualizacao = DateTime.Now;

                ProdutoRepository.Insert(produto);
                return new OkObjectResult(resultado);
            }
            else
                return new BadRequestObjectResult(resultado);
        }

        public static IActionResult Update(string strDadosCadastroProduto)
        {
            var dadosProduto = DeserializeDadosProduto(strDadosCadastroProduto);
            var resultado = DadosValidos(dadosProduto);
            resultado.Acao = "Atualização de Produto";

            if (resultado.Inconsistencias.Count == 0)
            {
                var produto = ProdutoRepository.Get(dadosProduto.CodigoBarras);

                if (produto == null)
                {
                    resultado.Inconsistencias.Add(
                        "Produto não encontrado");
                    return new NotFoundObjectResult(resultado);
                }
                else
                {
                    produto.Nome = dadosProduto.Nome;
                    produto.Preco = dadosProduto.Preco.Value;
                    produto.UltimaAtualizacao = DateTime.Now;

                    if (ProdutoRepository.Update(produto))
                        return new OkObjectResult(resultado);
                    else
                    {
                        resultado.Inconsistencias.Add(
                            "Não foi possível alterar o Produto");
                        return new BadRequestObjectResult(resultado);
                    }
                }
            }

            return new BadRequestObjectResult(resultado);
        }

        public static bool UpdateReajustePreco(string dadosReajuste)
        {
            var reajuste = JsonSerializer.Deserialize<ReajustePreco>(
                dadosReajuste,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            
            if (reajuste.IndiceReajuste.HasValue &&
                reajuste.IndiceReajuste > 0 &&
               !String.IsNullOrWhiteSpace(reajuste.ObservacaoReajustePreco))
            {
                ProdutoRepository.UpdatePrecos(reajuste);
                return true;
            }
            else
                return false;
        }

        public static IActionResult Delete(string codigoBarras)
        {
            var resultado = new Resultado();
            resultado.Acao = "Exclusão de Produto";

            if (String.IsNullOrWhiteSpace(codigoBarras))
            {
                resultado.Inconsistencias.Add(
                    "Código de Barras não informado");
                return new BadRequestObjectResult(resultado);
            }

            Produto produto = ProdutoRepository.Get(
                codigoBarras.Trim());
            if (produto != null)
            {
                if (ProdutoRepository.Delete(produto))
                    return new OkObjectResult(resultado);
                else
                {
                    resultado.Inconsistencias.Add(
                        "Não foi possível excluir o Produto");
                    return new BadRequestObjectResult(resultado);
                }
            }
            else
            {
                resultado.Inconsistencias.Add(
                    "Produto não encontrado");
                return new NotFoundObjectResult(resultado);
            }
        }

        private static CadastroProduto DeserializeDadosProduto(string dados)
        {
            return JsonSerializer.Deserialize<CadastroProduto>(dados,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        private static Resultado DadosValidos(CadastroProduto produto)
        {
            var resultado = new Resultado();
            if (produto == null)
            {
                resultado.Inconsistencias.Add(
                    "Preencha os Dados do Produto");
            }
            else
            {
                if (String.IsNullOrWhiteSpace(produto.CodigoBarras))
                {
                    resultado.Inconsistencias.Add(
                        "Preencha o Código de Barras");
                }
                if (String.IsNullOrWhiteSpace(produto.Nome))
                {
                    resultado.Inconsistencias.Add(
                        "Preencha o Nome do Produto");
                }
                if (!produto.Preco.HasValue || produto.Preco <= 0)
                {
                    resultado.Inconsistencias.Add(
                        "O Preço do Produto deve ser maior do que zero");
                }
            }

            return resultado;
        }
    }
}
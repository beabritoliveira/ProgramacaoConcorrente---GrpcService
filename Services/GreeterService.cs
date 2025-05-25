using CriptoMoed.Model;
using Grpc.Core;
using GrpcService1;
using GrpcService1;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Text.Json;

namespace GrpcService1.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly HttpClient _httpClient;

        public GreeterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public override async Task<PostMoedaResponse> PostMoedas(PostMoedaRequest request, ServerCallContext context)
        {
            Console.WriteLine($"ID: {request.Id}, Nome: {request.Name}, Valor: {request.Valor}");

            var id = request.Id;
            var val = 0.0m;
            if (decimal.TryParse(request.Valor, out decimal valorDecimal))
            {
                // Uso do valor convertido
                val = valorDecimal;
            }
            var moeda = new
            {
                Id = id,
                Nome = request.Name,
                Valor = val
            };
            Console.WriteLine($"ID: {moeda.Id}, Nome: {moeda.Nome}, Valor: {moeda.Valor}");

            // Chamada ao Web Service
            var response = await _httpClient.PostAsJsonAsync($"http://localhost:5202/api/MoedasCB", moeda);
            response.EnsureSuccessStatusCode();

            // var data = await response.Content.ReadFromJsonAsync<dynamic>();
            return new PostMoedaResponse
            {
                Message = "Inseriu?",
            };
        }

        public override async Task<GetMoedasResponse> GetMoedas(Empty empty, ServerCallContext context)
        {
            var response = await _httpClient.GetAsync("http://localhost:5202/api/MoedasCB");
            string responseBody = await response.Content.ReadAsStringAsync();

            //Transforma o json recebido em uma lista de moedas
            var listaDeMoedas = JsonSerializer.Deserialize<List<Moedas>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //Pra converter o valor que é decimal na classe Carteira mas que é string no Proto
            var moedasLeitura = listaDeMoedas.Select(m => new PostMoedaRequest
            {
                Id = m.Id,
                Name = m.Nome,
                Valor = m.Valor.ToString()
            }).ToList();

            // Crie a resposta gRPC
            var moedasResponse = new GetMoedasResponse();
            // Coloca o response do Controller no formato que o Client consegue pegar => lista de GetMoedasResponse
            moedasResponse.Moedas.AddRange(moedasLeitura);

            return moedasResponse;
        }

        public override async Task<PostCarteiraResponse> PostCarteira(PostCarteiraRequest request, ServerCallContext context)
        {
            var val = 0.0m;
            var statusConta = "Neutro";
            if (decimal.TryParse(request.QtdMoedas, out decimal valorDecimal))
            {
                // Uso do valor convertido
                val = valorDecimal;
                if (val > 0)
                {
                    statusConta = "Positivo";
                }
            }
            var carteira = new
            {
                NumeroConta = request.NumeroConta,
                CodResponsavel = request.CodResponsavel,
                IdMoeda = request.IdMoeda,
                NomeResponsavel = request.NomeResponsavel,
                QtdMoedas = val,
                statusConta = statusConta
        };
            Console.WriteLine(carteira);

            var response = await _httpClient.PostAsJsonAsync($"http://localhost:5202/api/Carteiras", carteira);
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            return new PostCarteiraResponse
            {
                Message = "Inseriu a carteira " + carteira.NumeroConta
            };
        }

        public override async Task<DeleteCarteiraResponse> DeleteCarteira(DeleteCarteiraRequest request, ServerCallContext context)
        {
            var id_carteira = request.IdCarteira;
            Console.WriteLine(id_carteira);
            var url = $"http://localhost:5202/api/Carteiras/{id_carteira}";
            var response = await _httpClient.DeleteAsync(url);

            return new DeleteCarteiraResponse { Message = "Deletou " + id_carteira};
        }

        public override async Task<ComprarResponse> ComprarMoeda(ComprarRequest request, ServerCallContext context)
        {
            decimal.TryParse(request.Valor, out decimal valorDecimal);

            if (valorDecimal <= 0)
            {
                return new ComprarResponse
                {
                    Mensagem = "Valor fora da margem de compra"
                };
            }
            var teste = request.IdConta + "?valor=" + valorDecimal;

            var url = $"http://localhost:5202/api/Carteiras/{teste}";
            Console.WriteLine(valorDecimal);
            Console.WriteLine(teste);

            var response = await _httpClient.PutAsJsonAsync(url, new { valorDecimal = request.Valor });
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            return new ComprarResponse
            {
                Mensagem = responseBody
            };
        }

        public override async Task<TransfResponse> TransferirMoeda(TransfRequest request, ServerCallContext context)
        {
            decimal.TryParse(request.Valor, out decimal valorDecimal);
            if (valorDecimal <= 0)
            {
                return new TransfResponse
                {
                    Mensagem = "Valor fora da margem de compra"
                };
            }

            // string id_origem, string id_destino, decimal valor
            var cabecalho = "?id_origem=" + request.IdOrigem + "&id_destino=" + request.IdDestino + "&valor="+ request.Valor;
            var url = $"http://localhost:5202/api/Carteiras/transferencia/{cabecalho}";
            Console.WriteLine(valorDecimal);
            var transf = new
            {
                IdOrigem = request.IdOrigem,
                IdDestino = request.IdDestino,
                Valor = valorDecimal
            };
            var response = await _httpClient.PutAsJsonAsync(url, transf);


            return new TransfResponse { Mensagem = "oi" };
        }

        public override async Task<ValMoedaRepost> AtualizarValMoeda(ValMoedaRequest request, ServerCallContext context)
        {
            decimal.TryParse(request.Valor, out decimal valorDecimal);
            if (valorDecimal <= 0)
            {
                return new ValMoedaRepost
                {
                    Messagem = "Valor fora da margem de compra"
                };
            }

            // string id_origem, string id_destino, decimal valor
            var cabecalho = request.IdMoeda + "?valor=" + request.Valor;
            var url = $"http://localhost:5202/api/MoedasCB/{cabecalho}";
            Console.WriteLine(valorDecimal);
            var novoValor = new
            {
                Id = request.IdMoeda,
                Valor = valorDecimal
            };

            var response = await _httpClient.PutAsJsonAsync(url, novoValor);

            return new ValMoedaRepost { Messagem = "Valor da moeda atualizado"};
        }

        public override async Task<VisuSaldoResponse> SaldoCarteira(VisuSaldoRequest request, ServerCallContext context)
        {
            var url = "http://localhost:5202/api/Carteiras/" + request.Id;
            var response = await _httpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
            return new VisuSaldoResponse
            {
                Messagem = responseBody
            };
        }
        public override async Task<ResponseSimuacao> SimularCompraTransferencia(RequestSimulacao request, ServerCallContext context)
        {
            decimal.TryParse(request.Valor, out decimal valorDecimal);
            var id_dt = request.IdDestino;
            if (request.IdDestino == null | request.IdDestino == "")
            {
                id_dt = "nulo";
            }
            var cabecalho = "?id_origem=" +request.IdOrigem + "&id_destino=" + id_dt + "&valor=" +valorDecimal;
            var url = "http://localhost:5202/api/Carteiras/simulacao" + cabecalho;
            var response = await _httpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
            return new ResponseSimuacao
            {
                Mensagem = responseBody
            };
        }

    }
}

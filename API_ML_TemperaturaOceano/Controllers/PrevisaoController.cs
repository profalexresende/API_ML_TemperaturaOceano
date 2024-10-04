using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;

namespace API_ML_TemperaturaOceano.Controllers
{
    public class DadosOceano
    {
        [LoadColumn(0)] public float Latitude { get; set; }
        [LoadColumn(1)] public float Longitude { get; set; }
        [LoadColumn(2)] public float Profundidade { get; set; }
        [LoadColumn(3)] public float Salinidade { get; set; }
        [LoadColumn(4)] public string EpocaDoAno { get; set; }
        [LoadColumn(5)] public float Temperatura { get; set; }
    }

    public class PrevisaoTemperatura
    {
        [ColumnName("Score")]
        public float TemperaturaPrevista { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PrevisaoController : ControllerBase
    {
        private readonly string caminhoModelo = Path.Combine(Environment.CurrentDirectory, "wwwroot", "MLModels", "ModeloTemperatura.zip");
        private readonly string caminhoTreinamento = Path.Combine(Environment.CurrentDirectory, "Data", "oceano-train.csv");
        private readonly MLContext mlContext;

        public PrevisaoController()
        {
            mlContext = new MLContext();

            if (!System.IO.File.Exists(caminhoModelo))
            {
                Console.WriteLine("Modelo não encontrado. Iniciando treinamento...");
                TreinarModelo();
            }
        }

        // Método para treinar o modelo e salvar como arquivo .zip
        private void TreinarModelo()
        {
            // Verificar se o diretório existe e criar se necessário
            var pastaModelo = Path.GetDirectoryName(caminhoModelo);
            if (!Directory.Exists(pastaModelo))
            {
                Directory.CreateDirectory(pastaModelo);
                Console.WriteLine($"Diretório criado: {pastaModelo}");
            }

            // Carregar dados de treinamento
            IDataView dadosTreinamento = mlContext.Data.LoadFromTextFile<DadosOceano>(
                path: caminhoTreinamento, hasHeader: true, separatorChar: ',');

            // Definir a pipeline de transformações e treinamento
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(DadosOceano.Temperatura))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EpocaDoAnoEncoded", inputColumnName: nameof(DadosOceano.EpocaDoAno)))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(DadosOceano.Latitude), nameof(DadosOceano.Longitude), nameof(DadosOceano.Profundidade),
                                                          nameof(DadosOceano.Salinidade), "EpocaDoAnoEncoded"))
                .Append(mlContext.Regression.Trainers.FastTree());

            // Treinamento do modelo
            var modelo = pipeline.Fit(dadosTreinamento);

            // Salvar o modelo treinado em um arquivo .zip
            mlContext.Model.Save(modelo, dadosTreinamento.Schema, caminhoModelo);
            Console.WriteLine($"Modelo treinado e salvo em: {caminhoModelo}");
        }

        // Endpoint para fazer previsões com o modelo treinado
        [HttpPost("prever")]
        public ActionResult<PrevisaoTemperatura> PreverTemperatura([FromBody] DadosOceano dados)
        {
            if (!System.IO.File.Exists(caminhoModelo))
            {
                return BadRequest("O modelo ainda não foi treinado.");
            }

            // Carregar o modelo salvo
            ITransformer modelo;
            using (var stream = new FileStream(caminhoModelo, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                modelo = mlContext.Model.Load(stream, out var modeloSchema);
            }

            // Criar o engine de previsão
            var enginePrevisao = mlContext.Model.CreatePredictionEngine<DadosOceano, PrevisaoTemperatura>(modelo);

            // Realizar a previsão
            var previsao = enginePrevisao.Predict(dados);

            return Ok(previsao);
        }
    }
}

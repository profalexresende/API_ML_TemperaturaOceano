Projeto de Previsão de Temperatura do Oceano com ML.NET

Este projeto é um exemplo de aplicação de Machine Learning utilizando o ML.NET para prever a temperatura média da superfície do oceano com base em parâmetros geográficos e sazonais. A API foi desenvolvida em C# no framework ASP.NET Core e inclui um pipeline completo que vai desde o treinamento do modelo até a geração de previsões.

Funcionalidades do Projeto
Treinamento de Modelo: Treina um modelo de regressão utilizando o algoritmo FastTree para prever a temperatura do oceano a partir de um conjunto de dados de treinamento.
Transformações de Dados: Realiza transformações nas variáveis categóricas (como época do ano) para que o modelo consiga interpretá-las corretamente.
Previsão via API: Permite que o usuário envie parâmetros como latitude, longitude, profundidade, salinidade e época do ano e obtenha a temperatura prevista em tempo real.
Persistência do Modelo: Após o treinamento, o modelo é salvo localmente em um arquivo .zip, permitindo que ele seja reutilizado sem precisar treinar novamente.

Estrutura dos Dados
O modelo foi treinado usando um arquivo CSV com os seguintes atributos:

Latitude: Coordenada geográfica que representa a latitude do ponto de medição.
Longitude: Coordenada geográfica que representa a longitude do ponto de medição.
Profundidade: Profundidade em que a temperatura foi coletada (em metros).
Salinidade: Nível de salinidade da água no ponto de medição.
Época do Ano: Estação em que a medição foi realizada (Verão, Inverno, Outono, Primavera).
Temperatura: Temperatura real medida no ponto (variável alvo que queremos prever).

Como Funciona a API
Treinamento do Modelo: Ao iniciar a aplicação, a API verifica se o modelo já está treinado e salvo. Caso contrário, inicia automaticamente o processo de treinamento utilizando os dados de um arquivo CSV (oceano-train.csv).

Carregamento de Dados: O modelo é treinado com dados históricos de temperatura da superfície do oceano.

Pipeline de Treinamento: A pipeline inclui:
Conversão de variáveis categóricas (One-Hot Encoding para a estação do ano);
Criação de um vetor de recursos (Features) com todas as variáveis independentes;
Treinamento do modelo utilizando o algoritmo de regressão FastTree.
Endpoint de Previsão: Após o treinamento, a API disponibiliza um endpoint /api/previsao/prever que permite ao usuário enviar novos dados para prever a temperatura do oceano.
Exemplos de Uso da API
Requisição POST para Previsão

Você pode realizar uma previsão enviando uma requisição POST para o endpoint /api/previsao/prever com o seguinte corpo:

{
  "Latitude": 10.0,
  "Longitude": -30.0,
  "Profundidade": 15,
  "Salinidade": 35,
  "EpocaDoAno": "Verão",
  "Temperatura": 0
}

# **Fintech Message Consumer**

## **Introdução**

Este microsserviço é responsável pelo consumo de mensagens enviadas para filas no RabbitMQ. O MS foi desenvolvido em C# .Net 8.

## **Filas**

**ClientProfileQueue**

- **Nome na Configuração da Aplicação:** `ClientProfileQueue`
- **Nome da fila no RabbitMQ:** `perguntas-perfil`
- **Durable**: `false`
- **Descrição:** Esta fila é utilizada para processar mensagens relacionadas as respostas das perguntas referentes a definição do perfil dos investidores (Conservador, Moderado, Agressivo).

Exemplo de evento a ser consumido:

```json
{
  "clientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "questions": [
    {
      "questionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "questionValue": 3
    },
    {
      "questionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "questionValue": 3
    },
    {
      "questionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "questionValue": 5
    },
    {
      "questionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "questionValue": 10
    },
    {
      "questionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "questionValue": 10
    }
  ]
}
```

**BuyProductQueue**

- **Nome na Configuração da Aplicação:** `BuyProductQueue`
- **Nome da fila no RabbitMQ:** `compra-produto`
- **Durable**: `true`
- **Descrição:** Esta fila é utilizada para processar mensagens relacionadas a compras de produtos de investimentos pelos usuários da aplicação FintechGrupo10.

Exemplo de evento a ser consumido:

```json
{
  "idProduto": "efc58f96-805b-4bb6-af0a-70d716211473",
  "idCliente": "170ae64c-8817-425a-ad79-518ded55052e",
  "quantidade": 10,
  "preco": 100
}
```

**SellProductQueue**

- **Nome na Configuração da Aplicação:** `SellProductQueue`
- **Nome da fila no RabbitMQ:** `venda-produto`
- **Durable**: `true`
- **Descrição:** Esta fila é utilizada para processar mensagens relacionadas a vendas de produtos de investimentos pelos usuários da aplicação FintechGrupo10.

Exemplo de evento a ser consumido:

```json
{
  "idProduto": "efc58f96-805b-4bb6-af0a-70d716211473",
  "idCliente": "170ae64c-8817-425a-ad79-518ded55052e",
  "quantidade": 10,
  "preco": 100
}
```

## **Como Executar**

1. Certifique-se de que o RabbitMQ está em execução e acessível.

O RabbitMQ está sendo utilizado através de uma plataforma online, a CloudAMQP. Obtenha as credenciais para realizar o login na plataforma, verifique se está em execução e com as filas criadas corretamente.

2. Configure as variáveis de ambiente ou o arquivo de configuração do aplicativo com as informações necessárias para executar a aplicação.
Configure o arquivo `appsettings.json` com as informações necessárias para cada campo.

3. Execute o projeto.

## **Dependências**

- .NET 8
- MongoDb
- RabbitMQ

# Micro-ondas Digital

Projeto ASP.NET Core MVC para o desafio de simulação de um micro-ondas digital.

## Tecnologias

- C#
- .NET 10
- ASP.NET Core MVC
- NUnit

## O que foi implementado

- Aquecimento manual com tempo de 1 a 120 segundos.
- Potência de 1 a 10, usando 10 quando o campo não é informado.
- Início rápido de 30 segundos.
- Pausar/cancelar limpando a operação atual.
- Programas pré-definidos com tempo, potência, alimento, instruções e caractere próprio.
- Cadastro de programas personalizados, validando nome e caractere repetidos.
- Um rascunho do nível 4: os programas personalizados ficam salvos em arquivo JSON.
- Testes unitários para as regras principais do domínio.

## Como executar

```bash
dotnet restore
dotnet run --project Benner.Microondas.Web
```

Depois abra o endereço informado no terminal.

## Como rodar os testes

```bash
dotnet test
```

## Observações

A persistência dos programas personalizados foi deixada em arquivo JSON para manter a solução simples. Na aplicação o arquivo é gravado em `App_Data`; nos testes é usado um repositório em memória.

This is a challenge by [Coodesh](https://coodesh.com/)

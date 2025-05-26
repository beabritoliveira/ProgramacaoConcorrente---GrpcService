# Trabalho de Programação Distribuída - Parte II

## Explicação

### Atividade 02: Desenvolvimento de um sistema para gerenciamento de carteiras de criptomoedas utilizando Web Services e interoperabilidade com RPC

As criptomoedas, como **Bitcoin** e **Ethereum**, já ganharam popularidade e valor. Com isso, surge a necessidade de desenvolver ferramentas que facilitem o gerenciamento dessas criptomoedas, como:

- Controle de compras e vendas
- Transferências
- Consultas de saldo

### Objetivo
O objetivo desta atividade é desenvolver um **sistema de gerenciamento de carteiras de criptomoedas**, implementando:

1. **Operações básicas de CRUD (Create, Read, Update e Delete)**
2. **Integração com serviço consumido via chamada remota de procedimento (RPC)**
3. **Proposição de novas funcionalidades para agregar valor ao sistema**

---

## Implementação do Web Services
As operações básicas do sistema devem ser implementadas utilizando **HTTP** e **JSON** para troca de dados. As operações incluem:

- **Criação** de uma nova carteira de criptomoedas
- **Consulta** do saldo da carteira
- **Adição** ou **remoção** de criptomoedas
- **Transferência** de criptomoedas entre carteiras
- **Exclusão** de uma carteira

### Disponibilização de serviço via Chamada Remota de Procedimento
Deve-se implementar serviços que serão **consumidos internamente** para atender às funcionalidades do usuário. Esses serviços não estarão expostos diretamente.

### Proposta de novas funcionalidades
Algumas sugestões de funcionalidades adicionais incluem:

- **Histórico de transações:** permitir que usuários consultem todas as transações realizadas, incluindo datas e valores.
- **Integração com corretoras ("de mentirinha" 😊):** permitir compras e vendas de criptomoedas diretamente no sistema, utilizando APIs de corretoras.

---

## Estrutura do Projeto
A arquitetura do sistema é dividida em **3 partes**:

1. **Controller** - Gerenciamento das requisições HTTP
2. **Client** - Interface para o usuário final
3. **GrpcService** - Serviço de comunicação remota
![{E39E3767-DBCF-487E-A175-7749C9CCC6DA}](https://github.com/user-attachments/assets/869b5b84-da64-4852-a54b-c173863eab1a)

### GrpcService

Implementa um serviço **gRPC** chamado `GreeterService`, que interage com um sistema de criptomoedas. Ele usa um **cliente HTTP** para fazer chamadas a um **Web Service** que gerencia moedas e carteiras.

#### Funcionamento

O `GreeterService` é uma classe que herda de `Greeter.GreeterBase`, o que significa que ele **implementa os métodos definidos** no arquivo de especificação **Proto**. No geral, ele fornece funcionalidades para:

- **Criar moedas** (`PostMoedas`)
- **Obter lista de moedas** (`GetMoedas`)
- **Criar carteiras** (`PostCarteira`)
- **Deletar carteiras** (`DeleteCarteira`)
- **Comprar moedas** (`ComprarMoeda`)
- **Transferir moedas entre carteiras** (`TransferirMoeda`)
- **Atualizar valor da moeda** (`AtualizarValMoeda`)
- **Consultar saldo de uma carteira** (`SaldoCarteira`)
- **Simular uma compra ou transferência de moedas** (`SimularCompraTransferencia`)

Cada um desses métodos **faz chamadas HTTP** para um serviço externo na **porta 5202**, que parece ser uma **API REST** que gerencia moedas e carteiras.

---

### Explicação detalhada de alguns métodos

#### `PostMoedas(PostMoedaRequest request, ServerCallContext context)`

- Recebe um **ID**, **Nome** e **Valor** de uma moeda.
- Envia para o **Web Service** (`/api/MoedasCB`) via uma requisição HTTP **POST**.
- Converte o **valor** para um formato decimal antes de enviar.
- Exibe os dados no console.

#### `GetMoedas(Empty empty, ServerCallContext context)`

- Consulta a lista de **moedas existentes** no Web Service (`/api/MoedasCB`) via **GET**.
- Converte os dados recebidos para uma **lista de objetos** `PostMoedaRequest`.
- Retorna essa lista ao **cliente gRPC**.

#### `PostCarteira(PostCarteiraRequest request, ServerCallContext context)`

- Cria uma **nova carteira** vinculada a um usuário.
- Transforma o valor recebido (`QtdMoedas`) para **decimal** e define um **statusConta** (`Positivo` ou `Neutro`).
- Envia os dados ao **Web Service** via **POST** (`/api/Carteiras`).
- Exibe a resposta no console.

---

### **Proto - Protocol Buffers (protobuf)**

A implementação usa **proto3**, uma linguagem de descrição de interfaces usada no **gRPC**. Ela define:

- Um serviço chamado `Greeter`, que contém vários **métodos RPC (Remote Procedure Call)**.
- **Mensagens** que representam **requisições e respostas** trocadas entre o **cliente gRPC** e o **servidor**.

#### **Definição do Serviço Greeter**

Este serviço inclui métodos que permitem **inserir, consultar, atualizar, comprar e transferir moedas**, além de **gerenciar carteiras**. Cada método é um **RPC** que recebe um objeto de **requisição** e retorna um objeto de **resposta**.

```proto
service Greeter { 
  rpc PostMoedas (PostMoedaRequest) returns (PostMoedaResponse); 
  rpc GetMoedas (Empty) returns (GetMoedasResponse); 
  rpc DeleteCarteira (DeleteCarteiraRequest) returns (DeleteCarteiraResponse); 
  rpc PostCarteira (PostCarteiraRequest) returns (PostCarteiraResponse); 
  rpc ComprarMoeda (ComprarRequest) returns (ComprarResponse); 
  rpc TransferirMoeda (TransfRequest) returns (TransfResponse); 
  rpc AtualizarValMoeda (ValMoedaRequest) returns (ValMoedaRepost); 
  rpc SaldoCarteira (VisuSaldoRequest) returns (VisuSaldoResponse); 
  rpc SimularCompraTransferencia (RequestSimulacao) returns (ResponseSimuacao); 
}

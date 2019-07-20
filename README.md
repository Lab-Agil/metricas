# Métricas

Projeto de extração do histórico de datas das transições do board via API do AzureDevOps.

## Objetivo

O objetivo é que esta ferramenta recupere dados de histórico de WorkItems de um projeto do AzureDevOps e gere informações de métricas.

## Utilização da API do AzureDevOps

A ferramenta usa APIs do AzureDevOps e para isso é necessário um **token** de acesso na conta onde existe o projeto que se deseja extrair os dados.
O token, assim como o a URI da API, ficam parametrizados no arquivo **appsettings.json**.

## appsettings.json

```json
{
    "ProjetoDesejado": "NomeDoProjeto",
    "Projetos": {
        "NomeDoProjeto": {
            "UriString": "",
            "PersonalAccessToken": "",
            "Project": ""
        }
    },
    "WiqlQuery": "SELECT [State], [Title] FROM WorkItems Where [Work Item Type] <> 'Task'"
}
```

- **ProjetoDesejado** aponta para um dos items que podem existir em **Projetos**

- **Projetos** contém um ou mais conjuntos de informações básicas para o acesso a API do AzureDevOps

  - **NomeDoProjeto** um valor definido por você para identificar o conjunto de informações de um projeto para extarir os dados

    - **UriString** URI da conta onde o projeto existe. Ex.: https://dev.azure.com/empresa-x

    - **PersonalAccessToken** é o token necessário para acesso a API do AzureDevOps que tem que ser criado para um usuário da conta onde existe o projeto

    - **Project** é o nome do projeto que existe na conta e que tem os WorkItems a serem recuperados os dados para gerar métricas

**WiqlQuery** é a query utulizada para executar na API do AzureDevOps e recuperar os WorkItems do projeto.

# Contribuições

São muito bem vindas! (=

O projeto está no ínicio e bem simples. Precisa de melhorias e novas implementações para crescer e oferecer mais recursos e funcionalidades.

Toda contribuição é de grande ajuda. Seja criando issues, enviando PRs, ou ajudando de outras formas.

Para executar o projeto para fazer implementações de código você vai precisar de uma conta no AzureDevOps (é free) e ter nessa conta um projeto com um board e WorkItems. Assim é possível ver a ferramenta funcionando.

Se preisar de qualquer ajuda tanto para executar o projeto quanto para criar uma conta, um projeto e um board no AzureDevOps fique a vontade para entrar em contato!

A documentação das APIs do AzureDevOps ajuda bastante para entender e utilizar: https://docs.microsoft.com/en-us/rest/api/azure/devops/
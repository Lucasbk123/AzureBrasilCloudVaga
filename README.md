# Desafio Técnico: Aplicação FullStack
Este projeto foi desenvolvido como parte de um desafio prático para demonstrar **autenticação com Azure Entra ID** e exibição de informações relacionadas ao usuário e ao tenant autenticado.  

A solução foi construída com **.NET 9**, **Blazor WebAssembly** no frontend, **Web API** no backend e utiliza **Redis** para cache de dados.

## 📌 Funcionalidades

- Login com **Azure Entra ID**.  
- Exibição das informações do usuário autenticado:
  - Nome  
  - Tenant ID  
- Exibição de informações adicionais do Tenant:
  - Tentativas de login recentes  
  - Usuários do Tenant  
  - Grupos do Tenant   


---

### 🛠️ Tecnologias Utilizadas

-   **Backend:** [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
-   **Fronted:** [Blazor Webassembly](https://dotnet.microsoft.com/pt-br/apps/aspnet/web-apps/blazor)
-   **Cache:** [Redis](https://github.com/redis/redis)
-   **Autenticação:** [Azure Entra ID](https://learn.microsoft.com/azure/active-directory/develop/)  

## 🛠️ Como Executar o Projeto

### 🔹 Pré-requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)  
- [Redis](https://redis.io/download)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)
-  Conta no Azure com acesso ao Entra ID

⚠️ **Observação Importante sobre Permissões e Redirect URI**  
- No **App Registration**, configure a **Redirect URI** para o Blazor WebAssembly, por exemplo:  
  `https://localhost:8080/authentication/login-callback`  
- Configure as permissões necessárias para acessar informações adicionais do **Tenant** via **Microsoft Graph API**, como:  
  - `User.Read.All` (delegada – para leitura de todos os usuários do Tenant)  
  - `Group.Read.All` (delegada – para leitura de todos os grupos do Tenant)  
  - `AuditLog.Read.All` (opcional – para consultar tentativas de login)  
  - `User.Read` (delegada – obrigatória para login e perfil básico) 
- Após configurar, **conceda o consentimento de administrador** (Admin Consent).

### 🧾 1. Clone o repositório 

Abra o terminal e execute:

```bash
git clone https://github.com/Lucasbk123/AzureBrasilCloudVaga.git
```

### 🐳 2. Iniciando a aplicação com docker compose

- navegue até o diretório raiz onde está localizado o `docker-compose.yml`:
```bash
cd .\AzureBrasilCloudVaga\infra\
```
 -   **Atualizer as configurações do Docker Compose:**
```yaml
services:
  cache:
    image: "docker.io/library/redis:7.4"
    command:
      - "-c"
      - "redis-server --requirepass $$REDIS_PASSWORD"
    entrypoint:
      - "/bin/sh"
    environment:
      REDIS_PASSWORD: "${CACHE_PASSWORD}"
    expose:
      - "6379"
    networks:
      - "AzureBrasilCloud"
  apiservice:
    build:
      context: ../
      dockerfile: src/AzureBrasilCloudVaga.ApiService/Dockerfile
    environment:
      HTTP_PORTS: "${APISERVICE_PORT}"
      ConnectionStrings__cache: "cache:6379,password=${CACHE_PASSWORD}"
   # ⚠️ SUBSTITUA PELOS SEUS VALORES REAIS
      AzureAd__Instance: "https://login.microsoftonline.com/"
      AzureAd__Domain: "Domain"
      AzureAd__TenantId: "TenantId"
      AzureAd__ClientId: "ClientId"
      AzureAd__ClientSecret: "ClientSecret"
      AzureAd__Audience: "api://ClientId"
   # --------------------------------------------
    ports:
      - "5010:8080"
    expose:
      - "${APISERVICE_PORT}"
    depends_on:
      cache:
        condition: "service_started"
    networks:
      - "AzureBrasilCloud"
  webfrontend:
    build:
      context: ../src/AzureBrasilCloudVaga.Web
      dockerfile: dockerfile
    environment:
      HTTP_PORTS: "80"
   # ⚠️ SUBSTITUA PELOS SEUS VALORES REAIS
      AZURE_AUTHORITY: "https://login.microsoftonline.com/tenantId"
      CLIENT_ID: "CLIENT_ID"
      NEW_SCOPE: "api://CLIENT_ID/User.Read"
   # --------------------------------------------
      API_BASEURL: "http://localhost:5010"
    ports:
      - "8080:80"
    depends_on:
      apiservice:
        condition: "service_started"
    networks:
      - "AzureBrasilCloud"
networks:
  AzureBrasilCloud:
    driver: "bridge"
```

- Depois, execute o comando:
```bash
docker-compose up --build
```
✅ Pronto! o front estará disponível no seguinte endereço:
```bash
http://localhost:8080
```
### 💻 3. Inciado a aplicação pelo o visual studio

- navegue até o diretório:
```bash
cd .\AzureBrasilCloudVaga
```
- Abra o projeto no Visual Studio e edite  o arquivo appsettings.json do projeto **AzureBrasilCloudVaga.ApiService** 
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
## ⚠️ SUBSTITUA PELOS SEUS VALORES REAIS
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "Domain",
    "TenantId": "TenantId",
    "ClientId": "ClientId",
    "ClientSecret": "ClientSecret",
    "Audience": "Audience"
  },
  "AllowedHosts": "*"
}
```
e do projeto AzureBrasilCloudVaga.Web
```json
{
## ⚠️ SUBSTITUA PELOS SEUS VALORES REAIS
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/TenantId",
    "ClientId": "ClientId",
    "ValidateAuthority": true
# ---------------------------------------
  },
  "Api": {
    "BaseUrl": "https://localhost:7510",
## ⚠️ SUBSTITUA PELOS SEUS VALORES REAIS
    "Scopes": [
      "api://ClientId/User.Read"
    ]
# ---------------------------------------
  }
}
```
- Depois, é só executar o projeto AzureBrasilCloudVaga.AppHost

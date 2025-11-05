🚀 Minimal API — Projeto em .NET 8 com JWT

Bem-vindo ao repositório Minimal-API, um projeto simples e prático feito com .NET 8 que demonstra como criar uma API moderna, leve e segura utilizando Minimal API com autenticação via JWT (JSON Web Token).

🧩 Tecnologias Utilizadas

C#

.NET 8 SDK

Entity Framework Core (opcional)

Swagger / OpenAPI (documentação)

JWT (JSON Web Token) para autenticação

🔐 Autenticação JWT

Esta API utiliza JWT (JSON Web Token) para proteger rotas e garantir que apenas utilizadores autenticados tenham acesso a determinadas funcionalidades.

🔑 Exemplo de Fluxo de Autenticação

1 - Login → O utilizador envia e-mail e palavra-passe para o endpoint /login.

2 - Geração do Token → Se as credenciais forem válidas, a API devolve um token JWT.

3 - Acesso Autenticado → O token deve ser enviado no cabeçalho Authorization em cada pedido protegido:
Authorization: Bearer <teu_token_jwt>

4 - Validação → O servidor valida o token antes de processar a requisição.

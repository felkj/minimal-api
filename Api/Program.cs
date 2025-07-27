using System.IdentityModel.Tokens.Jwt; // Usado para manipular tokens JWT (JSON Web Tokens)
using System.Security.Claims;          // Usado para trabalhar com claims (informações sobre o usuário)
using System.Text;                     // Usado para codificação de texto (ex: UTF8)
using Microsoft.AspNetCore.Authentication.JwtBearer; // Middleware para autenticação JWT
using Microsoft.AspNetCore.Authorization;           // Atributos e funcionalidades para autorização
using Microsoft.AspNetCore.Identity;                // Pode ser usado para gerenciamento de usuários (não diretamente usado aqui para autenticação, mas é comum)
using Microsoft.AspNetCore.Mvc;                     // Classes base para Controllers e atributos como [FromBody], [FromRoute]
using Microsoft.EntityFrameworkCore;                // Core do Entity Framework para interação com banco de dados
using Microsoft.IdentityModel.Tokens;               // Classes para segurança de tokens, como chaves de assinatura
using Microsoft.OpenApi.Models;                     // Modelos para configurar o Swagger/OpenAPI
using minimal_api.Domain.DTO;                       // Data Transfer Objects (DTOs) para entrada/saída de dados
using minimal_api.Domain.Entity;                    // Entidades do domínio (modelos de dados)
using minimal_api.Domain.Enuns;                     // Enums usados no domínio
using minimal_api.Domain.Interface;                 // Interfaces de serviço e repositório
using minimal_api.Domain.ModelViews;                // ModelViews para representar dados de forma específica para a UI/API
using minimal_api.Domain.Services;                  // Implementações dos serviços de negócio
using minimal_api.Infra.Db;                         // Contexto do banco de dados (DbContext)
using minimal_api.Infra.Interface;                  // Interfaces de infraestrutura (ex: repositórios)

#region Builder
// Cria o construtor da aplicação web, configurando serviços e o pipeline de requisição.
var builder = WebApplication.CreateBuilder(args);

// Pega a chave secreta JWT da seção "Jwt" no arquivo appsettings.json.
// Se não encontrar, usa uma chave padrão "123456" (idealmente, deve ser uma chave forte e configurada).
var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "123456";

// Configura o esquema de autenticação para usar JWT Bearer.
builder.Services.AddAuthentication(option =>
{
    // Define o esquema padrão para autenticar e desafiar (pedir credenciais).
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option => // Adiciona o manipulador para tokens JWT Bearer.
{
    option.TokenValidationParameters = new TokenValidationParameters // Define os parâmetros para validar os tokens recebidos.
    {
        ValidateLifetime = true, // Valida se o token ainda é válido (não expirou).
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), // Chave usada para verificar a assinatura do token.
        ValidateIssuer = false,  // Não valida o emissor do token (para ambientes de desenvolvimento, em produção, deve ser true).
        ValidateAudience = false // Não valida a audiência do token (para desenvolvimento, em produção, deve ser true).
    };
});

// Habilita o serviço de autorização na aplicação.
builder.Services.AddAuthorization();

// Registra os serviços (classes de lógica de negócio) para injeção de dependência.
// 'Scoped' significa que uma nova instância é criada por requisição.
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

// Adiciona serviços para descobrir endpoints da API (necessário para o Swagger).
builder.Services.AddEndpointsApiExplorer();
// Configura o Swagger para gerar a documentação da API.
builder.Services.AddSwaggerGen(options =>
{
    // Adiciona uma definição de segurança para o Swagger UI.
    // Isso permite que você insira o token JWT diretamente na interface do Swagger para testar endpoints protegidos.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",      // Nome do cabeçalho HTTP para o token.
        Type = SecuritySchemeType.Http, // Tipo de esquema de segurança (HTTP).
        Scheme = "bearer",           // Esquema de autenticação (Bearer para JWT).
        BearerFormat = "JWT",        // Formato do token.
        In = ParameterLocation.Header, // O token será enviado no cabeçalho da requisição.
        Description = "Insira seu token JWT:" // Descrição para o usuário no Swagger UI.
    });

    // Adiciona um requisito de segurança global, associando a definição "Bearer" aos endpoints.
    // Isso faz com que o ícone de cadeado apareça no Swagger UI para endpoints protegidos.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer" // Refere-se à definição de segurança "Bearer" criada acima.
            }
            },
            new string[] {} // Escopos vazios, pois JWT geralmente não usa escopos definidos aqui.
        }
    });
});

// Configura o DbContexto para usar MySQL com Entity Framework Core.
builder.Services.AddDbContext<DbContexto>(options =>
{
    // Usa a string de conexão "mysql" do appsettings.json.
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        // Detecta automaticamente a versão do servidor MySQL para compatibilidade.
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
        );
});

// Constrói a aplicação web com todas as configurações de serviço.
var app = builder.Build();

// Habilita o middleware do Swagger para servir o arquivo JSON da especificação da API.
app.UseSwagger();
// Habilita o middleware do Swagger UI para servir a interface interativa no navegador.
app.UseSwaggerUI();
#endregion

#region Home
// Mapeia um endpoint GET para a rota raiz ("/").
// Retorna um JSON simples e permite acesso sem autenticação (AllowAnonymous).
// O .WithTags("Home") organiza este endpoint sob a tag "Home" no Swagger UI.
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home").AllowAnonymous();
#endregion

#region Admins
// Função auxiliar para gerar um token JWT para um objeto Admin.
string GerarTokenJwt(Admin admin)
{
    if (string.IsNullOrEmpty(key)) return string.Empty; // Retorna vazio se a chave não estiver configurada.
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)); // Cria a chave de segurança.
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); // Define as credenciais de assinatura.

    // Cria uma lista de "claims" (informações) que serão incluídas no token JWT.
    var claims = new List<Claim>()
    {
        new Claim("Email", admin.Email),
        new Claim("Role/Cargo", admin.Role), // Claim personalizada para o cargo/role.
        new Claim(ClaimTypes.Role, admin.Role) // Claim padrão para o cargo/role.
    };

    // Cria o token JWT com as claims, data de expiração e credenciais de assinatura.
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1), // Token expira em 1 dia.
        signingCredentials: credentials
    );

    // Escreve o token JWT como uma string.
    return new JwtSecurityTokenHandler().WriteToken(token);
}

// Endpoint de Login para administradores.
app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    // Tenta fazer o login usando o serviço de administrador.
    var adm = adminService.Login(loginDTO);
    if (adm != null)
    {
        string token = GerarTokenJwt(adm); // Gera o token JWT se o login for bem-sucedido.
        return Results.Ok(new AdmLogado // Retorna as informações do admin logado e o token.
        {
            Email = adm.Email,
            Role = adm.Role,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized(); // Retorna 401 Unauthorized se o login falhar.
    }
}).AllowAnonymous().WithTags("Administradores"); // Permite acesso sem autenticação e organiza no Swagger.

// Endpoint para buscar todos os administradores (com paginação opcional).
app.MapGet("/admin", ([FromQuery] int? pagina, IAdminService adminService) =>
{
    var adms = new List<AdminModelView>(); // Lista para armazenar os administradores formatados.
    var admins = adminService.FindAll(pagina); // Busca os administradores usando o serviço.
    foreach (var adm in admins) {
        adms.Add(new AdminModelView // Mapeia a entidade Admin para um AdminModelView.
        {
            Id = adm.Id,
            Email = adm.Email,
            Role = adm.Role
        });
    }
    return Results.Ok(adms); // Retorna a lista de administradores.
})
.RequireAuthorization() // Exige que o usuário esteja autenticado.
.RequireAuthorization(new AuthorizeAttribute {Roles = "admin"}) // Exige que o usuário tenha a role "admin".
.WithTags("Administradores"); // Organiza no Swagger.

// Endpoint para procurar administrador por ID.
app.MapGet("/admin/{id}", ([FromRoute] int id, IAdminService adminService) =>
{
    var admin = adminService.FindById(id); // Busca o administrador pelo ID.

    if (admin == null) return Results.NotFound(); // Retorna 404 Not Found se não encontrar.

    return Results.Ok(new AdminModelView // Retorna o administrador encontrado.
    {
            Id = admin.Id,
            Email = admin.Email,
            Role = admin.Role
    });

})
.RequireAuthorization() // Exige autenticação.
.RequireAuthorization(new AuthorizeAttribute {Roles = "admin"}) // Exige a role "admin".
.WithTags("Administradores"); // Organiza no Swagger.

// Endpoint para criação de novos administradores.
app.MapPost("/admin", ([FromBody] AdminDTO adminDTO, IAdminService adminService) =>
{
    // Objeto para armazenar mensagens de erro de validação.
    var validacao = new ErrorMessages
    {
        Messages = new List<string>()
    };

    // Realiza validações básicas dos campos do DTO.
    if (string.IsNullOrEmpty(adminDTO.Email))
        validacao.Messages.Add("Email não pode ser vazio");
    if (string.IsNullOrEmpty(adminDTO.Password))
        validacao.Messages.Add("Senha não pode ser vazia");
    if (adminDTO.Role == null)
        validacao.Messages.Add("Cargo/Perfil não pode ser vazio");

    if (validacao.Messages.Count > 0)
        return Results.BadRequest(validacao); // Retorna 400 Bad Request com as mensagens de erro.

        // Cria uma nova entidade Admin a partir do DTO.
        var admin = new Admin
        {
            Email = adminDTO.Email,
            Password = adminDTO.Password,
            // Define a role, usando "editor" como padrão se não for especificada.
            Role = adminDTO.Role.ToString() ?? Roles.editor.ToString()
        };
        adminService.Create(admin); // Cria o administrador usando o serviço.

        // Retorna 201 Created com a URL do novo recurso e o AdminModelView.
        return Results.Created($"/admin/{admin.Id}", new AdminModelView
    {
            Id = admin.Id,
            Email = admin.Email,
            Role = admin.Role
    });

})
.RequireAuthorization() // Exige autenticação.
.RequireAuthorization(new AuthorizeAttribute {Roles = "admin"}) // Exige a role "admin".
.WithTags("Administradores"); // Organiza no Swagger.
#endregion

#region Veiculos
// Função auxiliar para validar um VeiculoDTO.
ErrorMessages validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrorMessages
    {
        Messages = new List<string>()
    };

    // Realiza validações básicas para os campos do VeiculoDTO.
    if (string.IsNullOrEmpty(veiculoDTO.Name))
        validacao.Messages.Add("O nome não pode ser vazio");

    if (string.IsNullOrEmpty(veiculoDTO.Brand))
        validacao.Messages.Add("A marca não pode ser vazia");

    if (veiculoDTO.Year < 1950)
        validacao.Messages.Add("O ano não pode ser abaixo de 1950");

    return validacao; // Retorna o objeto com as mensagens de erro (se houver).
}

// Endpoint para criar novos veículos.
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoService veiculoService) =>
{
    var validacao = new ErrorMessages{
        Messages = new List<string>()
    };

    validacao = validaDTO(veiculoDTO); // Chama a função de validação.
    if (validacao.Messages.Count > 0)
        return Results.BadRequest(validacao); // Retorna 400 Bad Request se a validação falhar.

    // Cria uma nova entidade Veiculo a partir do DTO.
    var veiculo = new Veiculo
    {
        Name = veiculoDTO.Name,
        Brand = veiculoDTO.Brand,
        Year = veiculoDTO.Year
    };
    veiculoService.CreateVehicle(veiculo); // Cria o veículo usando o serviço.

    return Results.Created($"/veiculos/{veiculo.Id}", veiculo); // Retorna 201 Created.
})
.RequireAuthorization() // Exige autenticação.
.RequireAuthorization(new AuthorizeAttribute {Roles = "admin, editor"}) // Exige role "admin" ou "editor".
.WithTags("Veiculos"); // Organiza no Swagger.

// Endpoint para listar todos os veículos (com paginação opcional).
app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoService veiculoService) =>
{
    var veiculos = veiculoService.FindAll(pagina); // Busca os veículos usando o serviço.

    return Results.Ok(veiculos); // Retorna a lista de veículos.
})
.RequireAuthorization() // Exige autenticação.
.WithTags("Veiculos"); // Organiza no Swagger.

// Endpoint para procurar veículo por ID.
app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.FindById(id); // Busca o veículo pelo ID.

    if (veiculo == null) return Results.NotFound(); // Retorna 404 Not Found se não encontrar.

    return Results.Ok(veiculo); // Retorna o veículo encontrado.

})
.RequireAuthorization() // Exige autenticação.
.WithTags("Veiculos"); // Organiza no Swagger.

// Endpoint para atualizar veículo por ID.
app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoService veiculoService) =>
{
    ErrorMessages validacao = validaDTO(veiculoDTO); // Chama a função de validação.
    if (validacao.Messages.Count > 0)
        return Results.BadRequest(validacao); // Retorna 400 Bad Request se a validação falhar.

    // Busca o veículo por ID para verificar se existe.
    var veiculo = veiculoService.FindById(id);
    if (veiculo == null) return Results.NotFound(); // Retorna 404 Not Found se não existir.

    // Atualiza os campos do veículo com os dados do DTO.
    veiculo.Name = veiculoDTO.Name;
    veiculo.Brand = veiculoDTO.Brand;
    veiculo.Year = veiculoDTO.Year;
    
    veiculoService.UpdateVehicle(veiculo); // Atualiza o veículo no banco de dados.

    return Results.Ok(veiculo); // Retorna o veículo atualizado.

})
.RequireAuthorization() // Exige autenticação.
.RequireAuthorization(new AuthorizeAttribute {Roles = "admin"}) // Exige a role "admin".
.WithTags("Veiculos"); // Organiza no Swagger.

// Endpoint para apagar veículo por ID.
app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    // Busca o veículo por ID para verificar se existe.
    var veiculo = veiculoService.FindById(id);
    if (veiculo == null) return Results.NotFound(); // Retorna 404 Not Found se não existir.
    
    veiculoService.DeleteVehicle(veiculo); // Apaga o veículo usando o serviço.
    return Results.NoContent(); // Retorna 204 No Content (sucesso sem conteúdo de retorno).

})
.RequireAuthorization() // Exige autenticação.
.RequireAuthorization(new AuthorizeAttribute {Roles = "admin"}) // Exige a role "admin".
.WithTags("Veiculos"); // Organiza no Swagger.
#endregion

#region App
// Habilita o middleware do Swagger para servir o arquivo JSON da especificação da API.
app.UseSwagger();
// Habilita o middleware do Swagger UI para servir a interface interativa no navegador.
app.UseSwaggerUI();

// IMPORTANTE: A ordem dos middlewares de autenticação e autorização importa!
// 'UseAuthentication()' deve vir ANTES de 'UseAuthorization()'.
// Ele tenta identificar quem é o usuário.
app.UseAuthentication();
// 'UseAuthorization()' deve vir DEPOIS de 'UseAuthentication()'.
// Ele verifica se o usuário autenticado tem permissão para acessar o recurso.
app.UseAuthorization();

// Inicia a aplicação web e começa a escutar por requisições HTTP.
app.Run();
#endregion
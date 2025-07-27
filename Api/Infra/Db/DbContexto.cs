using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Namespace principal do Entity Framework Core
using minimal_api.Domain.Entity;    // Importa as entidades (modelos de dados) do seu domínio
using Microsoft.Extensions.Configuration; // Necessário para IConfiguration

namespace minimal_api.Infra.Db
{
    /// <summary>
    /// Representa o contexto do banco de dados para a aplicação, herdando de DbContext do Entity Framework Core.
    /// É a ponte entre as entidades do seu domínio e o banco de dados.
    /// </summary>
    public class DbContexto : DbContext
    {
        // Campo privado para armazenar a configuração da aplicação (appsettings.json).
        private readonly IConfiguration _configuracaoAppSettings;

        /// <summary>
        /// Construtor do DbContexto.
        /// Recebe IConfiguration via injeção de dependência para acessar a string de conexão do banco de dados.
        /// </summary>
        /// <param name="configuracaoAppSettings">Objeto de configuração da aplicação.</param>
        public DbContexto(IConfiguration configuracaoAppSettings)
        {
            _configuracaoAppSettings = configuracaoAppSettings;
        }

        // Propriedades DbSet representam as tabelas no banco de dados.
        // Cada DbSet corresponde a uma entidade do seu domínio.
        public DbSet<Admin> Admins { get; set; } = default!; // Tabela para a entidade Admin
        public DbSet<Veiculo> Veiculos { get; set; } = default!; // Tabela para a entidade Veiculo


        /// <summary>
        /// Este método é chamado pelo Entity Framework Core quando o modelo do banco de dados está sendo criado.
        /// É usado para configurar o modelo, incluindo mapeamento de entidades, chaves, relacionamentos e seeding de dados.
        /// </summary>
        /// <param name="modelBuilder">Construtor de modelo que permite configurar o modelo do banco de dados.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura a criação de uma "seed" (dados iniciais) para a tabela de Admins.
            // Isso insere um registro de administrador padrão no banco de dados quando as migrações são aplicadas.
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,                 // ID do administrador
                    Email = "admin@teste.com", // E-mail padrão
                    Password = "123456",    // Senha padrão (em um ambiente real, senhas devem ser hashadas!)
                    Role = "Admin"          // Cargo/Role do administrador
                }
            );
        }

        /// <summary>
        /// Este método é chamado pelo Entity Framework Core para configurar as opções do DbContext.
        /// É onde você especifica qual provedor de banco de dados (ex: MySQL, SQL Server) será usado e a string de conexão.
        /// </summary>
        /// <param name="optionsBuilder">Construtor de opções do DbContext.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Verifica se as opções do DbContext já foram configuradas (ex: via injeção de dependência no Program.cs).
            // Isso evita a reconfiguração se já tiver sido feito.
            if (!optionsBuilder.IsConfigured)
            {
                // Obtém a string de conexão chamada "mysql" do appsettings.json.
                var stringConnection = _configuracaoAppSettings.GetConnectionString("mysql")?.ToString();

                // Se a string de conexão não for nula ou vazia, configura o MySQL.
                if (!string.IsNullOrEmpty(stringConnection))
                {
                    // Configura o DbContext para usar MySQL com a string de conexão fornecida.
                    // ServerVersion.AutoDetect tenta identificar a versão correta do servidor MySQL.
                    optionsBuilder.UseMySql(stringConnection, ServerVersion.AutoDetect(stringConnection));
                }
            }
        }
    }
}
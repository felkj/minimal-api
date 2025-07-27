using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.Enuns; // Importa o enum de Roles (cargos/perfis)

namespace minimal_api.Domain.DTO
{
    /// <summary>
    /// DTO (Data Transfer Object) para transferência de dados de Administrador.
    /// Usado geralmente para receber dados de entrada (ex: criação ou atualização de um admin)
    /// ou para enviar dados específicos para o cliente.
    /// </summary>
    public class AdminDTO
    {
        /// <summary>
        /// O endereço de e-mail do administrador.
        /// 'default!' indica que o compilador deve assumir que esta propriedade será inicializada.
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// A senha do administrador.
        /// </summary>
        public string Password { get; set; } = default!;

        /// <summary>
        /// O cargo ou perfil do administrador, usando o enum 'Roles'.
        /// O '?' indica que a propriedade é anulável (pode ser null).
        /// </summary>
        public Roles? Role { get; set; } = default!;
    }
}
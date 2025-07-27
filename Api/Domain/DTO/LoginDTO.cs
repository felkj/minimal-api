using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.DTO
{
    /// <summary>
    /// DTO (Data Transfer Object) para dados de login.
    /// Usado para receber as credenciais (e-mail e senha) de um usuário tentando se autenticar.
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// O endereço de e-mail do usuário para login.
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// A senha do usuário para login.
        /// </summary>
        public string Password { get; set; } = default!;
    }
}
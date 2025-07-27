using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.DTO
{
    /// <summary>
    /// Record DTO (Data Transfer Object) para transferência de dados de Veículo.
    /// 'record' é um tipo de referência que oferece imutabilidade e concisão,
    /// ideal para DTOs onde os dados não devem ser alterados após a criação.
    /// </summary>
    public record VeiculoDTO
    {
        /// <summary>
        /// O nome do veículo (ex: "Civic", "Corolla").
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// A marca do veículo (ex: "Honda", "Toyota").
        /// </summary>
        public string Brand { get; set; } = default!;

        /// <summary>
        /// O ano de fabricação do veículo.
        /// </summary>
        public int Year { get; set; } = default!;
    }
}
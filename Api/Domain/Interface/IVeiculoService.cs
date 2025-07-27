using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.Entity;

namespace minimal_api.Domain.Interface
{
    public interface IVeiculoService
    {
        //Lista todos os veiculos
        List<Veiculo> FindAll(int? pagina = 1, string? name = null, string? brand = null);

        //Procura veiculo por Id
        Veiculo? FindById(int id);

        //Cadastra um veiculo no Banco de Dados
        void CreateVehicle(Veiculo veiculo);

        //Atualiza um veiculo ja existente
        void UpdateVehicle( Veiculo veiculo);

        //Apaga um veiculo ja exitente pelo id
        void DeleteVehicle(Veiculo veiculo);
    }
}
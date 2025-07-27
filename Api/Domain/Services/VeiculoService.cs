using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entity;
using minimal_api.Domain.Interface;
using minimal_api.Infra.Db;

namespace minimal_api.Domain.Services
{
    public class VeiculoService : IVeiculoService
    {
        public readonly DbContexto _contexto;

        public VeiculoService(DbContexto db)
        {
            _contexto = db;
        }

        public void CreateVehicle(Veiculo veiculo)
        {
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
        }

        public void DeleteVehicle(Veiculo veiculo)
        {
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }

        public List<Veiculo> FindAll(int? pagina = 1, string? name = null, string? brand = null)
        {

            var query = _contexto.Veiculos.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(v => EF.Functions.Like(v.Name.ToLower(), $"%{name}%"));
            }
            int itensPorPagina = 10;
            if (pagina != null)
            {
              query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);  
            }
            
            return query.ToList();
        }

        public Veiculo? FindById(int id)
        {
            return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
        }

        public void UpdateVehicle(Veiculo veiculo)
        {
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Entity;
using minimal_api.Infra.Db;
using minimal_api.Infra.Interface;

namespace minimal_api.Domain.Services
{
    public class AdminService : IAdminService
    {

        public readonly DbContexto _contexto;
        public AdminService(DbContexto db)
        {
            _contexto = db;
        }

        public Admin? Create(Admin admin)
        {
            _contexto.Admins.Add(admin);
            _contexto.SaveChanges();

            return admin;
        }

        public List<Admin> FindAll(int? pagina)
        {
             var query = _contexto.Admins.AsQueryable();
            
            int itensPorPagina = 10;
            if (pagina != null)
            {
              query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);  
            }
            
            return query.ToList();
        }

        public Admin? FindById(int id)
        {
            return _contexto.Admins.Where(a => a.Id == id).FirstOrDefault();
        }

        public Admin? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Admins.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
            return adm;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Entity;

namespace minimal_api.Infra.Interface
{
    public interface IAdminService
    {
        Admin? Login(LoginDTO loginDTO);
        Admin? Create(Admin admin);
        Admin? FindById(int id);
        List<Admin> FindAll(int? pagina);

    }
}
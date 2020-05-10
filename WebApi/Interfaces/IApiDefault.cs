using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Interfaces
{
    public interface IApiDefault
    {
        Task<IActionResult> GetAll();
        Task<IActionResult> GetById(object id);
    }
}

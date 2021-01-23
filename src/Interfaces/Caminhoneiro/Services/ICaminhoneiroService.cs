using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CPTruckAPI.src.Caminhoneiro.Models;

namespace CPTruckAPI.src.Caminhoneiro.Interfaces
{
    public interface ICaminhoneiroService
    {
        Task<JsonResult> EnviarCaminhoneiro(Caminhoneiros caminhoneiro);

        Task<JsonResult> AuthCaminhoneiro(string cpf, string senha);
    }
}
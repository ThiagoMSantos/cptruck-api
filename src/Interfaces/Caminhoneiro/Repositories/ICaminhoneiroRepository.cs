using System;
using System.Threading.Tasks;
using CPTruckAPI.src.Caminhoneiro.Models;

namespace CPTruckAPI.src.Caminhoneiro.Interfaces{
    public interface ICaminhoneiroRepository
    {
        Task<Boolean> PostCaminhoneiro(Caminhoneiros caminhoneiro);
        
        Task<Caminhoneiros> AuthCaminhoneiro(string cpf, string senha);

    }
}
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CPTruckAPI.src.Caminhoneiro.Models;
using Microsoft.IdentityModel.Tokens;
using System;

namespace CPTruckAPI.src.Caminhoneiro.Interfaces
{
    public interface ICaminhoneiroAuth
    {
        Task<String> Authenticate(string cpf, string senha);

    }
}
using CPTruckAPI.src.Caminhoneiro.Interfaces;
using CPTruckAPI.src.Caminhoneiro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace CPTruckAPI.src.Caminhoneiro.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class CaminhoneiroController : ControllerBase
    {
        private readonly ICaminhoneiroService _CaminhoneiroService;
        public CaminhoneiroController(ICaminhoneiroService CaminhoneiroService)
        {
            _CaminhoneiroService = CaminhoneiroService;
        }

        ///<summary>
        ///Adicionar Caminhoneiros
        ///</summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> EnviarCaminhoneiro([FromBody] Caminhoneiros caminhoneiro)
        {
            var result = await _CaminhoneiroService.EnviarCaminhoneiro(caminhoneiro);
            return result;
        }

        ///<summary>
        ///Autenticar Caminhoneiro
        ///</summary>
        [HttpPost("auth")]
        [AllowAnonymous]
        public async Task<JsonResult> AutenticarCaminhoneiro([FromBody] LoginDTO camlogin)
        {
            var result = await _CaminhoneiroService.AuthCaminhoneiro(camlogin.CPF, camlogin.Senha);
            return result;
        }
    }
}
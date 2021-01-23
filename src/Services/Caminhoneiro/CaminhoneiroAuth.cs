using CPTruckAPI.src.Caminhoneiro.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CPTruckAPI.src.Caminhoneiro.Interfaces;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CPTruckAPI.src.Caminhoneiro.Services{
  public class CaminhoneiroAuth : ICaminhoneiroAuth{
    private readonly ICaminhoneiroRepository _repository;
    private readonly IOptions<TokenSettings> _config;

    public CaminhoneiroAuth(ICaminhoneiroRepository repository, IOptions<TokenSettings> config)
    {
        this._repository = repository;
        this._config = config;
    }

    public async Task<String> Authenticate(string cpf, string senha){
      var caminhoneiro = await _repository.AuthCaminhoneiro(cpf, senha);

      if(caminhoneiro == null)
        return null;

      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_config.Value.ToString());

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
          {
              new Claim(ClaimTypes.Name, caminhoneiro.Nome),
              new Claim(ClaimTypes.Role, RoleFactory(caminhoneiro.Auditoria))
          }),
        Expires = DateTime.UtcNow.AddHours(5),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);

      caminhoneiro.Senha = null;

      return tokenHandler.WriteToken(token);
    }

    private static string RoleFactory(int role){
      switch(role)
      {
        case 1:
          return "Usuário";
        case 2:
          return "Administrador";
        
        default:
          throw new Exception();
          
      }
    }
  }

}
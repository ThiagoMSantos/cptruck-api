using System;
using System.Globalization;
using System.Runtime.CompilerServices;
namespace CPTruckAPI.src.Caminhoneiro.Models
{
    public class Caminhoneiros{
      public string Id {get;set;}
      public string Nome { get; set; }        
      public string Email { get; set; }
      public string Senha { get; set; }  
      public string CPF { get; set; }
      public string CNH { get; set; }
      public string RNTRC { get; set; }
      public string Telefone { get; set; }
      public int Auditoria {get;set;}
      
      public Caminhoneiros() {
        this.Id = Guid.NewGuid().ToString();
        this.Auditoria = 1;
      }

    }

    public class LoginDTO{
      public string Senha { get; set; }  
      public string CPF { get; set; }
      
    }
}
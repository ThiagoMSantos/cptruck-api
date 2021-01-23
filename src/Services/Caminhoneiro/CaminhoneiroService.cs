using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using CPTruckAPI.src.Caminhoneiro.Interfaces;
using CPTruckAPI.src.Caminhoneiro.Models;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;

namespace CPTruckAPI.src.Caminhoneiro.Services
{
    public class CaminhoneiroService : ICaminhoneiroService
    {
        private readonly ICaminhoneiroRepository _repository;
        private readonly IAsyncDocumentSession _session;
        private readonly ICaminhoneiroAuth _auth;

        public CaminhoneiroService(ICaminhoneiroRepository repository, IAsyncDocumentSession session, ICaminhoneiroAuth auth)
        {
            this._repository = repository;
            this._session = session;
            this._auth = auth;
        }

        private bool CpfValide(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;

            for (int j = 0; j < 10; j++)
                if (j.ToString().PadLeft(11, char.Parse(j.ToString())) == cpf)
                    return false;

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        private bool CnhValide(string cnh)
    {
      var firstChar = cnh[0];
      if (cnh.Length == 11 && cnh != new string('1', 11))
      {
        var dsc = 0;
        var v = 0;
        for (int i = 0, j = 9; i < 9; i++, j--)
        {
          v += (Convert.ToInt32(cnh[i].ToString()) * j);
        }

        var vl1 = v % 11;
        if (vl1 >= 10)
        {
          vl1 = 0;
          dsc = 2;
        }

        v = 0;
        for (int i = 0, j = 1; i < 9; ++i, ++j)
        {
          v += (Convert.ToInt32(cnh[i].ToString()) * j);
        }

        var x = v % 11;
        var vl2 = (x >= 10) ? 0 : x - dsc;

        return(vl1.ToString() + vl2.ToString() == cnh.Substring(cnh.Length - 2, 2));

      }

      return false;
    }

        public async Task<JsonResult> EnviarCaminhoneiro(Caminhoneiros caminhoneiro)
        {
            if(caminhoneiro != null){
                if(CpfValide(caminhoneiro.CPF)){
                    if(CnhValide(caminhoneiro.CNH)){
                        if(await _repository.PostCaminhoneiro(caminhoneiro)){
                            return new JsonResult(new { ds_mensagem = "Caminhoneiro cadastrado.", ic_sucesso = true });
                        } else{
                            return new JsonResult(new { ds_mensagem = "Caminhoneiro não cadastrado.", ic_sucesso = false });
                        }
                    } else{
                        return new JsonResult(new { ds_mensagem = "CNH Inválida.", ic_sucesso = false });
                    }
                } else{
                    return new JsonResult(new { ds_mensagem = "CPF Inválido.", ic_sucesso = false });
                }
            } else{
                return new JsonResult(new { ds_mensagem = "Informe os dados corretamente.", ic_sucesso = false });
            }
        }

        public async Task<JsonResult> AuthCaminhoneiro(string cpf, string senha){
            cpf = cpf.Trim().Replace(".", "").Replace("-", "");
            var result = await _auth.Authenticate(cpf, senha);
            if(result != null){
                return new JsonResult(new { token = result, ic_sucesso = true });
            } else{
                return new JsonResult(new { ds_mensagem = "Caminhoneiro não cadastrado.", ic_sucesso = false });
            }
        }
    }
}
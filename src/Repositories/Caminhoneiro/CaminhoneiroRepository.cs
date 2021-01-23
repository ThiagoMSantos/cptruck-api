using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CPTruckAPI.src.Caminhoneiro.Interfaces;
using CPTruckAPI.src.Caminhoneiro.Models;
using Raven.Client.Documents.Session;
using System.Linq;
using Raven.Client.Documents;

namespace CPTruckAPI.src.Caminhoneiro.Repositories{
    public class CaminhoneiroRepository : ICaminhoneiroRepository
    {
        private readonly IAsyncDocumentSession _session;

        public CaminhoneiroRepository(IAsyncDocumentSession session)
        {
            this._session = session;
        }
        
        public async Task<Boolean> PostCaminhoneiro(Caminhoneiros caminhoneiro)
        {
            await _session.StoreAsync(caminhoneiro);
            await _session.SaveChangesAsync();

            return true;
        }

        public async Task<Caminhoneiros> AuthCaminhoneiro(string cpf, string senha){

            List<Caminhoneiros> caminhoneiro;
            caminhoneiro = await _session
                .Query<Caminhoneiros>()
                .Where(x => x.CPF == cpf && x.Senha == senha)
                .ToListAsync();

            if(caminhoneiro.Count > 0){
                return caminhoneiro.First();
            }
            else{
                return null;
            }

        }
    }
}
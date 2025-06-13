using HD_Support_API.Components;
using HD_Support_API.Models;
using HD_Support_API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HD_Support_API.Repositorios
{
    public class SquadRepositorio : ISquadRepositorio
    {
        private readonly BancoContext _context;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public SquadRepositorio(BancoContext context, IUsuarioRepositorio usuarioRepositorio)
        {
            _context = context;
            _usuarioRepositorio = usuarioRepositorio;
        }
        public async Task<List<Squad>> ListarSquads()
        {
            return await _context.Squad.ToListAsync();
        }
        public async Task<(Squad squad, List<Usuarios> usuarios)> BuscarSquadPorId(int id)
        {
            var squad = await _context.Squad.FindAsync(id);
            if (squad == null) return (null, null);

            var usuariosRelacionados = await _context.Usuarios
                .Where(u => u.Idf_Squad == id)
                .ToListAsync();

            return (squad, usuariosRelacionados);
        }

        public async Task<List<Usuarios>> BuscarUsuarios(int id_squad)
        {
            return await _context.Usuarios
                .Where(u => u.Idf_Squad == id_squad)
                .ToListAsync();
        }

        public async Task<Squad> AdicionarSquad(Squad squad)
        {
            _context.Squad.Add(squad);
            await _context.SaveChangesAsync();
            return squad;
        }
        public async Task<Squad> AtualizarSquad(Squad squad, int id)
        {
            var existingSquad = await _context.Squad.FindAsync(id);
            if (existingSquad == null)
            {
                return null;
            }
            existingSquad.Nme_Squad = squad.Nme_Squad;
            existingSquad.Img_Squad = squad.Img_Squad;
            existingSquad.Local_Squad = squad.Local_Squad;
            existingSquad.Dta_Squad = squad.Dta_Squad;
            _context.Squad.Update(existingSquad);
            await _context.SaveChangesAsync();
            return existingSquad;
        }
        public async Task<bool> ExcluirSquad(int id)
        {
            var squad = await _context.Squad.FindAsync(id);
            if (squad == null)
            {
                return false;
            }
            _context.Squad.Remove(squad);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ExcluirSquad(Squad squad)
        {
            _context.Squad.Remove(squad);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

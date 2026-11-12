
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonagemHabilidadeController : ControllerBase
    {
        private readonly DataContext _context;

        public PersonagemHabilidadeController(DataContext context)
        {
            _context = context;
        }

        // Adicionar habilidade ao personagem
        [HttpPost]
        public async Task<IActionResult> AddPersonagemHabilidadeAsync(PersonagemHabilidade novoPersonagemHabilidade)
        {
            try
            {
                Personagem? personagem = await _context.TB_PERSONAGENS
                    .Include(p => p.Arma)
                    .Include(p => p.PersonagemHabilidades)
                        .ThenInclude(ps => ps.Habilidade)
                    .FirstOrDefaultAsync(p => p.Id == novoPersonagemHabilidade.PersonagemId);

                if (personagem == null)
                    throw new Exception("Personagem não encontrado para o Id informado.");

                Habilidade? habilidade = await _context.TB_HABILIDADES
                    .FirstOrDefaultAsync(h => h.Id == novoPersonagemHabilidade.HabilidadeId);

                if (habilidade == null)
                    throw new Exception("Habilidade não encontrada.");

                PersonagemHabilidade ph = new PersonagemHabilidade();

                ph.Personagem = personagem;
                ph.Habilidade = habilidade;

                await _context.TB_PERSONAGENS_HABILIDADES.AddAsync(ph);

                int linhasAfetadas = await _context.SaveChangesAsync();

                return Ok(linhasAfetadas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Buscar habilidades de um personagem pelo ID
        [HttpGet("{personagemId}")]
        public async Task<IActionResult> GetHabilidadesPersonagem(int personagemId)
        {
            try
            {
                List<PersonagemHabilidade> phLista = await _context.TB_PERSONAGENS_HABILIDADES
                    .Include(p => p.Personagem)
                    .Include(p => p.Habilidade)
                    .Where(p => p.Personagem.Id == personagemId)
                    .ToListAsync();

                return Ok(phLista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Buscar todas as habilidades
        [HttpGet("GetHabilidades")]
        public async Task<IActionResult> GetHabilidades()
        {
            try
            {
                List<Habilidade> habilidades = await _context.TB_HABILIDADES
                    .ToListAsync();

                return Ok(habilidades);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("DeletePersonagemHabilidade")] 
        public async Task<IActionResult> DeleteAsync(PersonagemHabilidade ph) 
        { 
            try 
            { 
               PersonagemHabilidade? phRemover = await _context.TB_PERSONAGENS_HABILIDADES 
                    .FirstOrDefaultAsync(phBusca => phBusca.PersonagemId == ph.PersonagemId 
                     && phBusca.HabilidadeId == ph.HabilidadeId); 
                if(phRemover == null) 
                    throw new System.Exception("Personagem ou Habilidade não encontrados"); 
 
                _context.TB_PERSONAGENS_HABILIDADES.Remove(phRemover); 
                int linhasAfetadas = await _context.SaveChangesAsync(); 
                return Ok(linhasAfetadas); 
            } 
            catch (System.Exception ex) 
            { 
                return BadRequest(ex.Message); 
            } 
        } 
























    }
}

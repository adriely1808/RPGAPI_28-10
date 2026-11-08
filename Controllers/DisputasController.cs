using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models; // Importação da classe Disputa

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisputasController : ControllerBase
    {
        private readonly DataContext _context;

        public DisputasController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("Arma")]
        public async Task<IActionResult> AtaqueComArmaAsync(Disputa d)
        {
            try
            {
                Personagem? atacante = await _context.TB_PERSONAGENS
                .Include(p => p.Arma)
                .FirstOrDefaultAsync(p => p.Id == d.AtacanteId);

                Personagem? oponente = await _context.TB_PERSONAGENS.FirstOrDefaultAsync(p => p.Id == d.OponenteId);

                int Dano = atacante.Arma.Dano + (new Random().Next(atacante.Forca));
                Dano = Dano - new Random().Next(oponente.Defesa);

                if (Dano > 0)
                {
                    oponente.PontosVida = oponente.PontosVida - (int)Dano;
                }

                if (oponente.PontosVida <= 0)
                {
                    d.Narracao = $"{oponente.Nome}foi derrotado!";
                }

                _context.TB_PERSONAGENS.Update(oponente);
                await _context.SaveChangesAsync();

                StringBuilder dados = new StringBuilder();
                dados.AppendFormat("Atacante: {0}.", atacante.Nome);
                dados.AppendFormat(" Oponente: {0}.", oponente.Nome);
                dados.AppendFormat("Pontos de vida do atacante: {0}.", atacante.PontosVida);
                dados.AppendFormat("Pontos de vida do oponente: {0}.", oponente.PontosVida);
                dados.AppendFormat("Arma Utilizada: {0}.", atacante.Arma.Nome);
                dados.AppendFormat("Dano: {0}.", Dano);

                d.Narracao += dados.ToString();
                d.DataDisputa = DateTime.Now;
                _context.TB_DISPUTAS.Add(d);
                _context.SaveChanges();

                return Ok(d);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Habilidade")]
        public async Task<IActionResult> AtaqueComHabilidadeAsync(Disputa d)
        {
            try
            {
                Personagem atacante = await _context.TB_PERSONAGENS
                .Include(p => p.PersonagemHabilidades).ThenInclude(ph => ph.Habilidade)
                .FirstOrDefaultAsync(p => p.Id == d.AtacanteId);

                Personagem oponente = await _context.TB_PERSONAGENS
                .FirstOrDefaultAsync(p => p.Id == d.OponenteId);

                PersonagemHabilidade ph = await _context.TB_PERSONAGENS_HABILIDADES
                .Include(p => p.Habilidade).FirstOrDefaultAsync(phbusca => phbusca.HabilidadeId == d.HabilidadeId
                && phbusca.PersonagemId == d.AtacanteId);

                if (ph == null)
                {
                    d.Narracao = $"{atacante.Nome} não possui esta habilidade";

                    return BadRequest("Atacante não possui esta habilidade");

                }
                    
                else
                {
                    int Dano = ph.Habilidade.Dano + (new Random().Next(atacante.Inteligencia));
                    Dano = Dano - new Random().Next(oponente.Defesa);
                    if (Dano > 0)
                        oponente.PontosVida = oponente.PontosVida - Dano;
                    if (oponente.PontosVida <= 0)
                        d.Narracao += $"{oponente.Nome}foi derrotado!";

                    StringBuilder dados = new StringBuilder();
                    dados.AppendFormat(" Atacante: {0}. ", atacante.Nome);
                    dados.AppendFormat(" Oponente: {0}. ", oponente.Nome);
                    dados.AppendFormat(" Pontos de vida do atacante: {0}. ", atacante.PontosVida);
                    dados.AppendFormat(" Pontos de vida do oponente: {0}. ", oponente.PontosVida);
                    dados.AppendFormat(" Habilidade Utilizada: {0}. ", ph.Habilidade.Nome);
                    dados.AppendFormat(" Dano: {0}. ", Dano);

                    d.Narracao += dados.ToString();
                    d.DataDisputa = DateTime.Now;
                    _context.TB_DISPUTAS.Add(d);
                    _context.SaveChanges();

                    return Ok(d);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("PersonagemRandom")]

        public async Task<IActionResult> sorteio()
        {
            List<Personagem> personagens = 
            await _context.TB_PERSONAGENS.ToListAsync();

            //sorteio com numero de quantidade de personagens
            
            int sorteio = new Random().Next(personagens.Count);

            //busca na lista pelo indice sorteado (não é o id )

            Personagem p = personagens[sorteio];

            string msg = 
            string.Format("N Sorteado {0}. Personagem: {1}", sorteio, p.Nome);

            return Ok(msg);



        }
        




































    }
}
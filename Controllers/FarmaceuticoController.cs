using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SNGPC_B.Api.Data;
using SNGPC_B.Api.Models;

namespace SNGPC_B.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FarmaceuticosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FarmaceuticosController> _logger;

        public FarmaceuticosController(
            AppDbContext context,
            ILogger<FarmaceuticosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ==================== CREATE ====================

        /// <summary>
        /// Cria um novo farmacêutico
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Farmaceutico), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Farmaceutico>> Create([FromBody] Farmaceutico farmaceutico)
        {
            try
            {
                _logger.LogInformation("Criando farmacêutico: {Nome}", farmaceutico.Nome);


                if (string.IsNullOrWhiteSpace(farmaceutico.Nome))
                {
                    return BadRequest(new { message = "Nome é obrigatório" });
                }

                if (string.IsNullOrWhiteSpace(farmaceutico.CRF) || farmaceutico.CRF.Length != 10)
                {
                    return BadRequest(new { message = "CRF deve ter exatamente 10 caracteres" });
                }

                if (string.IsNullOrWhiteSpace(farmaceutico.CRFUF) || farmaceutico.CRFUF.Length != 2)
                {
                    return BadRequest(new { message = "CRFUF deve ter exatamente 2 caracteres (sigla do estado)" });
                }

                if (string.IsNullOrWhiteSpace(farmaceutico.CPF) || farmaceutico.CPF.Length != 11)
                {
                    return BadRequest(new { message = "CPF deve ter exatamente 11 dígitos" });
                }


                var cpfExistente = await _context.Farmaceuticos
                    .AnyAsync(f => f.CPF == farmaceutico.CPF);

                if (cpfExistente)
                {
                    return BadRequest(new { message = $"CPF {farmaceutico.CPF} já está cadastrado" });
                }


                var crfExistente = await _context.Farmaceuticos
                    .AnyAsync(f => f.CRF == farmaceutico.CRF && f.CRFUF == farmaceutico.CRFUF);

                if (crfExistente)
                {
                    return BadRequest(new { message = $"CRF {farmaceutico.CRF}-{farmaceutico.CRFUF} já está cadastrado" });
                }


                _context.Farmaceuticos.Add(farmaceutico);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Farmacêutico criado com sucesso. ID: {Id}", farmaceutico.Id);


                return CreatedAtAction(
                    nameof(GetById),
                    new { id = farmaceutico.Id },
                    farmaceutico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar farmacêutico");
                return StatusCode(500, new { message = "Erro interno ao criar farmacêutico", details = ex.Message });
            }
        }

        // ==================== READ (List) ====================

        /// <summary>
        /// Lista farmacêuticos com filtros e paginação
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetAll(
            [FromQuery] string? nome = null,
            [FromQuery] string? crfuf = null,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            try
            {
                _logger.LogInformation("Listando farmacêuticos. Filtros: Nome={Nome}, CRFUF={CRFUF}, Page={Page}, Size={Size}",
                    nome, crfuf, page, size);

                // Validar paginação
                if (page < 1) page = 1;
                if (size < 1) size = 10;
                if (size > 100) size = 100;

                var query = _context.Farmaceuticos.AsQueryable();


                if (!string.IsNullOrWhiteSpace(nome))
                {
                    query = query.Where(f => f.Nome.Contains(nome));
                }

                if (!string.IsNullOrWhiteSpace(crfuf))
                {
                    query = query.Where(f => f.CRFUF == crfuf.ToUpper());
                }


                var totalCount = await query.CountAsync();


                var farmaceuticos = await query
                    .OrderBy(f => f.Nome)
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToListAsync();

                _logger.LogInformation("Encontrados {Count} farmacêuticos (Total: {Total})",
                    farmaceuticos.Count, totalCount);


                var response = new
                {
                    data = farmaceuticos,
                    pagination = new
                    {
                        page,
                        size,
                        totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / size),
                        hasNext = page * size < totalCount,
                        hasPrevious = page > 1
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar farmacêuticos");
                return StatusCode(500, new { message = "Erro interno ao listar farmacêuticos", details = ex.Message });
            }
        }

        // ==================== READ (GetById) ====================

        /// <summary>
        /// Busca um farmacêutico por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Farmaceutico), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Farmaceutico>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Buscando farmacêutico com ID: {Id}", id);

                var farmaceutico = await _context.Farmaceuticos.FindAsync(id);

                if (farmaceutico == null)
                {
                    _logger.LogWarning("Farmacêutico com ID {Id} não encontrado", id);
                    return NotFound(new { message = $"Farmacêutico com ID {id} não encontrado" });
                }

                return Ok(farmaceutico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar farmacêutico com ID: {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao buscar farmacêutico", details = ex.Message });
            }
        }

        // ==================== UPDATE ====================

        /// <summary>
        /// Atualiza um farmacêutico existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Farmaceutico), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Farmaceutico>> Update(int id, [FromBody] Farmaceutico farmaceutico)
        {
            try
            {
                _logger.LogInformation("Atualizando farmacêutico com ID: {Id}", id);


                if (id != farmaceutico.Id)
                {
                    return BadRequest(new { message = "ID da URL não corresponde ao ID do objeto" });
                }


                var farmaceuticoExistente = await _context.Farmaceuticos.FindAsync(id);

                if (farmaceuticoExistente == null)
                {
                    _logger.LogWarning("Farmacêutico com ID {Id} não encontrado para atualização", id);
                    return NotFound(new { message = $"Farmacêutico com ID {id} não encontrado" });
                }


                if (string.IsNullOrWhiteSpace(farmaceutico.Nome))
                {
                    return BadRequest(new { message = "Nome é obrigatório" });
                }

                if (string.IsNullOrWhiteSpace(farmaceutico.CRF) || farmaceutico.CRF.Length != 10)
                {
                    return BadRequest(new { message = "CRF deve ter exatamente 10 caracteres" });
                }

                if (string.IsNullOrWhiteSpace(farmaceutico.CRFUF) || farmaceutico.CRFUF.Length != 2)
                {
                    return BadRequest(new { message = "CRFUF deve ter exatamente 2 caracteres" });
                }

                if (string.IsNullOrWhiteSpace(farmaceutico.CPF) || farmaceutico.CPF.Length != 11)
                {
                    return BadRequest(new { message = "CPF deve ter exatamente 11 dígitos" });
                }


                var cpfDuplicado = await _context.Farmaceuticos
                    .AnyAsync(f => f.CPF == farmaceutico.CPF && f.Id != id);

                if (cpfDuplicado)
                {
                    return BadRequest(new { message = $"CPF {farmaceutico.CPF} já está cadastrado para outro farmacêutico" });
                }


                var crfDuplicado = await _context.Farmaceuticos
                    .AnyAsync(f => f.CRF == farmaceutico.CRF && f.CRFUF == farmaceutico.CRFUF && f.Id != id);

                if (crfDuplicado)
                {
                    return BadRequest(new { message = $"CRF {farmaceutico.CRF}-{farmaceutico.CRFUF} já está cadastrado para outro farmacêutico" });
                }


                farmaceuticoExistente.Nome = farmaceutico.Nome;
                farmaceuticoExistente.CRF = farmaceutico.CRF;
                farmaceuticoExistente.CRFUF = farmaceutico.CRFUF;
                farmaceuticoExistente.CRFDataEmissao = farmaceutico.CRFDataEmissao;
                farmaceuticoExistente.CPF = farmaceutico.CPF;
                farmaceuticoExistente.LoginANVISA = farmaceutico.LoginANVISA;
                farmaceuticoExistente.SenhaANVISA = farmaceutico.SenhaANVISA;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Farmacêutico {Id} atualizado com sucesso", id);

                return Ok(farmaceuticoExistente);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Erro de concorrência ao atualizar farmacêutico {Id}", id);
                return StatusCode(409, new { message = "O registro foi modificado por outro usuário. Recarregue e tente novamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar farmacêutico {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao atualizar farmacêutico", details = ex.Message });
            }
        }

        // ==================== DELETE ====================

        /// <summary>
        /// Exclui um farmacêutico
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deletando farmacêutico com ID: {Id}", id);

                var farmaceutico = await _context.Farmaceuticos.FindAsync(id);

                if (farmaceutico == null)
                {
                    _logger.LogWarning("Farmacêutico com ID {Id} não encontrado para exclusão", id);
                    return NotFound(new { message = $"Farmacêutico com ID {id} não encontrado" });
                }

                _context.Farmaceuticos.Remove(farmaceutico);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Farmacêutico {Id} deletado com sucesso", id);

                return NoContent(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar farmacêutico {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao deletar farmacêutico", details = ex.Message });
            }
        }
    }
}
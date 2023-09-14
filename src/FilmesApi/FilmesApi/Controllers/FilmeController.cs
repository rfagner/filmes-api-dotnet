using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.DTOs;
using FilmesApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilmeController : ControllerBase
    {
        private readonly FilmeContext _context;
        private readonly IMapper _mapper;

        public FilmeController(FilmeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Adiciona um filme ao banco de dados
        /// </summary>
        /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
        /// <returns>IActionResult</returns>
        /// <response code="201">Caso inserção seja feita com sucesso</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)
        {
            Filme filme = _mapper.Map<Filme>(filmeDto);
            _context.Filmes.Add(filme);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaFilmePorId),
                new { id = filme.Id },
                filme);
        }

        /// <summary>
        /// Recupera uma lista paginada de filmes do banco de dados.
        /// </summary>
        /// <param name="skip">O número de filmes a serem ignorados antes de começar a listar.</param>
        /// <param name="take">O número máximo de filmes a serem listados.</param>
        /// <returns>Uma coleção de filmes paginada.</returns>
        /// <response code="200">Retorna um código 200 (OK) com a lista paginada de filmes.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<ReadFilmeDto> RecuperaFilmes([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take));
        }


        /// <summary>
        /// Recupera um filme pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do filme a ser recuperado.</param>
        /// <returns>Um filme com o ID especificado.</returns>
        /// <response code="200">Retorna um código 200 (OK) com o filme correspondente ao ID.</response>
        /// <response code="404">Retorna um código 404 (Not Found) se o filme não for encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult RecuperaFilmePorId(int id)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if (filme == null)
                return NotFound();
            var filmeDto = _mapper.Map<ReadFilmeDto>(filme);

            return Ok(filmeDto);
        }


        /// <summary>
        /// Atualiza um filme pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do filme a ser atualizado.</param>
        /// <param name="filmeDto">Objeto com os campos a serem atualizados no filme.</param>
        /// <returns>Um código 204 (No Content) indicando que a atualização foi bem-sucedida.</returns>
        /// <response code="204">Retorna um código 204 (No Content) se a atualização for bem-sucedida.</response>
        /// <response code="404">Retorna um código 404 (Not Found) se o filme não for encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if (filme == null)
                return NotFound();

            _mapper.Map(filmeDto, filme);
            _context.SaveChanges();
            return NoContent();
        }


        /// <summary>
        /// Atualiza parcialmente um filme pelo seu ID usando o método PATCH.
        /// </summary>
        /// <param name="id">O ID do filme a ser atualizado.</param>
        /// <param name="patch">Um objeto JSON contendo as atualizações parciais a serem aplicadas ao filme.</param>
        /// <returns>Um código 204 (No Content) indicando que a atualização parcial foi bem-sucedida.</returns>
        /// <response code="204">Retorna um código 204 (No Content) se a atualização parcial for bem-sucedida.</response>
        /// <response code="404">Retorna um código 404 (Not Found) se o filme não for encontrado.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> patch)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if (filme == null)
                return NotFound();

            var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

            patch.ApplyTo(filmeParaAtualizar, ModelState);

            if (!TryValidateModel(filmeParaAtualizar))
            {
                return ValidationProblem(ModelState);
            }


            _mapper.Map(filmeParaAtualizar, filme);
            _context.SaveChanges();
            return NoContent();
        }


        /// <summary>
        /// Exclui um filme pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do filme a ser excluído.</param>
        /// <returns>Um código 204 (No Content) indicando que a exclusão foi bem-sucedida.</returns>
        /// <response code="204">Retorna um código 204 (No Content) se a exclusão for bem-sucedida.</response>
        /// <response code="404">Retorna um código 404 (Not Found) se o filme não for encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeletaFilme(int id)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if (filme == null)
                return NotFound();

            _context.Remove(filme);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

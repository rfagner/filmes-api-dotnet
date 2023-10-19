﻿using System.ComponentModel.DataAnnotations;

namespace FilmesApi.Data.DTOs
{
    public class UpdateCinemaDto
    {       
        [Required(ErrorMessage = "O campo nome é obrigatório")]       
        public string Nome { get; set; }
    }
}

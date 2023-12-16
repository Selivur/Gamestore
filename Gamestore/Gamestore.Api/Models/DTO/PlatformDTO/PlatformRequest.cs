﻿using System.ComponentModel.DataAnnotations;

namespace Gamestore.Api.Models.DTO.PlatformDTO;

public class PlatformRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Type is required")]
    public string Type { get; set; }
}

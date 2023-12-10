using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TibiaMvc.Models.Enums;

namespace TibiaMvc.Models
{
    public class CharacterViewModel
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public string Name { get; set; } = string.Empty;
        public GenderEnum Gender { get; set; }
        public VocationEnum Vocation { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime AcessDate { get; set; }
        public SkillViewModel Skills { get; set; }
    }
}
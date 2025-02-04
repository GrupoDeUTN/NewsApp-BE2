using NewsApp.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;

namespace NewsApp.Themes
{
    public class ThemeEntidad : Entity<int>
    {
        public string Name  { get; set; }
        public IdentityUser User { get; set; }
        public ICollection<ThemeEntidad> Themes { get; set;}
        public int? ThemeId { get; set; }  // This represents the parent theme's ID

        public ICollection<NewsEntidad> listNews { get; set; } 

        public ThemeEntidad()
        {
            this.Themes = new List<ThemeEntidad>();
            this.listNews = new List<NewsEntidad>();

        }

        public Guid UserId { get; set; } // Propiedad de clave externa



    }
}

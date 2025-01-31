using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;

namespace NewsApp.News
{
    public class NewsEntidad : Entity<int>
    {
        public string? Author { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Url { get; set; }

        public string? UrlToImage { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string? Content { get; set; }

        public int ThemeId { get; set; } //Tema al que pertenece la noticia
    }
}
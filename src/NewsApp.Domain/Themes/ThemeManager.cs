﻿using Microsoft.AspNetCore.Identity;
using NewsApp.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace NewsApp.Themes
{
    public class ThemeManager : DomainService, IThemeManager
    {
        private readonly IRepository<ThemeEntidad, int> _repository;
        private readonly IRepository<NewsEntidad, int> _repositoryNew;
        public ThemeManager(IRepository<ThemeEntidad, int> repository, IRepository<NewsEntidad, int> repositoryNew)
        {
            _repository = repository;
            _repositoryNew = repositoryNew;
        }

        public async Task<ThemeEntidad> CreateAsyncOrUpdate(int? id, string name, int? parentId, Volo.Abp.Identity.IdentityUser identityUser)
        {
            ThemeEntidad theme = null;            

            if (id is not null)
            {
                // Si el id no es nulo significa que se modifica el tema, además si el tema a modificar es un tema hijo va el ID y en parentID es null
                theme = await _repository.GetAsync(id.Value, includeDetails: true);

                theme.Name = name;
            }
            else
            {
                //Si el id es nulo, es un tema nuevo
                theme = new ThemeEntidad { Name = name, User = identityUser };

                if (parentId is not null)
                {
                    // Si el parent id no es nulo, es un tema hijo de un tema padre.
                    var parentTheme = await _repository.GetAsync(parentId.Value, includeDetails: true);
                    parentTheme.Themes.Add(theme);
                }               
            };

            return theme;
        }


        //public async Task AddNewAsync(int idNoticia, Theme theme)
        //{
        //    if (theme == null)
        //    {
        //        throw new ArgumentNullException(nameof(theme), "El tema no puede ser nulo.");
        //    }

        //    // Obtener la noticia desde el repositorio usando el id
        //    var newsEntidad = await _repositoryNew.GetAsync(idNoticia, includeDetails: true);

        //    if (newsEntidad == null)
        //    {
        //        throw new ArgumentException("La noticia especificada no existe.", nameof(idNoticia));
        //    }

        //    // Agregar la noticia al tema
        //    theme.listNews.Add(newsEntidad);
       
        // }



        //Elimina todo las noticias y capertas de un tema, luego se elimna el mismo
        public async Task DeleteThemeRecursively(ThemeEntidad theme)
        {
            // Primero, eliminar todas las noticias asociadas al tema actual
            if (theme.listNews != null && theme.listNews.Any())
            {
                await DeleteNewsFromTheme(theme);
            }

            // Luego, proceder con la eliminación recursiva de los temas hijos
            if (theme.Themes != null && theme.Themes.Any())
            {
                foreach (var childTheme in theme.Themes.ToList())
                {
                    await DeleteThemeRecursively(childTheme);
                }
            }

            // Finalmente, eliminar el tema
            await _repository.DeleteAsync(theme, autoSave: true);
        }


        //Se encarga de eliminar todos las noticias de un tema
        public async Task DeleteNewsFromTheme(ThemeEntidad theme)
        {

            if (theme.listNews != null && theme.listNews.Any())
            {
                foreach (var news in theme.listNews.ToList())
                {

                    theme.listNews.Remove(news);

                }
            }

            // Actualizar el tema en el repositorio para reflejar los cambios
            await _repository.UpdateAsync(theme, autoSave: true);
        }
    }

}

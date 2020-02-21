using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }


        public async Task<IEnumerable<TagDomainModel>> GetAllTags()
        {
            //var data = _moviesRepository.GetCurrentMovies();
            var data = await _tagRepository.GetAll();


            if (data == null)
            {
                return null;
            }

            List<TagDomainModel> result = new List<TagDomainModel>();
            TagDomainModel model;
            foreach (var item in data)
            {
                model = new TagDomainModel
                {
                    Id = item.Id,
                    TagContent = item.TagContent
                };
                result.Add(model);
            }

            //List<TagDomainModel> topTenResults = result.OrderByDescending(x => x.Rating).Take(10).ToList();

            return result;

        }


    }
}

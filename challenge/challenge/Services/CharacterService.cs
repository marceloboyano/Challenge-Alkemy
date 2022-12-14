
using AutoMapper;
using challenge.QueryFilters;
using DataBase;
using DataBase.Repositories;
using static challenge.DTOs.Personajes.CharacterDto;
using static challenge.Services.ImageService;

namespace challenge.Services
{
    public class CharacterService : ICharacterService
    {

        private readonly ICharactersRepository _repo;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public CharacterService(ICharactersRepository repo, IMapper mapper, IImageService imageService)
            {
                _repo = repo;
                _mapper = mapper;
                _imageService = imageService;
            }

      
        public async Task<IEnumerable<Character>> GetCharacters(CharacterQueryFilter filters)
            {

                 var character = _repo.GetCharacterWithDetails();     


            if (filters.Name != null)
                {
                    character = character.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));
                }

                if (filters.MovieID != null)
                {

                    character = character.Where(x => x.Movies.Any(g => g.MoviesID == filters.MovieID));
                }

                if (filters.Age != null)
                {
                character = character.Where(x => x.Age == filters.Age);
                   
                }
              
            return await Task.FromResult(character);

            }

        public async Task InsertCharacters(CharacterForCreationDTO characterDTO)
        {
            string path = "";

            if (characterDTO.ImageFile is not null)
                path = await _imageService.StoreImage(characterDTO.ImageFile, ImageType.Character);

            var character = _mapper.Map<Character>(characterDTO);
            character.Image = path;

            await _repo.Create(character);    
        }          
      
        public async Task<bool> UpdateCharacters(int id, CharacterForUpdateDTO characterDTO)
        {
            var characterEntity = await _repo.GetById(id);

            if (characterEntity is null) return false;

            if(characterDTO.Name is not null)
            {
                characterEntity.Name = characterDTO.Name;
            }

            if (characterDTO.Age is not null)
            {
                characterEntity.Age = characterDTO.Age.Value;
            }

            if (characterDTO.Weight is not null)
            {
                characterEntity.Weight = characterDTO.Weight.Value;
            }

            if (characterDTO.History is not null)
            {
                characterEntity.History = characterDTO.History;
            }

            if (characterDTO.ImageFile is not null)
            {
                var path = await _imageService.StoreImage(characterDTO.ImageFile, ImageType.Character);

                characterEntity.Image = path;
            }


            return await _repo.Update(characterEntity);

        }


        public async Task<bool> DeleteCharacters(int id)
        {
         
           return  await _repo.Delete(id);

        }

        public async Task<CharacterForShowWithDetailsDTO> GetCharacterById(int id)
        {
            var character = await _repo.GetByIdWithDetail(id);

            var movieForShowWithDetails = _mapper.Map<CharacterForShowWithDetailsDTO>(character);

            return movieForShowWithDetails;
        }


    }
}


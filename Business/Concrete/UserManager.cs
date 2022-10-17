using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Core.Utilities.Hashing;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Concrete
{
    public class UserManager:IUserService
    {
        private readonly IUserDal _userDal;
        private readonly IFileService _fileService;

        public UserManager(IUserDal userDal, IFileService fileService)
        {
            _userDal = userDal;
            _fileService = fileService;
        }

        public void Add(RegisterAuthDto authDto)
        {

            string fileName = _fileService.FileSave(authDto.Image, "./Content/img/");



            var user = CreateUser(authDto, fileName);
             _userDal.Add(user);
        }

        private User CreateUser (RegisterAuthDto authDto, string fileName)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePassword(authDto.Password, out passwordHash, out passwordSalt);

            User user = new User();
            user.Id = 0;
            user.Email = authDto.Email;
            user.Name = authDto.Name;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.ImageUrl = fileName;
            return user;
        }

        public List<User> GetList()
        {
            return _userDal.GetAll();
        }

        public User GetByEmail(string email)
        {
            var result= _userDal.Get(p => p.Email == email);
            return result;
        }
    }
}

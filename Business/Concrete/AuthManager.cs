using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Business;
using Core.Utilities.Hashing;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using Entities.Dtos;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;

        public AuthManager(IUserService userService)
        {
            _userService = userService;
        }

        [ValidationAspect(typeof(UserValidator))] //işlemcen önce çalış
                                                  // [LogAspect] //işlem bittikten sonra çalış
        public IResult Register(RegisterAuthDto registerAuthDto)
        {
            #region MyRegion
            //Cross cutting Concerns - Uykulama Dikine Kesme
            //Fluent Validation
            //UserValidator userValidator=new UserValidator();
            //ValidationResult validationResult=userValidator.Validate(registerAuthDto);

            //List<string> results = new List<string>();
            //if (validationResult.IsValid)
            //{
            //    _userService.Add(registerAuthDto);
            //    results.Add("Kullanıcı kaydı başarıyla oluşturuldu");
            //    return results;
            //}
            //results = validationResult.Errors.Select(p=>p.ErrorMessage).ToList();


            #endregion

           

            //UserValidator userValidator=new UserValidator();
            // ValidationTool.Validate(userValidator,registerAuthDto);

            IResult result = BusinessRules.Run(
                CheckIfEmailExists(registerAuthDto.Email),
                CheckIfImageExtensionsAllow(registerAuthDto.Image.FileName),
                CheckIfImageIsLessThanOneMb(registerAuthDto.Image.Length)
                );

            if (result != null)
            {
                return result;
            }



            _userService.Add(registerAuthDto);
            return new SuccessResult("Kullanıcı kaydı başarıyla oluşturuldu.");
        }

        public string Login(LoginAuthDto loginAuthDto)
        {
            var user = _userService.GetByEmail(loginAuthDto.Email);
            var result = HashingHelper.VerifyPasswordHash(loginAuthDto.Password, user.PasswordHash, user.PasswordSalt);
            if (result)
            {
                return "Kullanıcı girişi başarılı";
            }

            return "Kullanıcı bilgileri hatalı";
        }
        private IResult CheckIfEmailExists(string email)
        {
            var list = _userService.GetByEmail(email);
            if (list != null)
            {
                return new ErrorResult("Bu mail adresi daha önce kullanılmış.");

            }

            return new SuccessResult("Kullanıcı kaydı başarıyla oluşturuldu.");
        }

        private IResult CheckIfImageIsLessThanOneMb(long imgSize)
        {
            decimal imgMbSize = Convert.ToDecimal(imgSize * 0.000001);
            if (imgMbSize > 1)
            {
                return new ErrorResult("Yüklediğiniz resmin boyutu en fazla 1 mb olmalıdır.");
            }

            return new SuccessResult();
        }

        private IResult CheckIfImageExtensionsAllow(string fileName)
        {

            var ext = fileName.Substring(fileName.LastIndexOf('.'));
            var extension = ext.ToLower();
            List<string> AllowFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png" };
            if (!AllowFileExtensions.Contains(extension))
            {
                return new ErrorResult("Eklediğiniz resim formata uygun değil !");
            }

            return new SuccessResult();
        }

    }
}

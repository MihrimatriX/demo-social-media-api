using FluentValidation;
using DemoSocialMedia.Application.Auth.DTOs;

namespace DemoSocialMedia.Application.Auth.Validators
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad alanı zorunludur.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad alanı zorunludur.");
            RuleFor(x => x.Nickname)
                .NotEmpty().WithMessage("Kullanıcı adı zorunludur.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre zorunludur.")
                .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
                .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
                .Matches(@"[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
                .Matches(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]").WithMessage("Şifre en az bir özel karakter içermelidir.");
            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Doğum tarihi zorunludur.")
                .Must(date => BeAValidDate(date.ToString())).WithMessage("Geçerli bir doğum tarihi giriniz.");
            RuleFor(x => x.IsAgreedKvkk)
                .Equal(true).WithMessage("KVKK metnini onaylamalısınız.");
            RuleFor(x => x.IsAgreedConsent)
                .Equal(true).WithMessage("Açık rıza metnini onaylamalısınız.");
        }

        private bool BeAValidDate(string? date)
        {
            if (string.IsNullOrWhiteSpace(date)) return false;
            return DateTime.TryParse(date, out _);
        }
    }
}
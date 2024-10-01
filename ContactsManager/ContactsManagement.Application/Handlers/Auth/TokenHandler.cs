﻿using ContactsManagement.Application.DTOs.Auth;
using ContactsManagement.Application.Interfaces.Auth;
using ContactsManagement.Application.Interfaces.User.ValidateUser;
using ContactsManagement.Infrastructure.Crypto;
using ContactsManagement.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ContactsManagement.Application.Handlers.Auth
{
    public class TokenHandler : ITokenHandler
    {
        private readonly ICryptoService _cryptoService;
        private readonly IValidateUserHandler _validateUserHandler;
        private readonly JwtSettings _jwtSettings;

        public TokenHandler(IValidateUserHandler userHandler, IOptions<JwtSettings> jwtSettings, ICryptoService cryptoService)
        {
            _validateUserHandler = userHandler;
            _jwtSettings = jwtSettings.Value;
            _cryptoService = cryptoService;
        }

        public async Task<LoginResponse> HandleAsync(LoginRequest request)
        {
            var response = new LoginResponse();
            var user = await _validateUserHandler.HandleAsync(request);

            if (user is null)
            {
                response.Success = false;
                response.Token = string.Empty;
            }
            else
            {
                var decryptedKey = _cryptoService.Decrypt(_jwtSettings.SecretKey);
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityKey = Encoding.ASCII.GetBytes(decryptedKey);
                var tokenProperties = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.UserType.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpiresInMinutes),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(securityKey),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenProperties);

                response.Success = true;
                response.Token = tokenHandler.WriteToken(token);
            }

            return response;
        }
    }
}
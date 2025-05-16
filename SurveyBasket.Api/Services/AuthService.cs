using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Authentication;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager,
    IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private int refreshTokenExpiresIn = 14;

   
    public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        //Check email 
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return null;
        }
        //Check Password
        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!isValidPassword)
        {
            return null;
        }

        //Generate JWT Token 
        var (token, expiresIn) = _jwtProvider.GenerateToken(user);


        //Genereate Refresh Token
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpires = DateTime.UtcNow.AddDays(refreshTokenExpiresIn);

        user.RefreshTokens.Add(new RefreshToken { Token = refreshToken, ExpiredOn = refreshTokenExpires });

        await _userManager.UpdateAsync(user);


        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpires);

    }

   
    public async Task<AuthResponse> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId == null) { return null; }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) { return null; }

        var userRefreshToken =  user.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken && r.IsActive);

        if (userRefreshToken == null) { return null; }
        userRefreshToken.RevokedOn = DateTime.UtcNow;


        //Generate JWT Token 
        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);


        //Genereate Refresh Token
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpires = DateTime.UtcNow.AddDays(refreshTokenExpiresIn);

        user.RefreshTokens.Add(new RefreshToken { Token = newRefreshToken, ExpiredOn = refreshTokenExpires });

        await _userManager.UpdateAsync(user);


        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpires);

    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId == null) 
            return false; 

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return false;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken && r.IsActive);

        if (userRefreshToken == null) 
             return false; 

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
       
        return true;
    }
    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

}

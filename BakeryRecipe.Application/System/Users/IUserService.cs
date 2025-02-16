﻿using BakeryRecipe.Application.ClaimTokens;
using BakeryRecipe.ViewModels.Pagination;
using BakeryRecipe.ViewModels.Response;
using BakeryRecipe.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BakeryRecipe.Application.System.Users
{
    public interface IUserService
    {
        Task<Token> Login(LoginRequestDTO request);
        Task<BaseResponse<string>> RefreshToken(RefreshTokenResponse refreshToken);
        Task<RegisterResponseDTO> Register(RegisterRequestDTO request);
        Task<bool> SendEmail(string email,string code);

        Task<BaseResponse<string>> ChangePassword(Guid id,string currentPass,string newPass);
        Task<bool> ForgotPassword(string email, string newPass);
        Task<UserDTO> UpdateProfile(UpdateUserRequest request,Guid userID);
        Task<ListUserResponse> GetUserList(PaginationFilter filter);
        Task<UserDTO> GetUser(Guid userId);
        Task<ProfileResponseDTO> GetProfile(RefreshToken refreshToken);

    }
}

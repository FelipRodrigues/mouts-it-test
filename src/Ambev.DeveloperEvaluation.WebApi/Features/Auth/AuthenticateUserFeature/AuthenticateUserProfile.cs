using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;

/// <summary>
/// AutoMapper profile for authentication-related mappings
/// </summary>
public sealed class AuthenticateUserProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticateUserProfile"/> class
    /// </summary>
    public AuthenticateUserProfile()
    {
        CreateMap<User, AuthenticateUserResponse>()
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        // Adicionando o mapeamento necessário para autenticação
        CreateMap<
            AuthenticateUserRequest,
            Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser.AuthenticateUserCommand
        >();

        // Adicionando o mapeamento de resultado para resposta
        CreateMap<
            Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser.AuthenticateUserResult,
            AuthenticateUserResponse
        >();
    }
}

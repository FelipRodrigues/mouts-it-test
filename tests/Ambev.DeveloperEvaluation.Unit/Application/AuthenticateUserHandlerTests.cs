using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class AuthenticateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly AuthenticateUserHandler _handler;

    public AuthenticateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        _handler = new AuthenticateUserHandler(
            _userRepository,
            _passwordHasher,
            _jwtTokenGenerator
        );
    }

    [Fact(DisplayName = "Given valid credentials When authenticating Then returns token")]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var command = new AuthenticateUserCommand { Email = "user@email.com", Password = "123456" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            Password = "hashedPassword",
            Username = "TestUser",
            Status = UserStatus.Active,
            Role = UserRole.Customer,
        };
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password).Returns(true);
        _jwtTokenGenerator.GenerateToken(user).Returns("jwt-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("jwt-token");
        result.Email.Should().Be(user.Email);
        result.Name.Should().Be(user.Username);
        result.Role.Should().Be(user.Role.ToString());
    }

    [Fact(
        DisplayName = "Given invalid password When authenticating Then throws UnauthorizedAccessException"
    )]
    public async Task Handle_InvalidPassword_ThrowsUnauthorized()
    {
        // Arrange
        var command = new AuthenticateUserCommand { Email = "user@email.com", Password = "wrong" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            Password = "hashedPassword",
            Username = "TestUser",
            Status = UserStatus.Active,
            Role = UserRole.Customer,
        };
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password).Returns(false);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact(
        DisplayName = "Given non-existent user When authenticating Then throws UnauthorizedAccessException"
    )]
    public async Task Handle_UserNotFound_ThrowsUnauthorized()
    {
        // Arrange
        var command = new AuthenticateUserCommand
        {
            Email = "notfound@email.com",
            Password = "123456",
        };
        _userRepository
            .GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact(
        DisplayName = "Given inactive user When authenticating Then throws UnauthorizedAccessException"
    )]
    public async Task Handle_InactiveUser_ThrowsUnauthorized()
    {
        // Arrange
        var command = new AuthenticateUserCommand { Email = "user@email.com", Password = "123456" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            Password = "hashedPassword",
            Username = "TestUser",
            Status = UserStatus.Inactive,
            Role = UserRole.Customer,
        };
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password).Returns(true);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User is not active");
    }
}

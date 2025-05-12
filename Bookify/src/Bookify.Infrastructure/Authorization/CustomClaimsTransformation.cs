using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure.Authorization;
//用于动态添加角色信息等声明（Claims）
internal sealed class CustomClaimsTransformation : IClaimsTransformation
{
    private readonly IServiceProvider _serviceProvider;

    public CustomClaimsTransformation(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 用于对当前登录用户的 ClaimsPrincipal 进行修改
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // 如果用户已经认证，并且已经有了角色和 Sub 声明，则不需要再进行 ClaimsTransformation
        if (principal.Identity is not { IsAuthenticated: true } ||
            principal.HasClaim(claim => claim.Type == ClaimTypes.Role) &&
            principal.HasClaim(claim => claim.Type == JwtRegisteredClaimNames.Sub))
        {
            return principal;
        }

        //创建一个作用域（scope），获取 AuthorizationService 实例，用于获取当前用户的角色信息。
        using IServiceScope scope = _serviceProvider.CreateScope();

        AuthorizationService authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();
        //从 Claims 中提取当前用户 ID
        string identityId = principal.GetIdentityId();
        //调用授权服务，获取当前用户的所有角色。
        UserRolesResponse userRoles = await authorizationService.GetRolesForUserAsync(identityId);
        //创建一个新的 Identity 并添加 sub（用户标识）Claim
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, userRoles.UserId.ToString()));
        //添加角色 Claims
        foreach (Role role in userRoles.Roles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
        }
        ///把新 Identity 添加到原始 ClaimsPrincipal 中，返回修改后的用户信息
        principal.AddIdentity(claimsIdentity);

        return principal;
    }
}

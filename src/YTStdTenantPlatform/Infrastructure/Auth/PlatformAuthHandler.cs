using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using YTStdLogger.Core;
using YTStdTenantPlatform.Infrastructure.Cache;

namespace YTStdTenantPlatform.Infrastructure.Auth
{
    /// <summary>平台认证处理器，负责从请求中解析并验证用户身份</summary>
    public static class PlatformAuthHandler
    {
        /// <summary>Authorization 请求头名称</summary>
        private const string AuthorizationHeader = "Authorization";

        /// <summary>Bearer Token 前缀</summary>
        private const string BearerPrefix = "Bearer ";

        /// <summary>Token 签名密钥（必须在启动时通过配置设置）</summary>
        private static volatile string _tokenSecret = string.Empty;

        /// <summary>Token 有效时长（秒）</summary>
        private static volatile int _tokenExpirySeconds = 7200;

        /// <summary>设置 Token 签名密钥</summary>
        public static void SetTokenSecret(string secret)
        {
            _tokenSecret = secret;
        }

        /// <summary>设置 Token 有效时长（秒）</summary>
        public static void SetTokenExpiry(int seconds)
        {
            _tokenExpirySeconds = seconds;
        }

        /// <summary>
        /// 从请求中尝试解析当前用户。
        /// 成功时返回 CurrentUser 实例，失败时返回 null。
        /// </summary>
        public static CurrentUser? TryResolveUser(HttpContext context)
        {
            var authHeader = context.Request.Headers[AuthorizationHeader].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var token = authHeader.Substring(BearerPrefix.Length).Trim();
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            // 解析 Token 为用户信息
            return TryResolveToken(token, context.TraceIdentifier);
        }

        /// <summary>从原始 Token 解析当前用户</summary>
        public static CurrentUser? TryResolveToken(string token, string traceId)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            return ParseToken(token, traceId);
        }

        /// <summary>
        /// 生成简易 Token（userId:timestamp:signature）。
        /// 生产环境应替换为标准 JWT 或等效安全方案。
        /// </summary>
        public static string GenerateToken(long userId, string username)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var payload = userId + ":" + username + ":" + timestamp;
            var signature = ComputeHmac(payload);
            return payload + ":" + signature;
        }

        /// <summary>获取当前 Token 有效期（秒）</summary>
        public static int GetTokenExpirySeconds()
        {
            return _tokenExpirySeconds;
        }

        /// <summary>解析 Token，验证签名和有效期</summary>
        private static CurrentUser? ParseToken(string token, string traceId)
        {
            // Token 格式: userId:username:timestamp:signature
            var parts = token.Split(':');
            if (parts.Length != 4)
            {
                Logger.Debug(0, 0, () => "[PlatformAuthHandler] Token 格式无效");
                return null;
            }

            if (!long.TryParse(parts[0], out var userId))
            {
                Logger.Debug(0, 0, () => "[PlatformAuthHandler] Token 中 UserId 无效");
                return null;
            }

            var username = parts[1];

            if (!long.TryParse(parts[2], out var timestamp))
            {
                Logger.Debug(0, 0, () => "[PlatformAuthHandler] Token 中时间戳无效");
                return null;
            }

            // 验证签名
            var payload = userId + ":" + username + ":" + timestamp;
            var expectedSignature = ComputeHmac(payload);
            if (!string.Equals(parts[3], expectedSignature, StringComparison.Ordinal))
            {
                Logger.Debug(0, 0, () => "[PlatformAuthHandler] Token 签名校验失败");
                return null;
            }

            // 验证有效期
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now - timestamp > _tokenExpirySeconds)
            {
                Logger.Debug(0, 0, () => "[PlatformAuthHandler] Token 已过期");
                return null;
            }

            // 从缓存获取用户角色
            var roles = GetUserRoles(userId);
            var permissions = GetUserPermissions(userId, roles);
            var isSuperAdmin = false;
            for (int i = 0; i < roles.Count; i++)
            {
                if (string.Equals(roles[i], "super_admin", StringComparison.OrdinalIgnoreCase))
                {
                    isSuperAdmin = true;
                    break;
                }
            }

            return new CurrentUser(userId, username, username, roles, permissions, isSuperAdmin, traceId);
        }

        /// <summary>从缓存获取用户角色列表</summary>
        private static IReadOnlyList<string> GetUserRoles(long userId)
        {
            var cache = PlatformCacheWarmer.UserRoleCache;
            if (cache.TryGetValue(userId, out var roles))
            {
                return roles;
            }
            return Array.Empty<string>();
        }

        /// <summary>从缓存获取用户所有权限编码</summary>
        private static IReadOnlyList<string> GetUserPermissions(long userId, IReadOnlyList<string> roles)
        {
            var rolePermCache = PlatformCacheWarmer.RoleCodePermissionCache;
            if (rolePermCache.Count == 0 || roles.Count == 0)
            {
                return Array.Empty<string>();
            }

            var permSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < roles.Count; i++)
            {
                var roleCode = roles[i];
                if (!rolePermCache.TryGetValue(roleCode, out var permissions))
                {
                    continue;
                }

                for (int j = 0; j < permissions.Count; j++)
                {
                    permSet.Add(permissions[j]);
                }
            }

            if (permSet.Count == 0)
            {
                return Array.Empty<string>();
            }

            var list = new List<string>(permSet.Count);
            foreach (var permission in permSet)
            {
                list.Add(permission);
            }

            return list;
        }

        /// <summary>计算 HMAC-SHA256 签名</summary>
        private static string ComputeHmac(string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_tokenSecret);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var hash = HMACSHA256.HashData(keyBytes, dataBytes);
            return Convert.ToHexString(hash);
        }
    }
}

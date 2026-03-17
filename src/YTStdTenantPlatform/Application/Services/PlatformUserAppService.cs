using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using YTStdLogger.Core;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Cache;

namespace YTStdTenantPlatform.Application.Services
{
    /// <summary>平台用户应用服务</summary>
    public static class PlatformUserAppService
    {
        /// <summary>获取用户分页列表</summary>
        public static async ValueTask<PagedResult<PlatformUserDto>> GetListAsync(
            int tenantId, long operatorId, PagedRequest request)
        {
            var (queryResult, data) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
            if (!queryResult.Success || data == null)
                return new PagedResult<PlatformUserDto> { Page = request.NormalizedPage, PageSize = request.NormalizedPageSize };

            var filtered = FilterUsers(data, request);
            var paged = Paginate(filtered, request);
            var items = new List<PlatformUserDto>(paged.Count);
            foreach (var u in paged)
                items.Add(MapToDto(u));

            return new PagedResult<PlatformUserDto>
            {
                Items = items,
                Total = filtered.Count,
                Page = request.NormalizedPage,
                PageSize = request.NormalizedPageSize
            };
        }

        /// <summary>获取用户详情</summary>
        public static async ValueTask<PlatformUserDto?> GetByIdAsync(int tenantId, long operatorId, long id)
        {
            var (queryResult, data) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
            if (!queryResult.Success || data == null) return null;

            foreach (var u in data)
            {
                if (u.Id == id && u.DeletedAt == null)
                    return MapToDto(u);
            }
            return null;
        }

        /// <summary>创建平台用户</summary>
        public static async ValueTask<ApiResult<long>> CreateAsync(
            int tenantId, long operatorId, CreatePlatformUserRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username))
                return ApiResult<long>.Fail("用户名不能为空");
            if (string.IsNullOrWhiteSpace(req.Email))
                return ApiResult<long>.Fail("邮箱不能为空");
            if (string.IsNullOrWhiteSpace(req.Password))
                return ApiResult<long>.Fail("密码不能为空");

            var salt = GenerateSalt();
            var hash = HashPassword(req.Password, salt);
            var now = DateTime.UtcNow;

            var user = new PlatformUser
            {
                Username = req.Username.Trim(),
                Email = req.Email.Trim(),
                Phone = req.Phone,
                DisplayName = req.DisplayName,
                PasswordHash = hash,
                PasswordSalt = salt,
                Status = "active",
                MfaEnabled = false,
                Remark = req.Remark,
                CreatedAt = now,
                UpdatedAt = now
            };

            var insResult = await PlatformUserCRUD.InsertAsync(tenantId, operatorId, user);
            if (!insResult.Success)
                return ApiResult<long>.Fail("创建用户失败: " + insResult.ErrorMessage);

            Logger.Info(tenantId, operatorId, "[PlatformUserAppService] 创建用户: " + req.Username);
            return ApiResult<long>.Ok(insResult.Id);
        }

        /// <summary>更新平台用户</summary>
        public static async ValueTask<ApiResult> UpdateAsync(
            int tenantId, long operatorId, long id, UpdatePlatformUserRequest req)
        {
            var (queryResult, allUsers) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
            if (!queryResult.Success || allUsers == null)
                return ApiResult.Fail("查询用户失败");

            PlatformUser? target = null;
            foreach (var u in allUsers)
            {
                if (u.Id == id && u.DeletedAt == null) { target = u; break; }
            }
            if (target == null) return ApiResult.Fail("用户不存在");

            if (req.DisplayName != null) target.DisplayName = req.DisplayName;
            if (req.Phone != null) target.Phone = req.Phone;
            if (req.Email != null) target.Email = req.Email;
            if (req.Remark != null) target.Remark = req.Remark;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await PlatformUserCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success)
                return ApiResult.Fail("更新用户失败: " + updResult.ErrorMessage);

            Logger.Info(tenantId, operatorId, "[PlatformUserAppService] 更新用户: " + target.Username);
            return ApiResult.Ok();
        }

        /// <summary>启用/禁用用户</summary>
        public static async ValueTask<ApiResult> SetStatusAsync(
            int tenantId, long operatorId, long id, string status)
        {
            var (queryResult, allUsers) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
            if (!queryResult.Success || allUsers == null)
                return ApiResult.Fail("查询用户失败");

            PlatformUser? target = null;
            foreach (var u in allUsers)
            {
                if (u.Id == id && u.DeletedAt == null) { target = u; break; }
            }
            if (target == null) return ApiResult.Fail("用户不存在");

            target.Status = status;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await PlatformUserCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success)
                return ApiResult.Fail("状态变更失败");

            Logger.Info(tenantId, operatorId,
                "[PlatformUserAppService] 用户状态变更: " + target.Username + " → " + status);
            return ApiResult.Ok();
        }

        /// <summary>映射实体到 DTO</summary>
        private static PlatformUserDto MapToDto(PlatformUser u) => new PlatformUserDto
        {
            Id = u.Id, Username = u.Username, Email = u.Email,
            Phone = u.Phone, DisplayName = u.DisplayName,
            Status = u.Status, MfaEnabled = u.MfaEnabled,
            LastLoginAt = u.LastLoginAt, CreatedAt = u.CreatedAt
        };

        /// <summary>过滤用户</summary>
        private static List<PlatformUser> FilterUsers(IReadOnlyList<PlatformUser> data, PagedRequest req)
        {
            var list = new List<PlatformUser>();
            foreach (var u in data)
            {
                if (u.DeletedAt != null) continue;
                if (!string.IsNullOrEmpty(req.Status) &&
                    !string.Equals(u.Status, req.Status, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!string.IsNullOrEmpty(req.Keyword) &&
                    u.Username.IndexOf(req.Keyword, StringComparison.OrdinalIgnoreCase) < 0 &&
                    u.DisplayName.IndexOf(req.Keyword, StringComparison.OrdinalIgnoreCase) < 0 &&
                    u.Email.IndexOf(req.Keyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                list.Add(u);
            }
            return list;
        }

        /// <summary>分页</summary>
        private static List<T> Paginate<T>(List<T> list, PagedRequest req)
        {
            var offset = req.Offset;
            var size = req.NormalizedPageSize;
            if (offset >= list.Count) return new List<T>();
            var end = Math.Min(offset + size, list.Count);
            return list.GetRange(offset, end - offset);
        }

        /// <summary>生成密码盐</summary>
        private static string GenerateSalt()
        {
            var bytes = new byte[16];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>哈希密码</summary>
        private static string HashPassword(string password, string salt)
        {
            var data = Encoding.UTF8.GetBytes(password + salt);
            var hash = SHA256.HashData(data);
            return Convert.ToHexString(hash);
        }
    }
}

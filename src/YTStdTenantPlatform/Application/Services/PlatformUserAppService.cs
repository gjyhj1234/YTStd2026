using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdLogger.Core;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Infrastructure.Cache;
using YTStdTenantPlatform.Application.Constants;
using YTStdTenantPlatform.Domain.Enums;

namespace YTStdTenantPlatform.Application.Services
{
    /// <summary>平台用户应用服务</summary>
    public static class PlatformUserAppService
    {
        /// <summary>获取用户分页列表</summary>
        public static async ValueTask<PagedResult<PlatformUserRepDTO>> GetListAsync(
            int tenantId, long operatorId, PagedRequest request)
        {
            var (queryResult, data) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
            if (!queryResult.Success || data == null)
                return new PagedResult<PlatformUserRepDTO> { Page = request.NormalizedPage, PageSize = request.NormalizedPageSize };

            var filtered = FilterUsers(data, request);
            var paged = Paginate(filtered, request);
            var items = new List<PlatformUserRepDTO>(paged.Count);
            foreach (var u in paged)
                items.Add(MapToDto(u));

            return new PagedResult<PlatformUserRepDTO>
            {
                Items = items,
                Total = filtered.Count,
                Page = request.NormalizedPage,
                PageSize = request.NormalizedPageSize
            };
        }

        /// <summary>获取用户详情</summary>
        public static async ValueTask<PlatformUserRepDTO?> GetByIdAsync(int tenantId, long operatorId, long id)
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
            int tenantId, long operatorId, CreatePlatformUserReqDTO req)
        {
            if (string.IsNullOrWhiteSpace(req.Username))
                return ApiResult<long>.Fail(ErrorCodes.UserUsernameRequired);
            if (string.IsNullOrWhiteSpace(req.Email))
                return ApiResult<long>.Fail(ErrorCodes.UserEmailRequired);
            if (string.IsNullOrWhiteSpace(req.Password))
                return ApiResult<long>.Fail(ErrorCodes.UserPasswordRequired);

            // 唯一性前置校验
            var (chkResult, existing) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
            if (chkResult.Success && existing != null)
            {
                foreach (var item in existing)
                {
                    if (item.DeletedAt != null) continue;
                    if (string.Equals(item.Username, req.Username.Trim(), StringComparison.OrdinalIgnoreCase))
                        return ApiResult<long>.Fail(ErrorCodes.UsernameExists);
                    if (string.Equals(item.Email, req.Email.Trim(), StringComparison.OrdinalIgnoreCase))
                        return ApiResult<long>.Fail(ErrorCodes.EmailExists);
                }
            }

            var salt = GenerateSalt();
            var hash = HashPassword(req.Password, salt);
            var now = DateTime.UtcNow;

            var user = new PlatformUser
            {
                Id = await DB.GetNextLongIdAsync(),
                Username = req.Username.Trim(),
                Email = req.Email.Trim(),
                Phone = req.Phone,
                DisplayName = req.DisplayName,
                PasswordHash = hash,
                PasswordSalt = salt,
                Status = (int)PlatformUserStatus.Active,
                MfaEnabled = false,
                Remark = req.Remark,
                CreatedAt = now,
                UpdatedAt = now
            };

            var insResult = await PlatformUserCRUD.InsertAsync(tenantId, operatorId, user);
            if (!insResult.Success)
            {
                // 唯一性后置复核
                var (rechkResult, rechkData) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
                if (rechkResult.Success && rechkData != null)
                {
                    foreach (var item in rechkData)
                    {
                        if (item.DeletedAt != null) continue;
                        if (string.Equals(item.Username, user.Username, StringComparison.OrdinalIgnoreCase))
                            return ApiResult<long>.Fail(ErrorCodes.UsernameExists);
                        if (string.Equals(item.Email, user.Email, StringComparison.OrdinalIgnoreCase))
                            return ApiResult<long>.Fail(ErrorCodes.EmailExists);
                    }
                }
                return ApiResult<long>.Fail(ErrorCodes.UserCreateFailed);
            }

            Logger.Info(tenantId, operatorId, "[PlatformUserAppService] 创建用户: " + req.Username);
            return ApiResult<long>.Ok(insResult.Id);
        }

        /// <summary>更新平台用户</summary>
        public static async ValueTask<ApiResult> UpdateAsync(
            int tenantId, long operatorId, long id, UpdatePlatformUserReqDTO req)
        {
            var (queryResult, allUsers) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
            if (!queryResult.Success || allUsers == null)
                return ApiResult.Fail(ErrorCodes.UserQueryFailed);

            PlatformUser? target = null;
            foreach (var u in allUsers)
            {
                if (u.Id == id && u.DeletedAt == null) { target = u; break; }
            }
            if (target == null) return ApiResult.Fail(ErrorCodes.UserNotFound);

            if (req.DisplayName != null) target.DisplayName = req.DisplayName;
            if (req.Phone != null) target.Phone = req.Phone;
            if (req.Email != null) target.Email = req.Email;
            if (req.Remark != null) target.Remark = req.Remark;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await PlatformUserCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success)
                return ApiResult.Fail(ErrorCodes.UserUpdateFailed);

            Logger.Info(tenantId, operatorId, "[PlatformUserAppService] 更新用户: " + target.Username);
            return ApiResult.Ok();
        }

        /// <summary>启用/禁用用户</summary>
        public static async ValueTask<ApiResult> SetStatusAsync(
            int tenantId, long operatorId, long id, string status)
        {
            var (queryResult, allUsers) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
            if (!queryResult.Success || allUsers == null)
                return ApiResult.Fail(ErrorCodes.UserQueryFailed);

            PlatformUser? target = null;
            foreach (var u in allUsers)
            {
                if (u.Id == id && u.DeletedAt == null) { target = u; break; }
            }
            if (target == null) return ApiResult.Fail(ErrorCodes.UserNotFound);

            if (!Enum.TryParse<PlatformUserStatus>(status, true, out var parsedStatus))
                return ApiResult.Fail(ErrorCodes.InvalidParameter);
            target.Status = (int)parsedStatus;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await PlatformUserCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success)
                return ApiResult.Fail(ErrorCodes.UserStatusChangeFailed);

            Logger.Info(tenantId, operatorId,
                "[PlatformUserAppService] 用户状态变更: " + target.Username + " → " + status);
            return ApiResult.Ok();
        }

        /// <summary>软删除平台用户</summary>
        public static async ValueTask<ApiResult> DeleteAsync(int tenantId, long operatorId, long id)
        {
            var (queryResult, allUsers) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
            if (!queryResult.Success || allUsers == null)
                return ApiResult.Fail(ErrorCodes.UserQueryFailed);

            PlatformUser? target = null;
            foreach (var u in allUsers)
            {
                if (u.Id == id && u.DeletedAt == null) { target = u; break; }
            }
            if (target == null) return ApiResult.Fail(ErrorCodes.UserNotFound);

            if (string.Equals(target.Username, "admin", StringComparison.OrdinalIgnoreCase))
                return ApiResult.Fail(ErrorCodes.UserCannotDeleteAdmin);

            target.DeletedAt = DateTime.UtcNow;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await PlatformUserCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success)
                return ApiResult.Fail(ErrorCodes.UserDeleteFailed);

            Logger.Info(tenantId, operatorId, "[PlatformUserAppService] 删除用户: " + target.Username);
            return ApiResult.Ok();
        }

        /// <summary>重置用户密码</summary>
        public static async ValueTask<ApiResult<ResetPasswordRepDTO>> ResetPasswordAsync(
            int tenantId, long operatorId, long id, string? newPassword)
        {
            var (queryResult, allUsers) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
            if (!queryResult.Success || allUsers == null)
                return ApiResult<ResetPasswordRepDTO>.Fail(ErrorCodes.UserQueryFailed);

            PlatformUser? target = null;
            foreach (var u in allUsers)
            {
                if (u.Id == id && u.DeletedAt == null) { target = u; break; }
            }
            if (target == null) return ApiResult<ResetPasswordRepDTO>.Fail(ErrorCodes.UserNotFound);

            string? generatedPassword = null;
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                generatedPassword = GenerateRandomPassword(12);
                newPassword = generatedPassword;
            }

            var salt = GenerateSalt();
            var hash = HashPassword(newPassword, salt);
            target.PasswordHash = hash;
            target.PasswordSalt = salt;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await PlatformUserCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success)
                return ApiResult<ResetPasswordRepDTO>.Fail(ErrorCodes.UserResetPasswordFailed);

            Logger.Info(tenantId, operatorId, "[PlatformUserAppService] 重置密码: " + target.Username);
            return ApiResult<ResetPasswordRepDTO>.Ok(new ResetPasswordRepDTO { GeneratedPassword = generatedPassword });
        }

        /// <summary>检查用户名是否存在</summary>
        public static async ValueTask<ApiResult<bool>> CheckUsernameExistsAsync(
            int tenantId, long operatorId, string username)
        {
            var (queryResult, allUsers) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
            if (!queryResult.Success || allUsers == null)
                return ApiResult<bool>.Fail(ErrorCodes.UserQueryFailed);

            foreach (var u in allUsers)
            {
                if (u.DeletedAt == null && string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase))
                    return ApiResult<bool>.Ok(true);
            }
            return ApiResult<bool>.Ok(false);
        }

        /// <summary>批量设置用户状态</summary>
        public static async ValueTask<ApiResult> BatchSetStatusAsync(
            int tenantId, long operatorId, long[] ids, PlatformUserStatus status)
        {
            var (queryResult, allUsers) = await PlatformUserCRUD.GetListAsync(tenantId, operatorId);
            if (!queryResult.Success || allUsers == null)
                return ApiResult.Fail(ErrorCodes.UserQueryFailed);

            var now = DateTime.UtcNow;
            var statusValue = (int)status;
            foreach (var u in allUsers)
            {
                if (u.DeletedAt != null) continue;
                bool matched = false;
                foreach (var id in ids)
                {
                    if (u.Id == id) { matched = true; break; }
                }
                if (!matched) continue;

                if (status == PlatformUserStatus.Disabled && u.Id == operatorId)
                    return ApiResult.Fail(ErrorCodes.UserCannotDisableSelf);

                u.Status = statusValue;
                u.UpdatedAt = now;
                var updResult = await PlatformUserCRUD.UpdateAsync(tenantId, operatorId, u);
                if (!updResult.Success)
                    return ApiResult.Fail(ErrorCodes.UserStatusChangeFailed);
            }

            Logger.Info(tenantId, operatorId,
                "[PlatformUserAppService] 批量状态变更 → " + status + "，数量: " + ids.Length);
            return ApiResult.Ok();
        }

        /// <summary>映射实体到 DTO</summary>
        private static PlatformUserRepDTO MapToDto(PlatformUser u) => new PlatformUserRepDTO
        {
            Id = u.Id, Username = u.Username, Email = u.Email,
            Phone = u.Phone, DisplayName = u.DisplayName,
            Status = ((PlatformUserStatus)u.Status).ToString(), MfaEnabled = u.MfaEnabled,
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
                    (!Enum.TryParse<PlatformUserStatus>(req.Status, true, out var statusFilter) ||
                     u.Status != (int)statusFilter))
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

        /// <summary>生成随机密码</summary>
        private static string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
            var result = new char[length];
            for (int i = 0; i < length; i++)
                result[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            return new string(result);
        }
    }
}

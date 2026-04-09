using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdLogger.Core;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;
using YTStdTenantPlatform.Application.Constants;
using YTStdTenantPlatform.Domain.Enums;

namespace YTStdTenantPlatform.Application.Services
{
    /// <summary>通知管理应用服务</summary>
    public static class NotificationAppService
    {
        // ──────────────────────────────────────────────────────
        // 通知模板
        // ──────────────────────────────────────────────────────

        /// <summary>获取通知模板分页列表</summary>
        public static async ValueTask<PagedResult<NotificationTemplateRepDTO>> GetTemplateListAsync(
            int tenantId, long operatorId, PagedRequest request)
        {
            var (result, data) = await NotificationTemplateCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new PagedResult<NotificationTemplateRepDTO> { Page = request.NormalizedPage, PageSize = request.NormalizedPageSize };

            var filtered = new List<NotificationTemplate>();
            foreach (var t in data)
            {
                if (!string.IsNullOrEmpty(request.Keyword) &&
                    t.TemplateName.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) < 0 &&
                    t.TemplateCode.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                filtered.Add(t);
            }

            var items = new List<NotificationTemplateRepDTO>();
            var offset = request.Offset;
            var size = request.NormalizedPageSize;
            for (int i = offset; i < filtered.Count && i < offset + size; i++)
                items.Add(MapTemplateToDto(filtered[i]));

            return new PagedResult<NotificationTemplateRepDTO>
            {
                Items = items, Total = filtered.Count,
                Page = request.NormalizedPage, PageSize = request.NormalizedPageSize
            };
        }

        /// <summary>获取通知模板详情</summary>
        public static async ValueTask<NotificationTemplateRepDTO?> GetTemplateByIdAsync(
            int tenantId, long operatorId, long id)
        {
            var (result, data) = await NotificationTemplateCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return null;
            foreach (var t in data)
            {
                if (t.Id == id)
                    return MapTemplateToDto(t);
            }
            return null;
        }

        /// <summary>创建通知模板</summary>
        public static async ValueTask<ApiResult<long>> CreateTemplateAsync(
            int tenantId, long operatorId, CreateNotificationTemplateReqDTO req)
        {
            if (string.IsNullOrWhiteSpace(req.TemplateCode))
                return ApiResult<long>.Fail(ErrorCodes.NotificationTemplateNameRequired);

            var now = DateTime.UtcNow;
            var entity = new NotificationTemplate
            {
                Id = await DB.GetNextLongIdAsync(),
                TemplateCode = req.TemplateCode.Trim(),
                TemplateName = req.TemplateName.Trim(),
                Channel = req.Channel,
                SubjectTemplate = req.SubjectTemplate,
                BodyTemplate = req.BodyTemplate,
                Status = (int)ActiveDisabledStatus.Active,
                CreatedAt = now,
                UpdatedAt = now
            };

            var insResult = await NotificationTemplateCRUD.InsertAsync(tenantId, operatorId, entity);
            if (!insResult.Success)
                return ApiResult<long>.Fail(ErrorCodes.NotificationTemplateCreateFailed);

            Logger.Debug(tenantId, operatorId,
                () => "[NotificationAppService] 创建通知模板: " + req.TemplateCode);
            return ApiResult<long>.Ok(entity.Id);
        }

        /// <summary>更新通知模板</summary>
        public static async ValueTask<ApiResult> UpdateTemplateAsync(
            int tenantId, long operatorId, long id, UpdateNotificationTemplateReqDTO req)
        {
            var (getResult, templates) = await NotificationTemplateCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || templates == null) return ApiResult.Fail(ErrorCodes.NotificationTemplateQueryFailed);

            NotificationTemplate? target = null;
            foreach (var t in templates) { if (t.Id == id) { target = t; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.NotificationTemplateNotFound);

            if (req.TemplateName != null) target.TemplateName = req.TemplateName;
            if (req.SubjectTemplate != null) target.SubjectTemplate = req.SubjectTemplate;
            if (req.BodyTemplate != null) target.BodyTemplate = req.BodyTemplate;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await NotificationTemplateCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.NotificationTemplateUpdateFailed);

            Logger.Debug(tenantId, operatorId,
                () => "[NotificationAppService] 更新通知模板: " + target.TemplateCode);
            return ApiResult.Ok();
        }

        /// <summary>设置通知模板状态（启用/禁用）</summary>
        public static async ValueTask<ApiResult> SetTemplateStatusAsync(
            int tenantId, long operatorId, long id, string status)
        {
            var (getResult, templates) = await NotificationTemplateCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || templates == null) return ApiResult.Fail(ErrorCodes.NotificationTemplateQueryFailed);

            NotificationTemplate? target = null;
            foreach (var t in templates) { if (t.Id == id) { target = t; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.NotificationTemplateNotFound);

            if (!Enum.TryParse<ActiveDisabledStatus>(status, true, out var parsedStatus))
                return ApiResult.Fail(ErrorCodes.InvalidParameter);
            target.Status = (int)parsedStatus;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await NotificationTemplateCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.NotificationTemplateStatusChangeFailed);

            Logger.Debug(tenantId, operatorId,
                () => "[NotificationAppService] 通知模板状态变更: " + target.TemplateCode + " → " + status);
            return ApiResult.Ok();
        }

        /// <summary>删除通知模板</summary>
        public static async ValueTask<ApiResult> DeleteTemplateAsync(int tenantId, long operatorId, long id)
        {
            var (getResult, templates) = await NotificationTemplateCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || templates == null) return ApiResult.Fail(ErrorCodes.NotificationTemplateQueryFailed);
            NotificationTemplate? target = null;
            foreach (var t in templates) { if (t.Id == id) { target = t; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.NotificationTemplateNotFound);
            var delResult = await NotificationTemplateCRUD.DeleteAsync(tenantId, operatorId, target.Id);
            if (!delResult.Success) return ApiResult.Fail(ErrorCodes.NotificationTemplateDeleteFailed);
            Logger.Debug(tenantId, operatorId, () => "[NotificationAppService] 删除模板: " + target.Id);
            return ApiResult.Ok();
        }

        // ──────────────────────────────────────────────────────
        // 通知
        // ──────────────────────────────────────────────────────

        /// <summary>获取通知分页列表</summary>
        public static async ValueTask<PagedResult<NotificationRepDTO>> GetNotificationListAsync(
            int tenantId, long operatorId, PagedRequest request)
        {
            var (result, data) = await NotificationCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null)
                return new PagedResult<NotificationRepDTO> { Page = request.NormalizedPage, PageSize = request.NormalizedPageSize };

            var filtered = new List<Notification>();
            foreach (var n in data)
            {
                if (!string.IsNullOrEmpty(request.Status) &&
                    !string.Equals(n.SendStatus, request.Status, StringComparison.OrdinalIgnoreCase))
                    continue;
                filtered.Add(n);
            }

            var items = new List<NotificationRepDTO>();
            var offset = request.Offset;
            var size = request.NormalizedPageSize;
            for (int i = offset; i < filtered.Count && i < offset + size; i++)
                items.Add(MapNotificationToDto(filtered[i]));

            return new PagedResult<NotificationRepDTO>
            {
                Items = items, Total = filtered.Count,
                Page = request.NormalizedPage, PageSize = request.NormalizedPageSize
            };
        }

        /// <summary>获取通知详情</summary>
        public static async ValueTask<NotificationRepDTO?> GetNotificationByIdAsync(
            int tenantId, long operatorId, long id)
        {
            var (result, data) = await NotificationCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return null;
            foreach (var n in data)
            {
                if (n.Id == id)
                    return MapNotificationToDto(n);
            }
            return null;
        }

        /// <summary>创建通知</summary>
        public static async ValueTask<ApiResult<long>> CreateNotificationAsync(
            int tenantId, long operatorId, CreateNotificationReqDTO req)
        {
            if (string.IsNullOrWhiteSpace(req.Recipient))
                return ApiResult<long>.Fail(ErrorCodes.InvalidParameter);
            if (string.IsNullOrWhiteSpace(req.Channel))
                return ApiResult<long>.Fail(ErrorCodes.InvalidParameter);
            if (string.IsNullOrWhiteSpace(req.Body))
                return ApiResult<long>.Fail(ErrorCodes.InvalidParameter);

            var entity = new Notification
            {
                Id = await DB.GetNextLongIdAsync(),
                TenantRefId = req.TenantRefId,
                TemplateId = req.TemplateId,
                Channel = req.Channel,
                Recipient = req.Recipient.Trim(),
                Subject = req.Subject,
                Body = req.Body,
                SendStatus = "pending",
                CreatedAt = DateTime.UtcNow
            };

            var insResult = await NotificationCRUD.InsertAsync(tenantId, operatorId, entity);
            if (!insResult.Success)
                return ApiResult<long>.Fail(ErrorCodes.NotificationCreateFailed);

            Logger.Debug(tenantId, operatorId,
                () => "[NotificationAppService] 创建通知: recipient=" + req.Recipient);
            return ApiResult<long>.Ok(entity.Id);
        }

        /// <summary>标记通知为已读</summary>
        public static async ValueTask<ApiResult> MarkNotificationReadAsync(
            int tenantId, long operatorId, long id)
        {
            var (getResult, notifications) = await NotificationCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || notifications == null) return ApiResult.Fail(ErrorCodes.NotificationQueryFailed);

            Notification? target = null;
            foreach (var n in notifications) { if (n.Id == id) { target = n; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.NotificationNotFound);

            target.ReadAt = DateTime.UtcNow;

            var updResult = await NotificationCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.NotificationMarkReadFailed);

            Logger.Debug(tenantId, operatorId,
                () => "[NotificationAppService] 标记通知已读: id=" + id);
            return ApiResult.Ok();
        }

        // ──────────────────────────────────────────────────────
        // Mapping helpers
        // ──────────────────────────────────────────────────────

        private static NotificationTemplateRepDTO MapTemplateToDto(NotificationTemplate t) => new NotificationTemplateRepDTO
        {
            Id = t.Id, TemplateCode = t.TemplateCode, TemplateName = t.TemplateName,
            Channel = t.Channel, SubjectTemplate = t.SubjectTemplate,
            BodyTemplate = t.BodyTemplate, Status = ((ActiveDisabledStatus)t.Status).ToString(),
            CreatedAt = t.CreatedAt
        };

        private static NotificationRepDTO MapNotificationToDto(Notification n) => new NotificationRepDTO
        {
            Id = n.Id, TenantRefId = n.TenantRefId, TemplateId = n.TemplateId,
            Channel = n.Channel, Recipient = n.Recipient,
            Subject = n.Subject, Body = n.Body,
            SendStatus = n.SendStatus, SentAt = n.SentAt,
            ReadAt = n.ReadAt, CreatedAt = n.CreatedAt
        };
    }
}

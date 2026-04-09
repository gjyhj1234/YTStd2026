using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YTStdAdo;
using YTStdLogger.Core;
using YTStdTenantPlatform.Application.Constants;
using YTStdTenantPlatform.Application.Dtos;
using YTStdTenantPlatform.Entity.TenantPlatform;

namespace YTStdTenantPlatform.Application.Services
{
    /// <summary>数据字典应用服务</summary>
    public static class DictionaryAppService
    {
        /// <summary>获取字典类型列表</summary>
        public static async ValueTask<List<DictionaryTypeRepDTO>> GetTypeListAsync(int tenantId, long operatorId)
        {
            var (result, data) = await PlatformDictionaryCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return new List<DictionaryTypeRepDTO>();

            var typeMap = new Dictionary<string, DictionaryTypeRepDTO>();
            foreach (var d in data)
            {
                if (!typeMap.TryGetValue(d.TypeCode, out var typeDto))
                {
                    typeDto = new DictionaryTypeRepDTO { TypeCode = d.TypeCode, TypeName = d.TypeName };
                    typeMap[d.TypeCode] = typeDto;
                }
                typeDto.ItemCount++;
            }

            var list = new List<DictionaryTypeRepDTO>();
            foreach (var kv in typeMap) list.Add(kv.Value);
            return list;
        }

        /// <summary>按类型获取字典项</summary>
        public static async ValueTask<List<DictionaryRepDTO>> GetByTypeAsync(int tenantId, long operatorId, string typeCode)
        {
            var (result, data) = await PlatformDictionaryCRUD.GetListAsync(tenantId, operatorId);
            if (!result.Success || data == null) return new List<DictionaryRepDTO>();

            var list = new List<DictionaryRepDTO>();
            foreach (var d in data)
            {
                if (string.Equals(d.TypeCode, typeCode, StringComparison.OrdinalIgnoreCase))
                    list.Add(MapToDto(d));
            }
            return list;
        }

        /// <summary>创建字典项</summary>
        public static async ValueTask<ApiResult<long>> CreateAsync(int tenantId, long operatorId, CreateDictionaryReqDTO req)
        {
            if (string.IsNullOrWhiteSpace(req.TypeCode))
                return ApiResult<long>.Fail(ErrorCodes.DictTypeCodeRequired);
            if (string.IsNullOrWhiteSpace(req.ItemCode))
                return ApiResult<long>.Fail(ErrorCodes.DictItemCodeRequired);

            // 唯一性前置校验（TypeCode + ItemCode 组合唯一）
            var (chkResult, existing) = await PlatformDictionaryCRUD.GetListAsync(tenantId, operatorId);
            if (chkResult.Success && existing != null)
            {
                foreach (var item in existing)
                {
                    if (string.Equals(item.TypeCode, req.TypeCode.Trim(), StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(item.ItemCode, req.ItemCode.Trim(), StringComparison.OrdinalIgnoreCase))
                        return ApiResult<long>.Fail(ErrorCodes.DictItemCodeExists);
                }
            }

            var now = DateTime.UtcNow;
            var dict = new PlatformDictionary
            {
                Id = await DB.GetNextLongIdAsync(),
                TypeCode = req.TypeCode.Trim(),
                TypeName = req.TypeName.Trim(),
                ItemCode = req.ItemCode.Trim(),
                ItemName = req.ItemName.Trim(),
                ItemValue = req.ItemValue,
                IsEnabled = true,
                SortOrder = req.SortOrder,
                Remark = req.Remark,
                CreatedBy = operatorId,
                CreatedAt = now,
                UpdatedAt = now
            };

            var insResult = await PlatformDictionaryCRUD.InsertAsync(tenantId, operatorId, dict);
            if (!insResult.Success)
            {
                // 唯一性后置复核
                var (rechkResult, rechkData) = await PlatformDictionaryCRUD.GetListAsync(tenantId, operatorId);
                if (rechkResult.Success && rechkData != null)
                {
                    foreach (var item in rechkData)
                    {
                        if (string.Equals(item.TypeCode, dict.TypeCode, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(item.ItemCode, dict.ItemCode, StringComparison.OrdinalIgnoreCase))
                            return ApiResult<long>.Fail(ErrorCodes.DictItemCodeExists);
                    }
                }
                return ApiResult<long>.Fail(ErrorCodes.DictCreateFailed);
            }

            Logger.Info(tenantId, operatorId, "[DictionaryAppService] 创建字典: " + req.TypeCode + "." + req.ItemCode);
            return ApiResult<long>.Ok(dict.Id);
        }

        /// <summary>更新字典项</summary>
        public static async ValueTask<ApiResult> UpdateAsync(int tenantId, long operatorId, long id, UpdateDictionaryReqDTO req)
        {
            var (getResult, items) = await PlatformDictionaryCRUD.GetListAsync(tenantId, operatorId);
            if (!getResult.Success || items == null) return ApiResult.Fail(ErrorCodes.DictQueryFailed);

            PlatformDictionary? target = null;
            foreach (var d in items) { if (d.Id == id) { target = d; break; } }
            if (target == null) return ApiResult.Fail(ErrorCodes.DictNotFound);

            if (req.ItemName != null) target.ItemName = req.ItemName;
            if (req.ItemValue != null) target.ItemValue = req.ItemValue;
            if (req.SortOrder.HasValue) target.SortOrder = req.SortOrder.Value;
            if (req.Remark != null) target.Remark = req.Remark;
            target.UpdatedBy = operatorId;
            target.UpdatedAt = DateTime.UtcNow;

            var updResult = await PlatformDictionaryCRUD.UpdateAsync(tenantId, operatorId, target);
            if (!updResult.Success) return ApiResult.Fail(ErrorCodes.DictUpdateFailed);

            Logger.Info(tenantId, operatorId, "[DictionaryAppService] 更新字典: id=" + id);
            return ApiResult.Ok();
        }

        /// <summary>删除字典项</summary>
        public static async ValueTask<ApiResult> DeleteAsync(int tenantId, long operatorId, long id)
        {
            var delResult = await PlatformDictionaryCRUD.DeleteAsync(tenantId, operatorId, id);
            if (!delResult.Success) return ApiResult.Fail(ErrorCodes.DictDeleteFailed);

            Logger.Info(tenantId, operatorId, "[DictionaryAppService] 删除字典: id=" + id);
            return ApiResult.Ok();
        }

        private static DictionaryRepDTO MapToDto(PlatformDictionary d)
        {
            return new DictionaryRepDTO
            {
                Id = d.Id, TypeCode = d.TypeCode, TypeName = d.TypeName,
                ItemCode = d.ItemCode, ItemName = d.ItemName, ItemValue = d.ItemValue,
                IsEnabled = d.IsEnabled, SortOrder = d.SortOrder,
                Remark = d.Remark, CreatedAt = d.CreatedAt
            };
        }
    }
}

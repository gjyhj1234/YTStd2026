<template>
  <div class="platform-users-page">
    <DxLoadPanel
      :visible="pageLoading"
      :show-indicator="true"
      :show-pane="true"
      :shading="true"
      :close-on-outside-click="false"
      shading-color="rgba(0,0,0,0.2)"
    />

    <!-- Page Header with title + icon triggers -->
    <div class="page-header">
      <div class="page-header-text">
        <h2 class="page-title">{{ $t('平台用户管理') }}</h2>
      </div>
      <div class="page-header-actions">
        <FunctionDescriptionCard v-model:visible="showDescription">
          <h4>{{ $t('功能用途') }}</h4>
          <p>{{ $t('平台用户管理用于管理系统级管理员账号。支持创建、编辑、删除用户，以及启用/禁用状态切换和密码重置操作。') }}</p>
          <h4>{{ $t('字段说明') }}</h4>
          <ul>
            <li><strong>{{ $t('用户名') }}</strong>：{{ $t('登录账号，创建后不可修改，仅允许字母、数字和下划线') }}</li>
            <li><strong>{{ $t('显示名') }}</strong>：{{ $t('用户展示名称，用于界面显示') }}</li>
            <li><strong>{{ $t('邮箱') }}</strong>：{{ $t('用户邮箱地址，用于通知和密码找回') }}</li>
            <li><strong>{{ $t('手机') }}</strong>：{{ $t('用户手机号码') }}</li>
            <li><strong>{{ $t('角色') }}</strong>：{{ $t('用户关联的平台角色，决定其权限范围') }}</li>
            <li><strong>{{ $t('状态') }}</strong>：{{ $t('已启用/已禁用，禁用后用户无法登录') }}</li>
          </ul>
          <h4>{{ $t('使用场景') }}</h4>
          <ul>
            <li>{{ $t('新员工入职时创建平台管理员账号') }}</li>
            <li>{{ $t('员工离职或岗位调整时禁用/启用账号') }}</li>
            <li>{{ $t('用户忘记密码时重置密码') }}</li>
            <li>{{ $t('按角色或时间范围筛选查询用户') }}</li>
          </ul>
        </FunctionDescriptionCard>
        <OperationGuideDrawer v-model:visible="showGuide">
          <ol class="guide-steps">
            <li>{{ $t('step_search') }}</li>
            <li>{{ $t('step_advanced') }}</li>
            <li>{{ $t('step_create') }}</li>
            <li>{{ $t('step_edit') }}</li>
            <li>{{ $t('step_status') }}</li>
            <li>{{ $t('step_reset_pwd') }}</li>
            <li>{{ $t('step_batch') }}</li>
            <li>{{ $t('step_delete') }}</li>
          </ol>
        </OperationGuideDrawer>
      </div>
    </div>

    <!-- Unified Search Area -->
    <div class="search-area">
      <div class="search-row">
        <div class="search-field">
          <label class="search-label">{{ $t('关键词') }}</label>
          <DxTextBox
            v-model:value="searchKeyword"
            :placeholder="$t('请输入用户名或显示名')"
            :show-clear-button="true"
            width="220"
            @enter-key="onSearch"
          />
        </div>
        <div class="search-field">
          <label class="search-label">{{ $t('状态') }}</label>
          <DxSelectBox
            v-model:value="searchStatus"
            :items="statusOptionsComputed"
            display-expr="label"
            value-expr="value"
            :placeholder="$t('请选择状态')"
            width="150"
            :show-clear-button="true"
          />
        </div>
        <template v-if="showAdvanced">
          <div class="search-field">
            <label class="search-label">{{ $t('角色') }}</label>
            <DxSelectBox
              v-model:value="searchRoleId"
              :items="allRoles"
              display-expr="Name"
              value-expr="Id"
              :placeholder="$t('请选择角色')"
              width="180"
              :show-clear-button="true"
              :search-enabled="true"
            />
          </div>
          <div class="search-field">
            <label class="search-label">{{ $t('创建时间') }}</label>
            <DxDateRangeBox
              v-model:start-date="searchDateStart"
              v-model:end-date="searchDateEnd"
              :start-date-label="$t('开始日期')"
              :end-date-label="$t('结束日期')"
              display-format="yyyy-MM-dd"
              width="300"
            />
          </div>
        </template>
        <div class="search-actions">
          <DxButton :text="$t('查询')" icon="search" type="default" @click="onSearch" />
          <DxButton :text="$t('重置')" icon="refresh" styling-mode="outlined" @click="onReset" />
          <DxButton
            :text="showAdvanced ? $t('收起') : $t('高级查询')"
            :icon="showAdvanced ? 'chevronup' : 'chevrondown'"
            styling-mode="text"
            @click="showAdvanced = !showAdvanced"
          />
        </div>
      </div>
    </div>

    <!-- Toolbar -->
    <DxToolbar class="grid-toolbar">
      <DxToolbarItem location="before">
        <template #default>
          <div class="toolbar-buttons">
            <DxButton
              v-if="authStore.hasPermission(PLATFORM_USER_CREATE)"
              :text="$t('新增')"
              icon="add"
              type="default"
              @click="openCreateDialog"
            />
            <DxButton
              v-if="authStore.hasPermission(PLATFORM_USER_ENABLE)"
              :text="$t('批量启用')"
              icon="check"
              styling-mode="outlined"
              :disabled="selectedRowKeys.length === 0"
              @click="onBatchEnable"
            />
            <DxButton
              v-if="authStore.hasPermission(PLATFORM_USER_DISABLE)"
              :text="$t('批量禁用')"
              icon="close"
              styling-mode="outlined"
              :disabled="selectedRowKeys.length === 0"
              @click="onBatchDisable"
            />
          </div>
        </template>
      </DxToolbarItem>
    </DxToolbar>

    <!-- Data Grid -->
    <DxDataGrid
      ref="gridRef"
      :data-source="dataSource"
      :show-borders="true"
      :hover-state-enabled="true"
      :column-auto-width="true"
      :column-hiding-enabled="true"
      :remote-operations="true"
      :no-data-text="$t('暂无数据')"
      :selected-row-keys="selectedRowKeys"
      :focused-row-enabled="true"
      :focused-row-key="focusedRowKey"
      :auto-navigate-to-focused-row="true"
      key-expr="Id"
      @selection-changed="onSelectionChanged"
    >
      <DxSelection mode="multiple" show-check-boxes-mode="always" />
      <DxPaging :page-size="20" />
      <DxPager
        :show-page-size-selector="true"
        :allowed-page-sizes="[10, 20, 50, 100]"
        :show-info="true"
        :show-navigation-buttons="true"
      />

      <DxColumn data-field="Id" :caption="$t('ID')" :width="80" :allow-sorting="false" :hiding-priority="0" />
      <DxColumn data-field="Username" :caption="$t('用户名')" :allow-sorting="true" />
      <DxColumn data-field="DisplayName" :caption="$t('显示名')" :allow-sorting="false" />
      <DxColumn data-field="Email" :caption="$t('邮箱')" :allow-sorting="false" :hiding-priority="2" />
      <DxColumn data-field="Phone" :caption="$t('手机')" :width="130" :allow-sorting="false" :hiding-priority="1" />
      <DxColumn data-field="RoleNames" :caption="$t('角色')" :allow-sorting="false" cell-template="rolesCell" :hiding-priority="3" />
      <DxColumn data-field="Status" :caption="$t('状态')" :width="100" :allow-sorting="false" cell-template="statusCell" />
      <DxColumn data-field="CreatedAt" :caption="$t('创建时间')" :width="180" :allow-sorting="true" cell-template="dateCell" :hiding-priority="4" />
      <DxColumn :caption="$t('操作')" :width="200" :allow-sorting="false" cell-template="actionCell" />

      <template #rolesCell="{ data: cellData }">
        <span class="role-tags">{{ (cellData.data.RoleNames || []).join(', ') || '-' }}</span>
      </template>

      <template #statusCell="{ data: cellData }">
        <span :class="cellData.data.Status === 'Active' ? 'status-enabled' : 'status-disabled'">
          {{ cellData.data.Status === 'Active' ? $t('已启用') : $t('已禁用') }}
        </span>
      </template>

      <template #dateCell="{ data: cellData }">
        {{ formatDate(cellData.value) }}
      </template>

      <template #actionCell="{ data: cellData }">
        <div class="action-buttons">
          <DxButton
            v-if="authStore.hasPermission(PLATFORM_USER_VIEW)"
            :text="$t('查看')"
            icon="search"
            styling-mode="text"
            @click="openDetail(cellData.data)"
          />
          <DxButton
            v-if="authStore.hasPermission(PLATFORM_USER_UPDATE)"
            :text="$t('编辑')"
            icon="edit"
            styling-mode="text"
            @click="openEditDialog(cellData.data)"
          />
          <DxDropDownButton
            :text="$t('更多')"
            icon="overflow"
            styling-mode="text"
            :items="getMoreActions(cellData.data)"
            display-expr="text"
            key-expr="id"
            :drop-down-options="{ width: 160 }"
            @item-click="onMoreActionClick($event, cellData.data)"
          />
        </div>
      </template>
    </DxDataGrid>

    <!-- Create/Edit Popup -->
    <DxPopup
      :visible="formDialogVisible"
      :title="isEditing ? $t('编辑用户') : $t('新增用户')"
      :width="600"
      :height="'auto'"
      :show-close-button="true"
      :drag-enabled="true"
      @hiding="onFormDialogHiding"
    >
      <template #content>
        <DxForm ref="formRef" :form-data="formData" label-mode="outside" :col-count="1">
          <DxSimpleItem
            data-field="Username"
            :label="{ text: $t('用户名') }"
            :editor-options="{ placeholder: $t('请输入用户名'), disabled: isEditing }"
          >
            <DxRequiredRule v-if="!isEditing" :message="$t('请输入用户名')" />
            <DxStringLengthRule v-if="!isEditing" :min="3" :max="50" :message="$t('用户名长度 3-50 个字符')" />
            <DxPatternRule v-if="!isEditing" :pattern="/^[a-zA-Z0-9_]+$/" :message="$t('用户名仅允许字母、数字和下划线')" />
            <DxAsyncRule
              v-if="!isEditing"
              :validation-callback="validateUsernameUnique"
              :message="$t('用户名已存在')"
            />
          </DxSimpleItem>
          <DxSimpleItem
            v-if="!isEditing"
            data-field="Password"
            :label="{ text: $t('密码') }"
            :editor-options="{ placeholder: $t('请输入密码'), mode: 'password' }"
          >
            <DxRequiredRule :message="$t('请输入密码')" />
            <DxStringLengthRule :min="6" :max="100" :message="$t('密码长度至少 6 个字符')" />
          </DxSimpleItem>
          <DxSimpleItem
            data-field="DisplayName"
            :label="{ text: $t('显示名') }"
            :editor-options="{ placeholder: $t('请输入显示名') }"
          >
            <DxRequiredRule :message="$t('请输入显示名')" />
            <DxStringLengthRule :min="2" :max="50" :message="$t('显示名长度 2-50 个字符')" />
          </DxSimpleItem>
          <DxSimpleItem
            data-field="Email"
            :label="{ text: $t('邮箱') }"
            :editor-options="{ placeholder: '' }"
          >
            <DxEmailRule :message="$t('请输入正确的邮箱地址')" />
            <DxStringLengthRule :max="100" :message="$t('邮箱长度不超过 100 个字符')" />
          </DxSimpleItem>
          <DxSimpleItem
            data-field="Phone"
            :label="{ text: $t('手机') }"
            :editor-options="{ placeholder: '' }"
          >
            <DxStringLengthRule :max="20" :message="$t('手机号长度不超过 20 个字符')" />
          </DxSimpleItem>
          <DxSimpleItem
            data-field="RoleIds"
            :label="{ text: $t('角色') }"
            editor-type="dxTagBox"
            :editor-options="roleEditorOptions"
            :validation-rules="roleValidationRules"
          />
        </DxForm>
        <div v-if="roleError" class="role-error">{{ roleError }}</div>
        <div class="dialog-buttons">
          <DxButton :text="$t('取消')" styling-mode="outlined" @click="closeFormDialog" />
          <DxButton :text="$t('确定')" type="default" :disabled="submitting" @click="onSubmitForm" />
        </div>
      </template>
    </DxPopup>

    <!-- Detail Popup -->
    <DxPopup
      :visible="detailDialogVisible"
      :title="$t('用户详情')"
      :width="500"
      :height="'auto'"
      :show-close-button="true"
      @hiding="detailDialogVisible = false"
    >
      <template #content>
        <div v-if="detailData" class="detail-content">
          <div class="detail-row">
            <span class="detail-label">{{ $t('ID') }}</span>
            <span class="detail-value">{{ detailData.Id }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('用户名') }}</span>
            <span class="detail-value">{{ detailData.Username }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('显示名') }}</span>
            <span class="detail-value">{{ detailData.DisplayName }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('邮箱') }}</span>
            <span class="detail-value">{{ detailData.Email || '-' }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('手机') }}</span>
            <span class="detail-value">{{ detailData.Phone || '-' }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('状态') }}</span>
            <span :class="detailData.Status === 'Active' ? 'status-enabled' : 'status-disabled'">
              {{ detailData.Status === 'Active' ? $t('已启用') : $t('已禁用') }}
            </span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('角色') }}</span>
            <span class="detail-value role-tags">{{ (detailData.RoleNames || []).join(', ') || '-' }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('创建时间') }}</span>
            <span class="detail-value">{{ formatDateTime(detailData.CreatedAt) }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('最后登录') }}</span>
            <span class="detail-value">{{ detailData.LastLoginAt ? formatDateTime(detailData.LastLoginAt) : '-' }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('备注') }}</span>
            <span class="detail-value">{{ detailData.Remark || '-' }}</span>
          </div>
        </div>
      </template>
    </DxPopup>

    <!-- Reset Password Result Popup -->
    <DxPopup
      :visible="resetPwdResultVisible"
      :title="$t('重置密码成功')"
      :width="400"
      :height="'auto'"
      :show-close-button="true"
      @hiding="resetPwdResultVisible = false"
    >
      <template #content>
        <div class="reset-pwd-result">
          <p>{{ $t('新密码已生成，请妥善保管：') }}</p>
          <div class="reset-pwd-value">{{ resetPwdNewPassword }}</div>
          <DxButton :text="$t('确定')" type="default" @click="resetPwdResultVisible = false" />
        </div>
      </template>
    </DxPopup>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import CustomStore from 'devextreme/data/custom_store'
import type { LoadOptions } from 'devextreme/data'
import {
  DxDataGrid,
  DxColumn,
  DxPaging,
  DxPager,
  DxSelection
} from 'devextreme-vue/data-grid'
import { DxTextBox } from 'devextreme-vue/text-box'
import { DxSelectBox } from 'devextreme-vue/select-box'
import { DxDateRangeBox } from 'devextreme-vue/date-range-box'
import { DxButton } from 'devextreme-vue/button'
import { DxDropDownButton } from 'devextreme-vue/drop-down-button'
import { DxToolbar, DxItem as DxToolbarItem } from 'devextreme-vue/toolbar'
import { DxPopup } from 'devextreme-vue/popup'
import {
  DxForm,
  DxSimpleItem
} from 'devextreme-vue/form'
import {
  DxRequiredRule,
  DxStringLengthRule,
  DxPatternRule,
  DxAsyncRule,
  DxEmailRule
} from 'devextreme-vue/validator'
import { DxLoadPanel } from 'devextreme-vue/load-panel'
import FunctionDescriptionCard from '../../components/FunctionDescriptionCard.vue'
import OperationGuideDrawer from '../../components/OperationGuideDrawer.vue'
import {
  getPlatformUsersApi,
  getPlatformUserApi,
  createPlatformUserApi,
  updatePlatformUserApi,
  deletePlatformUserApi,
  enablePlatformUserApi,
  disablePlatformUserApi,
  resetPasswordApi,
  checkUsernameExistsApi,
  batchEnableUsersApi,
  batchDisableUsersApi
} from '../../api/platform-users'
import { getAllPlatformRolesApi } from '../../api/platform-roles'
import { useAuthStore } from '../../store/auth'
import { notifySuccess, confirmAction, confirmDelete } from '../../utils/notify'
import {
  PLATFORM_USER_VIEW,
  PLATFORM_USER_CREATE,
  PLATFORM_USER_UPDATE,
  PLATFORM_USER_DELETE,
  PLATFORM_USER_ENABLE,
  PLATFORM_USER_DISABLE,
  PLATFORM_USER_RESET_PWD
} from '../../constants/permissions'
import type { PlatformUserRepDTO } from '../../types/platform-users'
import type { PlatformRoleRepDTO } from '../../types/platform-roles'

const { t } = useI18n()
const authStore = useAuthStore()

const pageLoading = ref(false)
const showDescription = ref(false)
const showGuide = ref(false)
const showAdvanced = ref(false)
const searchKeyword = ref('')
const searchStatus = ref<string | null>(null)
const searchRoleId = ref<string | number | null>(null)
const searchDateStart = ref<Date | null>(null)
const searchDateEnd = ref<Date | null>(null)
const gridRef = ref()
const selectedRowKeys = ref<Array<string | number>>([])
const focusedRowKey = ref<string | number | null>(null)
const allRoles = ref<PlatformRoleRepDTO[]>([])

// Reactive status options using computed (fixes i18n switch issue)
const statusOptionsComputed = computed(() => [
  { value: null, label: t('全部') },
  { value: 'Active', label: t('已启用') },
  { value: 'Disabled', label: t('已禁用') }
])

// Reactive role editor options computed for DxTagBox reactivity
const roleEditorOptions = computed(() => ({
  items: allRoles.value,
  displayExpr: 'Name',
  valueExpr: 'Id',
  placeholder: t('请选择角色'),
  showSelectionControls: true,
  searchEnabled: true
}))

// Validation rules for RoleIds (must select at least one role)
const roleValidationRules = computed(() => [{
  type: 'custom',
  validationCallback: (e: { value: unknown }) => Array.isArray(e.value) && e.value.length > 0,
  message: t('请选择至少一个角色')
}])

// CustomStore for remote paging
const dataSource = new CustomStore({
  key: 'Id',
  load: async (loadOptions: LoadOptions) => {
    const page = loadOptions.skip !== undefined && loadOptions.take !== undefined
      ? Math.floor(loadOptions.skip / loadOptions.take) + 1
      : 1
    const pageSize = loadOptions.take || 20
    try {
      const result = await getPlatformUsersApi({
        Page: page,
        PageSize: pageSize,
        Keyword: searchKeyword.value || undefined,
        Status: searchStatus.value,
        RoleId: searchRoleId.value,
        CreatedAtStart: searchDateStart.value ? searchDateStart.value.toISOString() : null,
        CreatedAtEnd: searchDateEnd.value ? searchDateEnd.value.toISOString() : null
      })
      return {
        data: result.Items || [],
        totalCount: result.Total || 0
      }
    } catch {
      return { data: [], totalCount: 0 }
    }
  }
})

function onSelectionChanged(e: { selectedRowKeys: number[] }): void {
  selectedRowKeys.value = e.selectedRowKeys
}

function onSearch(): void {
  if (gridRef.value?.instance) {
    gridRef.value.instance.refresh()
  }
}

function onReset(): void {
  searchKeyword.value = ''
  searchStatus.value = null
  searchRoleId.value = null
  searchDateStart.value = null
  searchDateEnd.value = null
  onSearch()
}

// More actions dropdown for row operations (overflow pattern)
function getMoreActions(row: PlatformUserRepDTO): Array<{ id: string; text: string; icon: string }> {
  const actions: Array<{ id: string; text: string; icon: string }> = []
  if (authStore.hasPermission(PLATFORM_USER_ENABLE) && row.Status === 'Disabled') {
    actions.push({ id: 'enable', text: t('启用'), icon: 'check' })
  }
  if (authStore.hasPermission(PLATFORM_USER_DISABLE) && row.Status === 'Active') {
    actions.push({ id: 'disable', text: t('禁用'), icon: 'close' })
  }
  if (authStore.hasPermission(PLATFORM_USER_RESET_PWD)) {
    actions.push({ id: 'reset-pwd', text: t('重置密码'), icon: 'key' })
  }
  if (authStore.hasPermission(PLATFORM_USER_DELETE)) {
    actions.push({ id: 'delete', text: t('删除'), icon: 'trash' })
  }
  return actions
}

function onMoreActionClick(e: { itemData: { id: string } }, row: PlatformUserRepDTO): void {
  const actionId = e.itemData.id
  if (actionId === 'enable') onEnable(row)
  else if (actionId === 'disable') onDisable(row)
  else if (actionId === 'reset-pwd') onResetPassword(row)
  else if (actionId === 'delete') onDelete(row)
}

// Form dialog
const formDialogVisible = ref(false)
const isEditing = ref(false)
const editingId = ref<string | number | null>(null)
const submitting = ref(false)
const formRef = ref()
const formData = ref({
  Username: '',
  Password: '',
  DisplayName: '',
  Email: '',
  Phone: '',
  RoleIds: [] as Array<string | number>
})

async function openCreateDialog(): Promise<void> {
  isEditing.value = false
  editingId.value = null
  formData.value = { Username: '', Password: '', DisplayName: '', Email: '', Phone: '', RoleIds: [] as Array<string | number> }
  if (allRoles.value.length === 0) await loadRoles()
  formDialogVisible.value = true
}

async function openEditDialog(row: PlatformUserRepDTO): Promise<void> {
  isEditing.value = true
  editingId.value = row.Id
  if (allRoles.value.length === 0) await loadRoles()
  try {
    const detail = await getPlatformUserApi(row.Id)
    formData.value = {
      Username: detail.Username,
      Password: '',
      DisplayName: detail.DisplayName,
      Email: detail.Email || '',
      Phone: detail.Phone || '',
      RoleIds: detail.RoleIds ? [...detail.RoleIds] : []
    }
    formDialogVisible.value = true
  } catch {
    // error handled by interceptor
  }
}

function closeFormDialog(): void {
  formDialogVisible.value = false
}

function onFormDialogHiding(): void {
  formDialogVisible.value = false
}

async function validateUsernameUnique(params: { value: string }): Promise<boolean> {
  if (!params.value) return true
  try {
    const exists = await checkUsernameExistsApi(params.value)
    return !exists
  } catch {
    return true
  }
}

const roleError = ref('')

async function onSubmitForm(): Promise<void> {
  if (formRef.value?.instance) {
    const result = formRef.value.instance.validate()
    if (result && !result.isValid) return
  }
  // Explicit RoleIds validation (DxForm custom rules may not trigger for dxTagBox editor-type)
  if (!Array.isArray(formData.value.RoleIds) || formData.value.RoleIds.length === 0) {
    roleError.value = t('请选择至少一个角色')
    return
  }
  roleError.value = ''
  submitting.value = true
  try {
    let savedId: string | number | null = null
    if (isEditing.value && editingId.value) {
      await updatePlatformUserApi(editingId.value, {
        DisplayName: formData.value.DisplayName,
        Email: formData.value.Email,
        Phone: formData.value.Phone,
        RoleIds: formData.value.RoleIds
      })
      savedId = editingId.value
      notifySuccess('更新成功')
    } else {
      const newId = await createPlatformUserApi({
        Username: formData.value.Username,
        Password: formData.value.Password,
        DisplayName: formData.value.DisplayName,
        Email: formData.value.Email,
        Phone: formData.value.Phone,
        RoleIds: formData.value.RoleIds
      })
      savedId = newId
      notifySuccess('创建成功')
    }
    closeFormDialog()
    onSearch()
    // Highlight saved row after refresh
    if (savedId) {
      focusedRowKey.value = savedId
    }
  } catch {
    // error handled by interceptor
  } finally {
    submitting.value = false
  }
}

// Detail
const detailDialogVisible = ref(false)
const detailData = ref<PlatformUserRepDTO | null>(null)

async function openDetail(row: PlatformUserRepDTO): Promise<void> {
  try {
    detailData.value = await getPlatformUserApi(row.Id)
    detailDialogVisible.value = true
  } catch {
    // error handled by interceptor
  }
}

// Enable / Disable / Reset / Delete
async function onEnable(row: PlatformUserRepDTO): Promise<void> {
  const confirmed = await confirmAction('确认启用用户 {name}', { name: row.DisplayName })
  if (!confirmed) return
  try {
    await enablePlatformUserApi(row.Id)
    notifySuccess('启用成功')
    onSearch()
    focusedRowKey.value = row.Id
  } catch {
    // error handled by interceptor
  }
}

async function onDisable(row: PlatformUserRepDTO): Promise<void> {
  const confirmed = await confirmAction('确认禁用用户 {name}', { name: row.DisplayName })
  if (!confirmed) return
  try {
    await disablePlatformUserApi(row.Id)
    notifySuccess('禁用成功')
    onSearch()
    focusedRowKey.value = row.Id
  } catch {
    // error handled by interceptor
  }
}

// Reset password result
const resetPwdResultVisible = ref(false)
const resetPwdNewPassword = ref('')

async function onResetPassword(row: PlatformUserRepDTO): Promise<void> {
  const confirmed = await confirmAction('确认重置用户 {name} 的密码', { name: row.DisplayName })
  if (!confirmed) return
  try {
    const result = await resetPasswordApi(row.Id)
    const newPwd = result?.GeneratedPassword
    if (newPwd) {
      resetPwdNewPassword.value = newPwd
      resetPwdResultVisible.value = true
    } else {
      notifySuccess('重置密码成功')
    }
  } catch {
    // error handled by interceptor
  }
}

async function onDelete(row: PlatformUserRepDTO): Promise<void> {
  const confirmed = await confirmDelete(row.DisplayName)
  if (!confirmed) return
  try {
    await deletePlatformUserApi(row.Id)
    notifySuccess('删除成功')
    onSearch()
  } catch {
    // error handled by interceptor
  }
}

// Batch operations
async function onBatchEnable(): Promise<void> {
  if (selectedRowKeys.value.length === 0) return
  const confirmed = await confirmAction('确认批量启用选中用户')
  if (!confirmed) return
  try {
    await batchEnableUsersApi(selectedRowKeys.value)
    notifySuccess('批量启用成功')
    selectedRowKeys.value = []
    onSearch()
  } catch {
    // error handled by interceptor
  }
}

async function onBatchDisable(): Promise<void> {
  if (selectedRowKeys.value.length === 0) return
  const confirmed = await confirmAction('确认批量禁用选中用户')
  if (!confirmed) return
  try {
    await batchDisableUsersApi(selectedRowKeys.value)
    notifySuccess('批量禁用成功')
    selectedRowKeys.value = []
    onSearch()
  } catch {
    // error handled by interceptor
  }
}

// Formatting
function formatDate(value: string): string {
  if (!value) return ''
  const d = new Date(value)
  const y = d.getFullYear()
  const m = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  const h = String(d.getHours()).padStart(2, '0')
  const min = String(d.getMinutes()).padStart(2, '0')
  return `${y}-${m}-${day} ${h}:${min}`
}

function formatDateTime(value: string): string {
  if (!value) return ''
  const d = new Date(value)
  const y = d.getFullYear()
  const m = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  const h = String(d.getHours()).padStart(2, '0')
  const min = String(d.getMinutes()).padStart(2, '0')
  const sec = String(d.getSeconds()).padStart(2, '0')
  return `${y}-${m}-${day} ${h}:${min}:${sec}`
}

async function loadRoles(): Promise<void> {
  try {
    allRoles.value = await getAllPlatformRolesApi()
  } catch {
    allRoles.value = []
  }
}

onMounted(() => {
  loadRoles()
})
</script>

<style scoped>
.platform-users-page {
  padding: 16px;
}

.role-tags {
  color: #1890ff;
  font-size: 12px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.page-header-text {
  flex: 1;
}

.page-header-actions {
  display: flex;
  gap: 4px;
  align-items: center;
}

.page-title {
  margin: 0;
  font-size: 18px !important;
  font-weight: 600;
  color: #333;
}

.search-area {
  background: #fafafa;
  border-radius: 4px;
  padding: 12px 16px;
  margin-bottom: 12px;
}

.search-row {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  align-items: flex-end;
}

.search-field {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.search-label {
  font-size: 12px;
  color: #666;
  font-weight: 500;
}

.search-actions {
  display: flex;
  gap: 8px;
  align-items: flex-end;
  padding-bottom: 1px;
}

.grid-toolbar {
  margin-bottom: 8px;
}

.toolbar-buttons {
  display: flex;
  gap: 8px;
}

.action-buttons {
  display: flex;
  gap: 4px;
  align-items: center;
}

.status-enabled {
  color: #52c41a;
  background-color: #f6ffed;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
}

.status-disabled {
  color: #f5222d;
  background-color: #fff2f0;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
}

.dialog-buttons {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 20px;
  padding-top: 16px;
  border-top: 1px solid #f0f0f0;
}

.detail-content {
  padding: 8px 0;
}

.detail-row {
  display: flex;
  padding: 8px 0;
  border-bottom: 1px solid #f5f5f5;
}

.detail-label {
  width: 120px;
  color: #999;
  flex-shrink: 0;
}

.detail-value {
  flex: 1;
  color: #333;
}

.guide-steps {
  padding-left: 20px;
  line-height: 2.2;
}

.reset-pwd-result {
  text-align: center;
  padding: 16px 0;
}

.reset-pwd-value {
  font-size: 20px;
  font-weight: bold;
  color: #1890ff;
  padding: 16px;
  margin: 12px 0 20px;
  background: #f0f7ff;
  border-radius: 4px;
  letter-spacing: 1px;
  font-family: monospace;
}

.role-error {
  color: #f5222d;
  font-size: 12px;
  margin-top: 4px;
  padding-left: 2px;
}

/* Mobile responsive */
@media (max-width: 768px) {
  .platform-users-page {
    padding: 8px;
  }

  .page-title {
    font-size: 16px;
  }

  .search-area {
    padding: 8px 12px;
  }

  .search-row {
    gap: 8px;
  }

  .search-field :deep(.dx-textbox),
  .search-field :deep(.dx-selectbox) {
    width: 100% !important;
  }

  .search-field {
    flex: 1;
    min-width: 120px;
  }

  .toolbar-buttons {
    flex-wrap: wrap;
    gap: 4px;
  }
}
</style>

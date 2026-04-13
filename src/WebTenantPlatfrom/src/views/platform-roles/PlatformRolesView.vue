<template>
  <div class="platform-roles-page">
    <DxLoadPanel
      :visible="pageLoading"
      :show-indicator="true"
      :show-pane="true"
      :shading="true"
      :close-on-outside-click="false"
      shading-color="rgba(0,0,0,0.2)"
    />

    <h2 class="page-title">{{ $t('平台角色管理') }}</h2>
    <p class="page-subtitle">{{ $t('管理平台级角色，包括创建、编辑、启用/禁用、权限分配和成员管理') }}</p>

    <FunctionDescriptionCard v-model:visible="showDescription">
      <p>{{ $t('管理平台级角色，包括创建、编辑、启用/禁用、权限分配和成员管理') }}</p>
    </FunctionDescriptionCard>

    <!-- Search Bar -->
    <div class="search-bar">
      <DxTextBox
        v-model:value="searchKeyword"
        :placeholder="$t('请输入角色编码或名称')"
        :show-clear-button="true"
        width="280"
        @enter-key="onSearch"
      />
      <DxSelectBox
        v-model:value="searchStatus"
        :items="statusOptions"
        display-expr="label"
        value-expr="value"
        :placeholder="$t('请选择状态')"
        width="160"
        :show-clear-button="true"
      />
      <DxButton :text="$t('查询')" icon="search" type="default" @click="onSearch" />
      <DxButton :text="$t('重置')" icon="refresh" styling-mode="outlined" @click="onReset" />
    </div>

    <!-- Toolbar -->
    <DxToolbar class="grid-toolbar">
      <DxToolbarItem location="before">
        <template #default>
          <DxButton
            v-if="authStore.hasPermission(PLATFORM_ROLE_CREATE)"
            :text="$t('新增')"
            icon="add"
            type="default"
            @click="openCreateDialog"
          />
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
      :remote-operations="true"
      :no-data-text="$t('暂无数据')"
    >
      <DxPaging :page-size="20" />
      <DxPager
        :show-page-size-selector="true"
        :allowed-page-sizes="[10, 20, 50, 100]"
        :show-info="true"
        :show-navigation-buttons="true"
      />

      <DxColumn data-field="Id" :caption="$t('ID')" :width="80" :allow-sorting="false" />
      <DxColumn data-field="Code" :caption="$t('角色编码')" :allow-sorting="true" />
      <DxColumn data-field="Name" :caption="$t('角色名称')" :allow-sorting="true" />
      <DxColumn data-field="Description" :caption="$t('描述')" :allow-sorting="false" />
      <DxColumn data-field="Status" :caption="$t('状态')" :width="100" :allow-sorting="false" cell-template="statusCell" />
      <DxColumn data-field="CreatedAt" :caption="$t('创建时间')" :width="180" :allow-sorting="true" cell-template="dateCell" />
      <DxColumn :caption="$t('操作')" :width="480" :allow-sorting="false" cell-template="actionCell" />

      <template #statusCell="{ data: cellData }">
        <span :class="cellData.data.Status === 'active' ? 'status-enabled' : 'status-disabled'">
          {{ cellData.data.Status === 'active' ? $t('已启用') : $t('已禁用') }}
        </span>
      </template>

      <template #dateCell="{ data: cellData }">
        {{ formatDate(cellData.value) }}
      </template>

      <template #actionCell="{ data: cellData }">
        <div class="action-buttons">
          <DxButton
            v-if="authStore.hasPermission(PLATFORM_ROLE_VIEW)"
            :text="$t('查看')"
            icon="search"
            styling-mode="text"
            @click="openDetail(cellData.data)"
          />
          <DxButton
            v-if="authStore.hasPermission(PLATFORM_ROLE_UPDATE)"
            :text="$t('编辑')"
            icon="edit"
            styling-mode="text"
            @click="openEditDialog(cellData.data)"
          />
          <DxButton
            v-if="authStore.hasPermission(PLATFORM_ROLE_ENABLE) && cellData.data.Status === 'disabled'"
            :text="$t('启用')"
            icon="check"
            styling-mode="text"
            @click="onEnable(cellData.data)"
          />
          <DxButton
            v-if="authStore.hasPermission(PLATFORM_ROLE_DISABLE) && cellData.data.Status === 'active'"
            :text="$t('禁用')"
            icon="close"
            styling-mode="text"
            @click="onDisable(cellData.data)"
          />
          <DxButton
            v-if="authStore.hasPermission(PLATFORM_ROLE_ASSIGN_PERMISSION)"
            :text="$t('分配权限')"
            icon="hierarchy"
            styling-mode="text"
            @click="openPermissionDialog(cellData.data)"
          />
          <DxButton
            v-if="authStore.hasPermission(PLATFORM_ROLE_ASSIGN_MEMBER)"
            :text="$t('分配成员')"
            icon="group"
            styling-mode="text"
            @click="openMemberDialog(cellData.data)"
          />
          <DxButton
            v-if="authStore.hasPermission(PLATFORM_ROLE_DELETE)"
            :text="$t('删除')"
            icon="trash"
            styling-mode="text"
            type="danger"
            @click="onDelete(cellData.data)"
          />
        </div>
      </template>
    </DxDataGrid>

    <!-- Create/Edit Popup -->
    <DxPopup
      :visible="formDialogVisible"
      :title="isEditing ? $t('编辑角色') : $t('新增角色')"
      :width="600"
      :height="'auto'"
      :show-close-button="true"
      :drag-enabled="true"
      @hiding="onFormDialogHiding"
    >
      <template #content>
        <DxForm ref="formRef" :form-data="formData" label-mode="outside" :col-count="1">
          <DxSimpleItem
            data-field="Code"
            :label="{ text: $t('角色编码') }"
            :editor-options="{ placeholder: $t('请输入角色编码'), disabled: isEditing }"
          >
            <DxRequiredRule :message="$t('请输入角色编码')" />
            <DxStringLengthRule :min="2" :max="50" :message="$t('角色编码长度 2-50 个字符')" />
            <DxPatternRule :pattern="/^[a-zA-Z0-9_-]+$/" :message="$t('角色编码仅允许字母、数字、下划线和连字符')" />
            <DxAsyncRule
              v-if="!isEditing"
              :validation-callback="validateCodeUnique"
              :message="$t('角色编码已存在')"
            />
          </DxSimpleItem>
          <DxSimpleItem
            data-field="Name"
            :label="{ text: $t('角色名称') }"
            :editor-options="{ placeholder: $t('请输入角色名称') }"
          >
            <DxRequiredRule :message="$t('请输入角色名称')" />
            <DxStringLengthRule :min="2" :max="50" :message="$t('角色名称长度 2-50 个字符')" />
          </DxSimpleItem>
          <DxSimpleItem
            data-field="Description"
            :label="{ text: $t('描述') }"
            editor-type="dxTextArea"
            :editor-options="{ height: 100 }"
          >
            <DxStringLengthRule :max="500" :message="$t('描述长度不超过 500 个字符')" />
          </DxSimpleItem>
        </DxForm>
        <div class="dialog-buttons">
          <DxButton :text="$t('取消')" styling-mode="outlined" @click="closeFormDialog" />
          <DxButton :text="$t('确定')" type="default" :disabled="submitting" @click="onSubmitForm" />
        </div>
      </template>
    </DxPopup>

    <!-- Detail Popup -->
    <DxPopup
      :visible="detailDialogVisible"
      :title="$t('角色详情')"
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
            <span class="detail-label">{{ $t('角色编码') }}</span>
            <span class="detail-value">{{ detailData.Code }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('角色名称') }}</span>
            <span class="detail-value">{{ detailData.Name }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('描述') }}</span>
            <span class="detail-value">{{ detailData.Description || '-' }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('状态') }}</span>
            <span :class="detailData.Status === 'active' ? 'status-enabled' : 'status-disabled'">
              {{ detailData.Status === 'active' ? $t('已启用') : $t('已禁用') }}
            </span>
          </div>
          <div class="detail-row">
            <span class="detail-label">{{ $t('创建时间') }}</span>
            <span class="detail-value">{{ formatDateTime(detailData.CreatedAt) }}</span>
          </div>
        </div>
      </template>
    </DxPopup>

    <!-- Permission Binding Popup -->
    <DxPopup
      :visible="permDialogVisible"
      :title="$t('分配权限')"
      :width="800"
      :height="600"
      :show-close-button="true"
      @hiding="permDialogVisible = false"
    >
      <template #content>
        <DxTreeList
          :data-source="permTreeData"
          key-expr="Id"
          parent-id-expr="ParentId"
          :auto-expand-all="true"
          :show-borders="true"
          :column-auto-width="true"
          :selected-row-keys="selectedPermKeys"
          @selection-changed="onPermSelectionChanged"
        >
          <DxTreeListSelection mode="multiple" :recursive="true" />
          <DxTreeListColumn data-field="Name" :caption="$t('权限名称')" />
          <DxTreeListColumn data-field="Code" :caption="$t('权限编码')" />
          <DxTreeListColumn data-field="PermissionType" :caption="$t('权限类型')" :width="120" />
        </DxTreeList>
        <div class="dialog-buttons">
          <DxButton :text="$t('取消')" styling-mode="outlined" @click="permDialogVisible = false" />
          <DxButton :text="$t('确定')" type="default" :disabled="savingPerm" @click="onSavePermissions" />
        </div>
      </template>
    </DxPopup>

    <!-- Member Binding Popup -->
    <DxPopup
      :visible="memberDialogVisible"
      :title="$t('分配成员')"
      :width="800"
      :height="600"
      :show-close-button="true"
      @hiding="memberDialogVisible = false"
    >
      <template #content>
        <DxDataGrid
          :data-source="memberListData"
          :show-borders="true"
          :hover-state-enabled="true"
          :selected-row-keys="selectedMemberKeys"
          key-expr="Id"
          @selection-changed="onMemberSelectionChanged"
        >
          <DxMemberSelection mode="multiple" show-check-boxes-mode="always" />
          <DxMemberColumn data-field="Username" :caption="$t('用户名')" />
          <DxMemberColumn data-field="DisplayName" :caption="$t('显示名')" />
        </DxDataGrid>
        <div class="dialog-buttons">
          <DxButton :text="$t('取消')" styling-mode="outlined" @click="memberDialogVisible = false" />
          <DxButton :text="$t('确定')" type="default" :disabled="savingMember" @click="onSaveMembers" />
        </div>
      </template>
    </DxPopup>

    <OperationGuideDrawer v-model:visible="showGuide">
      <template #content>
        <div class="platform-roles-page">
          <!-- main content above -->
        </div>
      </template>
      <ol>
        <li>{{ $t('管理平台级角色，包括创建、编辑、启用/禁用、权限分配和成员管理') }}</li>
      </ol>
    </OperationGuideDrawer>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import CustomStore from 'devextreme/data/custom_store'
import type { LoadOptions } from 'devextreme/data'
import {
  DxDataGrid,
  DxColumn,
  DxPaging,
  DxPager
} from 'devextreme-vue/data-grid'
import {
  DxTreeList,
  DxColumn as DxTreeListColumn,
  DxSelection as DxTreeListSelection
} from 'devextreme-vue/tree-list'
import { DxTextBox } from 'devextreme-vue/text-box'
import { DxSelectBox } from 'devextreme-vue/select-box'
import { DxButton } from 'devextreme-vue/button'
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
  DxAsyncRule
} from 'devextreme-vue/validator'
import { DxLoadPanel } from 'devextreme-vue/load-panel'
import {
  DxDataGrid as DxMemberGrid,
  DxColumn as DxMemberColumn,
  DxSelection as DxMemberSelection
} from 'devextreme-vue/data-grid'
import FunctionDescriptionCard from '../../components/FunctionDescriptionCard.vue'
import OperationGuideDrawer from '../../components/OperationGuideDrawer.vue'
import {
  getPlatformRolesApi,
  getPlatformRoleApi,
  createPlatformRoleApi,
  updatePlatformRoleApi,
  deletePlatformRoleApi,
  enablePlatformRoleApi,
  disablePlatformRoleApi,
  checkRoleCodeExistsApi,
  getRolePermissionsApi,
  bindRolePermissionsApi,
  bindRoleMembersApi
} from '../../api/platform-roles'
import { getPermissionTreeApi } from '../../api/platform-permissions'
import { getPlatformUsersApi } from '../../api/platform-users'
import { useAuthStore } from '../../store/auth'
import { notifySuccess, confirmAction, confirmDelete } from '../../utils/notify'
import {
  PLATFORM_ROLE_VIEW,
  PLATFORM_ROLE_CREATE,
  PLATFORM_ROLE_UPDATE,
  PLATFORM_ROLE_DELETE,
  PLATFORM_ROLE_ENABLE,
  PLATFORM_ROLE_DISABLE,
  PLATFORM_ROLE_ASSIGN_PERMISSION,
  PLATFORM_ROLE_ASSIGN_MEMBER
} from '../../constants/permissions'
import type { PlatformRoleRepDTO } from '../../types/platform-roles'
import type { PlatformPermissionRepDTO, FlatPermission } from '../../types/platform-permissions'
import type { PlatformUserRepDTO } from '../../types/platform-users'

const { t } = useI18n()
const authStore = useAuthStore()

const pageLoading = ref(false)
const showDescription = ref(true)
const showGuide = ref(false)
const searchKeyword = ref('')
const searchStatus = ref<string | null>(null)
const gridRef = ref()

const statusOptions = [
  { value: null, label: t('全部') },
  { value: 'active', label: t('已启用') },
  { value: 'disabled', label: t('已禁用') }
]

// CustomStore for remote paging
const dataSource = new CustomStore({
  key: 'Id',
  load: async (loadOptions: LoadOptions) => {
    const page = loadOptions.skip !== undefined && loadOptions.take !== undefined
      ? Math.floor(loadOptions.skip / loadOptions.take) + 1
      : 1
    const pageSize = loadOptions.take || 20
    try {
      const result = await getPlatformRolesApi({
        page,
        pageSize,
        keyword: searchKeyword.value || undefined,
        status: searchStatus.value
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

function onSearch(): void {
  if (gridRef.value?.instance) {
    gridRef.value.instance.refresh()
  }
}

function onReset(): void {
  searchKeyword.value = ''
  searchStatus.value = null
  onSearch()
}

// Form dialog
const formDialogVisible = ref(false)
const isEditing = ref(false)
const editingId = ref<number | null>(null)
const submitting = ref(false)
const formRef = ref()
const formData = ref({
  Code: '',
  Name: '',
  Description: ''
})

function openCreateDialog(): void {
  isEditing.value = false
  editingId.value = null
  formData.value = { Code: '', Name: '', Description: '' }
  formDialogVisible.value = true
}

async function openEditDialog(row: PlatformRoleRepDTO): Promise<void> {
  isEditing.value = true
  editingId.value = row.Id
  try {
    const detail = await getPlatformRoleApi(row.Id)
    formData.value = {
      Code: detail.Code,
      Name: detail.Name,
      Description: detail.Description || ''
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

async function validateCodeUnique(params: { value: string }): Promise<boolean> {
  if (!params.value) return true
  try {
    const exists = await checkRoleCodeExistsApi(params.value)
    return !exists
  } catch {
    return true
  }
}

async function onSubmitForm(): Promise<void> {
  if (formRef.value?.instance) {
    const result = formRef.value.instance.validate()
    if (result && !result.isValid) return
  }
  submitting.value = true
  try {
    if (isEditing.value && editingId.value) {
      await updatePlatformRoleApi(editingId.value, {
        Name: formData.value.Name,
        Description: formData.value.Description || undefined
      })
      notifySuccess('更新成功')
    } else {
      await createPlatformRoleApi({
        Code: formData.value.Code,
        Name: formData.value.Name,
        Description: formData.value.Description || undefined
      })
      notifySuccess('创建成功')
    }
    closeFormDialog()
    onSearch()
  } catch {
    // error handled by interceptor
  } finally {
    submitting.value = false
  }
}

// Detail
const detailDialogVisible = ref(false)
const detailData = ref<PlatformRoleRepDTO | null>(null)

async function openDetail(row: PlatformRoleRepDTO): Promise<void> {
  try {
    detailData.value = await getPlatformRoleApi(row.Id)
    detailDialogVisible.value = true
  } catch {
    // error handled by interceptor
  }
}

// Enable / Disable / Delete
async function onEnable(row: PlatformRoleRepDTO): Promise<void> {
  const confirmed = await confirmAction('确认启用角色 {name}', { name: row.Name })
  if (!confirmed) return
  try {
    await enablePlatformRoleApi(row.Id)
    notifySuccess('启用成功')
    onSearch()
  } catch {
    // error handled by interceptor
  }
}

async function onDisable(row: PlatformRoleRepDTO): Promise<void> {
  const confirmed = await confirmAction('确认禁用角色 {name}', { name: row.Name })
  if (!confirmed) return
  try {
    await disablePlatformRoleApi(row.Id)
    notifySuccess('禁用成功')
    onSearch()
  } catch {
    // error handled by interceptor
  }
}

async function onDelete(row: PlatformRoleRepDTO): Promise<void> {
  const confirmed = await confirmDelete(row.Name)
  if (!confirmed) return
  try {
    await deletePlatformRoleApi(row.Id)
    notifySuccess('删除成功')
    onSearch()
  } catch {
    // error handled by interceptor
  }
}

// Permission binding
const permDialogVisible = ref(false)
const permTreeData = ref<FlatPermission[]>([])
const selectedPermKeys = ref<number[]>([])
const savingPerm = ref(false)
let permRoleId: number | null = null

function flattenPermTree(nodes: PlatformPermissionRepDTO[], result: FlatPermission[] = []): FlatPermission[] {
  for (const node of nodes) {
    result.push({
      Id: node.Id,
      Code: node.Code,
      Name: node.Name,
      PermissionType: node.PermissionType,
      ParentId: node.ParentId,
      Path: node.Path,
      Method: node.Method
    })
    if (node.Children && node.Children.length > 0) {
      flattenPermTree(node.Children, result)
    }
  }
  return result
}

async function openPermissionDialog(row: PlatformRoleRepDTO): Promise<void> {
  permRoleId = row.Id
  try {
    const [tree, existingIds] = await Promise.all([
      getPermissionTreeApi(),
      getRolePermissionsApi(row.Id)
    ])
    permTreeData.value = flattenPermTree(tree || [])
    selectedPermKeys.value = existingIds || []
    permDialogVisible.value = true
  } catch {
    // error handled by interceptor
  }
}

function onPermSelectionChanged(e: { selectedRowKeys: number[] }): void {
  selectedPermKeys.value = e.selectedRowKeys
}

async function onSavePermissions(): Promise<void> {
  if (!permRoleId) return
  savingPerm.value = true
  try {
    await bindRolePermissionsApi(permRoleId, selectedPermKeys.value)
    notifySuccess('保存成功')
    permDialogVisible.value = false
  } catch {
    // error handled by interceptor
  } finally {
    savingPerm.value = false
  }
}

// Member binding
const memberDialogVisible = ref(false)
const memberListData = ref<PlatformUserRepDTO[]>([])
const selectedMemberKeys = ref<number[]>([])
const savingMember = ref(false)
let memberRoleId: number | null = null

async function openMemberDialog(row: PlatformRoleRepDTO): Promise<void> {
  memberRoleId = row.Id
  try {
    const result = await getPlatformUsersApi({ Page: 1, PageSize: 100 })
    memberListData.value = result.Items || []
    selectedMemberKeys.value = []
    memberDialogVisible.value = true
  } catch {
    // error handled by interceptor
  }
}

function onMemberSelectionChanged(e: { selectedRowKeys: number[] }): void {
  selectedMemberKeys.value = e.selectedRowKeys
}

async function onSaveMembers(): Promise<void> {
  if (!memberRoleId) return
  savingMember.value = true
  try {
    await bindRoleMembersApi(memberRoleId, selectedMemberKeys.value)
    notifySuccess('保存成功')
    memberDialogVisible.value = false
  } catch {
    // error handled by interceptor
  } finally {
    savingMember.value = false
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

onMounted(() => {
  // Grid auto-loads via CustomStore
})
</script>

<style scoped>
.platform-roles-page {
  padding: 20px;
}

.page-title {
  margin: 0 0 4px 0;
  font-size: 24px;
  font-weight: 600;
  color: #333;
}

.page-subtitle {
  margin: 0 0 16px 0;
  font-size: 14px;
  color: #999;
}

.search-bar {
  display: flex;
  gap: 12px;
  margin-bottom: 16px;
  align-items: center;
}

.grid-toolbar {
  margin-bottom: 12px;
}

.action-buttons {
  display: flex;
  gap: 4px;
  flex-wrap: wrap;
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
</style>

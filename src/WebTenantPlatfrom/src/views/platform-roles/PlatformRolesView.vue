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

    <!-- Page Header -->
    <div class="page-header">
      <div class="page-header-text">
        <h2 class="page-title">{{ $t('平台角色管理') }}</h2>
      </div>
      <div class="page-header-actions">
        <FunctionDescriptionCard v-model:visible="showDescription">
          <h4>{{ $t('功能用途') }}</h4>
          <p>{{ $t('平台角色管理用于管理系统级角色定义。支持创建、编辑、删除角色，以及启用/禁用状态切换、权限分配和成员管理操作。') }}</p>
          <h4>{{ $t('字段说明') }}</h4>
          <ul>
            <li><strong>{{ $t('角色编码') }}</strong>：{{ $t('角色唯一标识，创建后不可修改，仅允许字母、数字、下划线和连字符') }}</li>
            <li><strong>{{ $t('角色名称') }}</strong>：{{ $t('角色显示名称，用于界面展示') }}</li>
            <li><strong>{{ $t('描述') }}</strong>：{{ $t('角色用途说明') }}</li>
            <li><strong>{{ $t('状态') }}</strong>：{{ $t('已启用/已禁用，禁用后角色权限不生效') }}</li>
          </ul>
          <h4>{{ $t('使用场景') }}</h4>
          <ul>
            <li>{{ $t('创建新角色并分配对应权限') }}</li>
            <li>{{ $t('为角色添加或移除成员用户') }}</li>
            <li>{{ $t('启用或禁用角色以控制权限生效范围') }}</li>
          </ul>
        </FunctionDescriptionCard>
        <OperationGuideDrawer v-model:visible="showGuide">
          <ol class="guide-steps">
            <li>{{ $t('step_search_role') }}</li>
            <li>{{ $t('step_create_role') }}</li>
            <li>{{ $t('step_edit_role') }}</li>
            <li>{{ $t('step_status_role') }}</li>
            <li>{{ $t('step_assign_perm') }}</li>
            <li>{{ $t('step_assign_member') }}</li>
            <li>{{ $t('step_delete_role') }}</li>
          </ol>
        </OperationGuideDrawer>
      </div>
    </div>

    <!-- Search Area -->
    <div class="search-area">
      <div class="search-row">
        <div class="search-field">
          <label class="search-label">{{ $t('关键词') }}</label>
          <DxTextBox
            v-model:value="searchKeyword"
            :placeholder="$t('请输入角色编码或名称')"
            :show-clear-button="true"
            width="220"
            @enter-key="onSearch"
          />
        </div>
        <div class="search-field">
          <label class="search-label">{{ $t('状态') }}</label>
          <DxSelectBox
            v-model:value="searchStatus"
            :items="statusOptions"
            display-expr="label"
            value-expr="value"
            :placeholder="$t('请选择状态')"
            width="150"
            :show-clear-button="true"
          />
        </div>
        <div class="search-actions">
          <DxButton :text="$t('查询')" icon="search" type="default" @click="onSearch" />
          <DxButton :text="$t('重置')" icon="refresh" styling-mode="outlined" @click="onReset" />
        </div>
      </div>
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
      :column-hiding-enabled="true"
      :remote-operations="true"
      :no-data-text="$t('暂无数据')"
      :focused-row-enabled="true"
      :focused-row-key="focusedRowKey"
      :auto-navigate-to-focused-row="true"
      key-expr="Id"
    >
      <DxPaging :page-size="20" />
      <DxPager
        :show-page-size-selector="true"
        :allowed-page-sizes="[10, 20, 50, 100]"
        :show-info="true"
        :show-navigation-buttons="true"
      />

      <DxColumn data-field="Id" :caption="$t('ID')" :width="80" :allow-sorting="false" :hiding-priority="0" />
      <DxColumn data-field="Code" :caption="$t('角色编码')" :allow-sorting="true" />
      <DxColumn data-field="Name" :caption="$t('角色名称')" :allow-sorting="true" />
      <DxColumn data-field="Description" :caption="$t('描述')" :allow-sorting="false" :hiding-priority="1" />
      <DxColumn data-field="Status" :caption="$t('状态')" :width="100" :allow-sorting="false" cell-template="statusCell" />
      <DxColumn data-field="CreatedAt" :caption="$t('创建时间')" :width="180" :allow-sorting="true" cell-template="dateCell" :hiding-priority="2" />
      <DxColumn :caption="$t('操作')" :width="200" :allow-sorting="false" cell-template="actionCell" />

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
            <span :class="detailData.Status === 'Active' ? 'status-enabled' : 'status-disabled'">
              {{ detailData.Status === 'Active' ? $t('已启用') : $t('已禁用') }}
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
      :height="'auto'"
      :max-height="'90vh'"
      :show-close-button="true"
      @hiding="permDialogVisible = false"
    >
      <template #content>
        <div class="perm-dialog-content">
          <!-- Quick Templates -->
          <div class="perm-templates">
            <span class="perm-templates-label">{{ $t('快捷模板') }}：</span>
            <DxButton
              v-for="tpl in permTemplates"
              :key="tpl.id"
              :text="tpl.label"
              styling-mode="outlined"
              @click="applyPermTemplate(tpl.id)"
            />
          </div>
          <!-- Search & Count -->
          <div class="perm-toolbar">
            <DxTextBox
              v-model:value="permSearchText"
              :placeholder="$t('搜索权限')"
              :show-clear-button="true"
              width="240"
              mode="search"
            />
            <span
              class="perm-count"
              :class="{ 'perm-count-clickable': true }"
              @click="togglePermFilter"
            >
              {{ $t('已选择 {selected}/{total} 项', { selected: selectedLeafPermCount, total: totalLeafPermCount }) }}
            </span>
          </div>
          <!-- Permission Tree (DxTreeView) -->
          <div class="perm-tree-wrapper">
            <DxTreeView
              ref="permTreeRef"
              :data-source="permTreeItems"
              :show-check-boxes-mode="'normal'"
              :selection-mode="'multiple'"
              :select-nodes-recursive="true"
              :search-enabled="false"
              :search-value="permSearchText"
              :search-expr="'Name'"
              key-expr="Id"
              parent-id-expr="ParentId"
              display-expr="Name"
              data-structure="plain"
              @selection-changed="onPermTreeSelectionChanged"
            />
          </div>
          <div class="perm-dialog-footer">
            <DxButton :text="$t('取消')" styling-mode="outlined" @click="permDialogVisible = false" />
            <DxButton :text="$t('保存')" type="default" :disabled="savingPerm" @click="onSavePermissions" />
          </div>
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
          <DxSelection mode="multiple" show-check-boxes-mode="always" />
          <DxColumn data-field="Username" :caption="$t('用户名')" />
          <DxColumn data-field="DisplayName" :caption="$t('显示名')" />
        </DxDataGrid>
        <div class="dialog-buttons">
          <DxButton :text="$t('取消')" styling-mode="outlined" @click="memberDialogVisible = false" />
          <DxButton :text="$t('保存')" type="default" :disabled="savingMember" @click="onSaveMembers" />
        </div>
      </template>
    </DxPopup>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, nextTick } from 'vue'
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
import { DxTreeView } from 'devextreme-vue/tree-view'
import { DxTextBox } from 'devextreme-vue/text-box'
import { DxSelectBox } from 'devextreme-vue/select-box'
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
  DxAsyncRule
} from 'devextreme-vue/validator'
import { DxLoadPanel } from 'devextreme-vue/load-panel'
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
  getRoleMembersApi,
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
const showDescription = ref(false)
const showGuide = ref(false)
const searchKeyword = ref('')
const searchStatus = ref<string | null>(null)
const gridRef = ref()
const focusedRowKey = ref<string | number | null>(null)

// Status options using computed for i18n reactivity
const statusOptions = computed(() => [
  { value: null, label: t('全部') },
  { value: 'Active', label: t('已启用') },
  { value: 'Disabled', label: t('已禁用') }
])

// CustomStore for remote paging + sorting
const dataSource = new CustomStore({
  key: 'Id',
  load: async (loadOptions: LoadOptions) => {
    const page = loadOptions.skip !== undefined && loadOptions.take !== undefined
      ? Math.floor(loadOptions.skip / loadOptions.take) + 1
      : 1
    const pageSize = loadOptions.take || 20

    // Extract sort params from loadOptions
    let sortField: string | undefined
    let sortOrder: string | undefined
    if (loadOptions.sort && Array.isArray(loadOptions.sort) && loadOptions.sort.length > 0) {
      const firstSort = loadOptions.sort[0] as { selector?: string; desc?: boolean }
      if (firstSort.selector) {
        sortField = firstSort.selector
        sortOrder = firstSort.desc ? 'desc' : 'asc'
      }
    }

    try {
      const result = await getPlatformRolesApi({
        Page: page,
        PageSize: pageSize,
        Keyword: searchKeyword.value || undefined,
        Status: searchStatus.value,
        SortField: sortField,
        SortOrder: sortOrder
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

// More actions dropdown for row operations (overflow pattern)
function getMoreActions(row: PlatformRoleRepDTO): Array<{ id: string; text: string; icon: string }> {
  const actions: Array<{ id: string; text: string; icon: string }> = []
  if (authStore.hasPermission(PLATFORM_ROLE_ENABLE) && row.Status === 'Disabled') {
    actions.push({ id: 'enable', text: t('启用'), icon: 'check' })
  }
  if (authStore.hasPermission(PLATFORM_ROLE_DISABLE) && row.Status === 'Active') {
    actions.push({ id: 'disable', text: t('禁用'), icon: 'close' })
  }
  if (authStore.hasPermission(PLATFORM_ROLE_ASSIGN_PERMISSION)) {
    actions.push({ id: 'assign-perm', text: t('分配权限'), icon: 'hierarchy' })
  }
  if (authStore.hasPermission(PLATFORM_ROLE_ASSIGN_MEMBER)) {
    actions.push({ id: 'assign-member', text: t('分配成员'), icon: 'group' })
  }
  if (authStore.hasPermission(PLATFORM_ROLE_DELETE)) {
    actions.push({ id: 'delete', text: t('删除'), icon: 'trash' })
  }
  return actions
}

function onMoreActionClick(e: { itemData: { id: string } }, row: PlatformRoleRepDTO): void {
  focusedRowKey.value = row.Id
  const actionId = e.itemData.id
  if (actionId === 'enable') onEnable(row)
  else if (actionId === 'disable') onDisable(row)
  else if (actionId === 'assign-perm') openPermissionDialog(row)
  else if (actionId === 'assign-member') openMemberDialog(row)
  else if (actionId === 'delete') onDelete(row)
}

// Form dialog
const formDialogVisible = ref(false)
const isEditing = ref(false)
const editingId = ref<string | number | null>(null)
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
  focusedRowKey.value = row.Id
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
      focusedRowKey.value = editingId.value
    } else {
      const newId = await createPlatformRoleApi({
        Code: formData.value.Code,
        Name: formData.value.Name,
        Description: formData.value.Description || undefined
      })
      notifySuccess('创建成功')
      if (newId) focusedRowKey.value = newId
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
  focusedRowKey.value = row.Id
  try {
    const detail = await getPlatformRoleApi(row.Id)
    if (detail) {
      detailData.value = detail
      detailDialogVisible.value = true
    }
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

// Permission binding (DxTreeView with templates, search, count)
const permDialogVisible = ref(false)
const permTreeRef = ref()
const permTreeItems = ref<FlatPermission[]>([])
const allPermItems = ref<FlatPermission[]>([])
const selectedPermKeys = ref<Array<string | number>>([])
const totalPermCount = ref(0)
const savingPerm = ref(false)
const permSearchText = ref('')
const showOnlySelected = ref(false)
let permRoleId: string | number | null = null

// Leaf node counts for permission selection display
const parentIds = computed(() => {
  const ids = new Set<string | number>()
  for (const item of allPermItems.value) {
    if (item.ParentId !== null && item.ParentId !== undefined) {
      ids.add(item.ParentId)
    }
  }
  return ids
})

const totalLeafPermCount = computed(() => {
  return allPermItems.value.filter(p => !parentIds.value.has(p.Id)).length
})

const selectedLeafPermCount = computed(() => {
  const pIds = parentIds.value
  return selectedPermKeys.value.filter(k => !pIds.has(k)).length
})

// Permission templates
const permTemplates = computed(() => [
  { id: 'admin', label: t('系统管理员') },
  { id: 'viewer', label: t('只读用户') }
])

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
  focusedRowKey.value = row.Id
  permSearchText.value = ''
  showOnlySelected.value = false
  try {
    const [tree, existingIds] = await Promise.all([
      getPermissionTreeApi(),
      getRolePermissionsApi(row.Id)
    ])
    const flat = flattenPermTree(tree || [])
    allPermItems.value = flat
    permTreeItems.value = flat
    totalPermCount.value = flat.length
    selectedPermKeys.value = existingIds || []
    permDialogVisible.value = true
    // After dialog opens, set selected keys on tree
    await nextTick()
    if (permTreeRef.value?.instance) {
      // Unselect all first
      permTreeRef.value.instance.unselectAll()
      // Select existing keys
      const ids = existingIds || []
      ids.forEach((id: string | number) => {
        permTreeRef.value.instance.selectItem(id)
      })
    }
  } catch {
    // error handled by interceptor
  }
}

function applyPermTemplate(templateId: string): void {
  if (!permTreeRef.value?.instance) return
  const tree = permTreeRef.value.instance
  if (templateId === 'admin') {
    // Select all
    tree.selectAll()
    selectedPermKeys.value = allPermItems.value.map(p => p.Id)
  } else if (templateId === 'viewer') {
    // Select only view/list permissions
    tree.unselectAll()
    const viewKeys: Array<string | number> = []
    for (const p of allPermItems.value) {
      const code = p.Code.toLowerCase()
      if (code.includes('view') || code.includes('list') || code.includes('detail')) {
        viewKeys.push(p.Id)
      }
    }
    viewKeys.forEach(key => tree.selectItem(key))
    selectedPermKeys.value = viewKeys
  }
}

function togglePermFilter(): void {
  showOnlySelected.value = !showOnlySelected.value
  if (showOnlySelected.value) {
    const selected = new Set(selectedPermKeys.value)
    // Also include parent nodes for selected items
    const parents = new Set<string | number>()
    for (const item of allPermItems.value) {
      if (selected.has(item.Id) && item.ParentId !== null) {
        parents.add(item.ParentId)
      }
    }
    permTreeItems.value = allPermItems.value.filter(p => selected.has(p.Id) || parents.has(p.Id))
  } else {
    permTreeItems.value = allPermItems.value
  }
}

function onPermTreeSelectionChanged(): void {
  if (!permTreeRef.value?.instance) return
  const nodes = permTreeRef.value.instance.getSelectedNodes()
  const keys: Array<string | number> = []
  nodes.forEach((node: { itemData?: { Id?: string | number } }) => {
    if (node.itemData && node.itemData.Id) {
      keys.push(node.itemData.Id)
    }
  })
  selectedPermKeys.value = keys
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
const selectedMemberKeys = ref<Array<string | number>>([])
const savingMember = ref(false)
let memberRoleId: string | number | null = null

async function openMemberDialog(row: PlatformRoleRepDTO): Promise<void> {
  memberRoleId = row.Id
  focusedRowKey.value = row.Id
  try {
    const [result, existingIds] = await Promise.all([
      getPlatformUsersApi({ Page: 1, PageSize: 100 }),
      getRoleMembersApi(row.Id)
    ])
    memberListData.value = result.Items || []
    selectedMemberKeys.value = existingIds || []
    memberDialogVisible.value = true
  } catch {
    // error handled by interceptor
  }
}

function onMemberSelectionChanged(e: { selectedRowKeys: Array<string | number> }): void {
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
</script>

<style scoped>
.platform-roles-page {
  padding: 16px;
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
  color: var(--base-text-color);
}

.search-area {
  background: var(--base-bg-darken-5, var(--base-bg));
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
  color: var(--base-text-color-alpha-7, var(--base-text-color));
}

.search-actions {
  display: flex;
  gap: 8px;
  align-items: flex-end;
}

.grid-toolbar {
  margin-bottom: 8px;
}

.action-buttons {
  display: flex;
  gap: 4px;
  align-items: center;
}

.status-enabled {
  color: var(--dx-color-success, #52c41a);
  background-color: rgba(82, 196, 26, 0.1);
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
}

.status-disabled {
  color: var(--dx-color-danger, #f5222d);
  background-color: rgba(245, 34, 45, 0.1);
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
  border-top: 1px solid var(--dx-color-border, #f0f0f0);
}

.detail-content {
  padding: 8px 0;
}

.detail-row {
  display: flex;
  padding: 8px 0;
  border-bottom: 1px solid var(--dx-color-border, #f5f5f5);
}

.detail-label {
  width: 120px;
  color: var(--base-text-color-alpha-7, var(--base-text-color));
  flex-shrink: 0;
}

.detail-value {
  flex: 1;
  color: var(--base-text-color);
}

.perm-dialog-content {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.perm-templates {
  display: flex;
  gap: 8px;
  align-items: center;
  flex-wrap: wrap;
}

.perm-templates-label {
  font-size: 13px;
  color: var(--base-text-color-alpha-7, var(--base-text-color));
}

.perm-toolbar {
  display: flex;
  align-items: center;
  gap: 16px;
}

.perm-count {
  font-size: 13px;
  color: var(--base-accent);
  white-space: nowrap;
}

.perm-count-clickable {
  cursor: pointer;
  text-decoration: underline;
}

.perm-tree-wrapper {
  overflow: auto;
  border: 1px solid var(--dx-color-border, #e8e8e8);
  border-radius: 4px;
  padding: 8px;
  min-height: 200px;
  max-height: 400px;
}

/* Last-level (leaf) permission items display horizontally */
.perm-tree-wrapper :deep(.dx-treeview-node:not(.dx-treeview-node-is-leaf) > .dx-treeview-node-container) {
  display: flex;
  flex-wrap: wrap;
  gap: 0;
}

.perm-tree-wrapper :deep(.dx-treeview-node-is-leaf) {
  display: inline-flex;
  min-width: 160px;
  max-width: 220px;
}

.perm-dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding-top: 12px;
  border-top: 1px solid var(--dx-color-border, #f0f0f0);
  flex-shrink: 0;
}

.guide-steps {
  padding-left: 20px;
  line-height: 2;
}

/* Mobile responsive */
@media (max-width: 768px) {
  .platform-roles-page {
    padding: 8px;
  }

  .page-title {
    font-size: 16px !important;
  }

  .search-area {
    padding: 8px 12px;
  }

  .search-row {
    flex-direction: column;
    gap: 8px;
  }

  .search-field {
    width: 100%;
    min-width: 0;
  }

  .search-field :deep(.dx-textbox),
  .search-field :deep(.dx-selectbox) {
    width: 100% !important;
    max-width: 100% !important;
  }

  .search-actions {
    width: 100%;
    flex-wrap: wrap;
  }
}
</style>

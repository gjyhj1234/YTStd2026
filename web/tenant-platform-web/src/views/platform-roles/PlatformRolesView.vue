<template>
  <div>
    <div class="page-header">
      <h2>{{ $t('平台角色管理') }}</h2>
      <div class="page-header-actions">
        <PageHelpEntry @click="showGuide = true" />
        <DxButton
          v-if="perm.has(PLATFORM_ROLE_CREATE)"
          :text="$t('新增角色')"
          icon="add"
          type="default"
          @click="openCreatePopup"
        />
      </div>
    </div>

    <FunctionDescriptionCard
      :purpose="$t('角色管理用于定义管理角色及权限范围')"
      :data-scope="$t('仅限平台级别角色非租户角色')"
      :permission-note="$t('需要角色查看权限')"
      :risk-note="$t('禁用角色将导致绑定用户失去权限')"
      :collapsible="true"
    />

    <div class="card">
      <div class="filter-bar">
        <DxTextBox
          v-model:value="filterKeyword"
          :placeholder="$t('搜索角色编码或名称')"
          :width="260"
          mode="search"
          value-change-event="input"
        />
        <DxSelectBox
          v-model:value="filterStatus"
          :items="statusOptions"
          display-expr="text"
          value-expr="value"
          :placeholder="$t('状态筛选')"
          :width="140"
          :show-clear-button="true"
        />
        <DxButton :text="$t('查询')" icon="search" @click="refreshGrid" />
      </div>

      <DxDataGrid
        ref="gridRef"
        :data-source="gridStore"
        :show-borders="true"
        :column-auto-width="true"
        :hover-state-enabled="true"
        :remote-operations="true"
        key-expr="Id"
      >
        <DxColumn data-field="Id" caption="ID" :width="80" :allow-sorting="false" />
        <DxColumn data-field="Code" :caption="$t('角色编码')" />
        <DxColumn data-field="Name" :caption="$t('角色名称')" />
        <DxColumn data-field="Description" :caption="$t('描述')" />
        <DxColumn data-field="Status" :caption="$t('状态')" cell-template="statusCell" :width="100" />
        <DxColumn data-field="CreatedAt" :caption="$t('创建时间')" cell-template="dateCell" />
        <DxColumn :caption="$t('操作')" cell-template="actionCell" :width="380" :allow-sorting="false" />
        <template #statusCell="{ data: cellData }">
          <StatusTag :status="cellData.value" />
        </template>
        <template #dateCell="{ data: cellData }">
          <span>{{ formatDateTime(cellData.value) }}</span>
        </template>
        <template #actionCell="{ data: cellData }">
          <DxButton
            v-if="perm.has(PLATFORM_ROLE_UPDATE)"
            :text="$t('编辑')"
            styling-mode="text"
            @click="onEdit(cellData.data)"
          />
          <DxButton
            v-if="cellData.data.Status === 'Active' && perm.has(PLATFORM_ROLE_DISABLE)"
            :text="$t('禁用')"
            styling-mode="text"
            type="danger"
            @click="onDisable(cellData.data.Id)"
          />
          <DxButton
            v-if="cellData.data.Status !== 'Active' && perm.has(PLATFORM_ROLE_ENABLE)"
            :text="$t('启用')"
            styling-mode="text"
            type="success"
            @click="onEnable(cellData.data.Id)"
          />
          <DxButton
            v-if="perm.has(PLATFORM_ROLE_ASSIGN_PERMISSION)"
            :text="$t('绑定权限')"
            styling-mode="text"
            @click="onBindPermissions(cellData.data)"
          />
          <DxButton
            v-if="perm.has(PLATFORM_ROLE_ASSIGN_MEMBER)"
            :text="$t('绑定成员')"
            styling-mode="text"
            @click="onBindMembers(cellData.data)"
          />
          <DxButton
            v-if="perm.has(PLATFORM_ROLE_DELETE)"
            :text="$t('删除')"
            styling-mode="text"
            type="danger"
            @click="onDeleteRole(cellData.data)"
          />
        </template>
        <DxPaging :page-size="20" />
        <DxPager :show-page-size-selector="true" :allowed-page-sizes="[10, 20, 50]" :show-info="true" />
      </DxDataGrid>
    </div>

    <!-- 新增角色弹窗 -->
    <DxPopup
      :visible="showCreatePopup"
      :title="$t('新增平台角色')"
      :width="480"
      :height="'auto'"
      :show-close-button="true"
      @hiding="showCreatePopup = false"
    >
      <DxForm ref="createFormRef" :form-data="createForm" :col-count="1" label-mode="floating">
        <DxSimpleItem data-field="Code" :validation-rules="codeRules">
          <DxLabel :text="$t('角色编码')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Name" :validation-rules="nameRules">
          <DxLabel :text="$t('角色名称')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Description" editor-type="dxTextArea">
          <DxLabel :text="$t('描述')" />
        </DxSimpleItem>
        <DxButtonItem>
          <DxButtonOptions :text="$t('提交')" type="default" :use-submit-behavior="false" @click="handleCreate" />
        </DxButtonItem>
      </DxForm>
    </DxPopup>

    <!-- 编辑角色弹窗 -->
    <DxPopup
      :visible="showEditPopup"
      :title="$t('编辑角色')"
      :width="480"
      :height="'auto'"
      :show-close-button="true"
      @hiding="showEditPopup = false"
    >
      <DxForm ref="editFormRef" :form-data="editForm" :col-count="1" label-mode="floating">
        <DxSimpleItem data-field="Name" :validation-rules="nameRules">
          <DxLabel :text="$t('角色名称')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Description" editor-type="dxTextArea">
          <DxLabel :text="$t('描述')" />
        </DxSimpleItem>
        <DxButtonItem>
          <DxButtonOptions :text="$t('保存')" type="default" :use-submit-behavior="false" @click="handleEdit" />
        </DxButtonItem>
      </DxForm>
    </DxPopup>

    <!-- 绑定权限弹窗 -->
    <DxPopup
      :visible="showPermPopup"
      :title="$t('绑定权限') + ' — ' + bindingRoleName"
      :width="600"
      :height="480"
      :show-close-button="true"
      @hiding="showPermPopup = false"
    >
      <DxTreeList
        :data-source="permTreeData"
        :show-borders="true"
        :column-auto-width="true"
        key-expr="Id"
        parent-id-expr="ParentId"
        :auto-expand-all="true"
        :selected-row-keys="boundPermissionIds"
        @selection-changed="onPermSelectionChanged"
      >
        <DxTreeSelection mode="multiple" :recursive="true" />
        <DxTreeColumn data-field="Code" :caption="$t('权限编码')" />
        <DxTreeColumn data-field="Name" :caption="$t('权限名称')" />
      </DxTreeList>
      <div style="text-align: right; margin-top: 12px">
        <DxButton :text="$t('保存')" type="default" @click="handleBindPermissions" />
      </div>
    </DxPopup>

    <!-- 绑定成员弹窗 -->
    <DxPopup
      :visible="showMemberPopup"
      :title="$t('绑定成员') + ' — ' + bindingRoleName"
      :width="500"
      :height="'auto'"
      :show-close-button="true"
      @hiding="showMemberPopup = false"
    >
      <DxDataGrid
        :data-source="allUsersData"
        :show-borders="true"
        :column-auto-width="true"
        :hover-state-enabled="true"
        key-expr="Id"
        :selected-row-keys="boundMemberIds"
        @selection-changed="onMemberSelectionChanged"
      >
        <DxMemberSelection mode="multiple" :show-check-boxes-mode="'always'" />
        <DxMemberColumn data-field="Username" :caption="$t('用户名')" />
        <DxMemberColumn data-field="DisplayName" :caption="$t('显示名称')" />
      </DxDataGrid>
      <div style="text-align: right; margin-top: 12px">
        <DxButton :text="$t('保存')" type="default" @click="handleBindMembers" />
      </div>
    </DxPopup>

    <OperationGuideDrawer
      v-model:visible="showGuide"
      :title="$t('角色管理操作指引')"
      :entry-path="$t('角色管理入口路径')"
      :steps="guideSteps"
      :field-notes="guideFieldNotes"
      :error-notes="guideErrorNotes"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { DxDataGrid, DxColumn as DxMemberColumn, DxPaging, DxPager, DxSelection as DxMemberSelection } from 'devextreme-vue/data-grid'
import { DxTreeList, DxSelection as DxTreeSelection, DxColumn as DxTreeColumn } from 'devextreme-vue/tree-list'
import { DxButton } from 'devextreme-vue/button'
import { DxTextBox } from 'devextreme-vue/text-box'
import { DxSelectBox } from 'devextreme-vue/select-box'
import { DxPopup } from 'devextreme-vue/popup'
import { DxForm, DxSimpleItem, DxLabel, DxButtonItem, DxButtonOptions } from 'devextreme-vue/form'
import CustomStore from 'devextreme/data/custom_store'
import { useI18n } from 'vue-i18n'
import StatusTag from '@/components/StatusTag.vue'
import FunctionDescriptionCard from '@/components/help/FunctionDescriptionCard.vue'
import OperationGuideDrawer from '@/components/help/OperationGuideDrawer.vue'
import PageHelpEntry from '@/components/help/PageHelpEntry.vue'
import { usePermission } from '@/composables/usePermission'
import { notifySuccess, confirmDelete, confirmAction } from '@/composables/useNotify'
import { formatDateTime } from '@/utils/format'
import {
  getPlatformRoles,
  createPlatformRole,
  updatePlatformRole,
  deletePlatformRole,
  enablePlatformRole,
  disablePlatformRole,
  getRolePermissions,
  bindRolePermissions,
  bindRoleMembers,
  checkRoleCodeExists,
  type PlatformRoleRepDTO,
  type CreatePlatformRoleReqDTO,
} from '@/api/platformRoles'
import { getPermissions, type PlatformPermissionRepDTO } from '@/api/platformPermissions'
import { getPlatformUsers, type PlatformUserRepDTO } from '@/api/platformUsers'
import {
  PLATFORM_ROLE_CREATE,
  PLATFORM_ROLE_UPDATE,
  PLATFORM_ROLE_DELETE,
  PLATFORM_ROLE_ENABLE,
  PLATFORM_ROLE_DISABLE,
  PLATFORM_ROLE_ASSIGN_PERMISSION,
  PLATFORM_ROLE_ASSIGN_MEMBER,
} from '@/constants/permissions'

interface FlatPermission {
  Id: number
  Code: string
  Name: string
  ParentId: number | null
}

const perm = usePermission()
const { t } = useI18n()
const showGuide = ref(false)
const showCreatePopup = ref(false)
const showEditPopup = ref(false)
const showPermPopup = ref(false)
const showMemberPopup = ref(false)
const filterKeyword = ref('')
const filterStatus = ref<string | undefined>(undefined)
const editingRoleId = ref<number>(0)
const bindingRoleId = ref<number>(0)
const bindingRoleName = ref('')
const boundPermissionIds = ref<number[]>([])
const selectedPermissionIds = ref<number[]>([])
const boundMemberIds = ref<number[]>([])
const selectedMemberIds = ref<number[]>([])
const gridRef = ref<InstanceType<typeof DxDataGrid> | null>(null)
const createFormRef = ref<InstanceType<typeof DxForm> | null>(null)
const editFormRef = ref<InstanceType<typeof DxForm> | null>(null)

const statusOptions = computed(() => [
  { text: t('enum.PlatformRoleStatus.Active'), value: 'Active' },
  { text: t('enum.PlatformRoleStatus.Disabled'), value: 'Disabled' },
])

const permTreeData = ref<FlatPermission[]>([])
const allUsersData = ref<PlatformUserRepDTO[]>([])

const gridStore = new CustomStore({
  key: 'Id',
  load: async (loadOptions) => {
    const page = ((loadOptions.skip ?? 0) / (loadOptions.take ?? 20)) + 1
    const pageSize = loadOptions.take ?? 20
    const res = await getPlatformRoles({
      Page: page,
      PageSize: pageSize,
      Keyword: filterKeyword.value || undefined,
      Status: filterStatus.value || undefined,
    })
    return {
      data: res.Data!.Items,
      totalCount: res.Data!.Total,
    }
  },
})

const createForm = reactive<CreatePlatformRoleReqDTO>({
  Code: '',
  Name: '',
  Description: '',
})

const editForm = reactive({ Name: '', Description: '' })

const codeRules = computed(() => [
  { type: 'required' as const, message: t('角色编码不能为空') },
  { type: 'stringLength' as const, min: 1, max: 100, message: t('角色编码最长100字') },
  {
    type: 'async' as const,
    validationCallback: async (params: { value?: string }) => {
      if (!params.value) return true
      const res = await checkRoleCodeExists(params.value)
      if (res.Data === true) throw new Error(t('角色编码已存在'))
      return true
    },
  },
])

const nameRules = computed(() => [
  { type: 'required' as const, message: t('角色名称不能为空') },
  { type: 'stringLength' as const, max: 100, message: t('角色名称最长100字') },
])

function flattenPermTree(nodes: PlatformPermissionRepDTO[]): FlatPermission[] {
  const result: FlatPermission[] = []
  for (const node of nodes) {
    result.push({ Id: node.Id, Code: node.Code, Name: node.Name, ParentId: node.ParentId })
    if (node.Children && node.Children.length > 0) {
      result.push(...flattenPermTree(node.Children))
    }
  }
  return result
}

function refreshGrid() {
  gridRef.value?.instance?.refresh()
}

function openCreatePopup() {
  Object.assign(createForm, { Code: '', Name: '', Description: '' })
  showCreatePopup.value = true
}

async function handleCreate() {
  const formInstance = createFormRef.value?.instance
  if (formInstance) {
    const result = formInstance.validate()
    if (!result.isValid) return
  }
  await createPlatformRole(createForm)
  showCreatePopup.value = false
  notifySuccess('创建成功')
  refreshGrid()
}

function onEdit(role: PlatformRoleRepDTO) {
  editingRoleId.value = role.Id
  Object.assign(editForm, { Name: role.Name, Description: role.Description ?? '' })
  showEditPopup.value = true
}

async function handleEdit() {
  const formInstance = editFormRef.value?.instance
  if (formInstance) {
    const result = formInstance.validate()
    if (!result.isValid) return
  }
  await updatePlatformRole(editingRoleId.value, editForm)
  showEditPopup.value = false
  notifySuccess('更新成功')
  refreshGrid()
}

async function onEnable(id: number) {
  const confirmed = await confirmAction('确认启用此角色')
  if (!confirmed) return
  await enablePlatformRole(id)
  notifySuccess('启用成功')
  refreshGrid()
}

async function onDisable(id: number) {
  const confirmed = await confirmAction('确认禁用此角色')
  if (!confirmed) return
  await disablePlatformRole(id)
  notifySuccess('禁用成功')
  refreshGrid()
}

async function onDeleteRole(role: PlatformRoleRepDTO) {
  const confirmed = await confirmDelete(role.Name)
  if (!confirmed) return
  await deletePlatformRole(role.Id)
  notifySuccess('删除成功')
  refreshGrid()
}

async function onBindPermissions(role: PlatformRoleRepDTO) {
  bindingRoleId.value = role.Id
  bindingRoleName.value = role.Name
  const [permRes, boundRes] = await Promise.all([
    getPermissions(),
    getRolePermissions(role.Id),
  ])
  permTreeData.value = flattenPermTree(permRes.Data!)
  boundPermissionIds.value = boundRes.Data ?? []
  selectedPermissionIds.value = boundRes.Data ?? []
  showPermPopup.value = true
}

function onPermSelectionChanged(e: { selectedRowKeys: number[] }) {
  selectedPermissionIds.value = e.selectedRowKeys
}

async function handleBindPermissions() {
  await bindRolePermissions(bindingRoleId.value, { PermissionIds: selectedPermissionIds.value })
  showPermPopup.value = false
  notifySuccess('保存成功')
}

async function onBindMembers(role: PlatformRoleRepDTO) {
  bindingRoleId.value = role.Id
  bindingRoleName.value = role.Name
  const usersRes = await getPlatformUsers({ Page: 1, PageSize: 100 })
  allUsersData.value = usersRes.Data!.Items
  boundMemberIds.value = []
  selectedMemberIds.value = []
  showMemberPopup.value = true
}

function onMemberSelectionChanged(e: { selectedRowKeys: number[] }) {
  selectedMemberIds.value = e.selectedRowKeys
}

async function handleBindMembers() {
  await bindRoleMembers(bindingRoleId.value, { UserIds: selectedMemberIds.value })
  showMemberPopup.value = false
  notifySuccess('保存成功')
}

const guideSteps = computed(() => [
  t('点击新增角色按钮填写编码名称和描述'),
  t('在列表中搜索或按状态筛选角色'),
  t('点击编辑修改角色信息'),
  t('点击禁用启用切换角色状态'),
  t('点击绑定权限为角色分配权限码'),
  t('点击绑定成员将用户添加到角色'),
])
const guideFieldNotes = computed(() => [
  t('角色编码全局唯一创建后不可修改'),
  t('角色名称用于界面显示的友好名称'),
  t('描述填写角色的职责和权限范围'),
])
const guideErrorNotes = computed(() => [
  t('角色编码已存在时创建将失败'),
  t('禁用角色后绑定用户将失去对应权限'),
  t('删除角色前需先解除权限和成员绑定'),
])
</script>

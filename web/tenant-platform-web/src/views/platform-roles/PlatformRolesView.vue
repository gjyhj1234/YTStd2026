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
          @click="showCreatePopup = true"
        />
      </div>
    </div>

    <FunctionDescriptionCard
      purpose="平台角色管理用于定义不同管理角色及其权限范围，支持角色的创建、编辑、启禁用以及权限和成员绑定。"
      data-scope="仅限平台级别角色，非租户角色。"
      permission-note="需要 platform:role:view 权限查看列表，platform:role:create 创建角色。"
      risk-note="禁用角色将导致绑定该角色的用户失去对应权限。"
      :collapsible="true"
    />

    <div class="card">
      <div class="filter-bar">
        <DxTextBox
          v-model:value="filterKeyword"
          :placeholder="$t('搜索角色编码 / 名称')"
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
        <DxButton :text="$t('查询')" icon="search" @click="loadData" />
      </div>

      <DxDataGrid
        :data-source="gridData"
        :show-borders="true"
        :column-auto-width="true"
        :hover-state-enabled="true"
        key-expr="Id"
      >
        <DxColumn data-field="Id" :caption="$t('common.id')" :width="60" />
        <DxColumn data-field="Code" :caption="$t('角色编码')" />
        <DxColumn data-field="Name" :caption="$t('角色名称')" />
        <DxColumn data-field="Description" :caption="$t('common.description')" />
        <DxColumn data-field="Status" :caption="$t('common.status')" cell-template="statusCell" :width="100" />
        <DxColumn data-field="CreatedAt" :caption="$t('common.createdAt')" cell-template="dateCell" />
        <DxColumn :caption="$t('common.actions')" cell-template="actionCell" :width="340" />
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
            :text="$t('common.disable')"
            styling-mode="text"
            type="danger"
            @click="onDisable(cellData.data.Id)"
          />
          <DxButton
            v-if="cellData.data.Status !== 'Active' && perm.has(PLATFORM_ROLE_ENABLE)"
            :text="$t('common.enable')"
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
      <DxForm :form-data="createForm" :col-count="1" label-mode="floating">
        <DxSimpleItem data-field="Code">
          <DxLabel :text="$t('角色编码')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Name">
          <DxLabel :text="$t('角色名称')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Description" editor-type="dxTextArea">
          <DxLabel :text="$t('common.description')" />
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
      <DxForm :form-data="editForm" :col-count="1" label-mode="floating">
        <DxSimpleItem data-field="Name">
          <DxLabel :text="$t('角色名称')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Description" editor-type="dxTextArea">
          <DxLabel :text="$t('common.description')" />
        </DxSimpleItem>
        <DxButtonItem>
          <DxButtonOptions :text="$t('common.save')" type="default" :use-submit-behavior="false" @click="handleEdit" />
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
      >
        <DxSelection mode="multiple" :recursive="true" />
        <DxColumn data-field="Code" :caption="$t('权限编码')" />
        <DxColumn data-field="Name" :caption="$t('权限名称')" />
      </DxTreeList>
      <div style="text-align: right; margin-top: 12px">
        <DxButton :text="$t('common.save')" type="default" @click="handleBindPermissions" />
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
      <p style="color: #999; font-size: 13px">{{ $t('成员绑定功能将在后续阶段完善') }}</p>
    </DxPopup>

    <OperationGuideDrawer
      v-model:visible="showGuide"
      title="角色管理操作指引"
      entry-path="左侧菜单 → 平台管理体系 → 角色管理"
      :steps="guideSteps"
      :field-notes="guideFieldNotes"
      :error-notes="guideErrorNotes"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { DxDataGrid, DxColumn, DxPaging, DxPager } from 'devextreme-vue/data-grid'
import { DxTreeList, DxSelection } from 'devextreme-vue/tree-list'
import { DxButton } from 'devextreme-vue/button'
import { DxTextBox } from 'devextreme-vue/text-box'
import { DxSelectBox } from 'devextreme-vue/select-box'
import { DxPopup } from 'devextreme-vue/popup'
import { DxForm, DxSimpleItem, DxLabel, DxButtonItem, DxButtonOptions } from 'devextreme-vue/form'
import { useI18n } from 'vue-i18n'
import StatusTag from '@/components/StatusTag.vue'
import FunctionDescriptionCard from '@/components/help/FunctionDescriptionCard.vue'
import OperationGuideDrawer from '@/components/help/OperationGuideDrawer.vue'
import PageHelpEntry from '@/components/help/PageHelpEntry.vue'
import { usePermission } from '@/composables/usePermission'
import { formatDateTime } from '@/utils/format'
import {
  getPlatformRoles,
  createPlatformRole,
  updatePlatformRole,
  enablePlatformRole,
  disablePlatformRole,
  bindRolePermissions,
  type PlatformRoleRepDTO,
  type CreatePlatformRoleReqDTO,
} from '@/api/platformRoles'
import { getPermissions, type PlatformPermissionRepDTO } from '@/api/platformPermissions'
import {
  PLATFORM_ROLE_CREATE,
  PLATFORM_ROLE_UPDATE,
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

const statusOptions = computed(() => [
  { text: t('status.Active'), value: 'Active' },
  { text: t('status.Disabled'), value: 'Disabled' },
])

const gridData = ref<PlatformRoleRepDTO[]>([])
const permTreeData = ref<FlatPermission[]>([])

const createForm = reactive<CreatePlatformRoleReqDTO>({
  Code: '',
  Name: '',
  Description: '',
})

const editForm = reactive({ Name: '', Description: '' })

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

async function loadData() {
  try {
    const res = await getPlatformRoles({
      Page: 1,
      PageSize: 20,
      Keyword: filterKeyword.value || undefined,
      Status: filterStatus.value || undefined,
    })
    gridData.value = res.Data!.Items
  } catch {
    // 接口未就绪时保持空列表
  }
}

async function handleCreate() {
  try {
    await createPlatformRole(createForm)
    showCreatePopup.value = false
    Object.assign(createForm, { Code: '', Name: '', Description: '' })
    await loadData()
  } catch {
    // 错误由 http 层统一处理
  }
}

function onEdit(role: PlatformRoleRepDTO) {
  editingRoleId.value = role.Id
  Object.assign(editForm, { Name: role.Name, Description: role.Description ?? '' })
  showEditPopup.value = true
}

async function handleEdit() {
  try {
    await updatePlatformRole(editingRoleId.value, editForm)
    showEditPopup.value = false
    await loadData()
  } catch {
    // 错误由 http 层统一处理
  }
}

async function onEnable(id: number) {
  try {
    await enablePlatformRole(id)
    await loadData()
  } catch {
    // 错误由 http 层统一处理
  }
}

async function onDisable(id: number) {
  try {
    await disablePlatformRole(id)
    await loadData()
  } catch {
    // 错误由 http 层统一处理
  }
}

async function onBindPermissions(role: PlatformRoleRepDTO) {
  bindingRoleId.value = role.Id
  bindingRoleName.value = role.Name
  try {
    const res = await getPermissions()
    permTreeData.value = flattenPermTree(res.Data!)
  } catch {
    permTreeData.value = []
  }
  showPermPopup.value = true
}

async function handleBindPermissions() {
  try {
    await bindRolePermissions(bindingRoleId.value, { PermissionIds: [] })
    showPermPopup.value = false
  } catch {
    // 错误由 http 层统一处理
  }
}

function onBindMembers(role: PlatformRoleRepDTO) {
  bindingRoleId.value = role.Id
  bindingRoleName.value = role.Name
  showMemberPopup.value = true
}

const guideSteps = [
  '点击【新增角色】按钮填写角色编码、名称和描述',
  '在列表中搜索或按状态筛选角色',
  '点击【编辑】修改角色信息',
  '点击【禁用/启用】切换角色状态',
  '点击【绑定权限】为角色分配权限码',
  '点击【绑定成员】将用户添加到角色',
]
const guideFieldNotes = [
  '角色编码：全局唯一，创建后不可修改',
  '角色名称：用于界面显示的友好名称',
  '描述：角色的职责和权限范围说明',
]
const guideErrorNotes = [
  '角色编码已存在时创建将失败',
  '禁用角色后绑定该角色的用户将失去对应权限',
  '删除角色前需先解除所有权限和成员绑定',
]

onMounted(loadData)
</script>

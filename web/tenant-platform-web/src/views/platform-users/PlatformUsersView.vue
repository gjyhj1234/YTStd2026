<template>
  <div>
    <div class="page-header">
      <h2>{{ $t('平台用户管理') }}</h2>
      <div class="page-header-actions">
        <PageHelpEntry @click="showGuide = true" />
        <DxButton
          v-if="perm.has(PLATFORM_USER_LOCK)"
          :text="$t('批量启用')"
          icon="check"
          styling-mode="outlined"
          :disabled="selectedIds.length === 0"
          @click="onBatchEnable"
        />
        <DxButton
          v-if="perm.has(PLATFORM_USER_UNLOCK)"
          :text="$t('批量禁用')"
          icon="close"
          styling-mode="outlined"
          :disabled="selectedIds.length === 0"
          @click="onBatchDisable"
        />
        <DxButton
          v-if="perm.has(PLATFORM_USER_CREATE)"
          :text="$t('新增用户')"
          icon="add"
          type="default"
          @click="openCreatePopup"
        />
      </div>
    </div>

    <FunctionDescriptionCard
      :purpose="$t('管理平台管理员账号')"
      :data-scope="$t('仅限平台管理员非租户用户')"
      :permission-note="$t('需要用户查看权限')"
      :risk-note="$t('禁用用户将导致无法登录')"
      :collapsible="true"
    />

    <div class="card">
      <div class="filter-bar">
        <DxTextBox
          v-model:value="filterKeyword"
          :placeholder="$t('搜索用户名或邮箱或姓名')"
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
        @selection-changed="onSelectionChanged"
      >
        <DxSelection mode="multiple" :show-check-boxes-mode="'always'" />
        <DxColumn data-field="Id" caption="ID" :width="80" :allow-sorting="false" />
        <DxColumn data-field="Username" :caption="$t('用户名')" />
        <DxColumn data-field="DisplayName" :caption="$t('显示名称')" />
        <DxColumn data-field="Email" :caption="$t('邮箱')" />
        <DxColumn data-field="Phone" :caption="$t('手机')" />
        <DxColumn data-field="Status" :caption="$t('状态')" cell-template="statusCell" :width="100" />
        <DxColumn data-field="LastLoginAt" :caption="$t('最后登录')" cell-template="dateCell" />
        <DxColumn data-field="CreatedAt" :caption="$t('创建时间')" cell-template="dateCell" />
        <DxColumn :caption="$t('操作')" cell-template="actionCell" :width="320" :allow-sorting="false" />
        <template #statusCell="{ data: cellData }">
          <StatusTag :status="cellData.value" />
        </template>
        <template #dateCell="{ data: cellData }">
          <span>{{ formatDateTime(cellData.value) }}</span>
        </template>
        <template #actionCell="{ data: cellData }">
          <DxButton
            :text="$t('详情')"
            styling-mode="text"
            @click="onShowDetail(cellData.data)"
          />
          <DxButton
            v-if="perm.has(PLATFORM_USER_UPDATE)"
            :text="$t('编辑')"
            styling-mode="text"
            @click="onEdit(cellData.data)"
          />
          <DxButton
            v-if="cellData.data.Status === 'Active' && perm.has(PLATFORM_USER_LOCK)"
            :text="$t('禁用')"
            styling-mode="text"
            type="danger"
            @click="onDisable(cellData.data.Id)"
          />
          <DxButton
            v-if="cellData.data.Status !== 'Active' && perm.has(PLATFORM_USER_UNLOCK)"
            :text="$t('启用')"
            styling-mode="text"
            type="success"
            @click="onEnable(cellData.data.Id)"
          />
          <DxButton
            v-if="perm.has(PLATFORM_USER_RESET_PASSWORD)"
            :text="$t('重置密码')"
            styling-mode="text"
            @click="onResetPassword(cellData.data)"
          />
          <DxButton
            v-if="perm.has(PLATFORM_USER_DELETE)"
            :text="$t('删除')"
            styling-mode="text"
            type="danger"
            @click="onDelete(cellData.data)"
          />
        </template>
        <DxPaging :page-size="20" />
        <DxPager :show-page-size-selector="true" :allowed-page-sizes="[10, 20, 50]" :show-info="true" />
      </DxDataGrid>
    </div>

    <!-- 新增用户弹窗 -->
    <DxPopup
      :visible="showCreatePopup"
      :title="$t('新增平台用户')"
      :width="480"
      :height="'auto'"
      :show-close-button="true"
      @hiding="showCreatePopup = false"
    >
      <DxForm
        ref="createFormRef"
        :form-data="createForm"
        :col-count="1"
        label-mode="floating"
      >
        <DxSimpleItem data-field="Username" :validation-rules="createUsernameRules">
          <DxLabel :text="$t('用户名')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="DisplayName" :validation-rules="requiredDisplayNameRules">
          <DxLabel :text="$t('显示名称')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Email" :validation-rules="emailRules">
          <DxLabel :text="$t('邮箱')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Phone">
          <DxLabel :text="$t('手机')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Password" :editor-options="{ mode: 'password' }" :validation-rules="passwordRules">
          <DxLabel :text="$t('密码')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Remark" editor-type="dxTextArea">
          <DxLabel :text="$t('备注')" />
        </DxSimpleItem>
        <DxButtonItem>
          <DxButtonOptions :text="$t('提交')" type="default" :use-submit-behavior="false" @click="handleCreate" />
        </DxButtonItem>
      </DxForm>
    </DxPopup>

    <!-- 编辑用户弹窗 -->
    <DxPopup
      :visible="showEditPopup"
      :title="$t('编辑用户')"
      :width="480"
      :height="'auto'"
      :show-close-button="true"
      @hiding="showEditPopup = false"
    >
      <DxForm
        ref="editFormRef"
        :form-data="editForm"
        :col-count="1"
        label-mode="floating"
      >
        <DxSimpleItem data-field="DisplayName" :validation-rules="requiredDisplayNameRules">
          <DxLabel :text="$t('显示名称')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Email" :validation-rules="emailRules">
          <DxLabel :text="$t('邮箱')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Phone">
          <DxLabel :text="$t('手机')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Remark" editor-type="dxTextArea">
          <DxLabel :text="$t('备注')" />
        </DxSimpleItem>
        <DxButtonItem>
          <DxButtonOptions :text="$t('保存')" type="default" :use-submit-behavior="false" @click="handleEdit" />
        </DxButtonItem>
      </DxForm>
    </DxPopup>

    <!-- 重置密码弹窗 -->
    <DxPopup
      :visible="showResetPwdPopup"
      :title="$t('重置密码')"
      :width="400"
      :height="'auto'"
      :show-close-button="true"
      @hiding="showResetPwdPopup = false"
    >
      <DxForm
        ref="resetPwdFormRef"
        :form-data="resetPwdForm"
        :col-count="1"
        label-mode="floating"
      >
        <DxSimpleItem data-field="NewPassword" :editor-options="{ mode: 'password' }" :validation-rules="passwordRules">
          <DxLabel :text="$t('新密码')" />
        </DxSimpleItem>
        <DxButtonItem>
          <DxButtonOptions :text="$t('提交')" type="default" :use-submit-behavior="false" @click="handleResetPassword" />
        </DxButtonItem>
      </DxForm>
    </DxPopup>

    <!-- 详情弹窗 -->
    <DxPopup
      :visible="showDetailPopup"
      :title="$t('用户详情')"
      :width="500"
      :height="'auto'"
      :show-close-button="true"
      @hiding="showDetailPopup = false"
    >
      <div v-if="detailUser" class="detail-list">
        <div class="detail-item"><span class="detail-label">ID</span><span>{{ detailUser.Id }}</span></div>
        <div class="detail-item"><span class="detail-label">{{ $t('用户名') }}</span><span>{{ detailUser.Username }}</span></div>
        <div class="detail-item"><span class="detail-label">{{ $t('显示名称') }}</span><span>{{ detailUser.DisplayName }}</span></div>
        <div class="detail-item"><span class="detail-label">{{ $t('邮箱') }}</span><span>{{ detailUser.Email }}</span></div>
        <div class="detail-item"><span class="detail-label">{{ $t('手机') }}</span><span>{{ detailUser.Phone ?? '-' }}</span></div>
        <div class="detail-item"><span class="detail-label">{{ $t('状态') }}</span><StatusTag :status="detailUser.Status" /></div>
        <div class="detail-item"><span class="detail-label">{{ $t('最后登录') }}</span><span>{{ formatDateTime(detailUser.LastLoginAt) }}</span></div>
        <div class="detail-item"><span class="detail-label">{{ $t('创建时间') }}</span><span>{{ formatDateTime(detailUser.CreatedAt) }}</span></div>
      </div>
    </DxPopup>

    <OperationGuideDrawer
      v-model:visible="showGuide"
      :title="$t('用户管理操作指引')"
      :entry-path="$t('用户管理入口路径')"
      :steps="guideSteps"
      :field-notes="guideFieldNotes"
      :error-notes="guideErrorNotes"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { DxDataGrid, DxColumn, DxPaging, DxPager, DxSelection } from 'devextreme-vue/data-grid'
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
  getPlatformUsers,
  createPlatformUser,
  updatePlatformUser,
  deletePlatformUser,
  enablePlatformUser,
  disablePlatformUser,
  resetPlatformUserPassword,
  checkUsernameExists,
  batchEnablePlatformUsers,
  batchDisablePlatformUsers,
  type PlatformUserRepDTO,
  type CreatePlatformUserReqDTO,
  type UpdatePlatformUserReqDTO,
} from '@/api/platformUsers'
import {
  PLATFORM_USER_CREATE,
  PLATFORM_USER_UPDATE,
  PLATFORM_USER_DELETE,
  PLATFORM_USER_RESET_PASSWORD,
  PLATFORM_USER_LOCK,
  PLATFORM_USER_UNLOCK,
} from '@/constants/permissions'

const perm = usePermission()
const { t } = useI18n()
const showGuide = ref(false)
const showCreatePopup = ref(false)
const showEditPopup = ref(false)
const showResetPwdPopup = ref(false)
const showDetailPopup = ref(false)
const filterKeyword = ref('')
const filterStatus = ref<string | undefined>(undefined)
const editingUserId = ref<number>(0)
const resetPwdUserId = ref<number>(0)
const detailUser = ref<PlatformUserRepDTO | null>(null)
const selectedIds = ref<number[]>([])
const gridRef = ref<InstanceType<typeof DxDataGrid> | null>(null)
const createFormRef = ref<InstanceType<typeof DxForm> | null>(null)
const editFormRef = ref<InstanceType<typeof DxForm> | null>(null)
const resetPwdFormRef = ref<InstanceType<typeof DxForm> | null>(null)

const statusOptions = computed(() => [
  { text: t('enum.PlatformUserStatus.Active'), value: 'Active' },
  { text: t('enum.PlatformUserStatus.Disabled'), value: 'Disabled' },
  { text: t('enum.PlatformUserStatus.Locked'), value: 'Locked' },
])

const gridStore = new CustomStore({
  key: 'Id',
  load: async (loadOptions) => {
    const page = ((loadOptions.skip ?? 0) / (loadOptions.take ?? 20)) + 1
    const pageSize = loadOptions.take ?? 20
    const res = await getPlatformUsers({
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

const createForm = reactive<CreatePlatformUserReqDTO>({
  Username: '',
  DisplayName: '',
  Email: '',
  Phone: '',
  Password: '',
  Remark: '',
})

const editForm = reactive<UpdatePlatformUserReqDTO>({
  DisplayName: '',
  Email: '',
  Phone: '',
  Remark: '',
})

const resetPwdForm = reactive({ NewPassword: '' })

const createUsernameRules = computed(() => [
  { type: 'required' as const, message: t('用户名不能为空') },
  { type: 'stringLength' as const, min: 2, max: 50, message: t('用户名长度2到50') },
  {
    type: 'async' as const,
    validationCallback: async (params: { value: string }) => {
      if (!params.value || params.value.length < 2) return true
      const res = await checkUsernameExists(params.value)
      if (res.Data === true) throw new Error(t('用户名已存在'))
      return true
    },
  },
])

const requiredDisplayNameRules = computed(() => [
  { type: 'required' as const, message: t('显示名称不能为空') },
  { type: 'stringLength' as const, max: 100, message: t('显示名称最长100字') },
])

const emailRules = computed(() => [
  { type: 'required' as const, message: t('邮箱不能为空') },
  { type: 'email' as const, message: t('邮箱格式不正确') },
])

const passwordRules = computed(() => [
  { type: 'required' as const, message: t('密码不能为空') },
  { type: 'stringLength' as const, min: 6, max: 128, message: t('密码长度至少6位') },
])

function refreshGrid() {
  gridRef.value?.instance?.refresh()
}

function onSelectionChanged(e: { selectedRowKeys: number[] }) {
  selectedIds.value = e.selectedRowKeys
}

function openCreatePopup() {
  Object.assign(createForm, { Username: '', DisplayName: '', Email: '', Phone: '', Password: '', Remark: '' })
  showCreatePopup.value = true
}

async function handleCreate() {
  const formInstance = createFormRef.value?.instance
  if (formInstance) {
    const result = formInstance.validate()
    if (!result.isValid) return
  }
  await createPlatformUser(createForm)
  showCreatePopup.value = false
  notifySuccess('创建成功')
  refreshGrid()
}

function onEdit(user: PlatformUserRepDTO) {
  editingUserId.value = user.Id
  Object.assign(editForm, {
    DisplayName: user.DisplayName,
    Email: user.Email,
    Phone: user.Phone ?? '',
    Remark: '',
  })
  showEditPopup.value = true
}

async function handleEdit() {
  const formInstance = editFormRef.value?.instance
  if (formInstance) {
    const result = formInstance.validate()
    if (!result.isValid) return
  }
  await updatePlatformUser(editingUserId.value, editForm)
  showEditPopup.value = false
  notifySuccess('更新成功')
  refreshGrid()
}

async function onEnable(id: number) {
  const confirmed = await confirmAction('确认启用此用户')
  if (!confirmed) return
  await enablePlatformUser(id)
  notifySuccess('启用成功')
  refreshGrid()
}

async function onDisable(id: number) {
  const confirmed = await confirmAction('确认禁用此用户')
  if (!confirmed) return
  await disablePlatformUser(id)
  notifySuccess('禁用成功')
  refreshGrid()
}

function onResetPassword(user: PlatformUserRepDTO) {
  resetPwdUserId.value = user.Id
  resetPwdForm.NewPassword = ''
  showResetPwdPopup.value = true
}

async function handleResetPassword() {
  const formInstance = resetPwdFormRef.value?.instance
  if (formInstance) {
    const result = formInstance.validate()
    if (!result.isValid) return
  }
  await resetPlatformUserPassword(resetPwdUserId.value, { NewPassword: resetPwdForm.NewPassword })
  showResetPwdPopup.value = false
  notifySuccess('重置密码成功')
}

async function onDelete(user: PlatformUserRepDTO) {
  const confirmed = await confirmDelete(user.Username)
  if (!confirmed) return
  await deletePlatformUser(user.Id)
  notifySuccess('删除成功')
  refreshGrid()
}

function onShowDetail(user: PlatformUserRepDTO) {
  detailUser.value = user
  showDetailPopup.value = true
}

async function onBatchEnable() {
  if (selectedIds.value.length === 0) return
  const confirmed = await confirmAction('确认批量启用选中用户')
  if (!confirmed) return
  await batchEnablePlatformUsers(selectedIds.value)
  notifySuccess('启用成功')
  selectedIds.value = []
  refreshGrid()
}

async function onBatchDisable() {
  if (selectedIds.value.length === 0) return
  const confirmed = await confirmAction('确认批量禁用选中用户')
  if (!confirmed) return
  await batchDisablePlatformUsers(selectedIds.value)
  notifySuccess('禁用成功')
  selectedIds.value = []
  refreshGrid()
}

const guideSteps = computed(() => [
  t('点击新增用户按钮创建管理员账号'),
  t('在列表中使用搜索框按用户名邮箱筛选'),
  t('点击编辑修改用户信息'),
  t('点击禁用启用切换用户状态'),
  t('点击重置密码设置新密码'),
  t('点击删除删除用户'),
])
const guideFieldNotes = computed(() => [
  t('用户名全局唯一创建后不可修改'),
  t('邮箱用于接收系统通知'),
  t('初始密码需告知用户建议登录后修改'),
])
const guideErrorNotes = computed(() => [
  t('用户名已存在时创建将失败'),
  t('禁用超级管理员需谨慎操作'),
])
</script>

<style scoped>
.detail-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.detail-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 13px;
  line-height: 1.6;
  padding: 4px 0;
  border-bottom: 1px solid var(--dx-color-border, #e0e0e0);
}
.detail-label {
  color: var(--dx-color-text-secondary, #666);
  min-width: 100px;
}
</style>

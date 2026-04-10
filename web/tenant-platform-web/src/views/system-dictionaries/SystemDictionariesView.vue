<template>
  <div>
    <div class="page-header">
      <h2>{{ $t('route.systemDictionaries') }}</h2>
      <div class="page-header-actions">
        <PageHelpEntry @click="showGuide = true" />
        <DxButton :text="$t('common.create')" icon="add" type="default" @click="showCreatePopup = true" />
      </div>
    </div>

    <FunctionDescriptionCard
      purpose="管理系统数据字典，用于定义下拉选项、枚举映射等可配置的键值对数据。"
      data-scope="全局字典数据，影响各模块下拉选项和显示文本。"
      permission-note="需要相应的管理权限。"
      risk-note="修改字典项可能影响已有数据的显示。"
      :collapsible="true"
    />

    <div class="card" style="margin-bottom: 16px">
      <h3 style="margin-bottom: 12px">{{ $t('字典类型') }}</h3>
      <DxDataGrid
        :data-source="typeList"
        :show-borders="true"
        :column-auto-width="true"
        :hover-state-enabled="true"
        key-expr="TypeCode"
        @row-click="onTypeSelect"
      >
        <DxColumn data-field="TypeCode" :caption="$t('类型编码')" />
        <DxColumn data-field="TypeName" :caption="$t('类型名称')" />
        <DxColumn data-field="ItemCount" :caption="$t('项数量')" :width="100" />
      </DxDataGrid>
    </div>

    <div v-if="selectedType" class="card">
      <h3 style="margin-bottom: 12px">{{ $t('字典项') }} — {{ selectedType }}</h3>
      <DxDataGrid
        :data-source="itemList"
        :show-borders="true"
        :column-auto-width="true"
        :hover-state-enabled="true"
        key-expr="Id"
      >
        <DxColumn data-field="ItemCode" :caption="$t('项编码')" />
        <DxColumn data-field="ItemName" :caption="$t('项名称')" />
        <DxColumn data-field="ItemValue" :caption="$t('项值')" />
        <DxColumn data-field="SortOrder" :caption="$t('排序')" :width="70" />
        <DxColumn data-field="IsEnabled" :caption="$t('common.status')" cell-template="statusCell" :width="80" />
        <DxColumn :caption="$t('common.actions')" cell-template="actionCell" :width="140" />
        <template #statusCell="{ data: cellData }">
          <StatusTag :status="cellData.value ? 'Active' : 'Disabled'" />
        </template>
        <template #actionCell="{ data: cellData }">
          <DxButton :text="$t('common.edit')" styling-mode="text" @click="onEditItem(cellData.data)" />
          <DxButton :text="$t('common.delete')" styling-mode="text" type="danger" @click="onDeleteItem(cellData.data.Id)" />
        </template>
        <DxPaging :page-size="20" />
      </DxDataGrid>
    </div>

    <DxPopup
      :visible="showCreatePopup"
      :title="$t('新增字典项')"
      :width="480"
      :height="'auto'"
      :show-close-button="true"
      @hiding="showCreatePopup = false"
    >
      <DxForm :form-data="createForm" :col-count="1" label-mode="floating">
        <DxSimpleItem data-field="TypeCode">
          <DxLabel :text="$t('类型编码')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="TypeName">
          <DxLabel :text="$t('类型名称')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="ItemCode">
          <DxLabel :text="$t('项编码')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="ItemName">
          <DxLabel :text="$t('项名称')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="ItemValue">
          <DxLabel :text="$t('项值')" />
        </DxSimpleItem>
        <DxSimpleItem data-field="Remark" editor-type="dxTextArea">
          <DxLabel :text="$t('common.remark')" />
        </DxSimpleItem>
        <DxButtonItem>
          <DxButtonOptions :text="$t('common.submit')" type="default" :use-submit-behavior="false" @click="handleCreate" />
        </DxButtonItem>
      </DxForm>
    </DxPopup>

    <OperationGuideDrawer
      v-model:visible="showGuide"
      title="字典管理操作指引"
      entry-path="左侧菜单 → 系统设置 → 字典管理"
      :steps="['选择左侧字典类型查看对应项', '点击新增创建字典项', '编辑/删除已有字典项']"
      :field-notes="['类型编码：字典分类标识', '项编码：同一类型内唯一', '项值：可选的附加数据']"
      :error-notes="['同类型下项编码重复将创建失败']"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { DxDataGrid, DxColumn, DxPaging } from 'devextreme-vue/data-grid'
import { DxButton } from 'devextreme-vue/button'
import { DxPopup } from 'devextreme-vue/popup'
import { DxForm, DxSimpleItem, DxLabel, DxButtonItem, DxButtonOptions } from 'devextreme-vue/form'
import StatusTag from '@/components/StatusTag.vue'
import FunctionDescriptionCard from '@/components/help/FunctionDescriptionCard.vue'
import OperationGuideDrawer from '@/components/help/OperationGuideDrawer.vue'
import PageHelpEntry from '@/components/help/PageHelpEntry.vue'
import {
  getDictionaryTypes,
  getDictionaryByType,
  createDictionary,
  deleteDictionary,
  type DictionaryTypeRepDTO,
  type DictionaryRepDTO,
  type CreateDictionaryReqDTO,
} from '@/api/dictionaries'

const showGuide = ref(false)
const showCreatePopup = ref(false)
const typeList = ref<DictionaryTypeRepDTO[]>([])
const itemList = ref<DictionaryRepDTO[]>([])
const selectedType = ref<string | null>(null)

const createForm = reactive<CreateDictionaryReqDTO>({
  TypeCode: '',
  TypeName: '',
  ItemCode: '',
  ItemName: '',
  ItemValue: '',
  Remark: '',
})

async function loadTypes() {
  try {
    const res = await getDictionaryTypes()
    typeList.value = res.Data ?? []
  } catch {
    // 接口未就绪时保持空列表
  }
}

async function loadItems(typeCode: string) {
  try {
    const res = await getDictionaryByType(typeCode)
    itemList.value = res.Data ?? []
  } catch {
    // 接口未就绪时保持空列表
  }
}

function onTypeSelect(e: { data?: DictionaryTypeRepDTO }) {
  if (e.data) {
    selectedType.value = e.data.TypeCode
    loadItems(e.data.TypeCode)
  }
}

async function handleCreate() {
  try {
    await createDictionary(createForm)
    showCreatePopup.value = false
    Object.assign(createForm, { TypeCode: '', TypeName: '', ItemCode: '', ItemName: '', ItemValue: '', Remark: '' })
    await loadTypes()
    if (selectedType.value) {
      await loadItems(selectedType.value)
    }
  } catch {
    // 错误由 http 层统一处理
  }
}

function onEditItem(_item: DictionaryRepDTO) {
  // 后续阶段完善编辑功能
}

async function onDeleteItem(id: number) {
  try {
    await deleteDictionary(id)
    if (selectedType.value) {
      await loadItems(selectedType.value)
    }
    await loadTypes()
  } catch {
    // 错误由 http 层统一处理
  }
}

onMounted(loadTypes)
</script>

using ProjectCohesion.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Services
{
    /// <summary>
    /// 内容标签页管理服务
    /// </summary>
    public class ContentTabsManager
    {

        private readonly UIViewModel uiViewModel;

        public ContentTabsManager(UIViewModel uiViewModel)
        {
            this.uiViewModel = uiViewModel;
        }

        /// <summary>
        /// 添加一个标签
        /// </summary>
        public void AddTab(Guid moduleGuid)
        {
            if (!uiViewModel.ContentTabs.Items.Contains(moduleGuid))
                uiViewModel.ContentTabs.Items.Add(moduleGuid);
        }

        /// <summary>
        /// 移除一个标签
        /// </summary>
        public void RemoveTab(Guid moduleGuid)
        {
            uiViewModel.ContentTabs.Items.Remove(moduleGuid);
            if (uiViewModel.ContentTabs.Selected == null)
            {
                var count = uiViewModel.ContentTabs.Items.Count;
                uiViewModel.ContentTabs.SelectedIndex = count - 1;
            }
        }

        /// <summary>
        /// 移除其他标签
        /// </summary>
        public void RemoveOtherTab(Guid moduleGuid)
        {
            foreach (var guid in uiViewModel.ContentTabs.Items)
            {
                if (guid != moduleGuid)
                    uiViewModel.ContentTabs.Items.Remove(moduleGuid);
            }
            uiViewModel.ContentTabs.SelectedIndex = 0;
        }

        /// <summary>
        /// 清空标签
        /// </summary>
        public void ClearTab()
        {
            uiViewModel.ContentTabs.Items.Clear();
        }

        /// <summary>
        /// 激活一个标签，如果标签不存在就创建一个
        /// </summary>
        public void ActivateTab(Guid moduleGuid)
        {
            if (!uiViewModel.ContentTabs.Items.Contains(moduleGuid))
                AddTab(moduleGuid);
            uiViewModel.ContentTabs.Selected = moduleGuid;
        }
    }
}

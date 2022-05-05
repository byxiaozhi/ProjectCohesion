using ProjectCohesion.Core.Modules;
using ProjectCohesion.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Services
{
    public class UIManager
    {
        private readonly ModuleManager moduleManager;

        public UIManager(ModuleManager moduleManager)
        {
            this.moduleManager = moduleManager;
        }

        /// <summary>
        /// 添加一个组件到主页顶部菜单
        /// </summary>
        public void AddTopMenuItem(string menu, string group, Guid moduleGuid)
        {
            var menuModules = moduleManager.GetModules<MenuModule>().Where(x => x.Type == "MainTopMenu");
            var menuModule = menuModules.Where(x => x.Name == menu).FirstOrDefault();
            if (menuModule == null)
            {
                // 如果不存在该菜单项，就注册一个
                menuModule = new MenuModule()
                {
                    Name = menu,
                    Type = "MainTopMenu",
                    Element = new()
                };
                moduleManager.RegisterModule(Guid.NewGuid(), menuModule);
            }
            var groupModule = menuModule.Element.Where(x => x.Name == group).FirstOrDefault();
            if (groupModule == null)
            {
                // 如果不存在该分组，就添加一个
                groupModule = new GroupModule()
                {
                    Name = group,
                    Element = new()
                };
                menuModule.Element.Add(groupModule);
            }
            if (!groupModule.Element.Where(x => x == moduleGuid).Any())
            {
                // 如果还未添加该菜单项，就添加一个
                groupModule.Element.Add(moduleGuid);
            }
        }

        /// <summary>
        /// 添加一个组件到侧边标签页
        /// </summary>
        private void AddSideTabsItem(string location, string title, Guid moduleGuid)
        {
            var groupModules = moduleManager.GetModules<GroupModule>().Where(x => x.Type == location);
            var groupModule = groupModules.Where(x => x.Name == title).FirstOrDefault();
            if(groupModule == null)
            {
                // 如果不存在该标签，就注册一个
                groupModule = new GroupModule()
                {
                    Name= title,
                    Type = location,
                    Element = new()
                };
                moduleManager.RegisterModule(Guid.NewGuid(), groupModule);
            }
            if (!groupModule.Element.Where(x => x == moduleGuid).Any())
            {
                // 如果还未添加该标签页，就添加一个
                groupModule.Element.Add(moduleGuid);
            }
        }

        /// <summary>
        /// 添加一个组件到左边标签页
        /// </summary>
        public void AddLeftTabsItem(string title, Guid moduleGuid)
        {
            AddSideTabsItem("MainLeftTab", title, moduleGuid);
        }

        /// <summary>
        /// 添加一个组件到右边标签页
        /// </summary>
        public void AddRightTabsItem(string title, Guid moduleGuid)
        {
            AddSideTabsItem("MainRightTab", title, moduleGuid);
        }
    }
}

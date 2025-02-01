using System;
using System.Collections.Generic;
using System.Linq;
using nadena.dev.ndmf;
using tech.onsen.vrc.ndmf.moveexmenu.editor;
using tech.onsen.vrc.ndmf.moveexmenu.runtime;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using Object = UnityEngine.Object;

[assembly: ExportsPlugin(typeof(MoveExpressionMenuItemProcessor))]

namespace tech.onsen.vrc.ndmf.moveexmenu.editor
{
    public class MoveExpressionMenuItemProcessor : Plugin<MoveExpressionMenuItemProcessor>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming)
                .Run("Remove FX layer", OnTransforming);
            InPhase(BuildPhase.Optimizing)
                .BeforePlugin("com.anatawa12.avatar-optimizer")
                .Run("Remove empty GameObject", OnOptimizing);
        }

        private void OnTransforming(BuildContext context)
        {
            var obj = context.AvatarRootObject.GetComponentInChildren<MoveExpressionMenuItem>();
            if (obj == null) return;

            var descriptor = context.AvatarRootObject.GetComponent<VRCAvatarDescriptor>();
            var groups = obj.replacements.GroupBy(k => string.IsNullOrEmpty(k.destination))
                .ToDictionary(k => k.Key, v => v.ToArray());
            foreach (var replacement in groups.GetValueOrDefault(false) ?? Enumerable.Empty<MoveInfo>())
            {
                MoveItem(descriptor.expressionsMenu, replacement);
            }

            foreach (var replacement in groups.GetValueOrDefault(true) ?? Enumerable.Empty<MoveInfo>())
            {
                MoveItem(descriptor.expressionsMenu, replacement);
            }
        }

        private void OnOptimizing(BuildContext context)
        {
            var obj = context.AvatarRootObject.GetComponentInChildren<MoveExpressionMenuItem>();
            if (obj == null) return;

            var gameObject = obj.gameObject;
            Object.DestroyImmediate(obj);
            if (gameObject.transform.childCount == 0 && gameObject.GetComponents<Component>().Length == 1)
            {
                Object.DestroyImmediate(gameObject);
            }
        }

        private void MoveItem(VRCExpressionsMenu expressionsMenu, MoveInfo info)
        {
            var sourcePaths = SplitPath(info.source, false);
            var menu = expressionsMenu;
            VRCExpressionsMenu.Control control = null;
            for (var i = 0; i < sourcePaths.Length; i++)
            {
                var path = sourcePaths[i];
                var index = menu.controls.FindIndex(c => c.name == path);
                if (index == -1) break;
                if (i == sourcePaths.Length - 1)
                {
                    control = menu.controls[index];
                    menu.controls.RemoveAt(index);
                }
                else
                {
                    menu = menu.controls[index].subMenu;
                }
            }

            // Destinationが空の場合は削除
            if (string.IsNullOrEmpty(info.destination)) return;
            var destinationPaths = SplitPath(info.destination, true);
            var targetParent = expressionsMenu;
            for (var i = 0; i < destinationPaths.Length; i++)
            {
                var path = destinationPaths[i];
                if (i == destinationPaths.Length - 1)
                {
                    break;
                }

                var index = targetParent.controls.FindIndex(c => c.name == path);
                if (index == -1)
                {
                    var newControl = new VRCExpressionsMenu.Control
                    {
                        name = path,
                        type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                        subMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>()
                    };
                    targetParent.controls.Add(newControl);
                    targetParent = newControl.subMenu;
                }
                else
                {
                    targetParent = targetParent.controls[index].subMenu;
                }
            }

            if (control != null && targetParent != null)
            {
                if (string.IsNullOrEmpty(destinationPaths[^1]))
                {
                    targetParent.controls.Add(control);
                }
                else
                {
                    control.name = destinationPaths[^1];
                }
            }
        }

        private string[] SplitPath(string path, bool allowTailEmpty)
        {
            var list = new List<string>();
            var i = 0;
            path = path.Trim();
            var tailSlash = path.EndsWith("/");
            path = path.TrimEnd('/');

            while (i < path.Length)
            {
                var start = i;
                while (i < path.Length)
                {
                    while (i < path.Length && path[i] != '/') i++;
                    if (i + 1 < path.Length && path[i + 1] == '/') i += 2;
                    else break;
                }

                list.Add(path.Substring(start, i - start).Replace("//", "/"));
                i++;
            }

            if (tailSlash && allowTailEmpty)
            {
                list.Add("");
            }

            return list.ToArray();
        }
    }
}

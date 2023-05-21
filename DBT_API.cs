using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;

public class DBT_API
{
    public class DBT_Instance
    {
        public class BlendState
        {
            public AnimatorControllerParameter parameter;
            public BlendTree Tree;

            public BlendState(MasterTree Master, AnimatorControllerParameter parameter, AnimationClip OffClip, AnimationClip OnClip, string Name)
            {
                Tree = Master.Master.CreateBlendTreeChild(1);
                Tree.name = Name;
                
                Master.ReLinkChildren();

                Tree.blendType = BlendTreeType.Simple1D;
                Tree.blendParameter = parameter.name;
                Tree.AddChild(OffClip, 0f); // Add Motion
                Tree.AddChild(OnClip, 1f); // Add Motion
            }
        }

        public class MasterTree
        {
            public MasterTree(AnimatorController controller, AnimatorState State, string Name)
            {
                Master = CreateBlendTree(controller, State, Name);
                Master.blendType = BlendTreeType.Direct;

                DummyParameter = new AnimatorControllerParameter
                {
                    name = $"{Name}_DummyFloat",
                    type = AnimatorControllerParameterType.Float,
                    defaultFloat = 1f
                };

                var parameters = controller.parameters.ToList();
                parameters.Add(DummyParameter);
                controller.parameters = parameters.ToArray();
            }

            public void ReLinkChildren()
            {
                // Re-Link All Child Trees To Master
                var ChildTrees = Master.children;
                for (var i = 0; i < ChildTrees.Length; i++)
                {
                    ChildTrees[i].directBlendParameter = DummyParameter.name;
                }
                Master.children = ChildTrees;
            }

            public BlendTree Master;

            public AnimatorControllerParameter DummyParameter;

            public List<BlendState> Trees = new List<BlendState>();
        }

        public AnimatorController controller;
        public AnimatorControllerLayer layer;
        public MasterTree masterTree;

        public DBT_Instance(AnimatorController controller, string LayerName)
        {
            var controllerPath = AssetDatabase.GetAssetPath(controller);

            var layer = new AnimatorControllerLayer
            {
                name = LayerName,
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine
                {
                    name = LayerName + " State Machine"
                },
            };

            controller.AddLayer(layer);

            AssetDatabase.AddObjectToAsset(layer.stateMachine, controllerPath);
            AssetDatabase.SaveAssets();

            this.layer = layer;

            this.controller = controller;

            var state = CreateState(layer, $"{LayerName}_BlendRootState");

            masterTree = new MasterTree(controller, state, $"{LayerName} Master Tree");
        }

        public static AnimatorState CreateState(AnimatorControllerLayer layer, string Name, Vector3? position = null)
        {
            var state = position == null ? layer.stateMachine.AddState(Name) : layer.stateMachine.AddState(Name, position.Value);
            state.writeDefaultValues = true;

            return state;
        }

        public static BlendTree CreateBlendTree(AnimatorController controller, AnimatorState State, string Name)
        {
            var tree = new BlendTree
            {
                name = Name,
                hideFlags = HideFlags.HideInHierarchy
            };

            AssetDatabase.AddObjectToAsset(tree, controller);

            State.motion = tree;

            return tree;
        }
    }
}

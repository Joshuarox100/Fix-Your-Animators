#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[InitializeOnLoad]
public class FixYourAnimators : ScriptableObject
{
    private static System.Type[] types = new System.Type[]
    {
        typeof(AnimatorState),
        typeof(AnimatorStateMachine),
        typeof(StateMachineBehaviour),
        typeof(AnimatorStateTransition),
        typeof(AnimatorTransition),
        typeof(BlendTree)
    };

    static FixYourAnimators()
    {
        Selection.selectionChanged += AutofixHideFlags;
        EditorApplication.quitting += RemoveDelegate;
    }

    public static void RemoveDelegate()
    {
        Selection.selectionChanged -= AutofixHideFlags;
        EditorApplication.quitting -= RemoveDelegate;
    }

    // Automatically corrects HideFlags for objects with types included in 'types' when trying to inspect them.
    public static void AutofixHideFlags()
    {
        bool flag = false;
        foreach (Object @object in Selection.objects.Where(obj => types.Contains(obj.GetType()) && obj.hideFlags == (HideFlags.HideInHierarchy | HideFlags.HideInInspector)))
        {
            if (AssetDatabase.GetAssetPath(@object).Length != 0)
                EditorUtility.SetDirty(@object);
            @object.hideFlags = HideFlags.HideInHierarchy;
            flag = true;
        }
        if (flag)
            Selection.selectionChanged();
    }

    // Update all Animator Controllers to use HideFlags of 1 instead of 3.
    [MenuItem("Tools/Joshuarox100/Fix Your Animators", priority = 1100)]
    public static void FixAllAnimators()
    {
        if (!EditorUtility.DisplayDialog("Fix Your Animators", "This will find and modify all Animator Controllers in your project to fix HideFlags.\nProceed?", "Yes", "Cancel"))
            return;

        EditorUtility.DisplayProgressBar("Fixing Your Animators...", "Initial Save", 0f);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayProgressBar("Fixing Your Animators...", "Finding Animator Controllers...", 0.01f);
        string[] controllerGUIDS = AssetDatabase.FindAssets("t:AnimatorController");
        for (int i = 0; i < controllerGUIDS.Length; i++)
        {
            AnimatorController source = (AnimatorController)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(controllerGUIDS[i]), typeof(AnimatorController));

            // Not an Animator Controller, skip.
            if (source == null)
                continue;

            EditorUtility.DisplayProgressBar("Fixing Your Animators...", "Fixing " + source.name, 0.01f + (0.99f * (i / (controllerGUIDS.Length - 1f))));
            EditorUtility.SetDirty(source);
            foreach (AnimatorControllerLayer layer in source.layers)
                FixAllAnimatorsHelper(layer.stateMachine);
        }
        EditorUtility.DisplayProgressBar("Fixing Your Animators...", "Final Save", 1f);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    private static void FixAllAnimatorsHelper(AnimatorStateMachine machine)
    {
        machine.hideFlags = HideFlags.HideInHierarchy;
        foreach (var subStateMachine in machine.stateMachines)
            FixAllAnimatorsHelper(subStateMachine.stateMachine);
        foreach (var childState in machine.states)
        {
            childState.state.hideFlags = HideFlags.HideInHierarchy;
            if (childState.state.motion != null && childState.state.motion.GetType() == typeof(BlendTree))
                childState.state.motion.hideFlags = HideFlags.HideInHierarchy;
            foreach (var stateBehavior in childState.state.behaviours)
                stateBehavior.hideFlags = HideFlags.HideInHierarchy;
            foreach (var stateTransition in childState.state.transitions)
                stateTransition.hideFlags = HideFlags.HideInHierarchy;
        }
        foreach (var anyStateTransition in machine.anyStateTransitions)
            anyStateTransition.hideFlags = HideFlags.HideInHierarchy;
        foreach (var entryTransition in machine.entryTransitions)
            entryTransition.hideFlags = HideFlags.HideInHierarchy;
        foreach (var machineBehavior in machine.behaviours)
            machineBehavior.hideFlags = HideFlags.HideInHierarchy;
    }
}
#endif
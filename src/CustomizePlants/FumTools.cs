﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace FumLib
{
    public static class FumTools
    {
        public static void PrintAllPatches(Type type, string method)
        {
            try
            {
                var harmony = HarmonyInstance.Create("com.company.project.product");

                var original = type.GetMethod(method);
                var info = harmony.GetPatchInfo(original);

                if (info == null) return;

                foreach (var patch in info.Prefixes)
                {
                    Debug.Log("Prefix index: " + patch.index);
                    Debug.Log("Prefix owner: " + patch.owner);
                    Debug.Log("Prefix patch: " + patch.patch.Name);
                    Debug.Log("Prefix priority: " + patch.priority);
                    Debug.Log("Prefix before: " + String.Join(" ", patch.before.ToArray()));
                    Debug.Log("Prefix after: " + String.Join(" ", patch.after.ToArray()));
                }
                foreach (var patch in info.Postfixes)
                {
                    Debug.Log("Postfix index: " + patch.index);
                    Debug.Log("Postfix owner: " + patch.owner);
                    Debug.Log("Postfix patch: " + patch.patch.Name);
                    Debug.Log("Postfix priority: " + patch.priority);
                    Debug.Log("Postfix before: " + String.Join(" ", patch.before.ToArray()));
                    Debug.Log("Postfix after: " + String.Join(" ", patch.after.ToArray()));
                }
                foreach (var patch in info.Transpilers)
                {
                    Debug.Log("Transpiler index: " + patch.index);
                    Debug.Log("Transpiler owner: " + patch.owner);
                    Debug.Log("Transpiler patch: " + patch.patch.Name);
                    Debug.Log("Transpiler priority: " + patch.priority);
                    Debug.Log("Transpiler before: " + String.Join(" ", patch.before.ToArray()));
                    Debug.Log("Transpiler after: " + String.Join(" ", patch.after.ToArray()));
                }
                // all owners shortcut
                Debug.Log("All owners: " + String.Join(" ", info.Owners.ToArray()));
            } catch (Exception) { }
        }

        public static void RemoveComponent(GameObject go)
        {
            UnityEngine.Object.DestroyImmediate(go);
        }

        public static void RemoveComponent<UComponent>(this GameObject go) where UComponent : UnityEngine.Object
        {
            UComponent comp = go.GetComponent<UComponent>();
            if (comp != null)
                UnityEngine.Object.DestroyImmediate(comp);
        }

        public static void RemoveComponents<UComponent>(this GameObject go) where UComponent : UnityEngine.Object
        {
            UComponent[] comps = go.GetComponents<UComponent>();

            foreach (var comp in comps)
                UnityEngine.Object.DestroyImmediate(comp);
        }

        public static void RemoveDef(GameObject go, StateMachine.BaseDef def)
        {
            StateMachineController controller = go.GetComponent<StateMachineController>();
            if (controller != null)
            {
                if (def != null)
                    controller.cmpdef.defs.Remove(def);
            }
        }

        public static void RemoveDef<Def>(this GameObject go) where Def : StateMachine.BaseDef
        {
            StateMachineController controller = go.GetComponent<StateMachineController>();
            if (controller != null)
            {
                var def = go.GetDef<Def>();
                if (def != null)
                    controller.cmpdef.defs.Remove(def);
            }
        }

        public static SimHashes ToSimHash(this string simhash)
        {
            try
            {
                return (SimHashes)Hash.SDBMLower(simhash);
            }
            catch (System.Exception)
            {
                Debug.LogWarning($"Tried to convert {simhash} to SimHashes, which doesn't work.");
                return SimHashes.Vacuum;
            }
        }
    }
}

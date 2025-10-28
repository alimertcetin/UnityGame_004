#if UNITY_EDITOR

using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace XIV.Ecs
{
    public static class EcsDebugUtils
    {
        public static void DrawEntityInspector(Entity entity)
        {
            if (GUILayout.Button("Destroy Entity"))
            {
                entity.Destroy();
            }

            var redText = new GUIStyle
            {
                normal =
                {
                    textColor = Color.red
                }
            };

            var greenText = new GUIStyle
            {
                normal =
                {
                    textColor = Color.green
                }
            };


            if (entity.IsAlive() && entity != Entity.Invalid)
            {
                EditorGUILayout.LabelField("--- Entity Id: " + entity.entityId.id + "---",greenText);
            }
            else
            {
                EditorGUILayout.LabelField("--- ENTITY IS DESTROYED Entity Id: " + entity.entityId.id + "---",redText);
                return;
            }
            
            EntityData entityData = entity.world.entityDataList[entity.entityId.id];

            EditorGUILayout.Space(1);

            EditorGUILayout.LabelField("--- TAGS ---",greenText);

            foreach (var tagId in entityData.archetype.tagBitSet)
            {
                EditorGUILayout.LabelField(TagIdManager.GetTagName(tagId));
                if (GUILayout.Button("Remove Tag"))
                {
                    entity.world.RemoveTag(entity.entityId, tagId);
                    return;
                }
            }


            EditorGUILayout.LabelField("--- COMPONENTS ---",greenText);

            var archetype = entityData.archetype;
            int archetypeIdx = entityData.archetypeIndex;
            
            for (int componentPoolIdx = 0; componentPoolIdx < entityData.archetype.componentPools.Length; componentPoolIdx++)
            {
                var componentPool = archetype.componentPools[componentPoolIdx];
                var component = componentPool.Get(archetypeIdx);
                
                Type componentType = component.GetType();
                EditorGUILayout.LabelField("---" + componentType.Name + "---");
                if (GUILayout.Button("Remove Component"))
                {
                    entity.world.RemoveComponent(entity.entityId, ComponentIdManager.GetComponentId(componentType));
                    return;
                }
                
                MethodInfo onInspectorGUIMethod = componentType.GetMethod("OnInspectorGUI");
                
                FieldInfo[] fieldInfo = componentType.GetFields();
                
                foreach (var info in fieldInfo)
                {
                    if (info.FieldType == typeof(float))
                    {
                        float value = (float) info.GetValue(component);
                        float newValue = EditorGUILayout.FloatField(info.Name,value);

                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (value != newValue)
                        {
                            info.SetValue(component,newValue);
                            componentPool.Set(archetypeIdx,component);
                        }
                        
                    }else if (info.FieldType == typeof(double))
                    {
                        double value = (double) info.GetValue(component);
                        double newValue = EditorGUILayout.DoubleField(info.Name,value);

                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (value != newValue)
                        {
                            info.SetValue(component,newValue);
                            componentPool.Set(archetypeIdx,component);
                        }
                    }else if (info.FieldType == typeof(int))
                    {
                        int value = (int) info.GetValue(component);
                        int newValue = EditorGUILayout.IntField(info.Name, value);

                        if (value != newValue)
                        {
                            info.SetValue(component,newValue);
                            componentPool.Set(archetypeIdx,component);
                        }
                    }else if (info.FieldType == typeof(string))
                    {
                        string value = (string) info.GetValue(component);
                        string newValue = EditorGUILayout.TextField(info.Name, value);
                        if (value != newValue)
                        {
                            info.SetValue(component,newValue);
                            componentPool.Set(archetypeIdx,component);
                        }
                    }else if (info.FieldType == typeof(Vector3))
                    {
                        Vector3 value = (Vector3) info.GetValue(component);
                        Vector3 newValue = EditorGUILayout.Vector3Field(info.Name, value);
                        if (!value.Equals(newValue))
                        {
                            info.SetValue(component,newValue);
                            componentPool.Set(archetypeIdx,component);
                        }
                    }else if (info.FieldType == typeof(Vector2))
                    {
                        Vector2 value = (Vector2) info.GetValue(component);
                        Vector2 newValue = EditorGUILayout.Vector2Field(info.Name, value);
                        if (!value.Equals(newValue))
                        {
                            info.SetValue(component,newValue);
                            componentPool.Set(archetypeIdx, component);
                        }
                    }
                    else if (info.FieldType == typeof(LayerMask))
                    {
                        var value = (LayerMask) info.GetValue(component);
                        EditorGUILayout.LabelField(info.Name + ": " + LayerMaskToString(value));
                    }else if (info.FieldType == typeof(bool))
                    {
                        var value = (bool) info.GetValue(component);
                        var newValue = EditorGUILayout.Toggle(info.Name + ": " + value, value);
                        if (!value.Equals(newValue))
                        {
                            info.SetValue(component,newValue);
                            componentPool.Set(archetypeIdx, component);
                        }
                    }
                    else if (typeof(IList).IsAssignableFrom(info.FieldType))
                    {
                        var value = (IList) info.GetValue(component);
                        if (value != null)
                        {
                            EditorGUILayout.LabelField(info.Name + ": " + value.Count);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("NULL");
                        }
                    }
                    else if (info.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                    {
                        var value = (UnityEngine.Object) info.GetValue(component);
                        EditorGUILayout.BeginHorizontal();
                        var newValue = EditorGUILayout.ObjectField(info.Name,value, info.FieldType, true);

                        if (value != newValue)
                        {
                            info.SetValue(component,newValue);
                            componentPool.Set(archetypeIdx, component);
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    
                    }
                    else
                    {
                        var value = info.GetValue(component);
                        string s = value == null ? " NULL" : value.ToString();
                        EditorGUILayout.LabelField(info.Name + ": " + s);
                    }
                    
                }
                
                // Draw custom inspector after the default one
                if (onInspectorGUIMethod != null)
                {
                    onInspectorGUIMethod.GetBaseDefinition().Invoke(component, null);
                    componentPool.Set(archetypeIdx, component);
                }
            
                EditorGUILayout.Space(2);
            }

            EditorGUILayout.LabelField("--- FILTERS ---",greenText);

            foreach (var filter in entity.world.filters)
            {
                if (filter.query.archetypes.Contains(entityData.archetype))
                {
                    EditorGUILayout.LabelField(filter.filterName);
                }
            }
        }
        
        public static string LayerMaskToString(LayerMask layerMask)
        {
             string layerNames = "";
             for (int i = 0; i < 32; i++)
             {
                 int value = layerMask.value;
                 int c = 1 << i;
                 if ((value & c) != 0)
                 {
                     layerNames += LayerMask.LayerToName(i) + ", ";
                 }
             }
             return layerNames;
         }
    }
}

#endif

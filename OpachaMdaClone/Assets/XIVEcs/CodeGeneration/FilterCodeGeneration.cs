using UnityEngine;

namespace Dalak.Ecs
{
    public static class FilterCodeGeneration
    {
        public static void GenerateFilterCode()
        {
            var filters = "";
            const int MaxComponents = 6;
            for (int nComponents = 1; nComponents <= MaxComponents; nComponents++)
            {
                string compGenerics = "";
                for (int i = 0; i < nComponents; i++)
                {
                    if (i == nComponents - 1)
                    {
                        compGenerics += $"Component{i}";
                    }
                    else
                    {
                        compGenerics += $"Component{i},";
                    }
                }

                string s = $"public class Filter<{compGenerics}> : Filter\n";
                for (int i = 0; i < nComponents; i++)
                {
                    s += $"where Component{i} : struct, IComponent\n";
                }

                s += "{\n";

                    s += "public Filter()";
                    s += "\n{\n";
                    s += "query = new Query();\n";
                    for (int i = 0; i < nComponents; i++)
                    {
                        s += $"query.IncludeComp<Component{i}>();";
                    }
                    s += "\n}\n";
                    

                    s += $"public new Filter<{compGenerics}> Exclude<T>() where T : struct,IComponent\n";
                    s += "\n{\nquery.ExcludeComp<T>();\nreturn this;\n}\n";
                    
                    s += $"public new Filter<{compGenerics}> Tag<T>() where T : struct,ITag\n";
                    s += "\n{\nquery.IncludeTag<T>();\nreturn this;\n}\n";
                    
                    s += $"public new Filter<{compGenerics}> ExcludeTag<T>() where T : struct,ITag\n";
                    s += "\n{\nquery.ExcludeTag<T>();\nreturn this;\n}\n";


                    var componentParameters = "";
                    for (int i = 0; i < nComponents; i++)
                    {
                        if (i == nComponents - 1)
                        {
                            componentParameters += $"ref Component{i} comp{i}";
                        }
                        else
                        {
                            componentParameters += $"ref Component{i} comp{i},";
                        }
                    }
                    
                    s += $"public delegate void IteratorFunc({componentParameters});\n";
                    s += $"public delegate void IteratorFuncWithEntity(Entity entity,{componentParameters});\n";
                    
                    
                    // FOREACH
                    s += "public void ForEach(IteratorFunc iteratorFunc)\n";
                    s += "{\n";
                    
                    s += "Lock();\n";
                    
                    s += "int nArchetypes = query.archetypes.Count;\n";
                    s += "for(int a = 0; a < nArchetypes; a++)\n";
                        s += "{\n";
                        s += "var archetype = query.archetypes[a];";

                        for (int i = 0; i < nComponents; i++)
                        {
                            s += $"var componentPool{i} = archetype.GetComponentPool<Component{i}>();\n";
                        }

                        s += "int nEntities = numberOfEntitiesBuffer[a];\n";
                        s += "for (int i = 0 ; i < nEntities; i ++)\n";
                        s += "{\n";

                        var poolReferences = "";
                        for (int i = 0; i < nComponents; i++)
                        {
                            if (i == nComponents - 1)
                            {
                                poolReferences += $"ref componentPool{i}[i]";
                            }
                            else
                            {
                                poolReferences += $"ref componentPool{i}[i],";
                            }
                        }
                        
                        s += $"iteratorFunc({poolReferences});";
                        s += "}\n";

                        
                        s += "}\n";
                    
                    s += "Unlock();\n";
                    
                    s += "}\n";
                    
                    
                    // FOREACH WITH ENTITY
                    s += "public void ForEach(IteratorFuncWithEntity iteratorFunc)\n";
                    s += "{\n";
                    
                    s += "Lock();\n";

                    s += "int nArchetypes = query.archetypes.Count;\n";
                    s += "for(int a = 0; a < nArchetypes; a++)\n";
                    s += "{\n";
                    s += "var archetype = query.archetypes[a];";

                    for (int i = 0; i < nComponents; i++)
                    {
                        s += $"var componentPool{i} = archetype.GetComponentPool<Component{i}>();\n";
                    }

                    s += "int nEntities = numberOfEntitiesBuffer[a];\n";
                    s += "for (int i = 0 ; i < nEntities; i ++)\n";
                    s += "{\n";

                        
                    s += $"iteratorFunc(archetype.entities[i], {poolReferences});";
                    s += "}\n";

                        
                    s += "}\n";
                    
                    s += "Unlock();\n";
                    
                    s += "}\n";
                    
                
                s += "}";

                filters += s;

            }
            
            Debug.Log(filters);
        }
        
    }
}


// public class Filter<Component0,Component1> : Filter 
//         where Component0 : struct,IComponent
//         where Component1 : struct,IComponent
//     {
//         public Filter()
//         {
//             query = new Query();
//             query.IncludeComp<Component0>();
//             query.IncludeComp<Component1>();
//         }
//         
//         public new Filter<Component0,Component1> Exclude<ExcludedComponent>() 
//             where ExcludedComponent : struct,IComponent
//         {
//             query.ExcludeComp<ExcludedComponent>();
//             return this;
//         }
//
//         public new Filter<Component0,Component1> Tag<T>()
//             where T : struct,ITag
//         {
//             query.IncludeTag<T>();
//             return this;
//         }
//         
//         public new Filter<Component0,Component1> ExcludeTag<T>()
//             where T : struct,ITag
//         {
//             query.ExcludeTag<T>();
//             return this;
//         }
//         
//
//         public delegate void IteratorFunc(ref Component0 component0, ref Component1 component1);
//         public new delegate void IteratorFuncWithEntity(Entity entity,ref Component0 component0, ref Component1 component1);
//
//         public void ForEach(IteratorFunc iteratorFunc)
//         {
//             world.LockComponentOperation();
//             foreach (var archetype in query.archetypes)
//             {
//                 var componentPool0 = archetype.GetComponentPool<Component0>();
//                 var componentPool1 = archetype.GetComponentPool<Component1>();
//
//                 for (int i = 0; i < archetype.entities.numberOfItems; i++)
//                 {
//                     iteratorFunc(ref componentPool0[i],ref componentPool1[i]);
//                 }
//             }
//             world.UnlockComponentOperation();
//         }
//
//         public void ForEach(IteratorFuncWithEntity iteratorFunc)
//         {
//             world.LockComponentOperation();
//             foreach (var archetype in query.archetypes)
//             {
//                 var componentPool0 = archetype.GetComponentPool<Component0>();
//                 var componentPool1 = archetype.GetComponentPool<Component1>();
//
//                 for (int i = 0; i < archetype.entities.numberOfItems; i++)
//                 {
//                     iteratorFunc(archetype.entities[i],ref componentPool0[i], ref componentPool1[i]);
//                 }
//             }
//             world.UnlockComponentOperation();
//         }
//         
//     }
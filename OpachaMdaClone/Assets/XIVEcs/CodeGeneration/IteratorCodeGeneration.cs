using System.Collections.Generic;
using UnityEngine;

namespace Dalak.Ecs
{
    public static class IteratorCodeGeneration
    {
        const int nComponent = 6;

        public static void GenerateIteratorDelegates()
        {
            string s = "";

            for (int i = 2; i <= nComponent * 2; i++)
            {
                string t1 = "";
                string tRef = "";

                for (int j = 0; j < i; j++)
                {
                    if (j == i - 1)
                    {
                        t1 += $"T{j}";
                        tRef += $"ref T{j} t{j}";
                    }
                    else
                    {
                        t1 += $"T{j},";
                        tRef += $"ref T{j} t{j},";
                    }
                }
                
                s += $"public delegate void Iterator{i}<{t1}>({tRef});\n";

            }


            for (int nComponent1 = 1; nComponent1 <= nComponent; nComponent1++)
            {
                for (int nComponent2 = 1; nComponent2 <= nComponent; nComponent2++)
                {
                    string componentGenerics = "";
                    string componentGenericsWithRefAndEntity = "Entity entity0,";

                    for (int c1 = 0; c1 < nComponent1; c1++)
                    {
                        componentGenerics += $"T1{c1},";
                        componentGenericsWithRefAndEntity += $"ref T1{c1} t1{c1},";
                    }
                    
                    componentGenericsWithRefAndEntity += "Entity entity1,";

                    for (int c2 = 0; c2 < nComponent2; c2++)
                    {
                        if (c2 == nComponent2 - 1)
                        {
                            componentGenerics += $"T2{c2}";
                            componentGenericsWithRefAndEntity += $"ref T2{c2} t2{c2}";
                        }
                        else
                        {
                            componentGenerics += $"T2{c2},";
                            componentGenericsWithRefAndEntity += $"ref T2{c2} t2{c2},";
                        }
                    }

                    s += $"public delegate void IteratorWithEntity{nComponent1}_{nComponent2}<{componentGenerics}>({componentGenericsWithRefAndEntity});\n";
                }
            }
            
            Debug.Log(s);
        }

        public static void GenerateIteratorFunctions()
        {
            string s = "";


            for (int nCompFilter1 = 1; nCompFilter1 <= nComponent; nCompFilter1++)
            {
                for (int nCompFilter2 = 1; nCompFilter2 <= nComponent; nCompFilter2++)
                {
                    var filterGenerics = "";
                    var filterGenerics1 = "";
                    var filterGenerics2 = "";
                    
                    for (int i = 0; i < nCompFilter1; i++)
                    {
                        filterGenerics += $"F1C{i},";

                        if (i == nCompFilter1 - 1)
                        {
                            filterGenerics1 += $"F1C{i}";
                        }
                        else
                        {
                            filterGenerics1 += $"F1C{i},";
                        }
                    }
                    
                    
                    for (int i = 0; i < nCompFilter2; i++)
                    {
                        if (i == nCompFilter2 - 1)
                        {
                            filterGenerics2 += $"F2C{i}";
                        }
                        else
                        {
                            filterGenerics2 += $"F2C{i},";
                        }
                        
                        if (i == nCompFilter2 - 1)
                        {
                            filterGenerics += $"F2C{i}";
                        }
                        else
                        {
                            filterGenerics += $"F2C{i},";
                        }
                    }
                    
                    
                    // ITERATE
                    s += $"public static void Iterate<{filterGenerics}>(" +
                         $"Filter<{filterGenerics1}> filter1, " +
                         $"Filter<{filterGenerics2}> filter2, " +
                         $"Iterator{nCompFilter1 + nCompFilter2}<{filterGenerics}> iterator)";

                    s += "\n";
                    for (int i = 0; i < nCompFilter1; i++)
                    {
                        s += $"where F1C{i} : struct,IComponent\n";
                    }
                    
                    for (int i = 0; i < nCompFilter2; i++)
                    {
                        s += $"where F2C{i} : struct,IComponent\n";
                    }
                    
                    s += "\n{\n";

                    s += "\tLock(filter1,filter2);\n";
                    s += "\tint nArchetypes = filter1.query.archetypes.Count;\n";

                    s += "\tfor(int a = 0; a < nArchetypes; a++)\n";
                    s += "\t{\n";
                    s += "\tvar archetype1 = filter1.query.archetypes[a];\n";

                        for (int i = 0; i < nCompFilter1; i++)
                        {
                            s += $"\t\tvar filter1ComponentPool{i} = archetype1.GetComponentPool<F1C{i}>();\n";
                        }
                    

                        s += "\t\tfor(int i0 = 0; i0< numberOfEntitiesBuffer1[a]; i0++)\n";
                        s += "\t\t{\n";
                            for (int i = 0; i < nCompFilter1; i++)
                            {
                                s += $"\t\t\tref var f1c{i} = ref filter1ComponentPool{i}[i0];\n";
                            }
                        
                            s += "\t\t\tint nArchetypes2 = filter2.query.archetypes.Count;\n";
                            s += "\t\t\tfor(int a2 = 0; a2 < nArchetypes2;a2++)\n";
                            s += "\t\t\t{\n";
                                s += "\t\t\tvar archetype2 = filter2.query.archetypes[a2];\n";
                                for (int i = 0; i < nCompFilter2; i++)
                                {
                                    s += $"\t\t\tvar filter2ComponentPool{i} = archetype2.GetComponentPool<F2C{i}>();\n";
                                }
                                s += "\t\t\t\tfor(int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)\n";
                                    s += "\t\t\t\t{\n";
                                    for (int i = 0; i < nCompFilter2; i++)
                                    {
                                        s += $"\t\t\tref var f2c{i} = ref filter2ComponentPool{i}[i1];\n";
                                    }

                                    s += "\t\t\titerator(";
                                    for (int i = 0; i < nCompFilter1; i++)
                                    {
                                        s += $"ref f1c{i},";
                                    }

                                    for (int i = 0; i < nCompFilter2; i++)
                                    {
                                        if (i == nCompFilter2 - 1)
                                        {
                                            s += $"ref f2c{i}";
                                        }
                                        else
                                        {
                                            s += $"ref f2c{i},";
                                        }
                                    }
                                    s += ");\n";
                                    s += "\t\t\t\t}\n";
                            s += "\t\t\t}\n";
                        s += "\t\t}\n";
                    
                    
                    s += "\t}\n";
                    
                    s += "Unlock(filter1,filter2);\n";
                    s += "\n}\n";
                    
                    
                    // ITERATE WITH ENTITY
                    s += $"public static void Iterate<{filterGenerics}>(" +
                         $"Filter<{filterGenerics1}> filter1, " +
                         $"Filter<{filterGenerics2}> filter2, " +
                         $"IteratorWithEntity{nCompFilter1}_{nCompFilter2}<{filterGenerics}> iterator)";

                    s += "\n";
                    for (int i = 0; i < nCompFilter1; i++)
                    {
                        s += $"where F1C{i} : struct,IComponent\n";
                    }
                    
                    for (int i = 0; i < nCompFilter2; i++)
                    {
                        s += $"where F2C{i} : struct,IComponent\n";
                    }
                    
                    s += "\n{\n";

                    s += "\tLock(filter1,filter2);\n";
                    s += "\tint nArchetypes = filter1.query.archetypes.Count;\n";

                    s += "\tfor(int a = 0; a < nArchetypes; a++)\n";
                    s += "\t{\n";
                    s += "\tvar archetype1 = filter1.query.archetypes[a];\n";

                        for (int i = 0; i < nCompFilter1; i++)
                        {
                            s += $"\t\tvar filter1ComponentPool{i} = archetype1.GetComponentPool<F1C{i}>();\n";
                        }
                    
                        

                        s += "\t\tfor(int i0 = 0; i0 < numberOfEntitiesBuffer1[a]; i0++)\n";
                        s += "\t\t{\n";
                            for (int i = 0; i < nCompFilter1; i++)
                            {
                                s += $"\t\t\tref var f1c{i} = ref filter1ComponentPool{i}[i0];\n";
                            }
                            s += "\t\t\tvar entity0 = archetype1.entities[i0];\n";
                        
                            s += "\t\t\tint nArchetypes2 = filter2.query.archetypes.Count;\n";
                            s += "\t\t\tfor(int a2 = 0; a2 < nArchetypes2;a2++)\n";
                            s += "\t\t\t{\n";
                            s += "\t\t\tvar archetype2 = filter2.query.archetypes[a2];\n";
                                for (int i = 0; i < nCompFilter2; i++)
                                {
                                    s += $"\t\t\tvar filter2ComponentPool{i} = archetype2.GetComponentPool<F2C{i}>();\n";
                                }
                                s += "\t\t\t\tfor(int i1 = 0; i1 < numberOfEntitiesBuffer2[a2]; i1++)\n";
                                    s += "\t\t\t\t{\n";
                                    for (int i = 0; i < nCompFilter2; i++)
                                    {
                                        s += $"\t\t\tref var f2c{i} = ref filter2ComponentPool{i}[i1];\n";
                                    }
                                    s += "\t\t\titerator(entity0,";
                                    for (int i = 0; i < nCompFilter1; i++)
                                    {
                                        s += $"ref f1c{i},";
                                    }

                                    s += "archetype2.entities[i1],";
                                    for (int i = 0; i < nCompFilter2; i++)
                                    {
                                        if (i == nCompFilter2 - 1)
                                        {
                                            s += $"ref f2c{i}";
                                        }
                                        else
                                        {
                                            s += $"ref f2c{i},";
                                        }
                                    }
                                    s += ");\n";
                                    s += "\t\t\t\t}\n";
                            s += "\t\t\t}\n";
                        s += "\t\t}\n";
                    
                    
                    s += "\t}\n";
                    
                    s += "\tUnlock(filter1,filter2);\n";
                    s += "\n}\n";
                }
            }
            
            Debug.Log(s);
        }
    }
}
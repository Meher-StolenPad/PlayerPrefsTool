using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal static class TypesExtension
    {
        public static PrefsSerialzable GetAttribute(Type t)
        {
            // Get instance of the attribute.
            PrefsSerialzable MyAttribute =
                (PrefsSerialzable)Attribute.GetCustomAttribute(t, typeof(PrefsSerialzable));

            if (MyAttribute == null)
            {
                Debug.Log("The attribute was not found.");
            }
            else
            {
                // Get the Name value.
                Debug.Log("The Name Attribute is "+ MyAttribute.NameOnPrefs);
            }
            return MyAttribute;
        }

        public static void Save(ref this int t)
        {
            //FieldInfo field_info = typeof(int).GetField("");

            PrefsSerialzable custom_attributes = GetAttribute(t.GetType());

            Debug.Log("ddzdz" + custom_attributes.NameOnPrefs);
            if(custom_attributes != null)
            {
                AdvancedPlayerPrefs.SetInt(((PrefsSerialzable)custom_attributes).NameOnPrefs, t);
            }

        }
        public static int Load(ref this int t)
        {
            t = AdvancedPlayerPrefs.GetInt(t.GetHashCode().ToString());
            return t;
        }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PrefsSerialzable: Attribute 
    {
        public PrefsSerialzable(string nameOnPrefs)
        {
            NameOnPrefs = nameOnPrefs;
        }

        public string NameOnPrefs { get; set; }
    }

}

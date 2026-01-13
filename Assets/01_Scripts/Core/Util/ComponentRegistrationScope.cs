using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ComponentRegistrationScope : LifetimeScope
{
    [SerializeField] private List<MonoBehaviour> behavioursToRegister;
    protected override void Configure(IContainerBuilder builder)
    {
        foreach (var mb in behavioursToRegister)
        {
            Type type = mb.GetType();

            Debug.Log("Registering Component: " + mb.gameObject.name);
            Debug.Log("Type: " + type.ToString());
            
            MethodInfo method = typeof(ContainerBuilderUnityExtensions)
                                .GetMethod("RegisterComponent", BindingFlags.Static | BindingFlags.Public)
                                .MakeGenericMethod(type);

            method.Invoke(null, new object[] { builder, mb }); // builder = this, mb = component
        }
    }
}

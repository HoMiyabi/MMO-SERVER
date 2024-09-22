using System.Reflection;

namespace Kirara;

// [AttributeUsage(AttributeTargets.Method)]
// public class RouteAttribute : Attribute
// {
//     public string RouteKey { get; set; }
//
//     public RouteAttribute(string routeKey)
//     {
//         RouteKey = routeKey;
//     }
// }

// public static class RouteFnClass
// {
//     public static void RouteFn()
//     {
//         var assemblies = AppDomain.CurrentDomain.GetAssemblies();
//         foreach (var assembly in assemblies)
//         {
//             var types = assembly.GetTypes();
//             foreach (var type in types)
//             {
//                 var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static |
//                                 BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
//                 foreach (var method in methods)
//                 {
//                     var routeAttribute = method.GetCustomAttribute<RouteAttribute>();
//                     if (routeAttribute != null)
//                     {
//                         // method.GetParameters()
//                     }
//                 }
//             }
//         }
//     }
// }
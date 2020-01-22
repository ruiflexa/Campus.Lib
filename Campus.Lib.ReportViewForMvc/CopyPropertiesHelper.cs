using System;
using System.Reflection;

namespace Campus.Lib.ReportViewerForMvc
{
    internal static class CopyPropertiesHelper
    {
        internal static void Copy<T>(ref T obj, T properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties", "Value cannot be null.");
            }

            Copy<T, T>(ref obj, properties);
        }

        internal static void Copy<T1, T2>(ref T1 obj, T2 properties)
        {
            Type objType = obj.GetType();
            Type propertiesType = properties.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            foreach (PropertyInfo propertyInfo in propertiesType.GetProperties(bindingFlags))
            {
                try
                {
                    if (propertyInfo.CanRead)
                    {
                        var valueToCopy = propertyInfo.GetValue(properties);
                        var objProperty = objType.GetProperty(propertyInfo.Name);

                        if (objProperty.CanWrite)
                        {
                            objProperty.SetValue(obj, valueToCopy);
                        }
                    }
                }
                catch (NullReferenceException ex)
                {
                    throw ex;
                }
                catch (TargetInvocationException) { } //Do nothing, just like my boss.
            }
        }

        internal static void CopyEvents<T>(ref T obj, T source)
        {
            var theType = obj.GetType();
            var events = theType.GetEvents();
            foreach (var oneEvent in events)
            {
                var fieldInfo = theType.GetField(oneEvent.Name, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (fieldInfo == null && obj is System.Web.UI.Control)
                {
                    var webControlFieldInfo = theType.GetField("Event" + oneEvent.Name, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy);
                    if (webControlFieldInfo != null)
                    {
                        var castedObj = obj as System.Web.UI.Control;
                        var castedSource = source as System.Web.UI.Control;
                        CopyWebControlEvents(castedObj, castedSource, oneEvent.Name);
                    }
                }
                if (fieldInfo != null)
                {
                    var sourceDelegate = fieldInfo.GetValue(source) as Delegate;
                    if (sourceDelegate != null)
                    {
                        foreach (var invocation in sourceDelegate.GetInvocationList())
                        {
                            var addHandler = oneEvent.GetAddMethod();
                            object[] addHandlerArgs = { invocation };
                            addHandler.Invoke(obj, addHandlerArgs);
                        }
                    }
                }
            }
        }

        private static void CopyWebControlEvents(System.Web.UI.Control obj, System.Web.UI.Control source, string eventName)
        {
            var theType = typeof(System.Web.UI.Control);
            var propertyInfo = theType.GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            var eventHandlerList = propertyInfo.GetValue(source, new object[] { }) as System.ComponentModel.EventHandlerList;
            var fieldInfo = theType.GetField("Event" + eventName, BindingFlags.NonPublic | BindingFlags.Static);
            var oneEvent = theType.GetEvent(eventName);

            object eventKey = fieldInfo.GetValue(source);
            var eventHandler = eventHandlerList[eventKey] as Delegate;
            if (eventHandler != null)
            {
                foreach (var invocation in eventHandler.GetInvocationList())
                {
                    var addHandler = oneEvent.GetAddMethod();
                    object[] addHandlerArgs = { invocation };
                    addHandler.Invoke(obj, addHandlerArgs);
                }
            }
        }
    }
}

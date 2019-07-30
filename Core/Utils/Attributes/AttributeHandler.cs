using System;
using System.Reflection;
using Godot;
using Wayfarer.Core.Utils.Debug;

namespace Wayfarer.Core.Utils.Attributes
{
    public static class AttributeHandler
    {
        
        public static void SetupAttributes(this Node node)
        {
            Type type = node.GetType();
            
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                foreach (Attribute attribute in field.GetCustomAttributes())
                {
                    switch (attribute)
                    {
                        case GetAttribute getAttribute:
                            if (getAttribute.IsFieldNameThatHasPath)
                            {
                                SetNodeInstanceFromPathField(node, field, getAttribute);
                            }
                            else
                            {
                                SetNodeInstanceFromPath(node, field, getAttribute);
                            }
                            break;
                    }
                }
            }
            
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                foreach (Attribute attribute in property.GetCustomAttributes())
                {
                    switch (attribute)
                    {
                        case GetAttribute getAttribute:
                            if (getAttribute.IsFieldNameThatHasPath)
                            {
                                SetNodeInstanceFromPathField(node, property, getAttribute);
                            }
                            else
                            {
                                SetNodeInstanceFromPath(node, property, getAttribute);
                            }
                            break;
                    }
                }
            }
        }

        private static void SetNodeInstanceFromPathField(Node node, FieldInfo field, GetAttribute attribute)
        {
            Type type = node.GetType();
            NodePath path = GetNodePathFromSource(node, type, attribute);
            SetNodeInstanceToMember(node, field, path);
        }

        private static void SetNodeInstanceFromPathField(Node node, PropertyInfo property, GetAttribute attribute)
        {
            Type type = node.GetType();
            NodePath path = GetNodePathFromSource(node, type, attribute);
            SetNodeInstanceToMember(node, property, path);
        }
        
        private static NodePath GetNodePathFromSource(Node node, Type type, GetAttribute attribute)
        {
            NodePath pathFromField = GetNodePathFromSourceField(node, type, attribute);
            
            if (pathFromField == null)
            {
                return GetNodePathFromSourceProperty(node, type, attribute);
            }
            
            return pathFromField;
        }

        private static NodePath GetNodePathFromSourceProperty(Node node, Type type, GetAttribute attribute)
        {
            PropertyInfo sourceProperty = type.GetProperty(attribute.NodePathSourceFieldName);

            if (sourceProperty == null)
            {
                Log.Error("Tried to get NodePath from a Property, but the Property " + attribute.NodePathSourceFieldName + " was null (in Node: " + node.Name + ")", true);
                return null;
            }
            else if (!typeof(NodePath).IsAssignableFrom(sourceProperty.PropertyType))
            {
                Log.Error("NodePath can not be assigned to " + sourceProperty.PropertyType, true);
            }

            return (NodePath) sourceProperty.GetValue(node);
        }
        
        private static NodePath GetNodePathFromSourceField(Node node, Type type, GetAttribute attribute)
        {
            FieldInfo sourceField = type.GetField(attribute.NodePathSourceFieldName);
            
            if (sourceField == null)
            {
                Log.Error("Tried to get NodePath from a Field, but the Field " + attribute.NodePathSourceFieldName + " was null (in Node: " + node.Name + ")", true);
                return null;
            }
            
            if (!typeof(NodePath).IsAssignableFrom(sourceField.FieldType))
            {
                Log.Error("NodePath can not be assigned to " + sourceField.FieldType, true);
            }
            
            return (NodePath) sourceField.GetValue(node);
        }

        private static void SetNodeInstanceFromPath(Node node, FieldInfo field, GetAttribute attribute)
        {
            SetNodeInstanceToMember(node, field, attribute.NodePath);
        }

        private static void SetNodeInstanceFromPath(Node node, PropertyInfo property, GetAttribute attribute)
        {
            SetNodeInstanceToMember(node, property, attribute.NodePath);
        }

        private static void SetNodeInstanceToMember(Node node, FieldInfo field, string path)
        {
            Node targetNode = node.GetNode(path);
            if (targetNode == null)
            {
                Log.Error("Target Node was null, couldn't set Node instance to field " + field.Name, true);
            }

            try
            {
                field.SetValue(node, targetNode);
            }
            catch (ArgumentException e)
            {
                Log.Error(e.Message, true);
            }
        }

        private static void SetNodeInstanceToMember(Node node, PropertyInfo property, string path)
        {
            Node value = node.GetNode(path);
            if (value == null)
            {
                Log.Error("Target Node was null, couldn't set Node instance to property " + property.Name, true);
            }

            try
            {
                property.SetValue(node, value);
            }
            catch (ArgumentException e)
            {
                Log.Error(e.Message, true);
            }
        }
    }
}